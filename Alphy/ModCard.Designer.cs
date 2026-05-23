namespace Alphy
{
    partial class ModCard
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
            this.picItem = new System.Windows.Forms.PictureBox();
            this.chkSelect = new MaterialSkin.Controls.MaterialCheckbox();
            this.lblModName = new System.Windows.Forms.Label();
            this.lblReplaces = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).BeginInit();
            this.SuspendLayout();
            // 
            // picItem
            // 
            this.picItem.BackColor = System.Drawing.Color.Transparent;
            this.picItem.Location = new System.Drawing.Point(15, 5);
            this.picItem.Name = "picItem";
            this.picItem.Size = new System.Drawing.Size(120, 120);
            this.picItem.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picItem.TabIndex = 0;
            this.picItem.TabStop = false;
            // 
            // chkSelect
            // 
            this.chkSelect.AutoSize = false;
            this.chkSelect.Depth = 0;
            this.chkSelect.Location = new System.Drawing.Point(0, 130);
            this.chkSelect.Margin = new System.Windows.Forms.Padding(0);
            this.chkSelect.MouseLocation = new System.Drawing.Point(-1, -1);
            this.chkSelect.MouseState = MaterialSkin.MouseState.HOVER;
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.ReadOnly = false;
            this.chkSelect.Ripple = true;
            this.chkSelect.Size = new System.Drawing.Size(35, 35);
            this.chkSelect.TabIndex = 1;
            this.chkSelect.Text = "";
            this.chkSelect.UseVisualStyleBackColor = true;
            // 
            // lblModName
            // 
            this.lblModName.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblModName.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblModName.Location = new System.Drawing.Point(35, 137);
            this.lblModName.Name = "lblModName";
            this.lblModName.Size = new System.Drawing.Size(110, 40);
            this.lblModName.TabIndex = 2;
            this.lblModName.Text = "Mod Name";
            // 
            // lblReplaces
            // 
            this.lblReplaces.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblReplaces.ForeColor = System.Drawing.Color.DarkGray;
            this.lblReplaces.Location = new System.Drawing.Point(35, 175);
            this.lblReplaces.Name = "lblReplaces";
            this.lblReplaces.Size = new System.Drawing.Size(110, 45);
            this.lblReplaces.TabIndex = 3;
            this.lblReplaces.Text = "(Replaces ...)";
            // 
            // ModCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblReplaces);
            this.Controls.Add(this.lblModName);
            this.Controls.Add(this.chkSelect);
            this.Controls.Add(this.picItem);
            this.Name = "ModCard";
            this.Size = new System.Drawing.Size(150, 230);
            ((System.ComponentModel.ISupportInitialize)(this.picItem)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.PictureBox picItem;
        private MaterialSkin.Controls.MaterialCheckbox chkSelect;
        private System.Windows.Forms.Label lblModName;
        private System.Windows.Forms.Label lblReplaces;
    }
}