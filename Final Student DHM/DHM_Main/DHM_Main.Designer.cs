namespace DHM_Main
{
    partial class DHM_Main
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DHM_Main));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.Camera_View_button = new System.Windows.Forms.ToolStripButton();
			this.FTView_Button = new System.Windows.Forms.ToolStripButton();
			this.PhaseView_Button = new System.Windows.Forms.ToolStripButton();
			this.IntensityView_Button = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.propButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.ReconMinTxt = new System.Windows.Forms.ToolStripTextBox();
			this.ReconDistSliderHost = new DHM_Main.ToolStripTraceBarItem();
			this.ReconMaxTxt = new System.Windows.Forms.ToolStripTextBox();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.playStopButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.camSelButton = new System.Windows.Forms.ToolStripButton();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.icImagingControl1 = new TIS.Imaging.ICImagingControl();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Camera_View_button,
            this.FTView_Button,
            this.PhaseView_Button,
            this.IntensityView_Button,
            this.toolStripSeparator1,
            this.propButton,
            this.toolStripSeparator4,
            this.ReconMinTxt,
            this.ReconDistSliderHost,
            this.ReconMaxTxt,
            this.toolStripSeparator2,
            this.toolStripLabel1,
            this.playStopButton,
            this.toolStripSeparator3,
            this.camSelButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 24);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(800, 28);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "ToolBar";
			// 
			// Camera_View_button
			// 
			this.Camera_View_button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.Camera_View_button.Image = ((System.Drawing.Image)(resources.GetObject("Camera_View_button.Image")));
			this.Camera_View_button.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Camera_View_button.Name = "Camera_View_button";
			this.Camera_View_button.Size = new System.Drawing.Size(23, 25);
			this.Camera_View_button.Text = "Camera View";
			this.Camera_View_button.Click += new System.EventHandler(this.Camera_View_button_Click);
			// 
			// FTView_Button
			// 
			this.FTView_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.FTView_Button.Image = global::DHM_Main.Properties.Resources.F;
			this.FTView_Button.ImageTransparentColor = System.Drawing.Color.White;
			this.FTView_Button.Name = "FTView_Button";
			this.FTView_Button.Size = new System.Drawing.Size(23, 25);
			this.FTView_Button.Text = "toolStripButton1";
			this.FTView_Button.Click += new System.EventHandler(this.ToolStripButton1_Click);
			// 
			// PhaseView_Button
			// 
			this.PhaseView_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.PhaseView_Button.Image = global::DHM_Main.Properties.Resources.P;
			this.PhaseView_Button.ImageTransparentColor = System.Drawing.Color.White;
			this.PhaseView_Button.Name = "PhaseView_Button";
			this.PhaseView_Button.Size = new System.Drawing.Size(23, 25);
			this.PhaseView_Button.Text = "toolStripButton2";
			this.PhaseView_Button.Click += new System.EventHandler(this.ToolStripButton2_Click);
			// 
			// IntensityView_Button
			// 
			this.IntensityView_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.IntensityView_Button.Image = global::DHM_Main.Properties.Resources.I;
			this.IntensityView_Button.ImageTransparentColor = System.Drawing.Color.White;
			this.IntensityView_Button.Name = "IntensityView_Button";
			this.IntensityView_Button.Size = new System.Drawing.Size(23, 25);
			this.IntensityView_Button.Text = "toolStripButton3";
			this.IntensityView_Button.Click += new System.EventHandler(this.ToolStripButton3_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 28);
			// 
			// propButton
			// 
			this.propButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.propButton.Image = ((System.Drawing.Image)(resources.GetObject("propButton.Image")));
			this.propButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.propButton.Name = "propButton";
			this.propButton.Size = new System.Drawing.Size(23, 25);
			this.propButton.Text = "Properties Button";
			this.propButton.Click += new System.EventHandler(this.PropButton_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 28);
			// 
			// ReconMinTxt
			// 
			this.ReconMinTxt.Name = "ReconMinTxt";
			this.ReconMinTxt.Size = new System.Drawing.Size(100, 28);
			this.ReconMinTxt.Text = "-300";
			this.ReconMinTxt.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.ReconMinTxt.Leave += new System.EventHandler(this.ReconMinTxt_Leave);
			// 
			// ReconDistSliderHost
			// 
			this.ReconDistSliderHost.Name = "ReconDistSliderHost";
			this.ReconDistSliderHost.Size = new System.Drawing.Size(104, 25);
			this.ReconDistSliderHost.Text = "Reconstruction Distance";
			// 
			// ReconMaxTxt
			// 
			this.ReconMaxTxt.Name = "ReconMaxTxt";
			this.ReconMaxTxt.Size = new System.Drawing.Size(100, 28);
			this.ReconMaxTxt.Text = "300";
			this.ReconMaxTxt.Leave += new System.EventHandler(this.ReconMaxTxt_Leave);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 28);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(168, 25);
			this.toolStripLabel1.Text = "Reconstrustion Distance: 0mm";
			// 
			// playStopButton
			// 
			this.playStopButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.playStopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.playStopButton.Image = global::DHM_Main.Properties.Resources.NEXT;
			this.playStopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.playStopButton.Name = "playStopButton";
			this.playStopButton.Size = new System.Drawing.Size(23, 25);
			this.playStopButton.Text = "Play/Stop Button";
			this.playStopButton.Click += new System.EventHandler(this.PlayStopButton_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 28);
			// 
			// camSelButton
			// 
			this.camSelButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.camSelButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.camSelButton.Image = ((System.Drawing.Image)(resources.GetObject("camSelButton.Image")));
			this.camSelButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.camSelButton.Name = "camSelButton";
			this.camSelButton.Size = new System.Drawing.Size(23, 25);
			this.camSelButton.Text = "Select Camera";
			this.camSelButton.Click += new System.EventHandler(this.CameraButton_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(800, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// icImagingControl1
			// 
			this.icImagingControl1.BackColor = System.Drawing.Color.White;
			this.icImagingControl1.DeviceListChangedExecutionMode = TIS.Imaging.EventExecutionMode.Invoke;
			this.icImagingControl1.DeviceLostExecutionMode = TIS.Imaging.EventExecutionMode.AsyncInvoke;
			this.icImagingControl1.ImageAvailableExecutionMode = TIS.Imaging.EventExecutionMode.MultiThreaded;
			this.icImagingControl1.ImageRingBufferSize = 0;
			this.icImagingControl1.LiveCaptureLastImage = false;
			this.icImagingControl1.LiveDisplay = false;
			this.icImagingControl1.LiveDisplayDefault = false;
			this.icImagingControl1.LiveDisplayPosition = new System.Drawing.Point(0, 0);
			this.icImagingControl1.LiveShowLastBuffer = false;
			this.icImagingControl1.Location = new System.Drawing.Point(0, 52);
			this.icImagingControl1.Name = "icImagingControl1";
			this.icImagingControl1.Size = new System.Drawing.Size(800, 50);
			this.icImagingControl1.TabIndex = 4;
			this.icImagingControl1.TabStop = false;
			this.icImagingControl1.Visible = false;
			// 
			// DHM_Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.icImagingControl1);
			this.Controls.Add(this.toolStrip1);
			this.Controls.Add(this.menuStrip1);
			this.IsMdiContainer = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "DHM_Main";
			this.Text = "DHM Main Window";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DHM_Main_FormClosing);
			this.Shown += new System.EventHandler(this.DHM_Main_Shown);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.icImagingControl1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton Camera_View_button;
        private System.Windows.Forms.MenuStrip menuStrip1;
		private TIS.Imaging.ICImagingControl icImagingControl1;
		private System.Windows.Forms.ToolStripButton FTView_Button;
		private System.Windows.Forms.ToolStripButton PhaseView_Button;
		private System.Windows.Forms.ToolStripButton IntensityView_Button;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripTextBox ReconMinTxt;
		private DHM_Main.ToolStripTraceBarItem ReconDistSliderHost;
		private System.Windows.Forms.ToolStripTextBox ReconMaxTxt;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripLabel toolStripLabel1;
		private System.Windows.Forms.ToolStripButton playStopButton;
		private System.Windows.Forms.ToolStripButton camSelButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton propButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
	}
}

