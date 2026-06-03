namespace UDPRebroadcaster.Augmentations
{
    partial class SELCWSChannelIDSettingsForm
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
            GenerationGroup = new GroupBox();
            IncrementedRadio = new RadioButton();
            RandomRadio = new RadioButton();
            StationLabelsLabel = new Label();
            StationLabelsPanel = new FlowLayoutPanel();
            OkBtn = new Button();
            CancelBtn = new Button();
            GenerationGroup.SuspendLayout();
            SuspendLayout();
            // 
            // GenerationGroup
            // 
            GenerationGroup.Controls.Add(IncrementedRadio);
            GenerationGroup.Controls.Add(RandomRadio);
            GenerationGroup.Location = new Point(12, 12);
            GenerationGroup.Name = "GenerationGroup";
            GenerationGroup.Size = new Size(425, 76);
            GenerationGroup.TabIndex = 0;
            GenerationGroup.TabStop = false;
            GenerationGroup.Text = "Unique Channel ID Generation";
            // 
            // IncrementedRadio
            // 
            IncrementedRadio.Location = new Point(16, 22);
            IncrementedRadio.Name = "IncrementedRadio";
            IncrementedRadio.Size = new Size(384, 20);
            IncrementedRadio.TabIndex = 0;
            IncrementedRadio.TabStop = true;
            IncrementedRadio.Text = "&Incremented (original + N + 1 per destination)";
            IncrementedRadio.UseVisualStyleBackColor = true;
            // 
            // RandomRadio
            // 
            RandomRadio.Location = new Point(16, 46);
            RandomRadio.Name = "RandomRadio";
            RandomRadio.Size = new Size(384, 20);
            RandomRadio.TabIndex = 1;
            RandomRadio.TabStop = true;
            RandomRadio.Text = "&Random (fresh ulong per destination, generated each Start)";
            RandomRadio.UseVisualStyleBackColor = true;
            // 
            // StationLabelsLabel
            // 
            StationLabelsLabel.Location = new Point(12, 98);
            StationLabelsLabel.Name = "StationLabelsLabel";
            StationLabelsLabel.Size = new Size(416, 18);
            StationLabelsLabel.TabIndex = 1;
            StationLabelsLabel.Text = "Configuration Frame &Station Labels:";
            // 
            // StationLabelsPanel
            // 
            StationLabelsPanel.AutoScroll = true;
            StationLabelsPanel.BorderStyle = BorderStyle.Fixed3D;
            StationLabelsPanel.FlowDirection = FlowDirection.TopDown;
            StationLabelsPanel.Location = new Point(12, 119);
            StationLabelsPanel.Name = "StationLabelsPanel";
            StationLabelsPanel.Size = new Size(425, 313);
            StationLabelsPanel.TabIndex = 2;
            StationLabelsPanel.WrapContents = false;
            // 
            // OkBtn
            // 
            OkBtn.DialogResult = DialogResult.OK;
            OkBtn.Location = new Point(255, 445);
            OkBtn.Name = "OkBtn";
            OkBtn.Size = new Size(88, 27);
            OkBtn.TabIndex = 3;
            OkBtn.Text = "&OK";
            OkBtn.UseVisualStyleBackColor = true;
            OkBtn.Click += OkBtn_Click;
            // 
            // CancelBtn
            // 
            CancelBtn.DialogResult = DialogResult.Cancel;
            CancelBtn.Location = new Point(349, 445);
            CancelBtn.Name = "CancelBtn";
            CancelBtn.Size = new Size(88, 27);
            CancelBtn.TabIndex = 4;
            CancelBtn.Text = "&Cancel";
            CancelBtn.UseVisualStyleBackColor = true;
            // 
            // SELCWSChannelIDSettingsForm
            // 
            AcceptButton = OkBtn;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = CancelBtn;
            ClientSize = new Size(449, 484);
            Controls.Add(CancelBtn);
            Controls.Add(OkBtn);
            Controls.Add(StationLabelsPanel);
            Controls.Add(StationLabelsLabel);
            Controls.Add(GenerationGroup);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SELCWSChannelIDSettingsForm";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "SEL CWS Channel ID — Settings";
            GenerationGroup.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox GenerationGroup;
        private System.Windows.Forms.RadioButton IncrementedRadio;
        private System.Windows.Forms.RadioButton RandomRadio;
        private System.Windows.Forms.Label StationLabelsLabel;
        private System.Windows.Forms.FlowLayoutPanel StationLabelsPanel;
        private System.Windows.Forms.Button OkBtn;
        private System.Windows.Forms.Button CancelBtn;
    }
}
