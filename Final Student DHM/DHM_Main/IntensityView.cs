using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.UI;

namespace DHM_Main {
	public partial class IntensityView : Form {
		private bool showColorMap=false;

		public IntensityView() {
			InitializeComponent();
		}
		public void Display_Image(Emgu.CV.UMat inImage) {
			//if inImage is empty, do nothing
			if (inImage == null) return;
			else {
				//If Function is called on non-UI thread
				if (imageBox1.InvokeRequired) {
					if (!showColorMap) {
						//send img to UI thred for updating
						imageBox1.Invoke(new MethodInvoker(() => Display_Image(inImage.Clone())));
					}

					else if (showColorMap) {
						//Create holder for colormapped img
						UMat dispImg = new UMat(inImage.Size, DepthType.Cv8U, 3);
						//Make colormap image
						CvInvoke.ApplyColorMap(inImage, dispImg, ColorMapType.Jet);
						imageBox1.Invoke(new MethodInvoker(() => Display_Image(dispImg.Clone())));
						dispImg.Dispose();
					}
				}
				else {
					//UI thread only Updates image
					IImage old = imageBox1.Image;
					imageBox1.Image = inImage;
					//imageBox1.Invalidate();
					//imageBox1.Update();
					//if there is old image, Dispose of it
					if (old != null) old.Dispose();
				}
			}
		}
		public void Display_Stop() {
			if (imageBox1.Image != null) imageBox1.Image.Dispose();
			System.Threading.Monitor.Enter(imageBox1);
			try {
				imageBox1.Image = null;
			}
			finally {
				System.Threading.Monitor.Exit(imageBox1);
			}
		}
		private void ImageBox_OnZoomScaleChange(object sender, EventArgs e) {
			/// please have only imageBoxes send events here, 
			/// and please set imageBoxes' SizeModes to "Normal",
			/// or else they bug out(,other than AutoSize).
			FitImageToImageBox((ImageBox)sender, onlyIfTiny: true);
		}
		private void ImageBox_Resize(object sender, EventArgs e) {
			FitImageToImageBox((ImageBox)sender, onlyIfTiny: true);
		}
		public void FitImageToImageBox(ImageBox imgBox, bool onlyIfTiny = false) {
			/// Fits the image to the control size but without stretching, and that the entire image is in view.
			/// If onlyIfTiny is true, only fit the image to the control if image is smaller than control. 
			/// Should only be called on imageBox Resize or ZoomScaleChange.
			/// 
			/// In an ImageBox, DisplayedImage is Image but with user-defined operations applied to it.
			/// The size you see is the display image size multiplied by ZoomScale
			/// 
			/// ZoomScale = ImageSize_After/ImageSize_Original
			/// 
			/// Derivation: If you scale the width of the original image such that it matches the control width,
			/// and similarly with height, you get 2 ZoomScales(or 2 scaledSizes). 
			/// One makes the scaled image touch the sides of the control along its length, the other,
			/// the same but along its width. How to choose?
			/// Simply put, the smaller one. Because that's the size that fits entirely in the control's size.
			if (imgBox.DisplayedImage == null) {
				return;
			}
			double zoomScaleByWidth = (double)imgBox.Width / imgBox.DisplayedImage.Size.Width;
			double zoomScaleByHeight = (double)imgBox.Height / imgBox.DisplayedImage.Size.Height;
			double fitZoomScale = Math.Min(zoomScaleByWidth, zoomScaleByHeight);

			//only fit image to control if scaled image fits entirely inside the control, and not already 
			//perfectly fit
			if (onlyIfTiny && imgBox.ZoomScale >= fitZoomScale) {
				return;
			}

			imgBox.SetZoomScale(fitZoomScale, new Point(0, 0));
		}
		private void SaveButton_Click(object sender, EventArgs e) {
			// Call the save file dialog to enter the file name of the image
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.jfif, *.png *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp | All files | *.* ";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
				// Save the image.
				imageBox1.Image.Save(saveFileDialog1.FileName);
			}
		}

		private void ResetZoomButton_Click(object sender, EventArgs e) {
			imageBox1.SetZoomScale(1d, new Point(0, 0));

		}
		private void ColormapButton_Click(object sender, EventArgs e) {
			ToolStripButton button = (ToolStripButton)sender;
			showColorMap = !showColorMap;
			if (showColorMap) {
				button.BackColor = Color.Chartreuse;
			}
			else {
				button.BackColor = SystemColors.Control; //used to be GhostWhite
			}
		}

	}
}
