namespace DHM_Main {
	partial class CamView2 {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code (no longer)

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
			this.PlayStopButton = new System.Windows.Forms.ToolStripButton();
			this.CameraButton = new System.Windows.Forms.ToolStripButton();
			this.SettingButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.overExposureButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.LoadButton = new System.Windows.Forms.ToolStripButton();
			this.resetZoomButton = new System.Windows.Forms.ToolStripButton();
			this.ControlStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.ToolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.GainSlider = new System.Windows.Forms.TrackBar();
			this.ExposureSlider = new System.Windows.Forms.TrackBar();
			this.GainSliderHost = new System.Windows.Forms.ToolStripControlHost(GainSlider);
			this.ExposureSliderHost = new System.Windows.Forms.ToolStripControlHost(ExposureSlider);
			this.AutoCheck = new System.Windows.Forms.CheckBox();
			this.AutoCheckHost = new System.Windows.Forms.ToolStripControlHost(AutoCheck);
			// 
			// toolStripContainer1.ContentPanel
			// 
			this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(800, 30);
			this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Top;
			this.toolStripContainer1.LeftToolStripPanelVisible = false;
			this.toolStripContainer1.RightToolStripPanelVisible = false;
			this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
			this.toolStripContainer1.Name = "toolStripContainer1";
			this.toolStripContainer1.Size = new System.Drawing.Size(800, 50);
			this.toolStripContainer1.TabIndex = 0;
			this.toolStripContainer1.Text = "toolStripContainer1";
			// 
			// toolStripContainer1.TopToolStripPanel
			// 
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
			// 
			// PlayStopButton
			// 
			this.PlayStopButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.PlayStopButton.Enabled = false;
			this.PlayStopButton.Image = ((System.Drawing.Image)Properties.Resources.NEXT);
			this.PlayStopButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.PlayStopButton.Name = "PlayStopButton";
			this.PlayStopButton.Size = new System.Drawing.Size(23, 22);
			this.PlayStopButton.Text = "PlayStopButton";
			// 
			// CameraButton
			// 
			this.CameraButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.CameraButton.Image = ((System.Drawing.Image)Properties.Resources.Camera_32xLG);
			this.CameraButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.CameraButton.Name = "CameraButton";
			this.CameraButton.Size = new System.Drawing.Size(23, 22);
			this.CameraButton.Text = "Camera Select";
			// 
			// SettingButton
			// 
			this.SettingButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.SettingButton.Enabled = false;
			this.SettingButton.Image = ((System.Drawing.Image)Properties.Resources.gear_32xLG);
			this.SettingButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.SettingButton.Name = "SettingButton";
			this.SettingButton.Size = new System.Drawing.Size(23, 22);
			this.SettingButton.Text = "Settings";
			// 
			// overExposureButton
			// 
			this.overExposureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.overExposureButton.Image = ((System.Drawing.Image)Properties.Resources.StatusAnnotations_Warning_16xLG_color);
			this.overExposureButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.overExposureButton.Name = "overExposureButton";
			this.overExposureButton.Size = new System.Drawing.Size(23, 22);
			this.overExposureButton.Text = "Show OverExposure";
			this.overExposureButton.Click += new System.EventHandler(this.OverExposureButton_Click);
			// 
			// LoadButton
			// 
			this.LoadButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.LoadButton.Image = ((System.Drawing.Image)Properties.Resources.Folder_Open);
			this.LoadButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.LoadButton.Name = "LoadButton";
			this.LoadButton.Size = new System.Drawing.Size(23, 22);
			this.LoadButton.Text = "Load Image";
			this.LoadButton.ToolTipText = "Load Image";
			//
			//Adding Buttons
			//
			this.toolStrip1.Items.Insert(4, this.overExposureButton);
			this.toolStrip1.Items.Insert(0, this.toolStripSeparator2);
			this.toolStrip1.Items.Insert(0, this.LoadButton);
			this.toolStrip1.Items.Insert(0, this.CameraButton);
			this.toolStrip1.Items.Insert(0, this.SettingButton);
			this.toolStrip1.Items.Insert(0, this.toolStripSeparator3);
			this.toolStrip1.Items.Insert(0, this.PlayStopButton);
			////
			//SELF GENERATED CODE
			////
			this.GainSlider.Name = "gain slider";
			this.GainSlider.AutoSize = false;
			this.GainSlider.Size = new System.Drawing.Size(104, 22);

			this.ExposureSlider.Name = "exposure";
			this.ExposureSlider.AutoSize = false;
			this.ExposureSlider.Size = new System.Drawing.Size(104, 22);

			// 
			// ControlStrip
			// 
			this.ControlStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.ControlStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripLabel1,
			this.GainSliderHost,
			this.ToolStripLabel2,
			this.ExposureSliderHost,
			this.AutoCheckHost});
			this.ControlStrip.Location = new System.Drawing.Point(251, 0);
			this.ControlStrip.Name = "ControlStrip";
			this.ControlStrip.Size = new System.Drawing.Size(306, 25);
			this.ControlStrip.TabIndex = 2;
			this.ControlStrip.Text = "ControlStrip";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(31, 22);
			this.toolStripLabel1.Text = "Gain";
			// 
			// GainSliderHost
			// 
			this.GainSliderHost.Name = "GainSliderHost";
			this.GainSliderHost.Size = new System.Drawing.Size(104, 22);
			this.GainSliderHost.Text = "GainSliderHost";
			// 
			// ToolStripLabel2
			// 
			this.ToolStripLabel2.Name = "ToolStripLabel2";
			this.ToolStripLabel2.Size = new System.Drawing.Size(55, 22);
			this.ToolStripLabel2.Text = "Exposure";
			// 
			// ExposureSliderHost
			// 
			this.ExposureSliderHost.Name = "ExposureSliderHost";
			this.ExposureSliderHost.Size = new System.Drawing.Size(104, 22);
			this.ExposureSliderHost.Text = "ExposureSliderHost";
			//
			//AutoCheckHost
			//
			this.AutoCheckHost.Name = "AutoCheckHost";
			this.AutoCheckHost.Size = new System.Drawing.Size(55, 22);
			this.AutoCheckHost.Text = "AutoCheckHost";
			//
			//AutoCheck
			//
			this.AutoCheck.Name = "AutoChecked";
			this.AutoCheck.Size = new System.Drawing.Size(55, 22);
			this.AutoCheck.Text = "Auto";
			this.AutoCheck.Checked = false;
			this.AutoCheck.Enabled = false;
			//
			// toolStripContainer1.ControlStrip
			//
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.ControlStrip);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			//
			//Form stuff
			//
			this.Controls.Add(this.toolStripContainer1);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Text = "CamView2";
		}

		#endregion

		private System.Windows.Forms.ToolStripContainer toolStripContainer1;
		private System.Windows.Forms.ToolStripButton PlayStopButton;
		private System.Windows.Forms.ToolStripButton CameraButton;
		private System.Windows.Forms.ToolStripButton SettingButton;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
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
		private System.Windows.Forms.ToolStripButton overExposureButton;
	}
}