using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Windows.Forms;
using static Alphy.Form1;

namespace Alphy
{
    public partial class FormPlugins : MaterialForm
    {
        private IAlphyHost _host;
        private Form1 _mainForm;
        private MaterialButton btnSwapper;
        private MaterialLabel lblStatus;

        public FormPlugins(IAlphyHost host, Form1 mainForm)
        {
            _host = host;
            _mainForm = mainForm;

            this.Text = "Plugin Manager";
            this.Size = new Size(600, 280);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.Sizable = false;

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.Indigo600, Primary.Indigo700, Primary.Indigo400, Accent.Indigo200, TextShade.WHITE);
            materialSkinManager.RemoveFormToManage(this);

            var card = new MaterialCard
            {
                Size = new Size(550, 150),
                Location = new Point(25, 95)
            };

            var lblTitle = new MaterialLabel
            {
                Text = "Alphy Swapper",
                Location = new Point(20, 20),
                FontType = MaterialSkinManager.fontType.H5,
                AutoSize = true
            };

            var lblDesc = new MaterialLabel
            {
                Text = "Advanced .upk and .bnk asset swapper.",
                Location = new Point(20, 60),
                Size = new Size(500, 30)
            };

            btnSwapper = new MaterialButton
            {
                Text = "LOADING...",
                Location = new Point(20, 95),
                AutoSize = false,
                Size = new Size(180, 36)
            };
            btnSwapper.Click += BtnSwapper_Click;

            lblStatus = new MaterialLabel
            {
                Text = "Checking version...",
                Location = new Point(215, 105),
                Size = new Size(300, 30)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblDesc);
            card.Controls.Add(btnSwapper);
            card.Controls.Add(lblStatus);
            this.Controls.Add(card);

            CheckInitialState();
        }

        private void CheckInitialState()
        {
            var existingPlugin = _mainForm.GetLoadedPlugins().FirstOrDefault(p => p.Name == "Alphy Swapper");
            if (existingPlugin != null)
            {
                lblStatus.Text = $"Installed (v{existingPlugin.Version})";
                btnSwapper.Text = "RUN SWAPPER";
            }
            else
            {
                lblStatus.Text = "Not installed";
                btnSwapper.Text = "INSTALL PLUGIN";
            }
        }

        private async void BtnSwapper_Click(object sender, EventArgs e)
        {
            btnSwapper.Enabled = false;

            string versionUrl = "https://raw.githubusercontent.com/AC-Storm-YT/alphy.github.io/refs/heads/main/swapper/data/version.txt";
            string dllUrl = "https://github.com/AC-Storm-YT/alphy.github.io/raw/refs/heads/main/swapper/data/Alphy%20Swapper.dll";
            string pluginsDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Alphy\Plugins");
            string dllPath = Path.Combine(pluginsDir, "Alphy Swapper.dll");

            try
            {
                Directory.CreateDirectory(pluginsDir);

                lblStatus.Text = "Checking for updates...";
                string remoteVersion = "";

                using (HttpClient client = new HttpClient())
                {
                    remoteVersion = (await client.GetStringAsync(versionUrl)).Trim();
                }

                var existingPlugin = _mainForm.GetLoadedPlugins().FirstOrDefault(p => p.Name == "Alphy Swapper");
                bool needsUpdate = existingPlugin == null || existingPlugin.Version != remoteVersion;

                if (needsUpdate)
                {
                    lblStatus.Text = $"Downloading v{remoteVersion}...";

                    using (HttpClient client = new HttpClient())
                    {
                        byte[] dllBytes = await client.GetByteArrayAsync(dllUrl);
                        File.WriteAllBytes(dllPath, dllBytes);
                    }

                    _mainForm.ReloadPlugin(dllPath);
                    existingPlugin = _mainForm.GetLoadedPlugins().FirstOrDefault(p => p.Name == "Alphy Swapper");
                }

                if (existingPlugin != null)
                {
                    lblStatus.Text = $"Installed (v{existingPlugin.Version})";
                    btnSwapper.Text = "RUN SWAPPER";

                    existingPlugin.ShowUI();
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
            finally
            {
                btnSwapper.Enabled = true;
            }
        }
    }
}