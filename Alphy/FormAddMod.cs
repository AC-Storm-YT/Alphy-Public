using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;

namespace Alphy
{
    public partial class FormAddMod : MaterialForm
    {
        private List<string> selectedFilePaths = new List<string>();

        public FormAddMod()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);

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

        private void btnSelectMod_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Multiselect = true;
                ofd.Filter = "Mod Files (*.upk;*.bnk)|*.upk;*.bnk|All Files (*.*)|*.*";
                ofd.Title = "Select Mod Files";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePaths = ofd.FileNames.ToList();
                    Log($"Selected {selectedFilePaths.Count} file(s).");
                    foreach (string file in selectedFilePaths)
                    {
                        Log($"- {Path.GetFileName(file)}");
                    }
                }
            }
        }

        private void btnAddMod_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtModName.Text)) { Log("Error: Please enter a Mod Name."); return; }
            if (string.IsNullOrWhiteSpace(txtReplaces.Text)) { Log("Error: Please specify what item this replaces."); return; }
            if (cmbCategory.SelectedIndex == -1) { Log("Error: Please select a Category."); return; }
            if (selectedFilePaths.Count == 0) { Log("Error: Please select at least one mod file."); return; }

            try
            {
                string category = cmbCategory.SelectedItem.ToString();

                string folderName = $"{txtModName.Text.Trim()} (Replaces {txtReplaces.Text.Trim()})";

                string baseModsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "mods");
                string categoryFolder = Path.Combine(baseModsPath, category);

                if (!Directory.Exists(categoryFolder))
                {
                    Log("Error: Required folders are missing.");
                    MessageBox.Show("The required mod folders do not exist yet.\n\nPlease restart Alphy so it can generate the missing folders automatically.", "Restart Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string targetFolder = Path.Combine(categoryFolder, folderName);

                Directory.CreateDirectory(targetFolder);
                Log($"Created folder: {category}\\{folderName}");

                foreach (string sourceFile in selectedFilePaths)
                {
                    string fileName = Path.GetFileName(sourceFile);
                    string destFile = Path.Combine(targetFolder, fileName);

                    File.Copy(sourceFile, destFile, true);
                    Log($"Imported: {fileName}");
                }

                Log("SUCCESS! Mod imported successfully.");
                MessageBox.Show("Mod successfully added! Alphy will now refresh your list.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                Log($"FATAL ERROR: {ex.Message}");
            }
        }
    }
}