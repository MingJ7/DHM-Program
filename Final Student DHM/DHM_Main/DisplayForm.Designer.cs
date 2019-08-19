namespace DHM_Main {
	partial class DisplayForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisplayForm));
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.imageBox1 = new Emgu.CV.UI.ImageBox();
			this.colMapButton = new System.Windows.Forms.ToolStripButton();
			this.resetZoomButton = new System.Windows.Forms.ToolStripButton();
			this.saveButton = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).BeginInit();
			this.SuspendLayout();
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton,
            this.toolStripSeparator1,
            this.resetZoomButton,
            this.colMapButton});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(800, 25);
			this.toolStrip1.TabIndex = 0;
			this.toolStrip1.Text = "toolStrip1";
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
			// colMapButton
			// 
			this.colMapButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.colMapButton.Image = ((System.Drawing.Image)(resources.GetObject("colMapButton.Image")));
			this.colMapButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.colMapButton.Name = "colMapButton";
			this.colMapButton.Size = new System.Drawing.Size(23, 22);
			this.colMapButton.Text = "toolStripButton1";
			this.colMapButton.Click += new System.EventHandler(this.ColMapButton_Click);
			// 
			// resetZoomButton
			// 
			this.resetZoomButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.resetZoomButton.Image = ((System.Drawing.Image)(resources.GetObject("resetZoomButton.Image")));
			this.resetZoomButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.resetZoomButton.Name = "resetZoomButton";
			this.resetZoomButton.Size = new System.Drawing.Size(23, 22);
			this.resetZoomButton.Text = "toolStripButton2";
			this.resetZoomButton.Click += new System.EventHandler(this.ResetZoomButton_Click);
			// 
			// saveButton
			// 
			this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
			this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(23, 22);
			this.saveButton.Text = "toolStripButton1";
			this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// DisplayForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.imageBox1);
			this.Controls.Add(this.toolStrip1);
			this.Name = "DisplayForm";
			this.Text = "DisplayForm";
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.imageBox1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		protected System.Windows.Forms.ToolStrip toolStrip1;
		protected System.Windows.Forms.ToolStripButton colMapButton;
		protected Emgu.CV.UI.ImageBox imageBox1;
		protected System.Windows.Forms.ToolStripButton resetZoomButton;
		protected System.Windows.Forms.ToolStripButton saveButton;
		protected System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
	}
}