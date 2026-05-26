using Alphy.Properties;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Alphy.Form1;

namespace Alphy
{
    public partial class Form1 : MaterialForm, IAlphyHost
    {
        private string gamePath = "";
        private readonly string backupFolder = Config.BackupFolder;

        private string currentCategory = "All";

        private class ModItem
        {
            public MaterialCheckbox Checkbox { get; set; }
            public string ModName { get; set; }
            public string[] Files { get; set; }
            public ModCard Card { get; set; }
            public string Category { get; set; }
        }

        private List<ModItem> activeMods = new List<ModItem>();

        public Form1()
        {
            InitializeComponent();

            LoadPlugins();

            string syncAppPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AlphyMods.exe");
            bool syncAppExists = File.Exists(syncAppPath);

            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo600, Primary.Indigo700, Primary.Indigo400, Accent.Indigo200, TextShade.WHITE);

            materialSkinManager.RemoveFormToManage(this);

            flowLayoutPanelMods.Paint += FlowLayoutPanelMods_Paint;
            flowLayoutPanelMods.Resize += (s, e) => flowLayoutPanelMods.Invalidate();
            flowLayoutPanelMods.Scroll += (s, e) => flowLayoutPanelMods.Invalidate(true);
            flowLayoutPanelMods.MouseWheel += (s, e) => flowLayoutPanelMods.Invalidate(true);

            typeof(FlowLayoutPanel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, flowLayoutPanelMods, new object[] { true });

            this.Shown += Form1_Shown;

            panelSidebar.BackColorChanged += (s, e) =>
            {
                if (panelSidebar.BackColor != Color.FromArgb(30, 30, 30))
                    panelSidebar.BackColor = Color.FromArgb(30, 30, 30);
            };

            panelBottom.BackColorChanged += (s, e) =>
            {
                if (panelBottom.BackColor != Color.FromArgb(40, 40, 40))
                    panelBottom.BackColor = Color.FromArgb(40, 40, 40);
            };

            panelSidebar.BackColor = Color.FromArgb(30, 30, 30);
            panelBottom.BackColor = Color.FromArgb(40, 40, 40);

            SidebarCategory_Click(btnCatAll, EventArgs.Empty);
        }

        private void FlowLayoutPanelMods_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            var state = g.Save();

            g.ResetTransform();

            Rectangle rect = flowLayoutPanelMods.ClientRectangle;

            if (rect.Width == 0 || rect.Height == 0) return;

            Color colorTopLeft = Color.FromArgb(15, 17, 30);
            Color colorBottomRight = Color.FromArgb(35, 43, 90);

            using (System.Drawing.Drawing2D.LinearGradientBrush brush = new System.Drawing.Drawing2D.LinearGradientBrush(rect, colorTopLeft, colorBottomRight, System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal))
            {
                g.FillRectangle(brush, rect);
            }

