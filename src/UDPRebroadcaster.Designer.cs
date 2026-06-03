namespace UDPRebroadcaster
{
    partial class UDPRebroadcaster
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UDPRebroadcaster));
            ShowData = new CheckBox();
            AutoListen = new CheckBox();
            About = new Button();
            Instructions = new Label();
            RebroadcastDestinations = new TextBox();
            Port = new TextBox();
            RebroadcastPortLabel = new Label();
            Status = new Label();
            Listen = new Button();
            ToolTipMessage = new ToolTip(components);
            AugmentationOptions = new ComboBox();
            AugmentationSettings = new Button();
            ListenOnPortLabel = new Label();
            SamplesPerSecLabel = new Label();
            SampleRate = new Label();
            SampleRateLabel = new Label();
            SampleCount = new Label();
            SamplesLabel = new Label();
            UDPFrame = new Label();
            AugmentationLabel = new Label();
            SuspendLayout();
            // 
            // ShowData
            // 
            ShowData.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ShowData.Location = new Point(703, 89);
            ShowData.Margin = new Padding(4, 3, 4, 3);
            ShowData.Name = "ShowData";
            ShowData.Size = new Size(85, 18);
            ShowData.TabIndex = 16;
            ShowData.Text = "&Show data";
            ShowData.TextAlign = ContentAlignment.BottomLeft;
            ToolTipMessage.SetToolTip(ShowData, "Enabling data display may lower data retransmission rate.");
            ShowData.CheckedChanged += ShowData_CheckedChanged;
            // 
            // AutoListen
            // 
            AutoListen.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AutoListen.Location = new Point(546, 89);
            AutoListen.Margin = new Padding(4, 3, 4, 3);
            AutoListen.Name = "AutoListen";
            AutoListen.Size = new Size(149, 18);
            AutoListen.TabIndex = 15;
            AutoListen.Text = "A&uto listen on start-up";
            AutoListen.TextAlign = ContentAlignment.BottomLeft;
            // 
            // About
            // 
            About.Location = new Point(325, 7);
            About.Margin = new Padding(4, 3, 4, 3);
            About.Name = "About";
            About.Size = new Size(88, 27);
            About.TabIndex = 3;
            About.Text = "&About...";
            About.Click += About_Click;
            // 
            // Instructions
            // 
            Instructions.Location = new Point(169, 65);
            Instructions.Margin = new Padding(4, 0, 4, 0);
            Instructions.Name = "Instructions";
            Instructions.Size = new Size(525, 24);
            Instructions.TabIndex = 9;
            Instructions.Text = "Format =  IP1:port, IP2:port, IP3:port - etc.";
            // 
            // RebroadcastDestinations
            // 
            RebroadcastDestinations.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            RebroadcastDestinations.Location = new Point(169, 37);
            RebroadcastDestinations.Margin = new Padding(4, 3, 4, 3);
            RebroadcastDestinations.Name = "RebroadcastDestinations";
            RebroadcastDestinations.Size = new Size(619, 23);
            RebroadcastDestinations.TabIndex = 8;
            RebroadcastDestinations.Text = "127.0.0.1:3060, 127.0.0.1:3070";
            RebroadcastDestinations.Leave += RebroadcastDestinations_Leave;
            // 
            // Port
            // 
            Port.Location = new Point(169, 9);
            Port.Margin = new Padding(4, 3, 4, 3);
            Port.Name = "Port";
            Port.Size = new Size(55, 23);
            Port.TabIndex = 1;
            Port.Text = "3050";
            // 
            // RebroadcastPortLabel
            // 
            RebroadcastPortLabel.Location = new Point(1, 37);
            RebroadcastPortLabel.Margin = new Padding(4, 0, 4, 0);
            RebroadcastPortLabel.Name = "RebroadcastPortLabel";
            RebroadcastPortLabel.Size = new Size(159, 23);
            RebroadcastPortLabel.TabIndex = 7;
            RebroadcastPortLabel.Text = "&Rebroadcast destinations:";
            RebroadcastPortLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Status
            // 
            Status.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            Status.Location = new Point(514, 61);
            Status.Margin = new Padding(4, 0, 4, 0);
            Status.Name = "Status";
            Status.Size = new Size(274, 28);
            Status.TabIndex = 17;
            Status.TextAlign = ContentAlignment.MiddleRight;
            // 
            // Listen
            // 
            Listen.Location = new Point(231, 7);
            Listen.Margin = new Padding(4, 3, 4, 3);
            Listen.Name = "Listen";
            Listen.Size = new Size(88, 27);
            Listen.TabIndex = 2;
            Listen.Text = "&Start";
            Listen.Click += Listen_Click;
            // 
            // ToolTipMessage
            // 
            ToolTipMessage.AutoPopDelay = 60000;
            ToolTipMessage.InitialDelay = 1;
            ToolTipMessage.ReshowDelay = 1;
            ToolTipMessage.ShowAlways = true;
            // 
            // AugmentationOptions
            // 
            AugmentationOptions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            AugmentationOptions.DropDownStyle = ComboBoxStyle.DropDownList;
            AugmentationOptions.FormattingEnabled = true;
            AugmentationOptions.Location = new Point(518, 8);
            AugmentationOptions.Margin = new Padding(4, 3, 4, 3);
            AugmentationOptions.Name = "AugmentationOptions";
            AugmentationOptions.Size = new Size(176, 23);
            AugmentationOptions.TabIndex = 5;
            ToolTipMessage.SetToolTip(AugmentationOptions, "Optional per-destination transformation applied before rebroadcast.\r\nApplied at Start; changes do not take effect until the listener is restarted.");
            AugmentationOptions.SelectedIndexChanged += AugmentationOptions_SelectedIndexChanged;
            // 
            // AugmentationSettings
            // 
            AugmentationSettings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            AugmentationSettings.Location = new Point(700, 7);
            AugmentationSettings.Margin = new Padding(4, 3, 4, 3);
            AugmentationSettings.Name = "AugmentationSettings";
            AugmentationSettings.Size = new Size(88, 27);
            AugmentationSettings.TabIndex = 6;
            AugmentationSettings.Text = "Se&ttings...";
            ToolTipMessage.SetToolTip(AugmentationSettings, "Configure the currently-selected augmentation.\r\nDisabled when the augmentation has no settings, or while the listener is running.");
            AugmentationSettings.Click += AugmentationSettings_Click;
            // 
            // ListenOnPortLabel
            // 
            ListenOnPortLabel.Location = new Point(10, 9);
            ListenOnPortLabel.Margin = new Padding(4, 0, 4, 0);
            ListenOnPortLabel.Name = "ListenOnPortLabel";
            ListenOnPortLabel.Size = new Size(149, 23);
            ListenOnPortLabel.TabIndex = 0;
            ListenOnPortLabel.Text = "&Listen on port:";
            ListenOnPortLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // SamplesPerSecLabel
            // 
            SamplesPerSecLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SamplesPerSecLabel.Location = new Point(418, 89);
            SamplesPerSecLabel.Margin = new Padding(4, 0, 4, 0);
            SamplesPerSecLabel.Name = "SamplesPerSecLabel";
            SamplesPerSecLabel.Size = new Size(103, 18);
            SamplesPerSecLabel.TabIndex = 14;
            SamplesPerSecLabel.Text = "samples/second";
            // 
            // SampleRate
            // 
            SampleRate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SampleRate.Location = new Point(381, 89);
            SampleRate.Margin = new Padding(4, 0, 4, 0);
            SampleRate.Name = "SampleRate";
            SampleRate.Size = new Size(36, 18);
            SampleRate.TabIndex = 13;
            SampleRate.Text = "0";
            SampleRate.TextAlign = ContentAlignment.TopRight;
            // 
            // SampleRateLabel
            // 
            SampleRateLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SampleRateLabel.Location = new Point(301, 89);
            SampleRateLabel.Margin = new Padding(4, 0, 4, 0);
            SampleRateLabel.Name = "SampleRateLabel";
            SampleRateLabel.Size = new Size(93, 18);
            SampleRateLabel.TabIndex = 12;
            SampleRateLabel.Text = "Sample Rate:";
            // 
            // SampleCount
            // 
            SampleCount.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            SampleCount.Location = new Point(67, 92);
            SampleCount.Margin = new Padding(4, 0, 4, 0);
            SampleCount.Name = "SampleCount";
            SampleCount.Size = new Size(142, 18);
            SampleCount.TabIndex = 11;
            SampleCount.Text = "0";
            // 
            // SamplesLabel
            // 
            SamplesLabel.Location = new Point(11, 92);
            SamplesLabel.Margin = new Padding(4, 0, 4, 0);
            SamplesLabel.Name = "SamplesLabel";
            SamplesLabel.Size = new Size(56, 18);
            SamplesLabel.TabIndex = 10;
            SamplesLabel.Text = "Samples:";
            // 
            // UDPFrame
            // 
            UDPFrame.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            UDPFrame.BorderStyle = BorderStyle.Fixed3D;
            UDPFrame.FlatStyle = FlatStyle.Flat;
            UDPFrame.Font = new Font("Courier New", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            UDPFrame.Location = new Point(10, 110);
            UDPFrame.Margin = new Padding(4, 0, 4, 0);
            UDPFrame.Name = "UDPFrame";
            UDPFrame.Size = new Size(778, 296);
            UDPFrame.TabIndex = 18;
            UDPFrame.Text = "UDPFrame";
            // 
            // AugmentationLabel
            // 
            AugmentationLabel.Location = new Point(419, 8);
            AugmentationLabel.Margin = new Padding(4, 0, 4, 0);
            AugmentationLabel.Name = "AugmentationLabel";
            AugmentationLabel.Size = new Size(91, 23);
            AugmentationLabel.TabIndex = 4;
            AugmentationLabel.Text = "Au&gmentation:";
            AugmentationLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // UDPRebroadcaster
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(803, 416);
            Controls.Add(AugmentationSettings);
            Controls.Add(AugmentationOptions);
            Controls.Add(AugmentationLabel);
            Controls.Add(ShowData);
            Controls.Add(AutoListen);
            Controls.Add(About);
            Controls.Add(Instructions);
            Controls.Add(RebroadcastDestinations);
            Controls.Add(Port);
            Controls.Add(RebroadcastPortLabel);
            Controls.Add(Status);
            Controls.Add(Listen);
            Controls.Add(ListenOnPortLabel);
            Controls.Add(SamplesPerSecLabel);
            Controls.Add(SampleRate);
            Controls.Add(SampleRateLabel);
            Controls.Add(SampleCount);
            Controls.Add(SamplesLabel);
            Controls.Add(UDPFrame);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MinimumSize = new Size(819, 455);
            Name = "UDPRebroadcaster";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "UDP Rebroadcaster";
            FormClosing += UDPRebroadcaster_FormClosing;
            Load += UDPRebroadcaster_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.CheckBox ShowData;
        internal System.Windows.Forms.CheckBox AutoListen;
        internal System.Windows.Forms.Button About;
        internal System.Windows.Forms.Label Instructions;
        internal System.Windows.Forms.TextBox RebroadcastDestinations;
        internal System.Windows.Forms.TextBox Port;
        internal System.Windows.Forms.Label RebroadcastPortLabel;
        internal System.Windows.Forms.Label Status;
        internal System.Windows.Forms.Button Listen;
        internal System.Windows.Forms.ToolTip ToolTipMessage;
        internal System.Windows.Forms.Label ListenOnPortLabel;
        internal System.Windows.Forms.Label SamplesPerSecLabel;
        internal System.Windows.Forms.Label SampleRate;
        internal System.Windows.Forms.Label SampleRateLabel;
        internal System.Windows.Forms.Label SampleCount;
        internal System.Windows.Forms.Label SamplesLabel;
        internal System.Windows.Forms.Label UDPFrame;
        internal System.Windows.Forms.Label AugmentationLabel;
        internal System.Windows.Forms.ComboBox AugmentationOptions;
        internal System.Windows.Forms.Button AugmentationSettings;
    }
}

