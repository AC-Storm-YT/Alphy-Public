namespace Alphy
{
    partial class FormAddMod
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
            this.txtModName = new MaterialSkin.Controls.MaterialTextBox();
            this.txtReplaces = new MaterialSkin.Controls.MaterialTextBox();
            this.cmbCategory = new MaterialSkin.Controls.MaterialComboBox();
            this.txtConsole = new System.Windows.Forms.RichTextBox();
            this.btnSelectMod = new MaterialSkin.Controls.MaterialButton();
            this.btnAddMod = new MaterialSkin.Controls.MaterialButton();
            this.SuspendLayout();
            // 
            // txtModName
            // 
            this.txtModName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtModName.Depth = 0;
            this.txtModName.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtModName.Hint = "Mod Name (e.g. Titanium White Fennec)";
            this.txtModName.Location = new System.Drawing.Point(17, 78);
            this.txtModName.MaxLength = 50;
            this.txtModName.MouseState = MaterialSkin.MouseState.OUT;
            this.txtModName.Multiline = false;
            this.txtModName.Name = "txtModName";
            this.txtModName.Size = new System.Drawing.Size(416, 50);
            this.txtModName.TabIndex = 0;
            this.txtModName.Text = "";
            // 
            // txtReplaces
            // 
            this.txtReplaces.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReplaces.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReplaces.Depth = 0;
            this.txtReplaces.Font = new System.Drawing.Font("Roboto", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.txtReplaces.Hint = "Replaces Item (e.g. Octane)";
            this.txtReplaces.Location = new System.Drawing.Point(17, 140);
            this.txtReplaces.MaxLength = 50;
            this.txtReplaces.MouseState = MaterialSkin.MouseState.OUT;
            this.txtReplaces.Multiline = false;
            this.txtReplaces.Name = "txtReplaces";
            this.txtReplaces.Size = new System.Drawing.Size(416, 50);
            this.txtReplaces.TabIndex = 1;
            this.txtReplaces.Text = "";
            // 
            // cmbCategory
            // 
            this.cmbCategory.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbCategory.AutoResize = false;
            this.cmbCategory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cmbCategory.Depth = 0;
            this.cmbCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
            this.cmbCategory.DropDownHeight = 174;
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.DropDownWidth = 121;
            this.cmbCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
            this.cmbCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(222)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Hint = "Select Category";
            this.cmbCategory.IntegralHeight = false;
            this.cmbCategory.ItemHeight = 43;
            this.cmbCategory.Location = new System.Drawing.Point(17, 202);
            this.cmbCategory.MaxDropDownItems = 4;
            this.cmbCategory.MouseState = MaterialSkin.MouseState.OUT;
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(416, 49);
            this.cmbCategory.StartIndex = 0;
            this.cmbCategory.TabIndex = 2;
            // 
            // txtConsole
            // 
            this.txtConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsole.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtConsole.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtConsole.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtConsole.ForeColor = System.Drawing.Color.LimeGreen;
            this.txtConsole.Location = new System.Drawing.Point(17, 264);
            this.txtConsole.Name = "txtConsole";
            this.txtConsole.ReadOnly = true;
            this.txtConsole.Size = new System.Drawing.Size(416, 172);
            this.txtConsole.TabIndex = 3;
            this.txtConsole.Text = "System: Ready to import mod.\n";
            // 
            // btnAddMod
            // 
            this.btnAddMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddMod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAddMod.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnAddMod.Depth = 0;
            this.btnAddMod.HighEmphasis = true;
            this.btnAddMod.Icon = null;
            this.btnAddMod.Location = new System.Drawing.Point(17, 449);
            this.btnAddMod.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnAddMod.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnAddMod.Name = "btnAddMod";
            this.btnAddMod.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnAddMod.Size = new System.Drawing.Size(92, 36);
            this.btnAddMod.TabIndex = 5;
            this.btnAddMod.Text = "ADD MOD";
            this.btnAddMod.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Contained;
            this.btnAddMod.UseAccentColor = true;
            this.btnAddMod.Click += new System.EventHandler(this.btnAddMod_Click);
            // 
            // btnSelectMod
            // 
            this.btnSelectMod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectMod.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnSelectMod.Density = MaterialSkin.Controls.MaterialButton.MaterialButtonDensity.Default;
            this.btnSelectMod.Depth = 0;
            this.btnSelectMod.HighEmphasis = true;
            this.btnSelectMod.Icon = null;
            this.btnSelectMod.Location = new System.Drawing.Point(269, 449);
            this.btnSelectMod.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.btnSelectMod.MouseState = MaterialSkin.MouseState.HOVER;
            this.btnSelectMod.Name = "btnSelectMod";
            this.btnSelectMod.NoAccentTextColor = System.Drawing.Color.Empty;
            this.btnSelectMod.Size = new System.Drawing.Size(164, 36);
            this.btnSelectMod.TabIndex = 4;
            this.btnSelectMod.Text = "SELECT MOD FILES";
            this.btnSelectMod.Type = MaterialSkin.Controls.MaterialButton.MaterialButtonType.Outlined;
            this.btnSelectMod.UseAccentColor = false;
            this.btnSelectMod.Click += new System.EventHandler(this.btnSelectMod_Click);
            // 
            // FormAddMod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(450, 500);
            this.Controls.Add(this.btnAddMod);
            this.Controls.Add(this.btnSelectMod);
            this.Controls.Add(this.txtConsole);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.txtReplaces);
            this.Controls.Add(this.txtModName);
            this.MaximizeBox = false;
            this.Name = "FormAddMod";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Import New Mod";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private MaterialSkin.Controls.MaterialTextBox txtModName;
        private MaterialSkin.Controls.MaterialTextBox txtReplaces;
        private MaterialSkin.Controls.MaterialComboBox cmbCategory;
        private System.Windows.Forms.RichTextBox txtConsole;
        private MaterialSkin.Controls.MaterialButton btnSelectMod;
        private MaterialSkin.Controls.MaterialButton btnAddMod;
    }
}