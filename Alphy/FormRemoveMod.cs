using Alphy.Properties;
using System;
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
                "Decal", "Goal Explosion", "Hat", "Paint", "Trail", "Wheels"
            });
        }

        private void Log(string message)
        {
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

                try
                {
                    if (Directory.Exists(folderPath))
                    {
                        string[] modFiles = Directory.GetFiles(folderPath);
                        bool wasEnabled = savedSettings.ContainsKey(modName);

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
                                        }
                                    }
                                }
                            }
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
    }
}