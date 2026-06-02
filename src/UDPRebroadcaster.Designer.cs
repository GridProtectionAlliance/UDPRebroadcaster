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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UDPRebroadcaster));
            this.ShowData = new System.Windows.Forms.CheckBox();
            this.AutoListen = new System.Windows.Forms.CheckBox();
            this.About = new System.Windows.Forms.Button();
            this.Instructions = new System.Windows.Forms.Label();
            this.RebroadcastDestinations = new System.Windows.Forms.TextBox();
            this.Port = new System.Windows.Forms.TextBox();
            this.RebroadcastPortLabel = new System.Windows.Forms.Label();
            this.Status = new System.Windows.Forms.Label();
            this.Listen = new System.Windows.Forms.Button();
            this.ToolTipMessage = new System.Windows.Forms.ToolTip(this.components);
            this.ListenOnPortLabel = new System.Windows.Forms.Label();
            this.SamplesPerSecLabel = new System.Windows.Forms.Label();
            this.SampleRate = new System.Windows.Forms.Label();
            this.SampleRateLabel = new System.Windows.Forms.Label();
            this.SampleCount = new System.Windows.Forms.Label();
            this.SamplesLabel = new System.Windows.Forms.Label();
            this.UDPFrame = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ShowData
            // 
            this.ShowData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowData.Location = new System.Drawing.Point(458, 80);
            this.ShowData.Name = "ShowData";
            this.ShowData.Size = new System.Drawing.Size(80, 16);
            this.ShowData.TabIndex = 33;
            this.ShowData.Text = "&Show data";
            this.ShowData.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            this.ToolTipMessage.SetToolTip(this.ShowData, "Enabling data display may lower data retransmission rate.");
            this.ShowData.CheckedChanged += new System.EventHandler(this.ShowData_CheckedChanged);
            // 
            // AutoListen
            // 
            this.AutoListen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AutoListen.Location = new System.Drawing.Point(321, 80);
            this.AutoListen.Name = "AutoListen";
            this.AutoListen.Size = new System.Drawing.Size(137, 16);
            this.AutoListen.TabIndex = 21;
            this.AutoListen.Text = "A&uto listen on start-up";
            this.AutoListen.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // About
            // 
            this.About.Location = new System.Drawing.Point(281, 8);
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(75, 23);
            this.About.TabIndex = 23;
            this.About.Text = "&About...";
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // Instructions
            // 
            this.Instructions.Location = new System.Drawing.Point(145, 56);
            this.Instructions.Name = "Instructions";
            this.Instructions.Size = new System.Drawing.Size(392, 21);
            this.Instructions.TabIndex = 25;
            this.Instructions.Text = "Format =  IP1:port, IP2:port, IP3:port - etc.";
            // 
            // RebroadcastDestinations
            // 
            this.RebroadcastDestinations.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RebroadcastDestinations.Location = new System.Drawing.Point(145, 32);
            this.RebroadcastDestinations.Name = "RebroadcastDestinations";
            this.RebroadcastDestinations.Size = new System.Drawing.Size(397, 20);
            this.RebroadcastDestinations.TabIndex = 20;
            this.RebroadcastDestinations.Text = "127.0.0.1:3060, 127.0.0.1:3070";
            // 
            // Port
            // 
            this.Port.Location = new System.Drawing.Point(145, 8);
            this.Port.Name = "Port";
            this.Port.Size = new System.Drawing.Size(48, 20);
            this.Port.TabIndex = 18;
            this.Port.Text = "3050";
            // 
            // RebroadcastPortLabel
            // 
            this.RebroadcastPortLabel.Location = new System.Drawing.Point(1, 32);
            this.RebroadcastPortLabel.Name = "RebroadcastPortLabel";
            this.RebroadcastPortLabel.Size = new System.Drawing.Size(136, 20);
            this.RebroadcastPortLabel.TabIndex = 19;
            this.RebroadcastPortLabel.Text = "&Rebroadcast destinations:";
            this.RebroadcastPortLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Status
            // 
            this.Status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Status.Location = new System.Drawing.Point(361, 8);
            this.Status.Name = "Status";
            this.Status.Size = new System.Drawing.Size(181, 24);
            this.Status.TabIndex = 24;
            this.Status.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // Listen
            // 
            this.Listen.Location = new System.Drawing.Point(201, 8);
            this.Listen.Name = "Listen";
            this.Listen.Size = new System.Drawing.Size(75, 23);
            this.Listen.TabIndex = 22;
            this.Listen.Text = "&Start";
            this.Listen.Click += new System.EventHandler(this.Listen_Click);
            // 
            // ToolTipMessage
            // 
            this.ToolTipMessage.AutoPopDelay = 60000;
            this.ToolTipMessage.InitialDelay = 1;
            this.ToolTipMessage.ReshowDelay = 1;
            this.ToolTipMessage.ShowAlways = true;
            // 
            // ListenOnPortLabel
            // 
            this.ListenOnPortLabel.Location = new System.Drawing.Point(9, 8);
            this.ListenOnPortLabel.Name = "ListenOnPortLabel";
            this.ListenOnPortLabel.Size = new System.Drawing.Size(128, 20);
            this.ListenOnPortLabel.TabIndex = 17;
            this.ListenOnPortLabel.Text = "&Listen on port:";
            this.ListenOnPortLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SamplesPerSecLabel
            // 
            this.SamplesPerSecLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SamplesPerSecLabel.Location = new System.Drawing.Point(222, 80);
            this.SamplesPerSecLabel.Name = "SamplesPerSecLabel";
            this.SamplesPerSecLabel.Size = new System.Drawing.Size(88, 16);
            this.SamplesPerSecLabel.TabIndex = 30;
            this.SamplesPerSecLabel.Text = "samples/second";
            // 
            // SampleRate
            // 
            this.SampleRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SampleRate.Location = new System.Drawing.Point(190, 80);
            this.SampleRate.Name = "SampleRate";
            this.SampleRate.Size = new System.Drawing.Size(31, 16);
            this.SampleRate.TabIndex = 29;
            this.SampleRate.Text = "0";
            this.SampleRate.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // SampleRateLabel
            // 
            this.SampleRateLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SampleRateLabel.Location = new System.Drawing.Point(122, 80);
            this.SampleRateLabel.Name = "SampleRateLabel";
            this.SampleRateLabel.Size = new System.Drawing.Size(80, 16);
            this.SampleRateLabel.TabIndex = 28;
            this.SampleRateLabel.Text = "Sample Rate:";
            // 
            // SampleCount
            // 
            this.SampleCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SampleCount.Location = new System.Drawing.Point(54, 80);
            this.SampleCount.Name = "SampleCount";
            this.SampleCount.Size = new System.Drawing.Size(68, 16);
            this.SampleCount.TabIndex = 27;
            this.SampleCount.Text = "0";
            // 
            // SamplesLabel
            // 
            this.SamplesLabel.Location = new System.Drawing.Point(6, 80);
            this.SamplesLabel.Name = "SamplesLabel";
            this.SamplesLabel.Size = new System.Drawing.Size(48, 16);
            this.SamplesLabel.TabIndex = 26;
            this.SamplesLabel.Text = "Samples:";
            // 
            // UDPFrame
            // 
            this.UDPFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UDPFrame.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.UDPFrame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.UDPFrame.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UDPFrame.Location = new System.Drawing.Point(9, 104);
            this.UDPFrame.Name = "UDPFrame";
            this.UDPFrame.Size = new System.Drawing.Size(533, 254);
            this.UDPFrame.TabIndex = 31;
            this.UDPFrame.Text = "UDPFrame";
            // 
            // UDPRebroadcaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 367);
            this.Controls.Add(this.ShowData);
            this.Controls.Add(this.AutoListen);
            this.Controls.Add(this.About);
            this.Controls.Add(this.Instructions);
            this.Controls.Add(this.RebroadcastDestinations);
            this.Controls.Add(this.Port);
            this.Controls.Add(this.RebroadcastPortLabel);
            this.Controls.Add(this.Status);
            this.Controls.Add(this.Listen);
            this.Controls.Add(this.ListenOnPortLabel);
            this.Controls.Add(this.SamplesPerSecLabel);
            this.Controls.Add(this.SampleRate);
            this.Controls.Add(this.SampleRateLabel);
            this.Controls.Add(this.SampleCount);
            this.Controls.Add(this.SamplesLabel);
            this.Controls.Add(this.UDPFrame);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(570, 375);
            this.Name = "UDPRebroadcaster";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDP Rebroadcaster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UDPRebroadcaster_FormClosing);
            this.Load += new System.EventHandler(this.UDPRebroadcaster_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

