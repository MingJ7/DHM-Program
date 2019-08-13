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
	public partial class FTView : Form {
		bool fFtRectSelMode = false;
		bool fFtMouseDown = false;
		Rectangle fTSelRoi;
		internal Rectangle fTLtdSelRoi;
		internal Point fTSelRoiMaxMagAbsLoc;
		Point fFtMouseStartLocation;
		Point fFtMouseCurrentLocation;
		private bool showColorMap =false;

		public event EventHandler<Rect_Changed> Rect_Changed_Hdl;
		public FTView() {
			InitializeComponent();
		}
		/// <summary>
		/// UPDATE NEEDED
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void FFTRectSelToolStripButton_Click(object sender, EventArgs e) {
			//TODO: there must be a neater way of doing this
			//toggle between rectangle selection mode and navigating the image
			ToolStripButton button = (ToolStripButton)sender;
			fFtRectSelMode = !fFtRectSelMode;
			if (fFtRectSelMode) {
				button.BackColor = Color.Chartreuse;
				Chg_ImgBox_Func(ImageBox.FunctionalModeOption.Minimum);
			}
			else {
				button.BackColor = SystemColors.Control; //used to be GhostWhite
#if DEBUG
				Chg_ImgBox_Func(ImageBox.FunctionalModeOption.Everything);
#else
                Chg_ImgBox_Func(ImageBox.FunctionalModeOption.PanAndZoom);
#endif
			}
		}
		public void Chg_ImgBox_Func(Emgu.CV.UI.ImageBox.FunctionalModeOption option) {
			imageBox1.FunctionalMode = option;
		}
		private void ImageBox2_MouseDown(object sender, MouseEventArgs e) {
			if (fFtRectSelMode) {
				if (e.Button == MouseButtons.Left) {
					fFtMouseDown = true;
					fFtMouseStartLocation = e.Location;
				}
			}
		}
		private void ImageBox2_MouseMove(object sender, MouseEventArgs e) {
			ImageBox imgBox = (ImageBox)sender;
			if (fFtRectSelMode) {
				if (fFtMouseDown) {
					fFtMouseCurrentLocation = e.Location;
					UpdatefFtSelectedRoi(imgBox);
					Rect_Changed_Hdl?.Invoke(this.imageBox1, new Rect_Changed(fTSelRoi));
					//TODO: remove block below when done optimising disposal of objects
					imgBox.Invalidate();
					imgBox.Refresh();
				}
			}
		}
		private void ImageBox2_MouseUp(object sender, MouseEventArgs e) {
			ImageBox imgBox = (ImageBox)sender;
			if (fFtRectSelMode) {
				if (fFtMouseDown) {
					fFtMouseCurrentLocation = e.Location;
					fFtMouseDown = false;
					UpdatefFtSelectedRoi(imgBox);
					Rect_Changed_Hdl?.Invoke(this.imageBox1, new Rect_Changed(fTSelRoi));
					imgBox.Invalidate();
					imgBox.Refresh();
				}
			}
		}
		private void UpdatefFtSelectedRoi(ImageBox imgBox) {
			//TODO: Find out how the line below can return a rectangle of 0 width or height
			fTSelRoi = GetRect(GetImgBoxImgCoords(imgBox, fFtMouseStartLocation),
									  GetImgBoxImgCoords(imgBox, fFtMouseCurrentLocation));
		}
		private Rectangle GetRect(Point p1, Point p2) {
			//rectangle calculation that supports creating rectangle with cursor drags from bottom-right to top-left
			return new Rectangle {
				X = Math.Min(p1.X, p2.X),
				Y = Math.Min(p1.Y, p2.Y),
				Height = Math.Abs(p1.Y - p2.Y) + 1,
				Width = Math.Abs(p1.X - p2.X) + 1
			};
		}
		public Point GetImgBoxImgCoords(ImageBox imgBox, Point controlCoords) {
			int offsetX = (int)(controlCoords.X / imgBox.ZoomScale);
			int offsetY = (int)(controlCoords.Y / imgBox.ZoomScale);
			int horizontalScrollBarValue = imgBox.HorizontalScrollBar.Visible ? imgBox.HorizontalScrollBar.Value : 0;
			int verticalScrollBarValue = imgBox.VerticalScrollBar.Visible ? imgBox.VerticalScrollBar.Value : 0;
			//non image-limited point (may be out of image)
			Point p = new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
			return GetImageLimitedPoint(imgBox.DisplayedImage, p);
		}
		public Point GetImageLimitedPoint(IImage img, Point p) {
			return new Point {
				X = p.X.LimitToRange(0, img.Size.Width - 1),
				Y = p.Y.LimitToRange(0, img.Size.Height - 1)
			};
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
					//if there is old image, Dispose of it
					if (old != null) old.Dispose();
				}
			}
		}


		private void ImageBoxRectangleSelect_Paint(object sender, PaintEventArgs e) {
			//drawing the limited selected rectangle
			if (this.fTLtdSelRoi != Rectangle.Empty) {
				using (Pen pen = new Pen(Color.Red, 1f)) {
					Rectangle fTLtdSelRect = GetRectFromRoi(fTLtdSelRoi);
					e.Graphics.DrawRectangle(pen, GetMinRectAroundRect(fTLtdSelRect));
				}
			}
			//drawing the selected rectangle
			if (fTSelRoi != Rectangle.Empty) {
				Rectangle fTSelRect = GetRectFromRoi(fTSelRoi);
				//TODO: try draw using imageBox's in-built operations. Check whether operations are reversible first.
				using (Pen pen = new Pen(Color.Chartreuse, 1f)) {
					//draw a rectangle around the specified rectangle area.
					e.Graphics.DrawRectangle(pen, GetMinRectAroundRect(fTSelRect));
				}
			}
			//drawing where the max magnitude point is found in selection rectangle
			if (this.fTSelRoiMaxMagAbsLoc != null) {
				using (Pen pen = new Pen(Color.Cyan, 1f)) {
					e.Graphics.DrawRectangle(pen, GetMinRectAroundPoint(this.fTSelRoiMaxMagAbsLoc));
				}
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
		public Rectangle GetRoiFromRect(Rectangle rect) {
			///Rectangles in UI: Width and Height represent the space between points,
			///and Location and Location+Size show exactly where the two corners of the
			///rectangle are.
			///Rectangles to create ROI in a matrix: rectangle represents a ROI, which
			///is a matrix. So Location represents the top-left element in (cols,rows), 
			///and Size represents the size of the matrix in (cols,rows).
			///Hence, if you want to select a ROI with a UI-returned rectangle, you have
			///to add 1 to the size.
			//add 1 to the size and return
			rect.Size += new Size(1, 1);
			return rect;
		}
		public Rectangle GetRectFromRoi(Rectangle roi) {
			///read "GetRoiFromRect()"
			//subtract one from the size and return
			roi.Size -= new Size(1, 1);
			return roi;
		}

		private void ToolStripButton1_Click(object sender, EventArgs e) {
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
		public Rectangle GetMinRectAroundRect(Rectangle rect) {
			///Assumes input rectangle has either zero or positive size.
			rect.Location -= new Size(1, 1);
			rect.Size += new Size(2, 2);
			return rect;
		}
		public Rectangle GetMinRectAroundPoint(Point point) {
			return new Rectangle {
				Location = point - new Size(1, 1),
				Size = new Size(2, 2)
			};
		}
	}
	public class Rect_Changed : EventArgs {
		public Rectangle rectangle;
		public Rect_Changed(Rectangle rectangle) {
			this.rectangle = rectangle;
		}
	}
}
