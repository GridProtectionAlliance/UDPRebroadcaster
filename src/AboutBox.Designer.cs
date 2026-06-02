namespace UDPRebroadcaster
{
    partial class AboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Logo = new System.Windows.Forms.PictureBox();
            this.Title = new System.Windows.Forms.Label();
            this.CopyrightLabel = new System.Windows.Forms.Label();
            this.CompanyUrl = new System.Windows.Forms.LinkLabel();
            this.Disclaimer = new System.Windows.Forms.TextBox();
            this.OkButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.SuspendLayout();
            //
            // Logo
            //
            this.Logo.Location = new System.Drawing.Point(12, 12);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(496, 96);
            this.Logo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.Logo.TabIndex = 0;
            this.Logo.TabStop = false;
            //
            // Title
            //
            this.Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Title.Location = new System.Drawing.Point(12, 116);
            this.Title.Name = "Title";
            this.Title.Size = new System.Drawing.Size(496, 22);
            this.Title.TabIndex = 1;
            this.Title.Text = "UDP Rebroadcaster";
            this.Title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // CopyrightLabel
            //
            this.CopyrightLabel.Location = new System.Drawing.Point(12, 140);
            this.CopyrightLabel.Name = "CopyrightLabel";
            this.CopyrightLabel.Size = new System.Drawing.Size(496, 18);
            this.CopyrightLabel.TabIndex = 2;
            this.CopyrightLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // CompanyUrl
            //
            this.CompanyUrl.Location = new System.Drawing.Point(12, 160);
            this.CompanyUrl.Name = "CompanyUrl";
            this.CompanyUrl.Size = new System.Drawing.Size(496, 18);
            this.CompanyUrl.TabIndex = 3;
            this.CompanyUrl.TabStop = true;
            this.CompanyUrl.Text = "http://www.gridprotectionalliance.org/";
            this.CompanyUrl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CompanyUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CompanyUrl_LinkClicked);
            //
            // Disclaimer
            //
            this.Disclaimer.Location = new System.Drawing.Point(12, 186);
            this.Disclaimer.Multiline = true;
            this.Disclaimer.Name = "Disclaimer";
            this.Disclaimer.ReadOnly = true;
            this.Disclaimer.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Disclaimer.Size = new System.Drawing.Size(496, 190);
            this.Disclaimer.TabIndex = 4;
            //
            // OkButton
            //
            this.OkButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OkButton.Location = new System.Drawing.Point(216, 384);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(88, 26);
            this.OkButton.TabIndex = 5;
            this.OkButton.Text = "&OK";
            this.OkButton.UseVisualStyleBackColor = true;
            //
            // AboutBox
            //
            this.AcceptButton = this.OkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.OkButton;
            this.ClientSize = new System.Drawing.Size(520, 420);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.Disclaimer);
            this.Controls.Add(this.CompanyUrl);
            this.Controls.Add(this.CopyrightLabel);
            this.Controls.Add(this.Title);
            this.Controls.Add(this.Logo);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AboutBox";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "About UDP Rebroadcaster";
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Label Title;
        private System.Windows.Forms.Label CopyrightLabel;
        private System.Windows.Forms.LinkLabel CompanyUrl;
        private System.Windows.Forms.TextBox Disclaimer;
        private System.Windows.Forms.Button OkButton;
    }
}
