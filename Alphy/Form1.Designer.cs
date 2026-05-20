namespace Alphy
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtConsole = new System.Windows.Forms.RichTextBox();
            this.tabControl1 = new MaterialSkin.Controls.MaterialTabControl();
            this.tabMods = new System.Windows.Forms.TabPage();
            this.flowLayoutPanelMods = new System.Windows.Forms.FlowLayoutPanel();
            this.btnUpdate = new MaterialSkin.Controls.MaterialButton();
            this.btnChangeFolder = new MaterialSkin.Controls.MaterialButton();
            this.btnSave = new MaterialSkin.Controls.MaterialButton();
            this.btnImportMod = new MaterialSkin.Controls.MaterialButton();
            this.btnRemoveModWizard = new MaterialSkin.Controls.MaterialButton();
            this.pictureBoxBackground = new System.Windows.Forms.PictureBox();

            this.tabControl1.SuspendLayout();
            this.tabMods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).BeginInit();
            this.SuspendLayout();

            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.LimeGreen;
            this.txtConsole.Location = new System.Drawing.Point(17, 78);
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(498, 202);
            this.txtConsole.TabIndex = 1;
            this.txtConsole.Text = "";

            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabMods);
            this.tabControl1.Depth = 0;
            this.tabControl1.Location = new System.Drawing.Point(17, 286);
            this.tabControl1.MouseState = MaterialSkin.MouseState.HOVER;
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(498, 315);
            this.tabControl1.TabIndex = 6;

            // 
            // tabMods
            // 
            this.tabMods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.tabMods.Controls.Add(this.flowLayoutPanelMods);
            this.tabMods.Location = new System.Drawing.Point(4, 22);
            this.tabMods.Name = "tabMods";
            this.tabMods.Padding = new System.Windows.Forms.Padding(3);
            this.tabMods.Size = new System.Drawing.Size(490, 289);
            this.tabMods.TabIndex = 0;
            this.tabMods.Text = "Modifications";

            // 
            // flowLayoutPanelMods
            // 
            this.flowLayoutPanelMods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelMods.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanelMods.Name = "flowLayoutPanelMods";
            this.flowLayoutPanelMods.Size = new System.Drawing.Size(484, 283);
            this.flowLayoutPanelMods.TabIndex = 0;

            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUpdate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnUpdate.Depth = 0;
            this.btnUpdate.HighEmphasis = true;
            this.btnUpdate.Icon = global::Alphy.Properties.Resources.arrows_rotate_solid;
            this.btnUpdate.Location = new System.Drawing.Point(185, 656);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnUpdate.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnUpdate.Size = new System.Drawing.Size(156, 36);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "Check Update";
            this.btnUpdate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnUpdate.UseAccentColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);

            // 
            // btnChangeFolder
            // 
            this.btnChangeFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeFolder.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnChangeFolder.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnChangeFolder.Depth = 0;
            this.btnChangeFolder.ForeColor = System.Drawing.Color.White;
            this.btnChangeFolder.HighEmphasis = true;
            this.btnChangeFolder.Icon = global::Alphy.Properties.Resources.folder_regular;
            this.btnChangeFolder.Location = new System.Drawing.Point(349, 656);
            this.btnChangeFolder.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnChangeFolder.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnChangeFolder.Name = "btnChangeFolder";
            this.btnChangeFolder.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnChangeFolder.Size = new System.Drawing.Size(166, 36);
            this.btnChangeFolder.TabIndex = 0;
            this.btnChangeFolder.Text = "Change Folder";
            this.btnChangeFolder.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnChangeFolder.UseAccentColor = true;
            this.btnChangeFolder.Click += new System.EventHandler(this.btnChangeFolder_Click);

            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSave.Depth = 0;
            this.btnSave.HighEmphasis = true;
            this.btnSave.Icon = global::Alphy.Properties.Resources.floppy_disk_regular;
            this.btnSave.Location = new System.Drawing.Point(17, 656);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSave.Size = new System.Drawing.Size(157, 36);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "SAVE CHANGES";
            this.btnSave.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSave.UseAccentColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            // 
            // btnImportMod
            // 
            this.btnImportMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImportMod.AutoSize = false;
            this.btnImportMod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnImportMod.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnImportMod.Depth = 0;
            this.btnImportMod.HighEmphasis = true;
            this.btnImportMod.Icon = null;
            this.btnImportMod.Location = new System.Drawing.Point(17, 610);
            this.btnImportMod.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnImportMod.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnImportMod.Name = "btnImportMod";
            this.btnImportMod.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnImportMod.Size = new System.Drawing.Size(244, 36);
            this.btnImportMod.TabIndex = 8;
            this.btnImportMod.Text = "IMPORT NEW MOD";
            this.btnImportMod.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnImportMod.UseAccentColor = true;
            this.btnImportMod.Click += new System.EventHandler(this.btnImportMod_Click);

            // 
            // btnRemoveModWizard
            // 
            this.btnRemoveModWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveModWizard.AutoSize = false;
            this.btnRemoveModWizard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemoveModWizard.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnRemoveModWizard.Depth = 0;
            this.btnRemoveModWizard.HighEmphasis = true;
            this.btnRemoveModWizard.Icon = null;
            this.btnRemoveModWizard.Location = new System.Drawing.Point(271, 610);
            this.btnRemoveModWizard.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnRemoveModWizard.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRemoveModWizard.Name = "btnRemoveModWizard";
            this.btnRemoveModWizard.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnRemoveModWizard.Size = new System.Drawing.Size(244, 36);
            this.btnRemoveModWizard.TabIndex = 9;
            this.btnRemoveModWizard.Text = "REMOVE MODS";
            this.btnRemoveModWizard.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnRemoveModWizard.UseAccentColor = false;
            this.btnRemoveModWizard.Click += new System.EventHandler(this.btnRemoveModWizard_Click);

            // 
            // pictureBoxBackground
            // 
            this.pictureBoxBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBoxBackground.Image = global::Alphy.Properties.Resources.background;
            this.pictureBoxBackground.Location = new System.Drawing.Point(3, 64);
            this.pictureBoxBackground.Name = "pictureBoxBackground";
            this.pictureBoxBackground.Size = new System.Drawing.Size(524, 643);
            this.pictureBoxBackground.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBoxBackground.TabIndex = 0;
            this.pictureBoxBackground.TabStop = false;

            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(530, 710);
            this.Controls.Add(this.btnRemoveModWizard);
            this.Controls.Add(this.btnImportMod);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.btnChangeFolder);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.pictureBoxBackground);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimumSize = new System.Drawing.Size(530, 710);
            this.Name = "Form1";
            this.Sizable = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alphy";
            this.tabControl1.ResumeLayout(false);
            this.tabMods.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxBackground)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.PictureBox pictureBoxBackground;
        private System.Windows.Forms.RichTextBox txtConsole;
        private MaterialSkin.Controls.MaterialButton btnSave;
        private MaterialSkin.Controls.MaterialButton btnChangeFolder;
        private MaterialSkin.Controls.MaterialButton btnUpdate;
        private MaterialSkin.Controls.MaterialButton btnImportMod;
        private MaterialSkin.Controls.MaterialButton btnRemoveModWizard;
        private MaterialSkin.Controls.MaterialTabControl tabControl1;
        private System.Windows.Forms.TabPage tabMods;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelMods;
    }
}