            g.Restore(state);
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
                        AddModToMemory(modName, modFiles, category);
                    }
                }
            }
            ApplyFilters();
        }

        private void AddModToMemory(string label, string[] files, string category)
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
                    foreach (var mod in activeMods.Where(m => m.Category == category && m.Checkbox != card.Checkbox))
                    {
                        mod.Checkbox.Checked = false;
                    }
                }
            };

            activeMods.Add(new ModItem { Checkbox = card.Checkbox, ModName = label, Files = files, Card = card, Category = category });
        }

        private void ApplyFilters()
        {
            flowLayoutPanelMods.SuspendLayout();
            flowLayoutPanelMods.Controls.Clear();

            string search = txtSearch.Text.Trim().ToLower();

            foreach (var mod in activeMods)
            {
                bool matchesCategory = currentCategory == "All" || mod.Category.ToLower() == currentCategory.ToLower();
                bool matchesSearch = string.IsNullOrEmpty(search) || mod.ModName.ToLower().Contains(search);

                if (matchesCategory && matchesSearch)
                {
                    flowLayoutPanelMods.Controls.Add(mod.Card);
                }
            }

            flowLayoutPanelMods.ResumeLayout();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void SidebarCategory_Click(object sender, EventArgs e)
        {
            Button clickedBtn = sender as Button;
            if (clickedBtn == null || clickedBtn.Tag == null) return;

            foreach (Control ctrl in panelSidebar.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
                    btn.ForeColor = System.Drawing.Color.DarkGray;
                }
            }

            clickedBtn.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
            clickedBtn.ForeColor = System.Drawing.Color.White;

            currentCategory = clickedBtn.Tag.ToString();
            ApplyFilters();
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
                        LogToConsole("CRITICAL: App update required to continue.", true);
                        ToggleControls(false);
                        Updater.CheckForUpdates();
                        return;
                    }
                    else if (latest == "Unknown")
                    {
                        LogToConsole("Error: Could not verify app version. Locked for safety.", true);
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
                LogToConsole("Error: Alphy does not support this version of the game.", true);
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
                LogToConsole("Error: Could not locate RocketLeague.exe. Verify path.", true);
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

        public void LogToConsole(string message, bool isError = false)
        {
            if (string.IsNullOrEmpty(message)) return;

            if (txtConsole.InvokeRequired)
            {
                txtConsole.Invoke(new Action(() => LogToConsole(message, isError)));
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtConsole.SelectionStart = txtConsole.TextLength;
            txtConsole.SelectionLength = 0;
            txtConsole.SelectionColor = isError ? Color.Salmon : Color.LightGreen;
            txtConsole.AppendText($"[{timestamp}] {message}{Environment.NewLine}");
            txtConsole.ScrollToCaret();
        }

        private void btnPlugins_Click(object sender, EventArgs e)
        {
            using (FormPlugins pluginsForm = new FormPlugins(this, this))
            {
                pluginsForm.ShowDialog();
            }
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
                LogToConsole("Abort: Game version mismatch detected. Modding blocked.", true);
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
                LogToConsole("FAILED: Some files could not be replaced. Verify file permissions.", true);

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
                this.Invoke((MethodInvoker)delegate { LogToConsole($"Lock Error: {fileName} is busy.", true); });
                return false;
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate { LogToConsole($"Error [{fileName}]: {ex.Message}", true); });
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

        private List<IAlphyPlugin> loadedPlugins = new List<IAlphyPlugin>();

        public List<IAlphyPlugin> GetLoadedPlugins()
        {
            return loadedPlugins;
        }

        public void ReloadPlugin(string dllPath)
        {
            try
            {
                byte[] dllBytes = File.ReadAllBytes(dllPath);
                var assembly = System.Reflection.Assembly.Load(dllBytes);

                foreach (Type type in assembly.GetTypes())
                {
                    if (typeof(IAlphyPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
                    {
                        IAlphyPlugin plugin = (IAlphyPlugin)Activator.CreateInstance(type);
                        plugin.Initialize(this);

                        var existing = loadedPlugins.FirstOrDefault(p => p.Name == plugin.Name);
                        if (existing != null)
                        {
                            loadedPlugins.Remove(existing);
                        }
                        loadedPlugins.Add(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                LogToConsole($"Plugin Error: {ex.Message}", true);
            }
        }

        private void LoadPlugins()
        {
            string pluginsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Alphy\Plugins");
            if (!Directory.Exists(pluginsDir)) Directory.CreateDirectory(pluginsDir);

            string[] pluginFiles = Directory.GetFiles(pluginsDir, "*.dll", SearchOption.AllDirectories);

            foreach (string dllPath in pluginFiles)
            {
                ReloadPlugin(dllPath);
            }
        }

        public void RefreshModList()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(RefreshModList));
                return;
            }

            LogToConsole("System: Plugin requested mod list refresh...");
            InitializeModList();

            _ = Task.Run(async () =>
            {
                await CreateInitialBackups();
                foreach (var mod in activeMods) { await mod.Card.LoadImageAsync(); }
                LoadSavedModStatesBackground();
            });
        }

        public interface IAlphyHost
        {
            void RefreshModList();
            void LogToConsole(string message, bool isError = false);
        }

        public interface IAlphyPlugin
        {
            string Name { get; }
            string Description { get; }
            string Version { get; }
            void Initialize(IAlphyHost host);
            void ShowUI();
        }
    }
}