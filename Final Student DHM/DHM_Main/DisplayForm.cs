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
		public event EventHandler<EventArgs> ReloadRequiredHandler;
		protected bool showColorMap = false;
		protected UMat disp_1 = new UMat(); 
		protected UMat disp_2 = new UMat();
		private byte run1disp_1, run1disp_2;
		~DisplayForm() {
			disp_1?.Dispose();
			disp_2?.Dispose();
		}
		public DisplayForm() {
			InitializeComponent();
		}
		public virtual void dispImg(UMat inImg) {
			UMat toUpdate = (imageBox1.Image == disp_1) ? disp_2 : disp_1;
			if (!showColorMap) {
				inImg.CopyTo(toUpdate);
			}
			else {
				CvInvoke.ApplyColorMap(inImg, toUpdate, ColorMapType.Jet);
			}
			if(run1disp_1==0||run1disp_2==0){
				run1disp_1 = disp_1.Bytes[0];
				run1disp_2 = disp_2.Bytes[0];
			}
			imageBox1.Image = toUpdate;
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
		internal virtual void AddSaveRawEH(EventHandler eh){
			SaveRawButton.Click += eh;
		}
		internal virtual void RemoveSaveRawEH(EventHandler eh) {
			SaveRawButton.Click -= eh;
		}
		protected virtual void ColEnabledChanged(){
			if (showColorMap) {
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
		protected virtual void chgToolStripbuttonBG(ToolStripButton button,bool enabled){
			if (enabled) {
				button.BackColor = Color.Chartreuse;
			}
			else {
				button.BackColor = SystemColors.Control; //used to be GhostWhite
			}
		}
		protected void InvokeReload(){
			ReloadRequiredHandler?.Invoke(this, new EventArgs());
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

		private void ColMapButton_Click(object sender, EventArgs e) {
			showColorMap = !showColorMap;
			chgToolStripbuttonBG((ToolStripButton)sender, showColorMap);
			ColEnabledChanged();
			ReloadRequiredHandler?.Invoke(this, new EventArgs());
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
