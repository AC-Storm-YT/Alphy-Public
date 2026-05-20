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

            foreach (MaterialCheckbox chk in boxesToRemove)
            {
                string folderPath = chk.Tag.ToString();
                string modName = chk.Text;

                try
                {
                    if (Directory.Exists(folderPath))
                    {
                        Directory.Delete(folderPath, true);
                        Log($"Deleted: {modName}");
                        successCount++;
                    }
                }
                catch (Exception ex)
                {
                    Log($"Failed to delete '{modName}': {ex.Message}");
                }
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