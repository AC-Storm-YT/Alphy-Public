using Alphy.Properties;
using MaterialSkin;
using MaterialSkin.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Alphy
{
    public partial class FormAddMod : MaterialForm
    {
        private List<string> selectedFilePaths = new List<string>();

        private class AlphaConsolePackItem
        {
            public string Name { get; set; }
            public string JsonFile { get; set; }
            public JObject Entry { get; set; }
        }

        private readonly string[] standardCategories = {
            "Antenna", "Banner", "Body", "Boost", "Boost Audio",
            "Decal", "Goal Explosion", "Hat", "Paint", "Trail", "Wheels", "Engine Audio",
            "Avatar Border"
        };

        private readonly string[] acCategories = {
            "Custom Decals", "Custom Balls", "Custom Boost Meter"
        };

        public FormAddMod()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            cmbCategory.Items.AddRange(standardCategories);
        }

        private void Log(string message)
        {
            if (txtConsole.InvokeRequired)
            {
                txtConsole.Invoke(new Action(() => Log(message)));
                return;
            }
            txtConsole.AppendText(message + "\n");
            txtConsole.ScrollToCaret();
        }

        private void btnSelectMod_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = false;
                ofd.Filter = "Mod Files (*.upk;*.bnk;*.zip)|*.upk;*.bnk;*.zip|All Files (*.*)|*.*";
                ofd.Title = "Select Mod File";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePaths.Clear();
                    selectedFilePaths.Add(ofd.FileName);
                    Log($"Selected file: {Path.GetFileName(ofd.FileName)}");

                    if (Path.GetExtension(ofd.FileName).ToLower() == ".zip")
                    {
                        cmbCategory.Items.Clear();
                        cmbCategory.Items.AddRange(acCategories);
                        ApplyAlphaConsoleZipDefaults(ofd.FileName, true);
                        Log("System: AlphaConsole format detected. Categories restricted to AC formats.");
                    }
                }
            }
        }

        private async void btnAddMod_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModName.Text)) { Log("Error: Enter Mod Name."); return; }
            if (cmbCategory.SelectedIndex == -1) { Log("Error: Select Category."); return; }
            if (selectedFilePaths.Count == 0) { Log("Error: Select file."); return; }

            try
            {
                string sourceFile = selectedFilePaths[0];
                if (Path.GetExtension(sourceFile).ToLower() == ".zip")
                    ApplyAlphaConsoleZipDefaults(sourceFile, false);

                string category = cmbCategory.SelectedItem.ToString();
                if (Path.GetExtension(sourceFile).ToLower() == ".zip")
                {
                    await ProcessAlphaConsoleZipAsync(sourceFile, category);
                }
                else
                {
                    string targetFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods", category, BuildModFolderLabel());
                    Directory.CreateDirectory(targetFolder);
                    File.Copy(sourceFile, Path.Combine(targetFolder, Path.GetFileName(sourceFile)), true);
                }

                Log("SUCCESS! Mod imported.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex) { Log($"FATAL ERROR: {ex.Message}"); }
        }

        private Task ProcessAlphaConsoleZipAsync(string zipPath, string selectedCategory)
        {
            Log("System: Processing AlphaConsole zip...");
            string extractPath = Path.Combine(Path.GetTempPath(), "AlphyForge_" + Guid.NewGuid());

            try
            {
                ZipFile.ExtractToDirectory(zipPath, extractPath);
                List<AlphaConsolePackItem> items = ReadAlphaConsolePackItems(extractPath, zipPath);
                if (TryProcessMultiBodyDecalPack(zipPath, selectedCategory, extractPath, items))
                {
                    Log("System: Multi-body custom decal manifest created.");
                    Log("System: This mod will be applied when you tick it and press Save.");
                    return Task.CompletedTask;
                }

                bool isBundle = items.Count > 1;
                int created = 0;

                foreach (AlphaConsolePackItem item in items)
                {
                    string category = DetectAlphaConsoleCategory(item.Entry);
                    if (!acCategories.Contains(category))
                        category = selectedCategory;

                    string replaces = DefaultReplacementForAlphaConsole(category, item.Entry);
                    string modName = isBundle ? item.Name : txtModName.Text.Trim();
                    string targetFolder = Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "mods",
                        category,
                        BuildModFolderLabel(modName, replaces));

                    Directory.CreateDirectory(targetFolder);

                    JObject manifest = BuildCustomManifest(zipPath, extractPath, item.JsonFile, item.Entry,
                        category, item.Name, modName, replaces);
                    CopyDirectory(extractPath, targetFolder);
                    CreateCustomModIcon(extractPath, targetFolder, category, manifest);

                    string manifestPath = Path.Combine(targetFolder, "alphy_custom_mod.alphycustom");
                    File.WriteAllText(manifestPath, manifest.ToString());
                    created++;
                }

                Log(isBundle
                    ? $"System: Custom texture pack split into {created} mods."
                    : "System: Custom texture manifest created.");
                Log("System: This mod will be applied when you tick it and press Save.");
                return Task.CompletedTask;
            }
            finally
            {
                if (Directory.Exists(extractPath))
                    Directory.Delete(extractPath, true);
            }
        }

        private JObject BuildCustomManifest(string zipPath,
                                            string extractPath,
                                            string jsonFile,
                                            JObject acEntry,
                                            string category,
                                            string packName,
                                            string modName = null,
                                            string replaces = null)
        {
            JObject body = acEntry["Body"] as JObject ?? acEntry;
            JObject textureMap = TextureMapForCustomTexture(acEntry, body);
            JObject manifest = new JObject
            {
                ["type"] = "alphy-custom-texture",
                ["version"] = 1,
                ["category"] = category,
                ["name"] = modName ?? txtModName.Text.Trim(),
                ["replaces"] = replaces ?? txtReplaces.Text.Trim(),
                ["sourceZip"] = Path.GetFileName(zipPath),
                ["sourcePack"] = packName
            };

            string jsonDir = jsonFile != null ? Path.GetDirectoryName(jsonFile) : extractPath;

            if (category == "Custom Decals")
            {
                string bodyId = ValueForKey(acEntry, "BodyID", "BodyId", "Body");
                string skinId = ValueForKey(acEntry, "SkinID", "SkinId", "Skin");
                string bodyDiffusePath = ResolveRelativeImage(extractPath, jsonDir, body,
                    "Diffuse", "Body_D", "D");
                string decalDiffusePath = ResolveRelativeImage(extractPath, jsonDir, body,
                    "1_Diffuse_Skin", "Diffuse_Skin");
                string skinPath = ResolveRelativeImage(extractPath, jsonDir, body,
                    "Skin");
                string maskPath = ResolveRelativeImage(extractPath, jsonDir, body,
                    "2_Diffuse_Skin_Mask", "Diffuse_Skin_Mask", "Skin_Mask", "Mask");
                string trimSheetPath = ResolveRelativeImage(extractPath, jsonDir, body,
                    "TrimSheet", "Trim_Sheet", "Trim", "Logo");
                int parsedBodyId = ParseIntOrDefault(bodyId, 23);
                int parsedSkinId = ParseIntOrDefault(skinId, 0);
                bool universalSkinPackage = parsedBodyId == -1 && parsedSkinId != 0;
                bool skinPackageOnly = parsedSkinId != 0 &&
                                       !string.IsNullOrWhiteSpace(skinPath) &&
                                       string.IsNullOrWhiteSpace(bodyDiffusePath) &&
                                       string.IsNullOrWhiteSpace(decalDiffusePath) &&
                                       string.IsNullOrWhiteSpace(maskPath);

                if (string.IsNullOrWhiteSpace(bodyDiffusePath))
                    bodyDiffusePath = decalDiffusePath;
                if (string.IsNullOrWhiteSpace(decalDiffusePath))
                    decalDiffusePath = skinPath;

                string skinPackageTexturePath = skinPath;
                if (universalSkinPackage)
                    skinPackageTexturePath = bodyDiffusePath;

                manifest["bodyId"] = parsedBodyId;
                manifest["skinId"] = parsedSkinId;
                manifest["carBody"] = BodyLabel(bodyId);
                manifest["diffusePath"] = (skinPackageOnly || universalSkinPackage) ? "" : bodyDiffusePath;
                manifest["skinPath"] = (skinPackageOnly || universalSkinPackage) ? "" : decalDiffusePath;
                manifest["maskPath"] = maskPath;
                manifest["skinPackageTexturePath"] = (skinPackageOnly || universalSkinPackage) ? skinPackageTexturePath : "";
                manifest["trimSheetPath"] = trimSheetPath;
                string chassisDiffusePath = ResolveRelativeImage(extractPath, jsonDir, body,
                    "Chassis.Diffuse", "Chassis_Diffuse", "ChassisDiffuse", "Chassis");
                JObject chassis = acEntry["Chassis"] as JObject ?? acEntry["dom_Chassis"] as JObject;
                if (string.IsNullOrWhiteSpace(chassisDiffusePath) && chassis != null)
                    chassisDiffusePath = ResolveRelativeImage(extractPath, jsonDir, chassis,
                        "Diffuse", "Chassis.Diffuse", "Chassis_Diffuse", "ChassisDiffuse");
                manifest["chassisDiffusePath"] = chassisDiffusePath;

                if (string.IsNullOrWhiteSpace(replaces) && string.IsNullOrWhiteSpace(txtReplaces.Text))
                {
                    txtReplaces.Text = manifest["carBody"]?.ToString();
                    manifest["replaces"] = txtReplaces.Text.Trim();
                }
            }
            else if (category == "Custom Balls")
            {
                manifest["diffusePath"] = ResolveRelativeImage(extractPath, jsonDir, textureMap,
                    "Diffuse", "Ball", "Ball_D", "Image", "Texture");
                if (string.IsNullOrWhiteSpace(manifest["diffusePath"]?.ToString()))
                    manifest["diffusePath"] = FirstImageRelativePath(extractPath);

                if (string.IsNullOrWhiteSpace(replaces) && string.IsNullOrWhiteSpace(txtReplaces.Text))
                {
                    txtReplaces.Text = "Default Ball";
                    manifest["replaces"] = txtReplaces.Text.Trim();
                }
            }
            else if (category == "Custom Boost Meter")
            {
                JObject textures = new JObject
                {
                    ["Background"] = ResolveRelativeImage(extractPath, jsonDir, textureMap,
                        "Background", "BoostMeter_Background"),
                    ["Fill"] = ResolveRelativeImage(extractPath, jsonDir, textureMap,
                        "Fill", "BoostMeter_Fill"),
                    ["Tintable"] = ResolveRelativeImage(extractPath, jsonDir, textureMap,
                        "Tintable", "Tint", "BoostMeter_FillTintablePortion"),
                    ["Glow"] = ResolveRelativeImage(extractPath, jsonDir, textureMap,
                        "Glow", "BoostMeter_Glow")
                };
                if (string.IsNullOrWhiteSpace(textures["Tintable"]?.ToString()) &&
                    !string.IsNullOrWhiteSpace(textures["Fill"]?.ToString()))
                {
                    textures["Tintable"] = textures["Fill"];
                }
                manifest["textures"] = textures;

                if (string.IsNullOrWhiteSpace(replaces) && string.IsNullOrWhiteSpace(txtReplaces.Text))
                {
                    txtReplaces.Text = "Boost Meter";
                    manifest["replaces"] = txtReplaces.Text.Trim();
                }
            }

            return manifest;
        }

        private List<AlphaConsolePackItem> ReadAlphaConsolePackItems(string extractPath, string zipPath)
        {
            List<AlphaConsolePackItem> items = new List<AlphaConsolePackItem>();
            foreach (string jsonFile in Directory.GetFiles(extractPath, "*.json", SearchOption.AllDirectories))
            {
                JObject acData;
                try
                {
                    acData = JObject.Parse(File.ReadAllText(jsonFile));
                }
                catch
                {
                    continue;
                }

                if (IsDirectAlphaConsoleEntry(acData))
                {
                    items.Add(new AlphaConsolePackItem
                    {
                        Name = Path.GetFileNameWithoutExtension(jsonFile),
                        JsonFile = jsonFile,
                        Entry = acData
                    });
                    continue;
                }

                foreach (JProperty prop in acData.Properties())
                {
                    if (prop.Value is JObject obj)
                    {
                        items.Add(new AlphaConsolePackItem
                        {
                            Name = prop.Name,
                            JsonFile = jsonFile,
                            Entry = obj
                        });
                    }
                }
            }

            if (items.Count == 0)
            {
                items.Add(new AlphaConsolePackItem
                {
                    Name = Path.GetFileNameWithoutExtension(zipPath),
                    JsonFile = null,
                    Entry = new JObject()
                });
            }

            return items;
        }

        private bool IsDirectAlphaConsoleEntry(JObject obj)
        {
            JObject body = obj["Body"] as JObject ?? obj;
            return obj["Params"] is JObject ||
                   obj["Body"] is JObject ||
                   HasAnyKey(obj, "BodyID", "BodyId", "SkinID", "SkinId") ||
                   HasAnyKey(body, "Background", "BoostMeter_Background", "Fill",
                       "BoostMeter_Fill", "Tintable", "Tint", "BoostMeter_FillTintablePortion",
                       "Glow", "BoostMeter_Glow", "Ball", "Ball_D", "Image", "Texture",
                       "Diffuse", "Normal", "1_Diffuse_Skin", "2_Diffuse_Skin_Mask",
                       "Diffuse_Skin", "Diffuse_Skin_Mask", "Skin_Mask");
        }

        private void ApplyAlphaConsoleZipDefaults(string zipPath, bool overwriteReplaces)
        {
            string category = "Custom Decals";
            string replaces = "Octane";

            if (TryReadMultiBodyReplacementLabelFromZip(zipPath, out string multiBodyReplaces))
            {
                category = "Custom Decals";
                replaces = multiBodyReplaces;
            }
            else if (TryReadAlphaConsoleZip(zipPath, out JObject acEntry))
            {
                category = DetectAlphaConsoleCategory(acEntry);
                replaces = DefaultReplacementForAlphaConsole(category, acEntry);
            }

            if (cmbCategory.Items.Contains(category))
                cmbCategory.SelectedItem = category;
            else
                cmbCategory.SelectedItem = "Custom Decals";

            if (overwriteReplaces || string.IsNullOrWhiteSpace(txtReplaces.Text))
                txtReplaces.Text = replaces;
        }

        private bool TryReadMultiBodyReplacementLabelFromZip(string zipPath, out string replaces)
        {
            replaces = "";
            try
            {
                List<AlphaConsolePackItem> items = new List<AlphaConsolePackItem>();
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    foreach (ZipArchiveEntry jsonEntry in archive.Entries
                                 .Where(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)))
                    {
                        JObject acData;
                        using (StreamReader reader = new StreamReader(jsonEntry.Open()))
                        {
                            acData = JObject.Parse(reader.ReadToEnd());
                        }

                        if (IsDirectAlphaConsoleEntry(acData))
                        {
                            items.Add(new AlphaConsolePackItem
                            {
                                Name = Path.GetFileNameWithoutExtension(jsonEntry.FullName),
                                JsonFile = jsonEntry.FullName,
                                Entry = acData
                            });
                            continue;
                        }

                        foreach (JProperty prop in acData.Properties())
                        {
                            if (prop.Value is JObject obj)
                            {
                                items.Add(new AlphaConsolePackItem
                                {
                                    Name = prop.Name,
                                    JsonFile = jsonEntry.FullName,
                                    Entry = obj
                                });
                            }
                        }
                    }
                }

                if (items.Count < 2 || items.Any(item => DetectAlphaConsoleCategory(item.Entry) != "Custom Decals"))
                    return false;

                List<AlphaConsolePackItem> selectedTargets = SelectOneDecalPerBody(items);
                if (selectedTargets.Count < 2)
                    return false;

                replaces = MultiBodyReplacementLabel(selectedTargets);
                return !string.IsNullOrWhiteSpace(replaces);
            }
            catch
            {
                return false;
            }
        }

        private bool TryReadAlphaConsoleZip(string zipPath, out JObject acEntry)
        {
            acEntry = new JObject();
            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    ZipArchiveEntry jsonEntry = archive.Entries
                        .FirstOrDefault(e => e.FullName.EndsWith(".json", StringComparison.OrdinalIgnoreCase));
                    if (jsonEntry == null) return false;

                    using (StreamReader reader = new StreamReader(jsonEntry.Open()))
                    {
                        JObject acData = JObject.Parse(reader.ReadToEnd());
                        JProperty first = acData.Properties().FirstOrDefault();
                        if (first != null && first.Value is JObject obj && acData["Body"] == null)
                            acEntry = obj;
                        else
                            acEntry = acData;
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        private bool TryProcessMultiBodyDecalPack(string zipPath,
                                                  string selectedCategory,
                                                  string extractPath,
                                                  List<AlphaConsolePackItem> items)
        {
            if (items == null || items.Count < 2)
                return false;

            List<AlphaConsolePackItem> decalItems = items
                .Where(item => DetectAlphaConsoleCategory(item.Entry) == "Custom Decals")
                .ToList();
            if (decalItems.Count != items.Count)
                return false;

            List<AlphaConsolePackItem> selectedTargets = SelectOneDecalPerBody(decalItems);
            if (selectedTargets.Count < 2)
                return false;

            string category = selectedCategory == "Custom Decals" ? selectedCategory : "Custom Decals";
            string replaces = MultiBodyReplacementLabel(selectedTargets);
            string modName = txtModName.Text.Trim();
            if (string.IsNullOrWhiteSpace(modName))
                modName = Path.GetFileNameWithoutExtension(zipPath);

            string targetFolder = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "mods",
                category,
                BuildModFolderLabel(modName, replaces));
            Directory.CreateDirectory(targetFolder);

            JObject manifest = BuildMultiBodyDecalManifest(zipPath, extractPath, selectedTargets, modName, replaces);
            CopyDirectory(extractPath, targetFolder);
            CreateCustomModIcon(extractPath, targetFolder, category, manifest);

            string manifestPath = Path.Combine(targetFolder, "alphy_custom_mod.alphycustom");
            File.WriteAllText(manifestPath, manifest.ToString());
            return true;
        }

        private JObject BuildMultiBodyDecalManifest(string zipPath,
                                                    string extractPath,
                                                    List<AlphaConsolePackItem> selectedTargets,
                                                    string modName,
                                                    string replaces)
        {
            JObject manifest = new JObject
            {
                ["type"] = "alphy-custom-texture",
                ["version"] = 1,
                ["category"] = "Custom Decals",
                ["name"] = modName,
                ["replaces"] = replaces,
                ["sourceZip"] = Path.GetFileName(zipPath),
                ["sourcePack"] = string.Join(" + ", selectedTargets.Select(item => item.Name))
            };

            JArray targets = new JArray();
            foreach (AlphaConsolePackItem item in selectedTargets)
            {
                string bodyLabel = BodyLabel(BodyIdForEntry(item.Entry).ToString());
                JObject single = BuildCustomManifest(zipPath, extractPath, item.JsonFile, item.Entry,
                    "Custom Decals", item.Name, item.Name, bodyLabel);
                JObject target = new JObject
                {
                    ["sourcePack"] = item.Name
                };
                CopyDecalFields(single, target);
                targets.Add(target);
            }

            manifest["decalTargets"] = targets;
            JObject firstTarget = targets.First as JObject;
            if (firstTarget != null)
                CopyDecalFields(firstTarget, manifest);

            return manifest;
        }

        private List<AlphaConsolePackItem> SelectOneDecalPerBody(IEnumerable<AlphaConsolePackItem> items)
        {
            Dictionary<string, AlphaConsolePackItem> selected =
                new Dictionary<string, AlphaConsolePackItem>(StringComparer.OrdinalIgnoreCase);

            foreach (IGrouping<string, AlphaConsolePackItem> group in items
                         .Where(item => IsSupportedMultiBodyLabel(BodyLabel(BodyIdForEntry(item.Entry).ToString())))
                         .GroupBy(item => BodyLabel(BodyIdForEntry(item.Entry).ToString()), StringComparer.OrdinalIgnoreCase))
            {
                selected[group.Key] = group
                    .OrderBy(DecalVariantScore)
                    .ThenBy(item => item.Name ?? "")
                    .First();
            }

            string[] preferredOrder = { "Fennec", "Dominus", "Octane" };
            return preferredOrder
                .Where(label => selected.ContainsKey(label))
                .Select(label => selected[label])
                .ToList();
        }

        private static int DecalVariantScore(AlphaConsolePackItem item)
        {
            string name = (item.Name ?? "").Trim();
            string file = Path.GetFileNameWithoutExtension(item.JsonFile ?? "").Trim();
            int score = name.Length + file.Length;
            if (EndsWithDigit(name)) score += 1000;
            if (EndsWithDigit(file)) score += 1000;
            return score;
        }

        private static bool EndsWithDigit(string text)
        {
            return !string.IsNullOrWhiteSpace(text) && char.IsDigit(text[text.Length - 1]);
        }

        private static bool IsSupportedMultiBodyLabel(string label)
        {
            return label == "Fennec" || label == "Dominus" || label == "Octane";
        }

        private string MultiBodyReplacementLabel(IEnumerable<AlphaConsolePackItem> items)
        {
            HashSet<string> labels = new HashSet<string>(
                items.Select(item => BodyLabel(BodyIdForEntry(item.Entry).ToString())),
                StringComparer.OrdinalIgnoreCase);
            string[] preferredOrder = { "Fennec", "Dominus", "Octane" };
            return string.Join(" ", preferredOrder.Where(label => labels.Contains(label)));
        }

        private static void CopyDecalFields(JObject source, JObject destination)
        {
            foreach (string key in new[]
            {
                "bodyId", "skinId", "carBody", "diffusePath", "skinPath",
                "maskPath", "skinPackageTexturePath", "trimSheetPath", "chassisDiffusePath", "sourcePack"
            })
            {
                if (source[key] != null)
                    destination[key] = source[key].DeepClone();
            }
        }

        private string DetectAlphaConsoleCategory(JObject acEntry)
        {
            JObject body = acEntry["Body"] as JObject ?? acEntry;
            JObject textureMap = TextureMapForCustomTexture(acEntry, body);

            if (HasAnyKey(textureMap, "Background", "BoostMeter_Background", "Fill",
                "BoostMeter_Fill", "Tintable", "Tint", "BoostMeter_FillTintablePortion",
                "Glow", "BoostMeter_Glow"))
            {
                return "Custom Boost Meter";
            }

            if (HasAnyKey(acEntry, "BodyID", "BodyId", "SkinID", "SkinId") ||
                HasAnyKey(body, "1_Diffuse_Skin", "2_Diffuse_Skin_Mask",
                    "Diffuse_Skin", "Diffuse_Skin_Mask", "Skin_Mask",
                    "Chassis.Diffuse", "Chassis_Diffuse", "ChassisDiffuse"))
            {
                return "Custom Decals";
            }

            if (HasAnyKey(textureMap, "Ball", "Ball_D", "Image", "Texture", "Diffuse", "Normal"))
                return "Custom Balls";

            return "Custom Decals";
        }

        private JObject TextureMapForCustomTexture(JObject acEntry, JObject body)
        {
            return acEntry["Params"] as JObject ??
                   body?["Params"] as JObject ??
                   body ??
                   acEntry;
        }

        private string DefaultReplacementForAlphaConsole(string category, JObject acEntry)
        {
            if (category == "Custom Decals")
            {
                JObject body = acEntry["Body"] as JObject ?? acEntry;
                string bodyId = ValueForKey(acEntry, "BodyID", "BodyId", "Body");
                if (string.IsNullOrWhiteSpace(bodyId))
                    bodyId = ValueForKey(body, "BodyID", "BodyId", "Body");
                return BodyLabel(bodyId);
            }

            if (category == "Custom Balls")
                return "Default Ball";

            if (category == "Custom Boost Meter")
                return "Boost Meter";

            return "";
        }

        private static bool HasAnyKey(JObject obj, params string[] names)
        {
            if (obj == null) return false;
            return names.Any(name => obj[name] != null);
        }

        private void CreateCustomModIcon(string extractPath, string targetFolder, string category, JObject manifest)
        {
            string source = FindCustomIconSource(extractPath, category, manifest);
            if (string.IsNullOrWhiteSpace(source) || !File.Exists(source))
                return;

            string destination = Path.Combine(targetFolder, "icon.png");
            try
            {
                if (Path.GetExtension(source).Equals(".png", StringComparison.OrdinalIgnoreCase))
                {
                    File.Copy(source, destination, true);
                    return;
                }

                using (Image image = Image.FromFile(source))
                {
                    image.Save(destination, ImageFormat.Png);
                }
            }
            catch
            {
            }
        }

        private string FindCustomIconSource(string extractPath, string category, JObject manifest)
        {
            if (category == "Custom Decals")
            {
                JArray targets = manifest["decalTargets"] as JArray;
                if (targets != null)
                {
                    foreach (JObject target in targets.OfType<JObject>())
                    {
                        string targetIcon = ManifestImagePath(extractPath, target["diffusePath"]?.ToString());
                        if (!string.IsNullOrWhiteSpace(targetIcon)) return targetIcon;

                        targetIcon = ManifestImagePath(extractPath, target["skinPath"]?.ToString());
                        if (!string.IsNullOrWhiteSpace(targetIcon)) return targetIcon;

                        targetIcon = ManifestImagePath(extractPath, target["skinPackageTexturePath"]?.ToString());
                        if (!string.IsNullOrWhiteSpace(targetIcon)) return targetIcon;
                    }
                }

                string found = ManifestImagePath(extractPath, manifest["diffusePath"]?.ToString());
                if (!string.IsNullOrWhiteSpace(found)) return found;

                found = ManifestImagePath(extractPath, manifest["skinPath"]?.ToString());
                if (!string.IsNullOrWhiteSpace(found)) return found;

                found = ManifestImagePath(extractPath, manifest["skinPackageTexturePath"]?.ToString());
                if (!string.IsNullOrWhiteSpace(found)) return found;
            }
            else if (category == "Custom Balls")
            {
                string found = ManifestImagePath(extractPath, manifest["diffusePath"]?.ToString());
                if (!string.IsNullOrWhiteSpace(found)) return found;
            }
            else if (category == "Custom Boost Meter")
            {
                JObject textures = manifest["textures"] as JObject;
                foreach (string key in new[] { "Fill", "Background", "Tintable", "Glow" })
                {
                    string found = ManifestImagePath(extractPath, textures?[key]?.ToString());
                    if (!string.IsNullOrWhiteSpace(found)) return found;
                }
            }

            string fallback = FirstImageRelativePath(extractPath);
            return string.IsNullOrWhiteSpace(fallback) ? "" : Path.Combine(extractPath, fallback);
        }

        private static string ManifestImagePath(string basePath, string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return "";

            string path = Path.IsPathRooted(relativePath)
                ? relativePath
                : Path.Combine(basePath, relativePath);
            return File.Exists(path) ? path : "";
        }

        private string BuildModFolderLabel()
        {
            return BuildModFolderLabel(txtModName.Text.Trim(), txtReplaces.Text.Trim());
        }

        private string BuildModFolderLabel(string name, string replaces)
        {
            string label = string.IsNullOrWhiteSpace(replaces) ? name : $"{name} (Replaces {replaces})";
            return SafePathSegment(label);
        }

        private static string SafePathSegment(string text)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                text = text.Replace(c, '_');
            return text.Trim();
        }

        private static int ParseIntOrDefault(string value, int fallback)
        {
            return int.TryParse(value, out int parsed) ? parsed : fallback;
        }

        private static string BodyLabel(string bodyId)
        {
            switch (ParseIntOrDefault(bodyId, -1))
            {
                case -1: return "Universal Decal";
                case 23: return "Octane";
                case 4284: return "Fennec";
                case 403:
                case 4770: return "Dominus";
                default: return string.IsNullOrWhiteSpace(bodyId) ? "Custom Body" : $"Body ID {bodyId}";
            }
        }

        private static int BodyIdForEntry(JObject acEntry)
        {
            JObject body = acEntry?["Body"] as JObject ?? acEntry;
            string bodyId = ValueForKey(acEntry, "BodyID", "BodyId", "Body");
            if (string.IsNullOrWhiteSpace(bodyId))
                bodyId = ValueForKey(body, "BodyID", "BodyId", "Body");
            return ParseIntOrDefault(bodyId, -1);
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

        private string ResolveRelativeImage(string extractPath, string jsonDir, JObject map, params string[] keys)
        {
            foreach (string key in keys)
            {
                string value = ValueForKey(map, key);
                string resolved = ResolveRelativeImageValue(extractPath, jsonDir, value);
                if (!string.IsNullOrWhiteSpace(resolved))
                    return resolved;
            }
            return "";
        }

        private string ResolveRelativeImageValue(string extractPath, string jsonDir, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";

            string normalized = value.Replace('/', Path.DirectorySeparatorChar);
            string candidate = Path.Combine(jsonDir ?? extractPath, normalized);
            if (File.Exists(candidate))
                return GetRelativePath(extractPath, candidate);

            candidate = Path.Combine(extractPath, normalized);
            if (File.Exists(candidate))
                return GetRelativePath(extractPath, candidate);

            string fileName = Path.GetFileName(normalized);
            if (string.IsNullOrWhiteSpace(fileName)) return "";

            string found = Directory.GetFiles(extractPath, fileName, SearchOption.AllDirectories).FirstOrDefault();
            return found == null ? "" : GetRelativePath(extractPath, found);
        }

        private static string FirstImageRelativePath(string extractPath)
        {
            string[] extensions = { ".png", ".jpg", ".jpeg", ".bmp", ".tga" };
            string found = Directory.GetFiles(extractPath, "*.*", SearchOption.AllDirectories)
                .FirstOrDefault(f => extensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
            return found == null ? "" : GetRelativePath(extractPath, found);
        }

        private static string GetRelativePath(string baseDir, string filePath)
        {
            if (!baseDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
                baseDir += Path.DirectorySeparatorChar;
            Uri baseUri = new Uri(baseDir);
            Uri fileUri = new Uri(filePath);
            return Uri.UnescapeDataString(baseUri.MakeRelativeUri(fileUri).ToString())
                .Replace('/', Path.DirectorySeparatorChar);
        }

        private static void CopyDirectory(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            foreach (string dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(Path.Combine(targetDir, GetRelativePath(sourceDir, dir)));
            }
            foreach (string file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
            {
                string dest = Path.Combine(targetDir, GetRelativePath(sourceDir, file));
                Directory.CreateDirectory(Path.GetDirectoryName(dest));
                File.Copy(file, dest, true);
            }
        }
    }
}
