using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace Alphy
{
    public partial class ModCard : UserControl
    {
        public MaterialCheckbox Checkbox => chkSelect;

        private string _category;
        private string _modName;
        private string _replacesName;

        public ModCard(string label, string category, string modName, string replacesName)
        {
            InitializeComponent();

            _category = category;
            _modName = modName;
            _replacesName = replacesName;

            chkSelect.Tag = category;

            lblModName.Text = string.IsNullOrEmpty(modName) ? label : modName;
            lblReplaces.Text = replacesName;

            this.Click += (s, e) => chkSelect.Checked = !chkSelect.Checked;
            picItem.Click += (s, e) => chkSelect.Checked = !chkSelect.Checked;
            lblModName.Click += (s, e) => chkSelect.Checked = !chkSelect.Checked;
            lblReplaces.Click += (s, e) => chkSelect.Checked = !chkSelect.Checked;
        }

        public async Task LoadImageAsync()
        {
            Image img = await ImageCache.GetModImageAsync(_category, _modName, _replacesName);
            if (img != null)
            {
                picItem.Image = img;
            }
        }
    }
}