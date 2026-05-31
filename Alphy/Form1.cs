using Alphy.Properties;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            string[] categories = { "Body", "Decal", "Wheels", "Boost", "Hat", "Antenna", "Trail", "Paint", "Boost Audio", "Goal Explosion", "Banner", "Engine Audio", "Avatar Border", "Custom Decals", "Custom Balls", "Custom Boost Meter" };

            foreach (string category in categories)
            {
                string categoryPath = Path.Combine(modsRoot, category);
                if (!Directory.Exists(categoryPath)) Directory.CreateDirectory(categoryPath);

                var modFolders = Directory.GetDirectories(categoryPath);

                foreach (string modPath in modFolders)
                {
                    string modName = Path.GetFileName(modPath);
                    bool customTextureCategory = IsCustomTextureCategory(category);
                    string[] modFiles = customTextureCategory
                        ? Directory.GetFiles(modPath)
                            .Where(f => f.EndsWith(".alphycustom", StringComparison.OrdinalIgnoreCase))
                            .ToArray()
                        : Directory.GetFiles(modPath)
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

            string localIconPath = IsCustomTextureCategory(category)
                ? GetCustomTextureIconPath(files.FirstOrDefault(), category)
                : "";

            ModCard card = new ModCard(label, category, modName, replacesName, localIconPath)
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

        private string GetCustomTextureIconPath(string manifestPath, string category)
        {
            if (string.IsNullOrWhiteSpace(manifestPath) || !File.Exists(manifestPath))
                return "";

            string modFolder = Path.GetDirectoryName(manifestPath);
            string iconPath = Path.Combine(modFolder, "icon.png");
            if (IsSupportedIconImage(iconPath))
                return iconPath;

            try
            {
                JObject manifest = JObject.Parse(File.ReadAllText(manifestPath));
                if (category == "Custom Decals")
                {
                    JArray targets = manifest["decalTargets"] as JArray;
                    if (targets != null)
                    {
                        foreach (JObject target in targets.OfType<JObject>())
                        {
                            string targetIcon = ResolveCustomIconImage(modFolder, target["diffusePath"]?.ToString());
                            if (!string.IsNullOrWhiteSpace(targetIcon)) return targetIcon;

                            targetIcon = ResolveCustomIconImage(modFolder, target["skinPath"]?.ToString());
                            if (!string.IsNullOrWhiteSpace(targetIcon)) return targetIcon;

                            targetIcon = ResolveCustomIconImage(modFolder, target["skinPackageTexturePath"]?.ToString());
                            if (!string.IsNullOrWhiteSpace(targetIcon)) return targetIcon;
                        }
                    }

                    string found = ResolveCustomIconImage(modFolder, manifest["diffusePath"]?.ToString());
                    if (!string.IsNullOrWhiteSpace(found)) return found;

                    found = ResolveCustomIconImage(modFolder, manifest["skinPath"]?.ToString());
                    if (!string.IsNullOrWhiteSpace(found)) return found;

                    found = ResolveCustomIconImage(modFolder, manifest["skinPackageTexturePath"]?.ToString());
                    if (!string.IsNullOrWhiteSpace(found)) return found;
                }
                else if (category == "Custom Balls")
                {
                    string found = ResolveCustomIconImage(modFolder, manifest["diffusePath"]?.ToString());
                    if (!string.IsNullOrWhiteSpace(found)) return found;
                }
                else if (category == "Custom Boost Meter")
                {
                    JObject textures = manifest["textures"] as JObject;
                    foreach (string key in new[] { "Fill", "Background", "Tintable", "Glow" })
                    {
                        string found = ResolveCustomIconImage(modFolder, textures?[key]?.ToString());
                        if (!string.IsNullOrWhiteSpace(found)) return found;
                    }
                }
            }
            catch
            {
            }

            return Directory.GetFiles(modFolder, "*.*", SearchOption.AllDirectories)
                .FirstOrDefault(IsSupportedIconImage) ?? "";
        }

        private string ResolveCustomIconImage(string modFolder, string value)
        {
            string path = ResolveManifestPath(modFolder, value);
            return IsSupportedIconImage(path) ? path : "";
        }

        private static bool IsSupportedIconImage(string path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
                return false;

            string ext = Path.GetExtension(path).ToLowerInvariant();
            return ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".bmp" || ext == ".gif";
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
                    if (IsCustomTextureCategory(mod.Category))
                    {
                        string manifestPath = mod.Files.FirstOrDefault();
                        string manifestHash = FileChecker.GetFileHash(manifestPath);
                        if (savedModData[modName] == manifestHash || savedModData[modName] == "custom")
                        {
                            this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = true; });
                        }
                        else
                        {
                            this.Invoke((MethodInvoker)delegate { mod.Checkbox.Checked = false; });
                            LogToConsole($"Notice: {modName} state reset (Manifest mismatch).");
                        }
                        continue;
                    }

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
            var savedModData = Config.LoadSettings();

            var modStates = activeMods.Select(m => new
            {
                ModName = m.ModName,
                Files = m.Files,
                Category = m.Category,
                IsChecked = m.Checkbox.Checked,
                WasSavedActive = savedModData.ContainsKey(m.ModName),
                SavedHash = savedModData.ContainsKey(m.ModName) ? savedModData[m.ModName] : ""
            }).ToList();

            await Task.Run(() =>
            {
                foreach (var mod in modStates.Where(m => !m.IsChecked))
                {
                    if (!mod.WasSavedActive)
                        continue;

                    if (IsCustomTextureCategory(mod.Category))
                    {
                        if (!HandleCustomTextureMod(mod.ModName, mod.Files.FirstOrDefault(), false, mod.Category))
                            overallSuccess = false;
                        continue;
                    }

                    foreach (string file in mod.Files)
                    {
                        HandleFileOperation(file, false);
                    }
                }
                foreach (var mod in modStates.Where(m => m.IsChecked))
                {
                    if (IsCustomTextureCategory(mod.Category))
                    {
                        string manifestPath = mod.Files.FirstOrDefault();
                        string manifestHash = FileChecker.GetFileHash(manifestPath);
                        bool customModUnchanged = mod.WasSavedActive &&
                            (string.Equals(mod.SavedHash, manifestHash, StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(mod.SavedHash, "custom", StringComparison.OrdinalIgnoreCase));

                        if (customModUnchanged && manifestHash != "error" && manifestHash != "null")
                        {
                            modHashesToSave[mod.ModName] = manifestHash;
                            continue;
                        }

                        if (!HandleCustomTextureMod(mod.ModName, mod.Files.FirstOrDefault(), true, mod.Category))
                        {
                            overallSuccess = false;
                            continue;
                        }

                        if (manifestHash != "error" && manifestHash != "null")
                        {
                            modHashesToSave[mod.ModName] = manifestHash;
                        }
                        continue;
                    }

                    if (mod.WasSavedActive && IsStandardModStillInstalled(mod.Files, mod.SavedHash))
                    {
                        modHashesToSave[mod.ModName] = mod.SavedHash;
                        continue;
                    }

                    bool installed = true;
                    foreach (string file in mod.Files)
                    {
                        if (!HandleFileOperation(file, true))
                        {
                            overallSuccess = false;
                            installed = false;
                        }
                    }

                    if (!installed)
                        continue;

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

        private static bool IsCustomTextureCategory(string category)
        {
            return category == "Custom Decals" ||
                   category == "Custom Balls" ||
                   category == "Custom Boost Meter";
        }

        private bool IsStandardModStillInstalled(string[] modFiles, string savedHash)
        {
            if (modFiles == null || modFiles.Length == 0 || string.IsNullOrWhiteSpace(savedHash))
                return false;

            string firstFile = modFiles[0];
            if (string.IsNullOrWhiteSpace(firstFile) || !File.Exists(firstFile))
                return false;

            string sourceHash = FileChecker.GetFileHash(firstFile);
            if (sourceHash == "error" || sourceHash == "null" ||
                !string.Equals(sourceHash, savedHash, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            string installedFilePath = Path.Combine(gamePath, Path.GetFileName(firstFile));
            string installedHash = FileChecker.GetFileHash(installedFilePath);
            return installedHash != "error" &&
                   installedHash != "null" &&
                   string.Equals(installedHash, savedHash, StringComparison.OrdinalIgnoreCase);
        }

        private bool HandleCustomTextureMod(string modName, string manifestPath, bool isInstall, string category)
        {
            if (string.IsNullOrWhiteSpace(manifestPath) || !File.Exists(manifestPath))
            {
                this.Invoke((MethodInvoker)delegate { LogToConsole($"Custom mod manifest missing: {modName}", true); });
                return false;
            }

            string backupDir = GetCustomTextureBackupDir(manifestPath, category);
            if (!isInstall && !Directory.Exists(backupDir))
            {
                return true;
            }

            string scriptPath = GetBackendFile("alphy_custom_texture_injector.py");
            if (string.IsNullOrWhiteSpace(scriptPath) || !File.Exists(scriptPath))
            {
                this.Invoke((MethodInvoker)delegate { LogToConsole("Custom texture backend missing: alphy_custom_texture_injector.py", true); });
                return false;
            }

            try
            {
                JObject config = BuildCustomTextureConfig(manifestPath, category, isInstall, backupDir);
                string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Alphy", "CustomTextureConfigs");
                Directory.CreateDirectory(configDir);
                string configPath = Path.Combine(configDir, SafePathSegment(modName) + (isInstall ? "_apply.json" : "_restore.json"));
                File.WriteAllText(configPath, config.ToString());

                this.Invoke((MethodInvoker)delegate
                {
                    if (isInstall)
                    {
                        LogToConsole($"Custom texture apply: {modName}");
                        LogToConsole("Please, wait while we apply your mods...");
                    }
                    else
                    {
                        LogToConsole($"Custom texture restore: {modName}");
                    }
                });

                return RunPythonBackend(scriptPath, $"--config \"{configPath}\"");
            }
            catch (Exception ex)
            {
                this.Invoke((MethodInvoker)delegate { LogToConsole($"Custom texture error [{modName}]: {ex.Message}", true); });
                return false;
            }
        }

        private JObject BuildCustomTextureConfig(string manifestPath, string category, bool isInstall, string backupDir)
        {
            JObject manifest = JObject.Parse(File.ReadAllText(manifestPath));
            string modFolder = Path.GetDirectoryName(manifestPath);
            string workDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Alphy", "CustomTextureWork");

            JObject config = new JObject
            {
                ["action"] = CustomTextureAction(category, isInstall),
                ["cookedDir"] = gamePath,
                ["backupDir"] = backupDir,
                ["workDir"] = workDir
            };

            string keysPath = GetBackendFile("keys.txt");
            if (!string.IsNullOrWhiteSpace(keysPath)) config["keysPath"] = keysPath;

            string texconvPath = GetBackendFile("texconv.exe");
            if (!string.IsNullOrWhiteSpace(texconvPath)) config["texconvPath"] = texconvPath;

            string itemsPath = GetBackendFile("items.json");
            if (!string.IsNullOrWhiteSpace(itemsPath) && File.Exists(itemsPath)) config["itemsPath"] = itemsPath;

            if (!isInstall)
                return config;

            if (category == "Custom Decals")
            {
                JArray manifestTargets = manifest["decalTargets"] as JArray;
                if (manifestTargets != null && manifestTargets.Count > 0)
                {
                    JArray resolvedTargets = new JArray();
                    foreach (JObject target in manifestTargets.OfType<JObject>())
                    {
                        resolvedTargets.Add(BuildResolvedDecalTarget(modFolder, target));
                    }
                    config["decalTargets"] = resolvedTargets;
                    config["carBody"] = manifest["replaces"]?.ToString() ?? "";
                }
                else
                {
                    JObject target = BuildResolvedDecalTarget(modFolder, manifest);
                    foreach (JProperty prop in target.Properties())
                    {
                        config[prop.Name] = prop.Value.DeepClone();
                    }
                }
            }
            else if (category == "Custom Balls")
            {
                config["diffusePath"] = ResolveManifestPath(modFolder, manifest["diffusePath"]?.ToString());
            }
            else if (category == "Custom Boost Meter")
            {
                JObject textures = new JObject();
                JObject manifestTextures = manifest["textures"] as JObject ?? new JObject();
                foreach (JProperty prop in manifestTextures.Properties())
                {
                    textures[prop.Name] = ResolveManifestPath(modFolder, prop.Value?.ToString());
                }
                if (string.IsNullOrWhiteSpace(textures["Tintable"]?.ToString()) &&
                    !string.IsNullOrWhiteSpace(textures["Fill"]?.ToString()))
                {
                    textures["Tintable"] = textures["Fill"];
                }
                config["textures"] = textures;
            }

            return config;
        }

        private JObject BuildResolvedDecalTarget(string modFolder, JObject manifest)
        {
            JObject target = new JObject
            {
                ["bodyId"] = manifest["bodyId"] ?? 23,
                ["skinId"] = manifest["skinId"] ?? 0,
                ["carBody"] = manifest["carBody"]?.ToString() ?? ""
            };

            string sourcePack = manifest["sourcePack"]?.ToString();
            int bodyId = ParseManifestInt(manifest["bodyId"], 23);
            int skinId = ParseManifestInt(manifest["skinId"], 0);
            if (bodyId == -1 && target["carBody"]?.ToString().StartsWith("Body ID", StringComparison.OrdinalIgnoreCase) == true)
                target["carBody"] = "Universal Decal";
            string diffusePath = ResolveManifestPath(modFolder, manifest["diffusePath"]?.ToString());
            string skinPath = ResolveManifestPath(modFolder, manifest["skinPath"]?.ToString());
            string maskPath = ResolveManifestPath(modFolder, manifest["maskPath"]?.ToString());
            string skinPackageTexturePath = ResolveManifestPath(modFolder, manifest["skinPackageTexturePath"]?.ToString());
            string trimSheetPath = ResolveManifestPath(modFolder, manifest["trimSheetPath"]?.ToString());
            if (string.IsNullOrWhiteSpace(maskPath))
            {
                maskPath = InferAlphaConsoleTexturePath(modFolder, sourcePack,
                    "2_Diffuse_Skin_Mask", "Diffuse_Skin_Mask", "Skin_Mask", "Mask");
            }
            if (string.IsNullOrWhiteSpace(trimSheetPath))
            {
                trimSheetPath = InferAlphaConsoleTexturePath(modFolder, sourcePack,
                    "TrimSheet", "Trim_Sheet", "Trim", "Logo");
            }
            if (string.IsNullOrWhiteSpace(skinPackageTexturePath))
                skinPackageTexturePath = InferAlphaConsoleSkinOnlyTexturePath(modFolder, sourcePack);

            if (bodyId == -1 && skinId > 0)
            {
                if (string.IsNullOrWhiteSpace(skinPackageTexturePath))
                    skinPackageTexturePath = !string.IsNullOrWhiteSpace(skinPath) ? skinPath : diffusePath;
                diffusePath = "";
                skinPath = "";
                maskPath = "";
            }

            if (!string.IsNullOrWhiteSpace(skinPackageTexturePath) && string.IsNullOrWhiteSpace(maskPath))
            {
                diffusePath = "";
                skinPath = "";
            }
            if (string.IsNullOrWhiteSpace(diffusePath) && !string.IsNullOrWhiteSpace(skinPath))
                diffusePath = skinPath;

            target["diffusePath"] = diffusePath;
            target["skinPath"] = skinPath;
            target["maskPath"] = maskPath;
            target["skinPackageTexturePath"] = skinPackageTexturePath;
            target["trimSheetPath"] = trimSheetPath;
            target["chassisDiffusePath"] = ResolveManifestPath(modFolder, manifest["chassisDiffusePath"]?.ToString());
            return target;
        }

        private static int ParseManifestInt(JToken token, int fallback)
        {
            if (token == null) return fallback;
            return int.TryParse(token.ToString(), out int parsed) ? parsed : fallback;
        }

        private string InferAlphaConsoleSkinOnlyTexturePath(string modFolder, string sourcePack)
        {
            if (!Directory.Exists(modFolder)) return "";

            foreach (string jsonFile in Directory.GetFiles(modFolder, "*.json", SearchOption.AllDirectories))
            {
                JObject root;
                try
                {
                    root = JObject.Parse(File.ReadAllText(jsonFile));
                }
                catch
                {
                    continue;
                }

                string jsonDir = Path.GetDirectoryName(jsonFile);
                foreach (JObject entry in AlphaConsoleEntries(root, sourcePack))
                {
                    JObject body = entry["Body"] as JObject ?? entry;
                    string skin = ResolveAlphaConsoleTexturePath(modFolder, jsonDir, body, "Skin");
                    if (string.IsNullOrWhiteSpace(skin))
                        continue;

                    bool hasOtherBodyTexture =
                        !string.IsNullOrWhiteSpace(ValueForKey(body, "Diffuse", "Body_D", "D",
                            "1_Diffuse_Skin", "Diffuse_Skin",
                            "2_Diffuse_Skin_Mask", "Diffuse_Skin_Mask", "Skin_Mask", "Mask"));
                    if (!hasOtherBodyTexture)
                        return skin;
                }
            }

            return "";
        }

        private string InferAlphaConsoleTexturePath(string modFolder, string sourcePack, params string[] keys)
        {
            if (!Directory.Exists(modFolder)) return "";

            foreach (string jsonFile in Directory.GetFiles(modFolder, "*.json", SearchOption.AllDirectories))
            {
                JObject root;
                try
                {
                    root = JObject.Parse(File.ReadAllText(jsonFile));
                }
                catch
                {
                    continue;
                }

                string jsonDir = Path.GetDirectoryName(jsonFile);
                foreach (JObject entry in AlphaConsoleEntries(root, sourcePack))
                {
                    JObject body = entry["Body"] as JObject ?? entry;
                    string resolved = ResolveAlphaConsoleTexturePath(modFolder, jsonDir, body, keys);
                    if (!string.IsNullOrWhiteSpace(resolved))
                        return resolved;
                }
            }

            return "";
        }

        private IEnumerable<JObject> AlphaConsoleEntries(JObject root, string sourcePack)
        {
            if (root == null) yield break;

            if (!string.IsNullOrWhiteSpace(sourcePack) && root[sourcePack] is JObject namedEntry)
            {
                yield return namedEntry;
                yield break;
            }

            if (root["Body"] is JObject)
            {
                yield return root;
                yield break;
            }

            foreach (JProperty prop in root.Properties())
            {
                if (prop.Value is JObject obj)
                    yield return obj;
            }
        }

        private string ResolveAlphaConsoleTexturePath(string modFolder, string jsonDir, JObject map, params string[] keys)
        {
            foreach (string key in keys)
            {
                string value = ValueForKey(map, key);
                string resolved = ResolveAlphaConsoleTextureValue(modFolder, jsonDir, value);
                if (!string.IsNullOrWhiteSpace(resolved))
                    return resolved;
            }
            return "";
        }

        private string ResolveAlphaConsoleTextureValue(string modFolder, string jsonDir, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";

            string normalized = value.Replace('/', Path.DirectorySeparatorChar);
            string candidate = Path.Combine(jsonDir ?? modFolder, normalized);
            if (File.Exists(candidate))
                return candidate;

            candidate = Path.Combine(modFolder, normalized);
            if (File.Exists(candidate))
                return candidate;

            string fileName = Path.GetFileName(normalized);
            if (string.IsNullOrWhiteSpace(fileName)) return "";

            string found = Directory.GetFiles(modFolder, fileName, SearchOption.AllDirectories).FirstOrDefault();
            return found ?? "";
        }

        private static string ValueForKey(JObject obj, params string[] names)
        {
            if (obj == null) return "";
            foreach (string name in names)
            {
                JToken token = obj[name];
                if (token != null && token.Type != JTokenType.Object && token.Type != JTokenType.Array)
                    return token.ToString();
            }
            return "";
        }

        private string CustomTextureAction(string category, bool isInstall)
        {
            if (category == "Custom Decals") return isInstall ? "apply" : "restore";
            if (category == "Custom Balls") return isInstall ? "apply_ball" : "restore_ball";
            if (category == "Custom Boost Meter") return isInstall ? "apply_boost_meter" : "restore_boost_meter";
            throw new InvalidOperationException($"Unsupported custom texture category: {category}");
        }

        private string ResolveManifestPath(string modFolder, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            return Path.IsPathRooted(value) ? value : Path.Combine(modFolder, value);
        }

        private string GetCustomTextureBackupDir(string manifestPath, string category)
        {
            string modFolderName = Path.GetFileName(Path.GetDirectoryName(manifestPath));
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
                            {
                                string line = FormatPythonBackendLog(args.Data);
                                this.Invoke((MethodInvoker)delegate { LogToConsole(line); });
                            }
                        };
                        process.ErrorDataReceived += (sender, args) =>
                        {
                            if (!ShouldSuppressPythonBackendLog(args.Data))
                            {
                                string line = FormatPythonBackendLog(args.Data);
                                this.Invoke((MethodInvoker)delegate { LogToConsole(line, true); });
                            }
                        };

                        process.Start();
                        process.BeginOutputReadLine();
                        process.BeginErrorReadLine();
                        process.WaitForExit();

                        if (process.ExitCode != 0)
                        {
                            this.Invoke((MethodInvoker)delegate
                            {
                                LogToConsole($"Custom texture backend exited with code {process.ExitCode}.", true);
                            });
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

            this.Invoke((MethodInvoker)delegate
            {
                LogToConsole($"Could not launch Python. Install Python or add it to PATH. {launchError?.Message}", true);
            });
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
            var filesToBackup = activeMods
                .Where(m => !IsCustomTextureCategory(m.Category))
                .SelectMany(m => m.Files)
                .Distinct()
                .ToList();

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
