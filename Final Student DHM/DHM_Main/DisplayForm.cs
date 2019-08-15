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
using Emgu.CV.UI;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
namespace DHM_Main {
	public partial class DisplayForm : Form {
		bool showColorMap = false;
		protected UMat disp_1,disp_2;
		~DisplayForm(){
			disp_1?.Dispose();
			disp_2?.Dispose();
		}
		public DisplayForm() {
			InitializeComponent();
		}
		public virtual void dispImg(UMat inImg){
			UMat toUpdate = imageBox1.Image == disp_1 ? disp_2 : disp_1;
			if(!showColorMap){
				inImg.CopyTo(toUpdate);
			}else{
				CvInvoke.ApplyColorMap(inImg, toUpdate, ColorMapType.Jet);
			}
			if (imageBox1.InvokeRequired) {
				imageBox1.Invoke(new MethodInvoker(() => imageBox1.Image = toUpdate));
			}
			else {
				imageBox1.Image = toUpdate;
			}
		}
		public virtual void updateImgSize(Size size){
			if (showColorMap) {
				disp_1.Create(size.Height, size.Width, DepthType.Cv8U, 3);
				disp_2.Create(size.Height, size.Width, DepthType.Cv8U, 3);
			}
			else {
				disp_1.Create(size.Height, size.Width, DepthType.Cv8U, 1);
				disp_2.Create(size.Height, size.Width, DepthType.Cv8U, 1);
			}
		}
		protected virtual void ColEnabledChanged(bool color){
			if (color) {
				disp_1.Create(disp_1.Rows, disp_1.Cols, DepthType.Cv8U, 3);
				disp_2.Create(disp_2.Rows, disp_2.Cols, DepthType.Cv8U, 3);
			}
			else {
				disp_1.Create(disp_1.Rows, disp_1.Cols, DepthType.Cv8U, 1);
				disp_2.Create(disp_2.Rows, disp_2.Cols, DepthType.Cv8U, 1);
			}
		}
		protected virtual void FitImageToImageBox(ImageBox imgBox, bool onlyIfTiny = false) {
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

		private void ImageBox_OnZoomScaleChange(object sender, EventArgs e) {
			/// please have only imageBoxes send events here, 
			/// and please set imageBoxes' SizeModes to "Normal",
			/// or else they bug out(,other than AutoSize).
			FitImageToImageBox((ImageBox)sender, onlyIfTiny: true);
		}
		private void ImageBox_Resize(object sender, EventArgs e) {
			FitImageToImageBox((ImageBox)sender, onlyIfTiny: true);
		}
		private void ResetZoomButton_Click(object sender, EventArgs e) {
			imageBox1.SetZoomScale(1d, new Point(0, 0));
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

	}
}
