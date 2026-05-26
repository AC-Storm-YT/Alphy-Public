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
            this.panelBottom = new System.Windows.Forms.Panel();
            this.btnChangeFolder = new MaterialSkin.Controls.MaterialButton();
            this.btnUpdate = new MaterialSkin.Controls.MaterialButton();
            this.btnRemoveModWizard = new MaterialSkin.Controls.MaterialButton();
            this.btnImportMod = new MaterialSkin.Controls.MaterialButton();
            this.btnSave = new MaterialSkin.Controls.MaterialButton();
            this.txtConsole = new System.Windows.Forms.RichTextBox();
            this.panelSidebar = new System.Windows.Forms.Panel();
            this.btnCatAudio = new System.Windows.Forms.Button();
            this.btnCatTopper = new System.Windows.Forms.Button();
            this.btnCatAntenna = new System.Windows.Forms.Button();
            this.btnCatBanner = new System.Windows.Forms.Button();
            this.btnCatPaint = new System.Windows.Forms.Button();
            this.btnCatTrail = new System.Windows.Forms.Button();
            this.btnCatExplosion = new System.Windows.Forms.Button();
            this.btnCatBoost = new System.Windows.Forms.Button();
            this.btnCatWheels = new System.Windows.Forms.Button();
            this.btnCatDecal = new System.Windows.Forms.Button();
            this.btnCatBody = new System.Windows.Forms.Button();
            this.btnCatAll = new System.Windows.Forms.Button();
            this.panelTop = new System.Windows.Forms.Panel();
            this.txtSearch = new MaterialSkin.Controls.MaterialTextBox();
            this.flowLayoutPanelMods = new System.Windows.Forms.FlowLayoutPanel();
            this.panelBottom.SuspendLayout();
            this.panelSidebar.SuspendLayout();
            this.panelTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.panelBottom.Controls.Add(this.btnChangeFolder);
            this.panelBottom.Controls.Add(this.btnUpdate);
            this.panelBottom.Controls.Add(this.btnRemoveModWizard);
            this.panelBottom.Controls.Add(this.btnImportMod);
            this.panelBottom.Controls.Add(this.btnSave);
            this.panelBottom.Controls.Add(this.txtConsole);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(3, 627);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(1214, 240);
            this.panelBottom.TabIndex = 0;
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
            this.btnChangeFolder.Location = new System.Drawing.Point(1035, 194);
            this.btnChangeFolder.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnChangeFolder.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnChangeFolder.Name = "btnChangeFolder";
            this.btnChangeFolder.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnChangeFolder.Size = new System.Drawing.Size(166, 36);
            this.btnChangeFolder.TabIndex = 12;
            this.btnChangeFolder.Text = "Change Folder";
            this.btnChangeFolder.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnChangeFolder.UseAccentColor = true;
            this.btnChangeFolder.Click += new System.EventHandler(this.btnChangeFolder_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnUpdate.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnUpdate.Depth = 0;
            this.btnUpdate.HighEmphasis = true;
            this.btnUpdate.Icon = global::Alphy.Properties.Resources.arrows_rotate_solid;
            this.btnUpdate.Location = new System.Drawing.Point(840, 194);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnUpdate.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnUpdate.Size = new System.Drawing.Size(156, 36);
            this.btnUpdate.TabIndex = 11;
            this.btnUpdate.Text = "Check Update";
            this.btnUpdate.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnUpdate.UseAccentColor = false;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnRemoveModWizard
            // 
            this.btnRemoveModWizard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRemoveModWizard.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnRemoveModWizard.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnRemoveModWizard.Depth = 0;
            this.btnRemoveModWizard.HighEmphasis = true;
            this.btnRemoveModWizard.Icon = null;
            this.btnRemoveModWizard.Location = new System.Drawing.Point(180, 194);
            this.btnRemoveModWizard.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnRemoveModWizard.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnRemoveModWizard.Name = "btnRemoveModWizard";
            this.btnRemoveModWizard.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnRemoveModWizard.Size = new System.Drawing.Size(125, 36);
            this.btnRemoveModWizard.TabIndex = 10;
            this.btnRemoveModWizard.Text = "REMOVE MODS";
            this.btnRemoveModWizard.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnRemoveModWizard.UseAccentColor = true;
            this.btnRemoveModWizard.Click += new System.EventHandler(this.btnRemoveModWizard_Click);
            // 
            // btnImportMod
            // 
            this.btnImportMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImportMod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnImportMod.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnImportMod.Depth = 0;
            this.btnImportMod.HighEmphasis = true;
            this.btnImportMod.Icon = null;
            this.btnImportMod.Location = new System.Drawing.Point(14, 194);
            this.btnImportMod.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnImportMod.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnImportMod.Name = "btnImportMod";
            this.btnImportMod.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnImportMod.Size = new System.Drawing.Size(148, 36);
            this.btnImportMod.TabIndex = 9;
            this.btnImportMod.Text = "IMPORT NEW MOD";
            this.btnImportMod.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnImportMod.UseAccentColor = true;
            this.btnImportMod.Click += new System.EventHandler(this.btnImportMod_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSave.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSave.Depth = 0;
            this.btnSave.HighEmphasis = true;
            this.btnSave.Icon = global::Alphy.Properties.Resources.floppy_disk_regular;
            this.btnSave.Location = new System.Drawing.Point(643, 194);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSave.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSave.Name = "btnSave";
            this.btnSave.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSave.Size = new System.Drawing.Size(157, 36);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "SAVE CHANGES";
            this.btnSave.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnSave.UseAccentColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtConsole.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.LimeGreen;
            this.txtConsole.Location = new System.Drawing.Point(14, 10);
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(1187, 175);
            this.txtConsole.TabIndex = 2;
            this.txtConsole.Text = "";
            // 
            // panelSidebar
            // 
            this.panelSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.panelSidebar.Controls.Add(this.btnCatAudio);
            this.panelSidebar.Controls.Add(this.btnCatTopper);
            this.panelSidebar.Controls.Add(this.btnCatAntenna);
            this.panelSidebar.Controls.Add(this.btnCatBanner);
            this.panelSidebar.Controls.Add(this.btnCatPaint);
            this.panelSidebar.Controls.Add(this.btnCatTrail);
            this.panelSidebar.Controls.Add(this.btnCatExplosion);
            this.panelSidebar.Controls.Add(this.btnCatBoost);
            this.panelSidebar.Controls.Add(this.btnCatWheels);
            this.panelSidebar.Controls.Add(this.btnCatDecal);
            this.panelSidebar.Controls.Add(this.btnCatBody);
            this.panelSidebar.Controls.Add(this.btnCatAll);
            this.panelSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelSidebar.Location = new System.Drawing.Point(3, 64);
            this.panelSidebar.Name = "panelSidebar";
            this.panelSidebar.Size = new System.Drawing.Size(220, 563);
            this.panelSidebar.TabIndex = 1;
            // 
            // btnCatAudio
            // 
            this.btnCatAudio.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatAudio.FlatAppearance.BorderSize = 0;
            this.btnCatAudio.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatAudio.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatAudio.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatAudio.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatAudio.Location = new System.Drawing.Point(0, 495);
            this.btnCatAudio.Name = "btnCatAudio";
            this.btnCatAudio.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatAudio.Size = new System.Drawing.Size(220, 45);
            this.btnCatAudio.TabIndex = 11;
            this.btnCatAudio.Tag = "Boost Audio";
            this.btnCatAudio.Text = " 🔊   Boost Audio";
            this.btnCatAudio.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatAudio.UseVisualStyleBackColor = true;
            this.btnCatAudio.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatTopper
            // 
            this.btnCatTopper.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatTopper.FlatAppearance.BorderSize = 0;
            this.btnCatTopper.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatTopper.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatTopper.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatTopper.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatTopper.Location = new System.Drawing.Point(0, 450);
            this.btnCatTopper.Name = "btnCatTopper";
            this.btnCatTopper.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatTopper.Size = new System.Drawing.Size(220, 45);
            this.btnCatTopper.TabIndex = 10;
            this.btnCatTopper.Tag = "Hat";
            this.btnCatTopper.Text = " 🎩   Toppers";
            this.btnCatTopper.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatTopper.UseVisualStyleBackColor = true;
            this.btnCatTopper.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatAntenna
            // 
            this.btnCatAntenna.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatAntenna.FlatAppearance.BorderSize = 0;
            this.btnCatAntenna.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatAntenna.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatAntenna.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatAntenna.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatAntenna.Location = new System.Drawing.Point(0, 405);
            this.btnCatAntenna.Name = "btnCatAntenna";
            this.btnCatAntenna.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatAntenna.Size = new System.Drawing.Size(220, 45);
            this.btnCatAntenna.TabIndex = 9;
            this.btnCatAntenna.Tag = "Antenna";
            this.btnCatAntenna.Text = " 📡   Antennas";
            this.btnCatAntenna.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatAntenna.UseVisualStyleBackColor = true;
            this.btnCatAntenna.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatBanner
            // 
            this.btnCatBanner.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatBanner.FlatAppearance.BorderSize = 0;
            this.btnCatBanner.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatBanner.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatBanner.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatBanner.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatBanner.Location = new System.Drawing.Point(0, 360);
            this.btnCatBanner.Name = "btnCatBanner";
            this.btnCatBanner.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatBanner.Size = new System.Drawing.Size(220, 45);
            this.btnCatBanner.TabIndex = 8;
            this.btnCatBanner.Tag = "Banner";
            this.btnCatBanner.Text = " 🎌   Banners";
            this.btnCatBanner.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatBanner.UseVisualStyleBackColor = true;
            this.btnCatBanner.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatPaint
            // 
            this.btnCatPaint.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatPaint.FlatAppearance.BorderSize = 0;
            this.btnCatPaint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatPaint.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatPaint.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatPaint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatPaint.Location = new System.Drawing.Point(0, 315);
            this.btnCatPaint.Name = "btnCatPaint";
            this.btnCatPaint.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatPaint.Size = new System.Drawing.Size(220, 45);
            this.btnCatPaint.TabIndex = 7;
            this.btnCatPaint.Tag = "Paint";
            this.btnCatPaint.Text = " 🎨   Paints";
            this.btnCatPaint.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatPaint.UseVisualStyleBackColor = true;
            this.btnCatPaint.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatTrail
            // 
            this.btnCatTrail.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatTrail.FlatAppearance.BorderSize = 0;
            this.btnCatTrail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatTrail.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatTrail.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatTrail.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatTrail.Location = new System.Drawing.Point(0, 270);
            this.btnCatTrail.Name = "btnCatTrail";
            this.btnCatTrail.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatTrail.Size = new System.Drawing.Size(220, 45);
            this.btnCatTrail.TabIndex = 6;
            this.btnCatTrail.Tag = "Trail";
            this.btnCatTrail.Text = " 🛤️   Trails";
            this.btnCatTrail.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatTrail.UseVisualStyleBackColor = true;
            this.btnCatTrail.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatExplosion
            // 
            this.btnCatExplosion.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatExplosion.FlatAppearance.BorderSize = 0;
            this.btnCatExplosion.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatExplosion.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatExplosion.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatExplosion.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatExplosion.Location = new System.Drawing.Point(0, 225);
            this.btnCatExplosion.Name = "btnCatExplosion";
            this.btnCatExplosion.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatExplosion.Size = new System.Drawing.Size(220, 45);
            this.btnCatExplosion.TabIndex = 5;
            this.btnCatExplosion.Tag = "Goal Explosion";
            this.btnCatExplosion.Text = " 💥   Goal Explosions";
            this.btnCatExplosion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatExplosion.UseVisualStyleBackColor = true;
            this.btnCatExplosion.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatBoost
            // 
            this.btnCatBoost.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatBoost.FlatAppearance.BorderSize = 0;
            this.btnCatBoost.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatBoost.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatBoost.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatBoost.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatBoost.Location = new System.Drawing.Point(0, 180);
            this.btnCatBoost.Name = "btnCatBoost";
            this.btnCatBoost.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatBoost.Size = new System.Drawing.Size(220, 45);
            this.btnCatBoost.TabIndex = 4;
            this.btnCatBoost.Tag = "Boost";
            this.btnCatBoost.Text = " 💨   Boosts";
            this.btnCatBoost.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatBoost.UseVisualStyleBackColor = true;
            this.btnCatBoost.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatWheels
            // 
            this.btnCatWheels.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatWheels.FlatAppearance.BorderSize = 0;
            this.btnCatWheels.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatWheels.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatWheels.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatWheels.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatWheels.Location = new System.Drawing.Point(0, 135);
            this.btnCatWheels.Name = "btnCatWheels";
            this.btnCatWheels.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatWheels.Size = new System.Drawing.Size(220, 45);
            this.btnCatWheels.TabIndex = 3;
            this.btnCatWheels.Tag = "Wheels";
            this.btnCatWheels.Text = " 🛞   Wheels";
            this.btnCatWheels.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatWheels.UseVisualStyleBackColor = true;
            this.btnCatWheels.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatDecal
            // 
            this.btnCatDecal.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatDecal.FlatAppearance.BorderSize = 0;
            this.btnCatDecal.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatDecal.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatDecal.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatDecal.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatDecal.Location = new System.Drawing.Point(0, 90);
            this.btnCatDecal.Name = "btnCatDecal";
            this.btnCatDecal.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatDecal.Size = new System.Drawing.Size(220, 45);
            this.btnCatDecal.TabIndex = 2;
            this.btnCatDecal.Tag = "Decal";
            this.btnCatDecal.Text = " 🏎️   Decals";
            this.btnCatDecal.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatDecal.UseVisualStyleBackColor = true;
            this.btnCatDecal.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatBody
            // 
            this.btnCatBody.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatBody.FlatAppearance.BorderSize = 0;
            this.btnCatBody.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatBody.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatBody.ForeColor = System.Drawing.Color.DarkGray;
            this.btnCatBody.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatBody.Location = new System.Drawing.Point(0, 45);
            this.btnCatBody.Name = "btnCatBody";
            this.btnCatBody.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatBody.Size = new System.Drawing.Size(220, 45);
            this.btnCatBody.TabIndex = 1;
            this.btnCatBody.TabStop = false;
            this.btnCatBody.Tag = "Body";
            this.btnCatBody.Text = " 🚗   Bodies";
            this.btnCatBody.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatBody.UseVisualStyleBackColor = true;
            this.btnCatBody.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // btnCatAll
            // 
            this.btnCatAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnCatAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCatAll.FlatAppearance.BorderSize = 0;
            this.btnCatAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCatAll.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnCatAll.ForeColor = System.Drawing.Color.White;
            this.btnCatAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatAll.Location = new System.Drawing.Point(0, 0);
            this.btnCatAll.Name = "btnCatAll";
            this.btnCatAll.Padding = new System.Windows.Forms.Padding(15, 0, 0, 0);
            this.btnCatAll.Size = new System.Drawing.Size(220, 45);
            this.btnCatAll.TabIndex = 0;
            this.btnCatAll.TabStop = false;
            this.btnCatAll.Tag = "All";
            this.btnCatAll.Text = " 📦   All Items";
            this.btnCatAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCatAll.UseVisualStyleBackColor = false;
            this.btnCatAll.Click += new System.EventHandler(this.SidebarCategory_Click);
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.panelTop.Controls.Add(this.txtSearch);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(223, 64);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(994, 80);
            this.panelTop.TabIndex = 2;
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.AnimateReadOnly = false;
            this.txtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSearch.Depth = 0;
            this.txtSearch.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtSearch.Hint = "Search mods...";
            this.txtSearch.LeadingIcon = null;
            this.txtSearch.Location = new System.Drawing.Point(22, 15);
            this.txtSearch.MaxLength = 50;
            this.txtSearch.MouseState = MaterialSkin.MouseState.OUT;
            this.txtSearch.Multiline = false;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(948, 50);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.Text = "";
            this.txtSearch.TrailingIcon = null;
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // flowLayoutPanelMods
            // 
            this.flowLayoutPanelMods.AutoScroll = true;
            this.flowLayoutPanelMods.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.flowLayoutPanelMods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelMods.Location = new System.Drawing.Point(223, 144);
            this.flowLayoutPanelMods.Name = "flowLayoutPanelMods";
            this.flowLayoutPanelMods.Padding = new System.Windows.Forms.Padding(15, 15, 0, 0);
            this.flowLayoutPanelMods.Size = new System.Drawing.Size(994, 483);
            this.flowLayoutPanelMods.TabIndex = 3;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1220, 870);
            this.Controls.Add(this.flowLayoutPanelMods);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.panelSidebar);
            this.Controls.Add(this.panelBottom);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1220, 870);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Alphy";
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panelSidebar.ResumeLayout(false);
            this.panelTop.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelSidebar;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnCatAll;
        private System.Windows.Forms.Button btnCatBody;
        private System.Windows.Forms.Button btnCatDecal;
        private System.Windows.Forms.Button btnCatWheels;
        private System.Windows.Forms.Button btnCatBoost;
        private System.Windows.Forms.Button btnCatExplosion;
        private System.Windows.Forms.Button btnCatTrail;
        private System.Windows.Forms.Button btnCatPaint;
        private System.Windows.Forms.Button btnCatBanner;
        private System.Windows.Forms.Button btnCatAntenna;
        private System.Windows.Forms.Button btnCatTopper;
        private System.Windows.Forms.Button btnCatAudio;
        private MaterialSkin.Controls.MaterialTextBox txtSearch;
    }
}