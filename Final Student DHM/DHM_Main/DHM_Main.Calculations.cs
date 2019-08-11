using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace DHM_Main {
	partial class DHM_Main {
		//a reference to the user-implemented framefilter in case you wanna change its parameters whle the program is running
		private TIS.Imaging.FrameFilter m_FrameFilter;

		bool firstTimeLoadingImage;
		bool video;//true means that new images are coming at video fps. False means "Open File..." image loading
		Size oldImgSize;

		double rDist;
		double wavelength;
		double sensorPixelWidth;
		double sensorPixelHeight;

		bool fFtRectSelMode = false;
		Rectangle ftSelectedRect;

		UMat srcImg;
		UMat magT, phT;

		UMat magFoT;
		UMat phFoT;
		Point[] selRectMaxMagLocations;
		double[] selRectMaxMagValues;
		Rectangle ctrRect;



		private void DhmForm_Load(object sender, EventArgs e) {
			video = false;
			firstTimeLoadingImage = true;
			InsertParameters();
			/*           
			//First thing in the morning, the program asks for access to the camera. If you don't give it, it won't wake up.
			//Thankfully you don't have to wait 5 minutes to rouse it from its sleep again.
			if (!icImagingControl1.DeviceValid) {
				icImagingControl1.ShowDeviceSettingsDialog();

				if (!icImagingControl1.DeviceValid) {
					Close();
					return;
				}
			}
			// Disable overlay bitmap
			icImagingControl1.OverlayBitmapPosition = TIS.Imaging.PathPositions.None;

			// Create an instance of the frame filter implementation
			TakeImageFilter TkeImgFilterImpl = new TakeImageFilter();

			// Create a FrameFilter object wrapping the implementation
			m_FrameFilter = icImagingControl1.FrameFilterCreate(TkeImgFilterImpl);

			// Set the FrameFilter as display frame filter.
			icImagingControl1.DisplayFrameFilters.Add(m_FrameFilter);

			icImagingControl1.LiveStart();
			*/
#if DEBUG
			//imageBox2.FunctionalMode = ImageBox.FunctionalModeOption.Everything;
#else
            //imageBox2.FunctionalMode = ImageBox.FunctionalModeOption.PanAndZoom;
#endif
		}

		public void InsertParameters(
				double mmRDist = -175, //reconstruction distance (mm)
				double nmWavelength = 632.816, //wavelength (nm) of monochromatic light, HeNe default in air is 632.816nm
				double umSensorPixelWidth = 3.75, //pixel size(um) of each CCD in camera, ours is  3.75x3.75um
				double umSensorPixelHeight = 3.75) {
			rDist = mmRDist * Math.Pow(10, -3);
			wavelength = nmWavelength * Math.Pow(10, -9);
			sensorPixelWidth = umSensorPixelWidth * Math.Pow(10, -6);
			sensorPixelHeight = umSensorPixelHeight * Math.Pow(10, -6);
		}
		//image processing
		public UMat GetBgr2Gray(UMat img) {
			//if not grayscale, convert bgr image to grayscale
			UMat gray = img.Clone();
			Bgr2Gray(gray);
			return gray;
		}
		public void Bgr2Gray(UMat img) {
			//if not grayscale, convert bgr image to grayscale
			if (img.NumberOfChannels != 1) {
				CvInvoke.CvtColor(img, img, ColorConversion.Bgr2Gray);
			}
		}
		public void InvertImage(ref UMat img) {
			UMat tempImg = 255 - img;
			img.Dispose();
			img = tempImg;
		}
		public void ZeroPadImage(UMat img) {
			//Pad image for more optimal DFT speed
			int m = CvInvoke.GetOptimalDFTSize(img.Rows);
			int n = CvInvoke.GetOptimalDFTSize(img.Cols);
			CvInvoke.CopyMakeBorder(img, img, 0, m - img.Rows, 0, n - img.Cols, BorderType.Constant, new MCvScalar(0));//pad with '0's
		}
		public VectorOfUMat ForwardFt(UMat img) {
			///outputs quadrant-rearranged {magnitude, phase} UMat array
			///FT stuff, reference: https://docs.opencv.org/master/d8/d01/tutorial_discrete_fourier_transform.html
			//convert image to 32bit float because spacial frequency domain values are way bigger than spatial domain values.
			img.ConvertTo(img, DepthType.Cv32F);

			//create imaginary image of zeros of same depthType and size as image representing real plane
			UMat zeros = new UMat(img.Size, img.Depth, 1);
			zeros.SetTo(new MCvScalar(0));

			//Dft accepts 2-channel images, so we use Merge to merge our 2 1-channel images into a single 2-channel image.
			//Merge accepts object arrays, so we create a VectorOfUMat of our 2 images to feed into Merge.
			VectorOfUMat vec = new VectorOfUMat(img, zeros);//img will be at 0 index of vector

			using (UMat cImg = new UMat()) {
				CvInvoke.Merge(vec, cImg);
				zeros.Dispose();// TODO: fix this bad programming and other instances of it.
				CvInvoke.Dft(cImg, cImg, DxtType.Forward, 0);//use back the same image memory
				SwitchQuadrants(cImg);
				CvInvoke.Split(cImg, vec);
			}

			//make the 2-channel array into 2 1-channel arrays
			return vec; //[0] index contains the real values and [1] index the complex values 

		}
		public UMat GetNorm4Disp(UMat inImg, bool log = false, bool norm = true) {
			UMat outImg = inImg.Clone();
			Norm4Disp(outImg, log, norm);
			return outImg;
		}
		public void Norm4Disp(UMat img, bool log = false, bool norm = true) {
			if (log) {
				using (UMat imgPlusOne = img + 1) {
					CvInvoke.Log(imgPlusOne, img);
				}
			}
			if (norm) {
				CvInvoke.Normalize(img, img, 0, 255, normType: NormType.MinMax, dType: DepthType.Cv8U);
			}
		}
		public void SwitchQuadrants(UMat img) {
			// crop the spectrum, if it has an odd number of rows or columns
			img = new UMat(img, new Rectangle(0, 0, img.Cols & -2, img.Rows & -2));

			// rearrange the quadrants of Fourier image  so that the origin is at the image center
			int cx = img.Cols / 2;
			int cy = img.Rows / 2;

			UMat q0 = new UMat(img, new Rectangle(0, 0, cx, cy));   // Top-Left - Create a ROI per quadrant
			UMat q1 = new UMat(img, new Rectangle(cx, 0, cx, cy));  // Top-Right
			UMat q2 = new UMat(img, new Rectangle(0, cy, cx, cy));  // Bottom-Left
			UMat q3 = new UMat(img, new Rectangle(cx, cy, cx, cy)); // Bottom-Right
			UMat tmp = new UMat();                           // swap quadrants (Top-Left with Bottom-Right)

			q0.CopyTo(tmp);
			q3.CopyTo(q0);
			tmp.CopyTo(q3);
			q1.CopyTo(tmp);                    // swap quadrant (Top-Right with Bottom-Left)
			q2.CopyTo(q1);
			tmp.CopyTo(q2);
		}
		public VectorOfUMat InverseFt(UMat re, UMat im) {
			///reference: https://stackoverflow.com/questions/16812950/how-do-i-compute-dft-and-its-reverse-with-emgu
			VectorOfUMat vec = new VectorOfUMat(re, im);
			using (UMat cImg = new UMat()) {
				CvInvoke.Merge(vec, cImg);//because Dft method accepts and outputs 2-channel image
				CvInvoke.Dft(cImg, cImg, DxtType.Inverse, 0);
				CvInvoke.Split(cImg, vec);
			}
			return vec;
		}
		public void ProcessSrcToEnd(UMat inImg) {
			///Displays the input image, forward fourier transform(fFt) it and display magnitude of that fFt. It then calls the next function.
			///Previous name was ProcessSrcTofFt.

			VectorOfUMat ft; //forward (Fourier) transform
			using (UMat grayImg = GetBgr2Gray(inImg)) { //make grayScale. It is a copy so as to not mess up the displaying.
				ft = ForwardFt(grayImg);
			}
			// get magnitude and phase from real and imaginary parts
			if (magT != null) {
				magT.Dispose();
			}
			magT = new UMat();
			if (phT != null) {
				phT.Dispose();
			}
			phT = new UMat();
			CvInvoke.CartToPolar(ft[0], ft[1], this.magT, this.phT);
			ft[0].Dispose();
			ft[1].Dispose();
			ft.Dispose();//TODO: check whether you have to dispose each UMat individually then the vector. Same for inverse fourier transform's output down there.

			//display magnitude of forward fourier transform
			fTView.Display_Image(GetNorm4Disp(this.magT, log: true));


			if (firstTimeLoadingImage) {
				this.firstTimeLoadingImage = false;
				// set rectangle selection to full image (I want user to see all boxes displaying image when start up)
				this.ftSelectedRect = new Rectangle(new Point(0, 0), magT.Size);
				this.oldImgSize = this.magT.Size;
			}
			else {
				//if the new image is smaller than the previous one,
				if (magT.Size.Width < oldImgSize.Width || magT.Size.Height < oldImgSize.Height) {//TODO: make this smaller-than-image checking a function cause it happens again at the end of processing, or somehow join the 2 to neaten it up.
																								 //limit the selected rectangle area to within the new image to avoid trying to read outside the image.
					LimitRectToWithinImage(this.magT, ref this.ftSelectedRect);
				}
			}



			//continue the chain
			ProcessSelRectToEnd(this.ftSelectedRect, this.magT, this.phT);
		}
		public void ProcessSelRectToEnd(Rectangle selRect, UMat magT, UMat phT) {
			///Takes the selected rectangle portion, crops it and pastes it onto a zero image with its maximum value at the bottom-right centre, 
			///calls the next function.
			///Previous name was ProcessSelRectToPasteRect

			UMat magSelT = new UMat(magT, selRect);
			UMat phSelT = new UMat(phT, selRect);

			//gets the values and locations of maximum and minimum points for each channel, but we only have 1 channel
			//so we only use the [0] index.
			magSelT.MinMax(out double[] minMagValues, out selRectMaxMagValues,
				out Point[] minMagLocations, out selRectMaxMagLocations);


			//create empty UMats to "paste" the first-order spectrum into
			if (magFoT != null) {
				magFoT.Dispose();
			}
			magFoT = new UMat(magT.Size, DepthType.Cv32F, 1); //magnitude of First-Order only frequency spectrum image
			magFoT.SetTo(new MCvScalar(0));
			if (phFoT != null) {
				phFoT.Dispose();
			}
			phFoT = magFoT.Clone();
			//find 0Hz point in image.
			//0Hz point is at the bottom-right of the centre, because the width and height are even numbers as I
			//cropped them earlier in SwitchQuadrants(). There's no +1 because coordinates start at index 0.
			Point foT0HzPoint = new Point(magFoT.Cols / 2, magFoT.Rows / 2);

			//find centered ROI on first-order-only, where the maximum value is placed at the 0Hz point on FoT
			this.ctrRect = new Rectangle(foT0HzPoint.Minus(selRectMaxMagLocations[0]), selRect.Size);

			LimitRectToWithinImage(magFoT, ref ctrRect);

			UMat magCtrFoT = new UMat(magFoT, ctrRect);
			UMat phCtrFoT = new UMat(phFoT, ctrRect);
			magSelT.CopyTo(magCtrFoT);
			phSelT.CopyTo(phCtrFoT);
			//dispose the UMat headers. I don't think they will free the memory unless it's the last UMat accessing the memory.
			magSelT.Dispose();
			phSelT.Dispose();
			magCtrFoT.Dispose();
			phCtrFoT.Dispose();

			/*
			//debugging: create a clone for displaying, process for displaying and display.
			//I made it complicated because normalisation and log should be done only in the ctredROI,
			//as any other area is just a 0.
			//I believe that speeds up the processing for display.
			UMat MagFoTCpy = magFoT.Clone();
			UMat magCtrFoTCpy = new UMat(MagFoTCpy, ctrRect);
			Norm4Disp(magCtrFoTCpy, log: true, norm: false);
			CvInvoke.Normalize(magCtrFoTCpy, magCtrFoTCpy, 0, 255, NormType.MinMax);
			MagFoTCpy.ConvertTo(MagFoTCpy, DepthType.Cv8U);
			imageBox3.Image = MagFoTCpy;
			*/

			//continue the chain
			ProcessAddPhaseToEnd(
				this.rDist,
				this.sensorPixelWidth,
				this.sensorPixelHeight,
				this.wavelength,
				this.magFoT,
				this.phFoT,
				this.ctrRect,
				this.selRectMaxMagLocations[0]);
		}
		public void ProcessAddPhaseToEnd(
			double rDist,
			double sensorPixelWidth,
			double sensorPixelHeight,
			double wavelength,
			UMat magFoT,
			UMat phFoT,
			Rectangle ctrRect,
			Point selRectMaxMagPoint) {
			///Based on a bunch of parameters (as you can see in the input), calculates the phase difference for each plane wave
			///between the aperture(hologram) plane and image plane. It adds this phase difference to each plane wave, turning the 
			///fourier spectrum into one at the image plane, and then inverse fourier transforms to get the predicted magnitude and 
			///phase of light at the image plane. It then displays the predicted magnitude and phase images.
			///This version modifies the original phF)). For Video mode.
			/// 
			/// This next part is just me rambling over the minimal parameters to choose and the ups and downs of each set. 
			///I went for just passing already calculated values instead of recalculating them.
			///1. create matrix of size ( phCtrFoT OR ctrRect, which contains location info )
			///2. value at each point is based on distance from ( rectSelMaxMagLocations[0] OR from ctrRect:
			///Point FoT0HzPoint = new Point(magFoT.Cols / 2, magFoT.Rows / 2);
			///Point rectSelMaxMagLocation = ctrRect.Location.Minus(FOT0HzPoint);
			///3. copy over to phCtrFoT, which if you choose ctrRect and null, you have to create another header.
			/// 
			///I choose (phCtrFoT, roiMaxMagLoc)!
			///
			///However, due to needing to clone phFoT, and hence phCtrFoT as well to preserve the original when in static
			///image mode, I chose (ctrRect, selRectMaxMagLocation) instead.


			//Calculate phase difference matrix to add to sinewaves at particular (x,y)s.
			//As much calculation is taken out of the loop as possible.
			//done with image class because it is faster than Matrix<type> class. 
			//Only difference between image class and umat/mat (for this function) is at CvInvoke.Add,
			//where umat/mat takes 100ms less time in 17s. Not worth the extra carefulness (yet).
			double sensorHeight = phFoT.Rows * sensorPixelHeight;
			double sensorWidth = phFoT.Cols * sensorPixelWidth;
			double xFreqInterval = 1 / sensorWidth;
			double yFreqInterval = 1 / sensorHeight;
			double xFreqITimesWavelength = xFreqInterval * wavelength;
			double yFreqITimesWavelength = yFreqInterval * wavelength;
			double wavenumber = 2 * Math.PI / wavelength;

			Image<Gray, float> phDiffMat = new Image<Gray, float>(ctrRect.Size);
			int rows = phDiffMat.Rows;
			int cols = phDiffMat.Cols;
			for (int y = 0; y < rows; ++y) {
				for (int x = 0; x < cols; ++x) {
					double xTFreq = x - selRectMaxMagPoint.X;//fourier transform frequency
					double yTFreq = y - selRectMaxMagPoint.Y;
					double gammaX = xTFreq * xFreqITimesWavelength;
					double gammaY = yTFreq * yFreqITimesWavelength;
					double gammaXSqred = gammaX * gammaX;
					double gammaYSqred = gammaY * gammaY;
					double insideSqrt = 1 - gammaXSqred - gammaYSqred;
					double sqrted = Math.Sqrt(insideSqrt);
					double sqrtedTimesWavenumber = sqrted * wavenumber;
					double ph = rDist * sqrtedTimesWavenumber;
					float fPh = (float)ph;
					phDiffMat.Data[y, x, 0] = fPh;
				}
			}

			//if video mode is off, add phase difference to a copy of the first-order-only image,
			//because we don't want to modify the input
			UMat phFoT2Process, phCtrFoT2Process; //variable 2

			if (video == false) {//create a copy to preserve the original
				phFoT2Process = phFoT.Clone(); //a copy
				phCtrFoT2Process = new UMat(phFoT2Process, ctrRect); //roi of the copy
			}
			else {
				phFoT2Process = phFoT; //not-a-copy
				phCtrFoT2Process = new UMat(phFoT, ctrRect); //roi of not-a-copy
			}
			CvInvoke.Add(phCtrFoT2Process, phDiffMat, phCtrFoT2Process, dtype: DepthType.Cv32F);
			phDiffMat.Dispose();




			VectorOfUMat iFt;
			//convert magnitude and phase to real and imaginary images to input into inverse fourier transform function
			using (UMat reFoT = new UMat(), imFoT = new UMat()) {
				CvInvoke.PolarToCart(magFoT, phFoT2Process, reFoT, imFoT);
				if (video == false) {
					//disposing phFoT2Process also takes care of phCtrFoT2Process, as the latter is an ROI of the first.
					phFoT2Process.Dispose();
				}
				iFt = InverseFt(reFoT, imFoT); // inverse fourier transform
			}

			// get magnitude and phase from real and imaginary parts
			UMat magFo = new UMat();
			UMat phFo = new UMat();
			CvInvoke.CartToPolar(iFt[0], iFt[1], magFo, phFo);
			iFt.Dispose(); //disposes the real and imaginary images inside it too.

			intensityView.Display_Image(GetNorm4Disp(magFo, log: true));

			phaseView.Display_Image(GetNorm4Disp(phFo));

			//if the new image is different size from the previous one,
			if (magT.Size.Width < oldImgSize.Width || magT.Size.Height < oldImgSize.Height) {
				//Make the images fit their imageBoxes so the user sees the whole image loaded at first.
				//FitImageToImageBox(imageBox1);
				//FitImageToImageBox(imageBox2);
				//FitImageToImageBox(imageBox3);
				//FitImageToImageBox(imageBox4);

			}
			this.oldImgSize = this.magFoT.Size;
		}


		private void OpenImageButton_Click(object sender, EventArgs e) {
			using (OpenFileDialog Openfile = new OpenFileDialog()) {
				if (Openfile.ShowDialog() == DialogResult.OK) {
					//load an image from filename
					this.srcImg = new UMat(Openfile.FileName, ImreadModes.AnyColor);
					ProcessSrcToEnd(this.srcImg);
				}
			}
		}
		private void ProcessImageButton_Click(object sender, EventArgs e) {
			ProcessSrcToEnd(this.srcImg);
		}

		//rectangle selection related functions!


		public Point GetImageLimitedPoint(IImage img, Point p) {
			return new Point {
				X = p.X.LimitToRange(0, img.Size.Width - 1),
				Y = p.Y.LimitToRange(0, img.Size.Height - 1)
			};
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
		public void LimitRectToWithinImage(IImage img, ref Rectangle rect) {
			//get the top-left(0th index) and bottom-right(1st index) point of a rectangle
			Point[] points = GetPointsFromRectangle(rect);

			//limit points to within image
			points[0] = GetImageLimitedPoint(img, points[0]);
			points[1] = GetImageLimitedPoint(img, points[1]);

			//get a rectangle from image-limited points
			rect = GetRect(points[0], points[1]);

		}
		public Point[] GetPointsFromRectangle(Rectangle rect) {
			//returns top-left and bottom-right point of rectangle
			return new Point[] {
				rect.Location,
				new Point(
					rect.Location.X + rect.Size.Width - 1,
					rect.Location.Y + rect.Size.Height - 1
				)
			};
		}

		private void ImageBoxRectangleSelect_Paint(object sender, PaintEventArgs e) {
			if (ftSelectedRect != null) {
				//TODO: draw using imageBox's in-built operations. Check whether operations are reversible first.
				using (Pen pen = new Pen(Color.Red, 0.2f)) {
					//draw a rectangle around the specified rectangle area.
					e.Graphics.DrawRectangle(pen, ftSelectedRect);
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
		private void On_Rect_Chg(object sender, Rect_Changed e){
			ftSelectedRect = e.rectangle;
			if (video == false) ProcessSelRectToEnd(ftSelectedRect, magT, phT);
		}
/*		private void SetZoomScale1xToolStripButton_Click(object sender, EventArgs e) {
			imageBox1.SetZoomScale(1d, new Point(0, 0));
			imageBox2.SetZoomScale(1d, new Point(0, 0));
			imageBox3.SetZoomScale(1d, new Point(0, 0));
			imageBox4.SetZoomScale(1d, new Point(0, 0));
		}*/
		/*
		private void RDistToolStripTraceBar_ValueChange(object sender, EventArgs e) {
			//calculat rDist from both mm and 10micron slider added together
			this.rDist = mmRDistToolStripTraceBarItem.tb.Value / 1000d
				+ tenMicronRDistToolStripTraceBarItem.tb.Value / 100000d;
			if (video == false) {
				ProcessAddPhaseToEnd(
					this.rDist,
					this.sensorPixelWidth,
					this.sensorPixelHeight,
					this.wavelength,
					this.magFoT,
					this.phFoT,
					this.ctrRect,
					this.selRectMaxMagLocations[0]);
			}
			toolStripLabel1.Text = String.Format("Reconstruction Distance: {0}mm", this.rDist * 1000);
		}*/
	}
}
