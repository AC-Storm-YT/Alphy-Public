using Alphy.Properties;
using System;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Alphy
{
    public partial class FormRemoveMod : MaterialForm
    {
        private readonly string baseModsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

        public FormRemoveMod()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            cmbCategory.Items.AddRange(new string[]
            {
                "Antenna", "Banner", "Body", "Boost", "Boost Audio",
                "Decal", "Goal Explosion", "Hat", "Paint", "Trail", "Wheels", "Engine Audio",
                "Avatar Border", "Custom Decals", "Custom Balls", "Custom Boost Meter"
            });
        }

        private void Log(string message)
        {
            if (txtConsole.InvokeRequired)
            {
                txtConsole.BeginInvoke((MethodInvoker)delegate { Log(message); });
                return;
            }

            txtConsole.AppendText(message + "\n");
            txtConsole.ScrollToCaret();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelMods.Controls.Clear();
            chkSelectAll.Checked = false;

            if (cmbCategory.SelectedIndex == -1) return;

            string category = cmbCategory.SelectedItem.ToString();
            string categoryPath = Path.Combine(baseModsPath, category);

            if (!Directory.Exists(categoryPath))
            {
                Log($"Notice: No '{category}' folder found.");
                return;
            }

            var modFolders = Directory.GetDirectories(categoryPath);

            if (modFolders.Length == 0)
            {
                Log($"Notice: No mods installed in '{category}'.");
                return;
            }

            Log($"Loaded {modFolders.Length} mods for {category}.");

            foreach (string folder in modFolders)
            {
                string modName = Path.GetFileName(folder);

                MaterialCheckbox chk = new MaterialCheckbox
                {
                    Text = modName,
                    Tag = folder,
                    AutoSize = true,
                    Margin = new Padding(5)
                };

                panelMods.Controls.Add(chk);
            }
        }
        private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in panelMods.Controls)
            {
                if (ctrl is MaterialCheckbox chk)
                {
                    chk.Checked = chkSelectAll.Checked;
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var boxesToRemove = panelMods.Controls.OfType<MaterialCheckbox>().Where(c => c.Checked).ToList();

            if (boxesToRemove.Count == 0)
            {
                Log("Error: No mods selected for removal.");
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to permanently delete {boxesToRemove.Count} mod(s)?\nThis cannot be undone.",
                "Confirm Deletion",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            int successCount = 0;
            string gamePath = Settings.Default.GamePath;
            string backupFolder = Config.BackupFolder;
            var savedSettings = Config.LoadSettings();
            bool configChanged = false;

            foreach (MaterialCheckbox chk in boxesToRemove)
            {
                string folderPath = chk.Tag.ToString();
                string modName = chk.Text;
                string category = cmbCategory.SelectedItem?.ToString() ?? Path.GetFileName(Path.GetDirectoryName(folderPath));

                try
                {
                    if (Directory.Exists(folderPath))
                    {
                        bool wasEnabled = savedSettings.ContainsKey(modName);
                        if (IsCustomTextureCategory(category))
                        {
                            string customBackupDir = GetCustomTextureBackupDir(folderPath, category, backupFolder);
                            bool hasCustomBackup = Directory.Exists(customBackupDir);
                            if ((wasEnabled || hasCustomBackup) &&
                                !RestoreCustomTextureMod(folderPath, category, modName, gamePath, backupFolder))
                            {
                                Log($"Skipped delete: {modName} is still active or could not be restored.");
                                continue;
                            }

                            if (wasEnabled)
                            {
                                savedSettings.Remove(modName);
                                configChanged = true;
                            }

                            Directory.Delete(folderPath, true);
                            TryDeleteDirectory(customBackupDir);
                            Log($"Deleted: {modName}");
                            successCount++;
                            continue;
                        }

                        string[] modFiles = Directory.GetFiles(folderPath);
                        bool canDelete = true;

                        if (!string.IsNullOrEmpty(gamePath) && Directory.Exists(gamePath))
                        {
                            foreach (string modFile in modFiles)
                            {
                                string fileName = Path.GetFileName(modFile);
                                string gameFile = Path.Combine(gamePath, fileName);
                                string backupFile = Path.Combine(backupFolder, fileName + ".bak");

                                bool isCurrentlyInjected = false;
                                if (File.Exists(gameFile) && File.Exists(modFile))
                                {
                                    string gameHash = FileChecker.GetFileHash(gameFile);
                                    string modHash = FileChecker.GetFileHash(modFile);

                                    if (gameHash != "error" && gameHash != "null" && gameHash == modHash)
                                    {
                                        isCurrentlyInjected = true;
                                    }
                                }

                                if (wasEnabled || isCurrentlyInjected)
                                {
                                    if (File.Exists(backupFile))
                                    {
                                        try
                                        {
                                            File.Copy(backupFile, gameFile, true);
                                            Log($"Restored vanilla file: {fileName}");
                                        }
                                        catch
                                        {
                                            Log($"Warning: Could not restore {fileName}. Game might be running.");
                                            canDelete = false;
                                        }
                                    }
                                    else
                                    {
                                        Log($"Warning: Missing vanilla backup for {fileName}. Delete skipped.");
                                        canDelete = false;
                                    }
                                }
                            }
                        }
                        else if (wasEnabled)
                        {
                            Log("Warning: Rocket League folder is missing. Delete skipped so the active mod can be restored later.");
                            canDelete = false;
                        }

                        if (!canDelete)
                        {
                            Log($"Skipped delete: {modName} could not be restored to vanilla first.");
                            continue;
                        }

                        if (wasEnabled)
                        {
                            savedSettings.Remove(modName);
                            configChanged = true;
                        }

                        Directory.Delete(folderPath, true);
                        Log($"Deleted: {modName}");
                        successCount++;

                        foreach (string modFile in modFiles)
                        {
                            string fileName = Path.GetFileName(modFile);
                            string backupFile = Path.Combine(backupFolder, fileName + ".bak");

                            if (File.Exists(backupFile))
                            {
                                var allRemainingFiles = Directory.GetFiles(baseModsPath, fileName, SearchOption.AllDirectories);
                                if (allRemainingFiles.Length == 0)
                                {
                                    try
                                    {
                                        File.Delete(backupFile);
                                        Log($"Cleared unused backup: {fileName}.bak");
                                    }
                                    catch { }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log($"Failed to delete '{modName}': {ex.Message}");
                }
            }

            if (configChanged)
            {
                Config.SaveSettings(savedSettings);
            }

            if (successCount > 0)
            {
                Log($"DONE: Successfully removed {successCount} mod(s).");
                this.DialogResult = DialogResult.OK;
                cmbCategory_SelectedIndexChanged(null, null);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static bool IsCustomTextureCategory(string category)
        {
            return category == "Custom Decals" ||
                   category == "Custom Balls" ||
                   category == "Custom Boost Meter";
        }

        private bool RestoreCustomTextureMod(string folderPath,
                                             string category,
                                             string modName,
                                             string gamePath,
                                             string backupFolder)
        {
            string backupDir = GetCustomTextureBackupDir(folderPath, category, backupFolder);
            if (!Directory.Exists(backupDir))
            {
                Log($"Notice: No custom texture backup found for {modName}.");
                return true;
            }

            if (string.IsNullOrWhiteSpace(gamePath) || !Directory.Exists(gamePath))
            {
                Log("Warning: Rocket League folder is missing. Custom texture restore skipped.");
                return false;
            }

            string scriptPath = GetBackendFile("alphy_custom_texture_injector.py");
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                Log("Error: Custom texture backend missing: alphy_custom_texture_injector.py");
                return false;
            }

            try
            {
                JObject config = new JObject
                {
                    ["action"] = CustomTextureRestoreAction(category),
                    ["cookedDir"] = gamePath,
                    ["backupDir"] = backupDir,
                    ["workDir"] = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                        "Alphy", "CustomTextureWork")
                };

                string keysPath = GetBackendFile("keys.txt");
                if (!string.IsNullOrWhiteSpace(keysPath) && File.Exists(keysPath))
                    config["keysPath"] = keysPath;

                string texconvPath = GetBackendFile("texconv.exe");
                if (!string.IsNullOrWhiteSpace(texconvPath) && File.Exists(texconvPath))
                    config["texconvPath"] = texconvPath;

                string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Alphy", "CustomTextureConfigs");
                Directory.CreateDirectory(configDir);
                string configPath = Path.Combine(configDir, SafePathSegment(modName) + "_remove_restore.json");
                File.WriteAllText(configPath, config.ToString());

                Log($"Restoring active custom texture before delete: {modName}");
                return RunPythonBackend(scriptPath, $"--config \"{configPath}\"");
            }
            catch (Exception ex)
            {
                Log($"Error: Could not restore custom texture '{modName}': {ex.Message}");
                return false;
            }
        }

        private string CustomTextureRestoreAction(string category)
        {
            if (category == "Custom Decals") return "restore";
            if (category == "Custom Balls") return "restore_ball";
            if (category == "Custom Boost Meter") return "restore_boost_meter";
            throw new InvalidOperationException($"Unsupported custom texture category: {category}");
        }

        private string GetCustomTextureBackupDir(string folderPath, string category, string backupFolder)
        {
            string modFolderName = Path.GetFileName(folderPath);
            return Path.Combine(backupFolder, "CustomTextures", SafePathSegment(category), SafePathSegment(modFolderName));
        }

        private string GetBackendFile(string fileName)
        {
            string appDataPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "AlphySwapper",
                "Backend",
                fileName);
            if (File.Exists(appDataPath)) return appDataPath;

            string localPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Backend", fileName);
            return File.Exists(localPath) ? localPath : appDataPath;
        }

        private bool RunPythonBackend(string scriptPath, string arguments)
        {
            var attempts = new[]
            {
                new { FileName = "python", Prefix = "" },
                new { FileName = "py", Prefix = "-3 " },
                new { FileName = "python3", Prefix = "" }
            };

            Exception launchError = null;
            foreach (var attempt in attempts)
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = attempt.FileName,
                    Arguments = $"{attempt.Prefix}\"{scriptPath}\" {arguments}",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                try
                {
                    using (Process process = new Process { StartInfo = psi })
                    {
                        process.OutputDataReceived += (sender, args) =>
                        {
                            if (!ShouldSuppressPythonBackendLog(args.Data))
                                Log(FormatPythonBackendLog(args.Data));
                        };
                        process.ErrorDataReceived += (sender, args) =>
                        {
                            if (!ShouldSuppressPythonBackendLog(args.Data))
                                Log(FormatPythonBackendLog(args.Data));
                        };

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            Log($"Custom texture backend exited with code {process.ExitCode}.");
                            return false;
                        }
                        return true;
                    }
                }
                catch (Win32Exception ex)
                {
                    launchError = ex;
                }
            }

            Log($"Could not launch Python. Install Python or add it to PATH. {launchError?.Message}");
            return false;
        }

        private static bool ShouldSuppressPythonBackendLog(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return true;

            string trimmed = line.Trim();
            return (line.IndexOf("cryptography", StringComparison.OrdinalIgnoreCase) >= 0 &&
                    line.IndexOf("32-bit Python", StringComparison.OrdinalIgnoreCase) >= 0) ||
                   trimmed.Equals("from cryptography.hazmat.bindings.openssl import binding",
                       StringComparison.OrdinalIgnoreCase);
        }

        private static string FormatPythonBackendLog(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return "";

            string trimmed = line.Trim();
            if (trimmed.StartsWith("{") && trimmed.EndsWith("}"))
            {
                try
                {
                    JObject payload = JObject.Parse(trimmed);
                    string message = payload["message"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }
                catch
                {
                }
            }

            return line;
        }

        private static string SafePathSegment(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "CustomMod";
            foreach (char c in Path.GetInvalidFileNameChars())
                text = text.Replace(c, '_');
            return text.Trim();
        }

        private static void TryDeleteDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
                return;

            try
            {
                Directory.Delete(path, true);
            }
            catch
            {
            }
        }
    }
}
