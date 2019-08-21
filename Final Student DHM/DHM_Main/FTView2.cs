using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.CvEnum;

namespace DHM_Main {
	public partial class FTView2 : DisplayForm {
		bool fFtRectSelMode = false;
		bool fFtMouseDown = false;
		Rectangle fTSelRoi;
		internal Rectangle fTLtdSelRoi;
		internal Point fTSelRoiMaxMagAbsLoc;
		Point fFtMouseStartLocation;
		Point fFtMouseCurrentLocation;
		public event EventHandler<Rect_Changed> Rect_Changed_Hdl;
		public FTView2() {
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
