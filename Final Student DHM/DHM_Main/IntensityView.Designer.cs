﻿namespace DHM_Main {
	partial class IntensityView {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(IntensityView));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.saveButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.resetZoomButton = new System.Windows.Forms.ToolStripButton();
			this.imageBox1 = new Emgu.CV.UI.ImageBox();
			this.button1 = new System.Windows.Forms.Button();
			this.ColormapButton = new System.Windows.Forms.ToolStripButton();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton,
            this.toolStripSeparator2,
            this.resetZoomButton,
            this.ColormapButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(800, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// saveButton
			// 
			this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
			this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(23, 22);
			this.saveButton.Text = "Save Intensity View";
			this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
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
			// imageBox1
			// 
			this.imageBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.imageBox1.Location = new System.Drawing.Point(0, 25);
			this.imageBox1.Name = "imageBox1";
			this.imageBox1.Size = new System.Drawing.Size(800, 425);
			this.imageBox1.TabIndex = 2;
			this.imageBox1.TabStop = false;
			this.imageBox1.OnZoomScaleChange += new System.EventHandler(this.ImageBox_OnZoomScaleChange);
			this.imageBox1.Resize += new System.EventHandler(this.ImageBox_Resize);
			// 
			// button1
			// 
			this.button1.Dock = System.Windows.Forms.DockStyle.Right;
			this.button1.Location = new System.Drawing.Point(772, 25);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(28, 425);
			this.button1.TabIndex = 3;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
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
			// IntensityView
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.imageBox1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "IntensityView";
			this.Text = "IntensityView";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton saveButton;
		private Emgu.CV.UI.ImageBox imageBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton resetZoomButton;
		private System.Windows.Forms.ToolStripButton ColormapButton;
	}
}