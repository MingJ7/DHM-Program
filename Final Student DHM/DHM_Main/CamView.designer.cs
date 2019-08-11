namespace DHM_Main
{
    partial class CamView
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CamView));
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.Display = new Emgu.CV.UI.ImageBox();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.PlayButton = new System.Windows.Forms.ToolStripButton();
			this.StopButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.CameraButton = new System.Windows.Forms.ToolStripButton();
			this.SettingButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.overExposureButton = new System.Windows.Forms.ToolStripButton();
			this.ColormapButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.SaveButton = new System.Windows.Forms.ToolStripButton();
			this.LoadButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.resetZoomButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripContainer1.ContentPanel.SuspendLayout();
			this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
			this.toolStripContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.Display)).BeginInit();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// toolStripContainer1
			// 
			this.toolStripContainer1.BottomToolStripPanelVisible = false;
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Controls.Add(this.Display);
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(800, 425);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.toolStripContainer1.LeftToolStripPanelVisible = false;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.RightToolStripPanelVisible = false;
			this.toolStripContainer1.Size = new System.Drawing.Size(800, 450);
			this.toolStripContainer1.TabIndex = 0;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// Display
			// 
			this.Display.Dock = System.Windows.Forms.DockStyle.Fill;
			this.Display.Location = new System.Drawing.Point(0, 0);
			this.Display.Name = "Display";
			this.Display.Size = new System.Drawing.Size(800, 425);
			this.Display.TabIndex = 2;
			this.Display.TabStop = false;
			this.Display.OnZoomScaleChange += new System.EventHandler(this.ImageBox_OnZoomScaleChange);
			this.Display.Resize += new System.EventHandler(this.ImageBox_Resize);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PlayButton,
            this.StopButton,
            this.toolStripSeparator1,
            this.CameraButton,
            this.SettingButton,
            this.toolStripSeparator2,
            this.overExposureButton,
            this.ColormapButton,
            this.toolStripSeparator3,
            this.SaveButton,
            this.LoadButton,
            this.toolStripSeparator4,
            this.resetZoomButton});
			this.toolStrip1.Location = new System.Drawing.Point(3, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(243, 25);
			this.toolStrip1.TabIndex = 0;
			// 
			// PlayButton
			// 
			this.PlayButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.PlayButton.Enabled = false;
			this.PlayButton.Image = ((System.Drawing.Image)(resources.GetObject("PlayButton.Image")));
			this.PlayButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.PlayButton.Name = "PlayButton";
			this.PlayButton.Size = new System.Drawing.Size(23, 22);
			this.PlayButton.Text = "PlayButton";
			// 
			// StopButton
			// 
			this.StopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.StopButton.Enabled = false;
			this.StopButton.Image = ((System.Drawing.Image)(resources.GetObject("StopButton.Image")));
			this.StopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.StopButton.Name = "StopButton";
			this.StopButton.Size = new System.Drawing.Size(23, 22);
			this.StopButton.Text = "StopButton";
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// CameraButton
			// 
			this.CameraButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.CameraButton.Image = ((System.Drawing.Image)(resources.GetObject("CameraButton.Image")));
			this.CameraButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CameraButton.Name = "CameraButton";
			this.CameraButton.Size = new System.Drawing.Size(23, 22);
			this.CameraButton.Text = "Camera Select";
			// 
			// SettingButton
			// 
			this.SettingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SettingButton.Enabled = false;
			this.SettingButton.Image = ((System.Drawing.Image)(resources.GetObject("SettingButton.Image")));
			this.SettingButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SettingButton.Name = "SettingButton";
			this.SettingButton.Size = new System.Drawing.Size(23, 22);
			this.SettingButton.Text = "Settings";
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// overExposureButton
			// 
			this.overExposureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.overExposureButton.Image = ((System.Drawing.Image)(resources.GetObject("overExposureButton.Image")));
			this.overExposureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.overExposureButton.Name = "overExposureButton";
			this.overExposureButton.Size = new System.Drawing.Size(23, 22);
			this.overExposureButton.Text = "Show OverExposure";
			this.overExposureButton.Click += new System.EventHandler(this.OverExposureButton_Click);
			// 
			// ColormapButton
			// 
			this.ColormapButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.ColormapButton.Image = ((System.Drawing.Image)(resources.GetObject("ColormapButton.Image")));
			this.ColormapButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.ColormapButton.Name = "ColormapButton";
			this.ColormapButton.Size = new System.Drawing.Size(23, 22);
			this.ColormapButton.Text = "Color Map";
			this.ColormapButton.Click += new System.EventHandler(this.ColormapButton_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// SaveButton
			// 
			this.SaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SaveButton.Enabled = false;
			this.SaveButton.Image = ((System.Drawing.Image)(resources.GetObject("SaveButton.Image")));
			this.SaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SaveButton.Name = "SaveButton";
			this.SaveButton.Size = new System.Drawing.Size(23, 22);
			this.SaveButton.Text = "Save Image";
			// 
			// LoadButton
			// 
			this.LoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.LoadButton.Image = ((System.Drawing.Image)(resources.GetObject("LoadButton.Image")));
			this.LoadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(23, 22);
			this.LoadButton.Text = "Load Image";
			this.LoadButton.ToolTipText = "Load Image";
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// resetZoomButton
			// 
			this.resetZoomButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.resetZoomButton.Image = ((System.Drawing.Image)(resources.GetObject("resetZoomButton.Image")));
			this.resetZoomButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.resetZoomButton.Name = "resetZoomButton";
			this.resetZoomButton.Size = new System.Drawing.Size(23, 22);
			this.resetZoomButton.Text = "Reset Zoom";
			this.resetZoomButton.Click += new System.EventHandler(this.ResetZoomButton_Click);
			// 
			// CamView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.toolStripContainer1);
			this.Name = "CamView";
			this.Text = "Camera View";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CamView_FormClosing);
			this.toolStripContainer1.ContentPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
			this.toolStripContainer1.TopToolStripPanel.PerformLayout();
			this.toolStripContainer1.ResumeLayout(false);
			this.toolStripContainer1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.Display)).EndInit();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton PlayButton;
        private System.Windows.Forms.ToolStripButton StopButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton CameraButton;
        private System.Windows.Forms.ToolStripButton SettingButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton ColormapButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton SaveButton;
        private System.Windows.Forms.ToolStripButton LoadButton;
        //Self Added
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripLabel ToolStripLabel2;
        private System.Windows.Forms.ToolStrip ControlStrip;
        private System.Windows.Forms.TrackBar GainSlider;
        private System.Windows.Forms.TrackBar ExposureSlider;
        private System.Windows.Forms.ToolStripControlHost GainSliderHost;
        private System.Windows.Forms.ToolStripControlHost ExposureSliderHost;
        private System.Windows.Forms.ToolStripControlHost AutoCheckHost;
        private System.Windows.Forms.CheckBox AutoCheck;
		private Emgu.CV.UI.ImageBox Display;
		private System.Windows.Forms.ToolStripButton overExposureButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripButton resetZoomButton;
	}
}
/*
            ////
            //SELF GENERATED CODE
            ////
            this.GainSlider = new System.Windows.Forms.TrackBar();
            this.ExposureSlider = new System.Windows.Forms.TrackBar();
            this.GainSliderHost = new System.Windows.Forms.ToolStripControlHost(GainSlider);
            this.ExposureSliderHost = new System.Windows.Forms.ToolStripControlHost(ExposureSlider);
            this.GainSlider.Name = "gain slider";
            this.GainSlider.AutoSize = false;
            this.GainSlider.Size = new System.Drawing.Size(104, 22);
            this.ExposureSlider.Name = "exposure";
            this.ExposureSlider.AutoSize = false;
            this.ExposureSlider.Size = new System.Drawing.Size(104, 22);
*/