using Alphy.Properties;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alphy
{
    public partial class Form1 : MaterialForm
    {
        private string gamePath = "";
        private readonly string backupFolder = Config.BackupFolder;

        private class ModItem
        {
            public MaterialCheckbox Checkbox { get; set; }
            public string ModName { get; set; }
            public string[] Files { get; set; }
            public ModCard Card { get; set; }
        }

        private List<ModItem> activeMods = new List<ModItem>();

        public Form1()
        {
            InitializeComponent();

            string syncAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlphyMods.exe");
            bool syncAppExists = File.Exists(syncAppPath);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(tabControl1, true, null);

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

            this.DrawerTabControl = tabControl1;
            this.DrawerUseColors = true;

            this.Shown += Form1_Shown;
        }

        private async void Form1_Shown(object sender, EventArgs e)
        {
            LogToConsole("System: Booting Alphy...");
            ToggleControls(false);

            ProcessCheck.HandleGameRunning();

            Config.InitializeDirectories();
            LoadSavedPath();
            InitializeModList();

            LogToConsole("System: Verifying digital seals...");

            await Task.Run(() =>
            {
                LoadSavedModStatesBackground();
            });

            LogToConsole($"Alphy Initialized. Version: {Updater.LocalVersion}");

            CheckVersionAndLockdown();
        }
        private void SmartUpdateBackups(string currentLocalVer)
        {
            string lastSeenVer = Settings.Default.LastGameVersion;

            if (!string.IsNullOrEmpty(lastSeenVer) && lastSeenVer != currentLocalVer)
            {
                LogToConsole($"System: Game update detected! ({lastSeenVer} -> {currentLocalVer})");
                LogToConsole("System: Performing Smart Hash-Sync to protect backups...");

                var allKnownMods = activeMods.SelectMany(m => m.Files).Distinct().ToList();

                foreach (string modFilePath in allKnownMods)
                {
                    string fileName = Path.GetFileName(modFilePath);
                    string gameFilePath = Path.Combine(gamePath, fileName);
                    string backupFilePath = Path.Combine(backupFolder, fileName + ".bak");

                    if (File.Exists(gameFilePath) && File.Exists(backupFilePath) && File.Exists(modFilePath))
                    {
                        if (FileChecker.IsNewVanillaFile(gameFilePath, backupFilePath, modFilePath))
                        {
                            try
                            {
                                File.Delete(backupFilePath);
                                File.Copy(gameFilePath, backupFilePath);
                                LogToConsole($"Updated Backup: {fileName}");
                            }
                            catch (Exception ex)
                            {
                                LogToConsole($"Sync Error [{fileName}]: {ex.Message}");
                            }
                        }
                    }
                }

                foreach (var mod in activeMods)
                {
                    this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = false; });
                }
                Config.SaveSettings(new Dictionary<string, string>());

                Settings.Default.LastGameVersion = currentLocalVer;
                Settings.Default.Save();

                LogToConsole("System: All backups synchronized with the new game version.");
            }
            else if (string.IsNullOrEmpty(lastSeenVer))
            {
                Settings.Default.LastGameVersion = currentLocalVer;
                Settings.Default.Save();
            }
        }

        private void InitializeModList()
        {
            tabControl1.TabPages.Clear();
            activeMods.Clear();

            string modsRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");

            if (!Directory.Exists(modsRoot))
            {
                Directory.CreateDirectory(modsRoot);
                LogToConsole("Created mods folder. Please restart Alphy.");
                return;
            }

            string[] categories = { "Body", "Decal", "Wheels", "Boost", "Hat", "Antenna", "Trail", "Paint", "Boost Audio", "Goal Explosion", "Banner" };

            foreach (string category in categories)
            {
                string categoryPath = Path.Combine(modsRoot, category);
                if (!Directory.Exists(categoryPath)) Directory.CreateDirectory(categoryPath);

                TabPage newTab = new TabPage(category) { BackColor = System.Drawing.Color.FromArgb(50, 50, 50) };

                FlowLayoutPanel panel = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    BackColor = System.Drawing.Color.Transparent,
                    AutoScroll = true
                };

                typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(panel, true, null);

                var modFolders = Directory.GetDirectories(categoryPath);

                foreach (string modPath in modFolders)
                {
                    string modName = Path.GetFileName(modPath);
                    string[] modFiles = Directory.GetFiles(modPath)
                        .Where(f => f.EndsWith(".upk", StringComparison.OrdinalIgnoreCase) ||
                                    f.EndsWith(".bnk", StringComparison.OrdinalIgnoreCase))
                        .ToArray();

                    if (modFiles.Length > 0)
                    {
                        AddModToPanel(panel, modName, modFiles, category);
                    }
                }

                newTab.Controls.Add(panel);
                tabControl1.TabPages.Add(newTab);
            }
        }

        private void AddModToPanel(FlowLayoutPanel panel, string label, string[] files, string category)
        {
            string modName = label;
            string replacesName = "";

            int replaceIndex = label.IndexOf("(Replaces", StringComparison.OrdinalIgnoreCase);

            if (replaceIndex > 0)
            {
                modName = label.Substring(0, replaceIndex).Trim();
                replacesName = label.Substring(replaceIndex).Trim();
            }
            else
            {
                int bracketIndex = label.IndexOf("(");
                if (bracketIndex > 0)
                {
                    modName = label.Substring(0, bracketIndex).Trim();
                    replacesName = label.Substring(bracketIndex).Trim();
                }
            }

            ModCard card = new ModCard(label, category, modName, replacesName)
            {
                Margin = new Padding(10, 5, 0, 0)
            };

            card.Checkbox.CheckedChanged += (s, e) => {
                if (card.Checkbox.Checked)
                {
                    foreach (var mod in activeMods.Where(m => (string)m.Checkbox.Tag == category && m.Checkbox != card.Checkbox))
                    {
                        mod.Checkbox.Checked = false;
                    }
                }
            };

            activeMods.Add(new ModItem { Checkbox = card.Checkbox, ModName = label, Files = files, Card = card });
            panel.Controls.Add(card);
        }

        private void LoadSavedModStatesBackground()
        {
            var savedModData = Config.LoadSettings();

            foreach (var mod in activeMods)
            {
                string modName = mod.ModName;

                if (savedModData.ContainsKey(modName))
                {
                    if (string.IsNullOrEmpty(gamePath))
                    {
                        this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = true; });
                        continue;
                    }

                    string savedHash = savedModData[modName];
                    string fileName = Path.GetFileName(mod.Files[0]);
                    string gameFilePath = Path.Combine(gamePath, fileName);
                    string backupFilePath = Path.Combine(backupFolder, fileName + ".bak");

                    if (File.Exists(gameFilePath))
                    {
                        string currentHash = FileChecker.GetFileHash(gameFilePath);

                        if (currentHash == savedHash)
                        {
                            this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = true; });
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = false; });
                            LogToConsole($"Notice: {modName} state reset (File mismatch).");

                            if (File.Exists(backupFilePath))
                            {
                                string backupHash = FileChecker.GetFileHash(backupFilePath);

                                if (currentHash != backupHash)
                                {
                                    try
                                    {
                                        File.Delete(backupFilePath);
                                        File.Copy(gameFilePath, backupFilePath);
                                        LogToConsole($"System: Updated vanilla backup for {fileName}.");
                                    }
                                    catch (Exception)
                                    {
                                        LogToConsole($"Warning: Could not update backup for {fileName}. File might be locked.");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = true; });
                    }
                }
            }
        }

        private async void CheckVersionAndLockdown()
        {
            LogToConsole("System: Checking server status...");

            await Task.Run(() => {
                bool isLocked = Lockdown.IsInLockdown();

                this.Invoke((MethodInvoker)delegate {
                    if (isLocked)
                    {
                        LogToConsole("ALERT: The app is currently in maintenance lockdown.");
                        ToggleControls(false);
                        return;
                    }

                    LogToConsole("System: Server active. Verifying app version...");
                    string current = Updater.LocalVersion;
                    string latest = Updater.GetServerVersion();

                    if (latest != "Unknown" && latest != current)
                    {
                        LogToConsole("CRITICAL: App update required to continue.");
                        ToggleControls(false);
                        Updater.CheckForUpdates();
                        return;
                    }
                    else if (latest == "Unknown")
                    {
                        LogToConsole("Error: Could not verify app version. Locked for safety.");
                        ToggleControls(false);
                        return;
                    }

                    LogToConsole("Status: App is up to date.");
                    PerformVersionLockdown();
                });
            });
        }

        private async void PerformVersionLockdown()
        {
            if (string.IsNullOrEmpty(gamePath))
            {
                LogToConsole("System: Please select your Rocket League folder.");
                btnChangeFolder.Enabled = true;
                return;
            }

            LogToConsole("System: Verifying game version compatibility...");

            string localVer = GameVersionCheck.GetLocalGameVersion(gamePath);
            string supportedVer = GameVersionCheck.GetSupportedVersion();

            string liveBuild = GameVersionCheck.GetLiveBuildVersion();
            string supportedBuild = GameVersionCheck.GetSupportedBuildVersion();

            bool isExeMismatch = (localVer != "Not Found" && localVer != "Error" && localVer != supportedVer);
            bool isBuildMismatch = (liveBuild != "Unknown" && supportedBuild != "Unknown" && liveBuild != supportedBuild);

            if (isExeMismatch || isBuildMismatch)
            {
                LogToConsole("Error: Alphy does not support this version of the game.");
                if (isExeMismatch)
                    LogToConsole($"Detected EXE: {localVer} | Supported EXE: {supportedVer}");

                if (isBuildMismatch)
                    LogToConsole($"Detected Live Build: {liveBuild} | Supported Build: {supportedBuild}");

                ToggleControls(false);

                MessageBox.Show(
                    "Your Rocket League version or build is not supported yet.\n\n" +
                    $"Detected EXE: {localVer}\nSupported EXE: {supportedVer}\n\n" +
                    $"Detected Live Build: {liveBuild}\nSupported Build: {supportedBuild}",
                    "Incompatible Game Version",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop
                );
            }
            else if (localVer == "Not Found")
            {
                LogToConsole("Error: Could not locate RocketLeague.exe. Verify path.");
                ToggleControls(false);
            }
            else
            {
                LogToConsole($"System: EXE Version verified: {localVer}");
                if (liveBuild != "Unknown") LogToConsole($"System: Build verified: {liveBuild}");

                SmartUpdateBackups(localVer);

                LogToConsole("System: Validating backups...");
                await CreateInitialBackups();

                LogToConsole("System: Synchronizing mod icons...");

                foreach (var mod in activeMods)
                {
                    await mod.Card.LoadImageAsync();
                }

                LogToConsole("System: Ready.");
                ToggleControls(true);
            }
        }

        private void ToggleControls(bool state)
        {
            btnSave.Enabled = state;
            btnImportMod.Enabled = state;
            btnRemoveModWizard.Enabled = state;
            foreach (var mod in activeMods)
            {
                mod.Checkbox.Enabled = state;
            }
            btnChangeFolder.Enabled = true;
            btnUpdate.Enabled = true;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            LogToConsole("System: Re-validating all versions...");
            CheckVersionAndLockdown();
        }

        private void LoadSavedPath()
        {
            if (!string.IsNullOrEmpty(Settings.Default.GamePath))
            {
                gamePath = Settings.Default.GamePath;
                LogToConsole("Path loaded from settings.");
            }
        }

        private void btnChangeFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select the 'rocketleague' root folder";
                if (fbd.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    string selected = fbd.SelectedPath;
                    string cookedPath = @"TAGame\CookedPCConsole";

                    gamePath = Directory.Exists(Path.Combine(selected, cookedPath))
                        ? Path.Combine(selected, cookedPath)
                        : selected;

                    Settings.Default.GamePath = gamePath;
                    Settings.Default.Save();
                    LogToConsole("Path updated.");

                    _ = CreateInitialBackups();
                    PerformVersionLockdown();
                }
            }
        }

        private void LogToConsole(string text)
        {
            if (txtConsole.InvokeRequired)
            {
                txtConsole.Invoke(new Action<string>(LogToConsole), text);
                return;
            }
            txtConsole.AppendText($"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
            txtConsole.SelectionStart = txtConsole.Text.Length;
            txtConsole.ScrollToCaret();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (ProcessCheck.IsGameRunning())
            {
                ProcessCheck.HandleGameRunning();
                return;
            }

            string localVer = GameVersionCheck.GetLocalGameVersion(gamePath);
            string supportedVer = GameVersionCheck.GetSupportedVersion();
            string liveBuild = GameVersionCheck.GetLiveBuildVersion();
            string supportedBuild = GameVersionCheck.GetSupportedBuildVersion();

            bool isExeMismatch = (localVer != "Not Found" && localVer != "Error" && localVer != supportedVer);
            bool isBuildMismatch = (liveBuild != "Unknown" && supportedBuild != "Unknown" && liveBuild != supportedBuild);

            if (localVer != supportedVer)
            {
                LogToConsole("Abort: Game version mismatch detected. Modding blocked.");
                PerformVersionLockdown();
                return;
            }

            if (isExeMismatch || isBuildMismatch)
            {
                LogToConsole("Abort: Game version or build mismatch detected. Modding blocked.");
                PerformVersionLockdown();
                return;
            }

            btnSave.Enabled = false;
            LogToConsole("System: Applying modifications and sealing hashes...");

            var modHashesToSave = new Dictionary<string, string>();
            bool overallSuccess = true;

            var modStates = activeMods.Select(m => new
            {
                ModName = m.ModName,
                Files = m.Files,
                IsChecked = m.Checkbox.Checked
            }).ToList();

            await Task.Run(() =>
            {
                foreach (var mod in modStates.Where(m => !m.IsChecked))
                {
                    foreach (string file in mod.Files)
                    {
                        HandleFileOperation(file, false);
                    }
                }
                foreach (var mod in modStates.Where(m => m.IsChecked))
                {
                    foreach (string file in mod.Files)
                    {
                        if (!HandleFileOperation(file, true))
                        {
                            overallSuccess = false;
                        }
                    }

                    string fileName = Path.GetFileName(mod.Files[0]);
                    string installedFilePath = Path.Combine(gamePath, fileName);

                    string hash = FileChecker.GetFileHash(installedFilePath);
                    if (hash != "error" && hash != "null")
                    {
                        modHashesToSave[mod.ModName] = hash;
                    }
                }
            });

            Config.SaveSettings(modHashesToSave);

            if (overallSuccess)
                LogToConsole("DONE: All modifications applied and settings secured.");
            else
                LogToConsole("FAILED: Some files could not be replaced. Verify file permissions.");

            btnSave.Enabled = true;
        }

        private bool HandleFileOperation(string modFilePath, bool isInstall)
        {
            string fileName = Path.GetFileName(modFilePath);
            string gameFile = Path.Combine(gamePath, fileName);
            string backupFile = Path.Combine(backupFolder, fileName + ".bak");

            try
            {
                if (isInstall)
                {
                    if (!File.Exists(modFilePath)) return false;
                    if (File.Exists(gameFile) && !File.Exists(backupFile))
                    {
                        File.Copy(gameFile, backupFile);
                    }
                    File.Copy(modFilePath, gameFile, true);
                }
                else
                {
                    if (File.Exists(backupFile))
                    {
                        File.Copy(backupFile, gameFile, true);
                    }
                }
                return true;
            }
            catch (IOException)
            {
                this.Invoke((MethodInvoker)delegate { LogToConsole($"Lock Error: {fileName} is busy."); });
                return false;
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate { LogToConsole($"Error [{fileName}]: {ex.Message}"); });
                return false;
            }
        }

        private async Task CreateInitialBackups()
        {
            var filesToBackup = activeMods.SelectMany(m => m.Files).Distinct().ToList();

            if (filesToBackup.Count == 0) return;

            int newBackups = 0;

            await Task.Run(() =>
            {
                foreach (string fullModPath in filesToBackup)
                {
                    string fileName = Path.GetFileName(fullModPath);
                    string sourcePath = Path.Combine(gamePath, fileName);
                    string destPath = Path.Combine(backupFolder, fileName + ".bak");

                    if (File.Exists(sourcePath) && !File.Exists(destPath))
                    {
                        try { File.Copy(sourcePath, destPath); newBackups++; }
                        catch { /* File might be locked by Rocket League */ }
                    }
                }
            });

            if (newBackups > 0) LogToConsole($"System: {newBackups} new original files backed up.");
        }

        private async void btnImportMod_Click(object sender, EventArgs e)
        {
            using (FormAddMod addModForm = new FormAddMod())
            {
                if (addModForm.ShowDialog() == DialogResult.OK)
                {
                    LogToConsole("System: New mod detected. Refreshing mod list...");

                    InitializeModList();

                    LogToConsole("System: Validating backups for new mods...");
                    await CreateInitialBackups();

                    LogToConsole("System: Synchronizing mod icons...");
                    foreach (var mod in activeMods)
                    {
                        await mod.Card.LoadImageAsync();
                    }

                    await Task.Run(() =>
                    {
                        LoadSavedModStatesBackground();
                    });

                    LogToConsole("System: Mod list refreshed.");
                }
            }
        }

        private async void btnRemoveModWizard_Click(object sender, EventArgs e)
        {
            using (FormRemoveMod removeModForm = new FormRemoveMod())
            {
                if (removeModForm.ShowDialog() == DialogResult.OK)
                {
                    LogToConsole("System: Mods were removed. Refreshing mod list...");

                    InitializeModList();

                    await CreateInitialBackups();

                    LogToConsole("System: Synchronizing mod icons...");
                    foreach (var mod in activeMods)
                    {
                        await mod.Card.LoadImageAsync();
                    }

                    await Task.Run(() =>
                    {
                        LoadSavedModStatesBackground();
                    });

                    LogToConsole("System: Mod list refreshed.");
                }
            }
        }
    }
}