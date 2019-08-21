namespace DHM_Main {
	partial class FTView2 {
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

		#region Windows Form Designer generated code (not really, not anymore)

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.Rect_Button = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.Text = "FTView1";
			// 
			// Rect_Button
			// 
			this.Rect_Button.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.Rect_Button.Image = ((System.Drawing.Image)(Properties.Resources.rounded_Rectangle_16xLG));
			this.Rect_Button.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.Rect_Button.Name = "Rect_Button";
			this.Rect_Button.Size = new System.Drawing.Size(23, 22);
			this.Rect_Button.Text = "Rectangle Select";
			this.Rect_Button.Click += new System.EventHandler(this.FFTRectSelToolStripButton_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			//
			// toolstrip1 UPDATE
			//
			this.toolStrip1.Items.Insert(0, toolStripSeparator2);
			this.toolStrip1.Items.Insert(0, Rect_Button);
			//
			// imageBox1 UPDATE
			//
			this.imageBox1.MouseDown += ImageBox2_MouseDown;
			this.imageBox1.MouseMove += ImageBox2_MouseMove;
			this.imageBox1.MouseUp += ImageBox2_MouseUp;
			this.imageBox1.Paint += ImageBoxRectangleSelect_Paint;
		}

		#endregion

		private System.Windows.Forms.ToolStripButton Rect_Button;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;

	}
}