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
	public partial class DHM_Main : Form {

		bool videoMode = false; //it's either in static image or videoMode mode

		UMat magT, phT;

		Rectangle fTSelRoi;
		Size oldMagTSize;
		Point fTSelRoiMaxMagAbsLoc;
		Rectangle fTLtdSelRoi;

		double rDist,
			   wavelength = Properties.Settings.Default.Wavelength/1000000000.0,
			   sensorPxWidth = Properties.Settings.Default.Pixel_w/ 1000000000.0, 
			   sensorPxHeight = Properties.Settings.Default.Pixel_h/ 1000000000.0;

		Rectangle ctrRoi;
		Point ctrRoiMaxMagLoc;
		UMat magFoT, phFoT;

		Image<Gray, double> unitPhDiffM;

		public void UpdateParameters() {
			wavelength = Properties.Settings.Default.Wavelength / 1000000000.0;
			sensorPxWidth = Properties.Settings.Default.Pixel_w / 1000000000.0;
			sensorPxHeight = Properties.Settings.Default.Pixel_h / 1000000000.0;
		}
		public void LoadImageToEnd(UMat srcImg) {
			///srcImg should be real-valued and either grayscale or BGR
			//convert to grayscale in case source image is BGR
			using (UMat grayImg = GetBgr2Gray(srcImg)) {
				DisposeIfExists(this.magT);
				DisposeIfExists(this.phT);
				ProcessForwardT(grayImg, out this.magT, out this.phT);
			}
			ProcessNDispMagT(this.magT);

			//on first image load, set default selected area to whole image
			if (this.fTSelRoi == Rectangle.Empty) {
				this.fTSelRoi = new Rectangle(new Point(0, 0), this.magT.Size);
			}//if the new image is smaller than the previous image, fTSelRoi might go out of bounds on the new image so limit it
			else if (this.magT.Size.Width < oldMagTSize.Width || this.magT.Size.Height < oldMagTSize.Height) {
				LimitRoiToImage2(magT, ref this.fTSelRoi);
			}
			oldMagTSize = magT.Size;

			//continue the chain
			SelectAreaToEnd(this.fTSelRoi, this.magT, this.phT);
		}
		public void SelectAreaToEnd(Rectangle selRoi, UMat magT, UMat phT) {
			DisposeIfExists(this.magFoT);
			DisposeIfExists(this.phFoT);
			ProcessCopyOverNCenter(magT, phT, selRoi,
								   out _, out _,
								   out _, out Point selRoiMaxMagLoc,
								   out this.ctrRoi, out this.ctrRoiMaxMagLoc,
								   out this.fTLtdSelRoi, out this.magFoT, out this.phFoT);
			this.fTSelRoiMaxMagAbsLoc = selRoi.Location + (Size)selRoiMaxMagLoc;


			//continue the chain
			InputParametersToEnd(this.rDist, this.sensorPxWidth, this.sensorPxHeight, this.wavelength,
								 this.magFoT, this.phFoT, this.ctrRoi, this.ctrRoiMaxMagLoc, ref this.unitPhDiffM);
		}
		public void InputParametersToEnd(
			double rDist,
			double sensorPxWidth,
			double sensorPxHeight,
			double wavelength,
			UMat magFoT,
			UMat phFoT,
			Rectangle ctrRoi,
			Point ctrRoiMaxMagLoc,
			ref Image<Gray, Double> unitPhDiffM
		) {
			ProcessAddPhDiffNInverseT(rDist, sensorPxWidth, sensorPxHeight, wavelength, magFoT,
									  phFoT, ctrRoi, ctrRoiMaxMagLoc,
									  out UMat magFo, out UMat phFo,
									  ref unitPhDiffM);

			ProcessNDispMagFo(magFo);
			ProcessNDispPhFo(phFo);

			magFo.Dispose();
			phFo.Dispose();
		}

		public UMat GetBgr2Gray(UMat inImg) {
			UMat gray = inImg.Clone();
			Bgr2Gray(gray);
			return gray;
		}
		public void Bgr2Gray(UMat inImg) {
			//if not grayscale, convert bgr image to grayscale
			if (inImg.NumberOfChannels != 1) {
				CvInvoke.CvtColor(inImg, inImg, ColorConversion.Bgr2Gray);
			}
		}
		public void ZeroPadImage(UMat img) {
			//Pad image for more optimal DFT speed
			int m = CvInvoke.GetOptimalDFTSize(img.Rows);
			int n = CvInvoke.GetOptimalDFTSize(img.Cols);
			CvInvoke.CopyMakeBorder(img, img, 0, m - img.Rows, 0, n - img.Cols, BorderType.Constant, new MCvScalar(0));//pad with '0's
		}
		public void ProcessNDispMagT(UMat magT) {
			if (fTView == null) return;
			fTView.fTLtdSelRoi = fTLtdSelRoi;
			fTView.fTSelRoiMaxMagAbsLoc = fTSelRoiMaxMagAbsLoc;
			fTView.Display_Image( GetNorm4Disp(magT, log: true));
		}
		public void ProcessNDispMagFo(UMat magFo) {
			if (intensityView == null) return;
			intensityView.Display_Image( GetNorm4Disp(magFo, log: true));
		}
		public void ProcessNDispPhFo(UMat phFo) {
			if (phaseView == null) return;
			phaseView.Display_Image( GetNorm4Disp(phFo));
		}
		public void ProcessForwardT(UMat inImg, out UMat magT, out UMat phT, bool zeroPad = false, bool switchQuadrants = true) {
			///Accepts a 1-channel image
			///magnitude and phase, cause I can't think of why you 
			// would wanna look at real and imaginary Transforms.
			///Also can't think of how you can get complex-valued images. 
			///Quadrant rearranging doesn't support odd rows or cols.
			///T stuff, reference: https://docs.opencv.org/master/d8/d01/tutorial_discrete_fourier_transform.html
			//convert image to 32bit float because spacial frequency
			//domain values are way bigger than spatial domain values.
			UMat re = new UMat();//32-bit float real image
			inImg.ConvertTo(re, DepthType.Cv32F);
			if (zeroPad) {
				//zero pad for faster dft
				ZeroPadImage(re);
			}
			//create imaginary image of zeros of same depthType
			//and size as image representing real plane
			UMat im;//imaginary
			using (Mat zeros = Mat.Zeros(re.Rows, re.Cols, re.Depth, 1)) {
				im = zeros.GetUMat(AccessType.ReadWrite);
			}

			/// Quick exerpt about VectorOfUMat:
			/// if you create a VectorOfUMat vec, only if the first UMat variable
			/// to store the object is the vector node, like
			/// VectorOfUMat vec = new VectorOfUmat(new Umat(), new Umat());
			/// vec.Push(someUMat.Clone());
			/// then vec.Dispose actually disposes all the objects. In this
			/// case, if you did:
			/// VectorOfUMat vec = new VectorOfUMat(inImg.Clone(), inImg.Clone());
			/// UMat one = vec[0];
			/// one.Dispose();
			/// one.Dispose actually does nothing.
			/// Otherwise, if
			/// UMat one = new UMat();
			/// UMat two = new UMat();
			/// VectorOfUMat vec = new VectorOfUmat(one);
			/// vec.Push(two);
			/// calling vec.Dispose() doesn't dispose the objects.
			/// you have to call one.Dispose and two.Dispose.
			/// Note: no matter whether the UMat's first variable stored
			/// in is in a vector node or not, calling vec[index].Dispose
			/// does NOTHING.
			//Dft accepts 2-channel images, so we use Merge to merge
			//our 2 1-channel images into a single 2-channel image.
			//Merge accepts object arrays, so we create a VectorOfUMat
			//of our 2 images to feed into Merge.

			using (VectorOfUMat vec = new VectorOfUMat(re, im))
			using (UMat cImg = new UMat()) {
				CvInvoke.Merge(vec, cImg);
				re.Dispose();
				im.Dispose();
				//TODO: check out the rows of interest, might
				//      speed up inverse fourier transform
				CvInvoke.Dft(cImg, cImg, DxtType.Forward, 0);//use back the same memory
															 //switch quadrants while images are still combined
				if (switchQuadrants) {
					SwitchQuadrants(cImg);
				}
				//make the 2-channel array into 2 1-channel arrays
				CvInvoke.Split(cImg, vec);//vec[0] is reT, vec[1] is imT
				magT = new UMat();
				phT = new UMat();
				CvInvoke.CartToPolar(vec[0], vec[1], magT, phT);
			}
		}
		public void ProcessCopyOverNCenter(UMat magT, UMat phT, Rectangle selRoi,
											out double selRoiMinMagVal, out double selRoiMaxMagVal,
											out Point selRoiMinMagLoc, out Point selRoiMaxMagLoc,
											out Rectangle ctrRoi, out Point ctrRoiMaxMagLoc,
											out Rectangle ltdSelRoi,
											out UMat magFoT, out UMat phFoT) {
			///Takes the selected roi, copies and pastes it
			///onto a zero image with its maximum value at the bottom-right
			///of the centre, calls the next function.

			//create UMats filled with '0's to "paste" the first-order's Transform into
			using (Mat zeros = Mat.Zeros(magT.Rows, magT.Cols, DepthType.Cv32F, 1)) {
				magFoT = zeros.GetUMat(AccessType.ReadWrite); //magnitude of First-Order's (forward fourier) Transform
			}
			phFoT = magFoT.Clone();

			using (UMat magSelT = new UMat(magT, selRoi)) {
				// get the values and locations of maximum and minimum points
				// for each channel, but we only have 1 channel
				// so we only use the [0] index.
				magSelT.MinMax(out double[] selRoiMinMagValues, out double[] selRoiMaxMagValues,
							   out Point[] selRoiMinMagLocations, out Point[] selRoiMaxMagLocations);
				selRoiMinMagVal = selRoiMinMagValues[0];
				selRoiMaxMagVal = selRoiMaxMagValues[0];
				selRoiMinMagLoc = selRoiMinMagLocations[0];
				selRoiMaxMagLoc = selRoiMaxMagLocations[0];
			}

			//find 0Hz point in image, which is at the bottom-right of the centre,
			//because the width and height are even numbers as I cropped them
			//earlier in SwitchQuadrants(). There's no +1 because coordinates
			//start at index 0.
			Point foT0HzLoc = new Point(magFoT.Cols / 2, magFoT.Rows / 2);

			//calculate ctrRoi on foT to paste into, where the copied's max value
			//is at foT's 0Hz point.
			ctrRoi = new Rectangle(foT0HzLoc - (Size)selRoiMaxMagLoc, selRoi.Size);
			//it's possible for ctrRoi to go out of the image if you select
			//a region more than (width or height)/2, and the max value point
			//is at a corner, so we limit it. However, this means that the sizes
			//of selRoi and ctrRoi are different, so we limit selRoi too.
			LimitRoiToImage2(magFoT, ref ctrRoi);
			//calculate ltdSelRoi by going backwards and "pasting" ctrRoi
			//on magT.
			//find the new maxMagLoc in ctrRoi (it might change after limiting)
			ctrRoiMaxMagLoc = foT0HzLoc - (Size)ctrRoi.Location;
			Point selRoiMaxMagAbsLoc = selRoi.Location + (Size)selRoiMaxMagLoc;//location relative to origin of magT
			ltdSelRoi = new Rectangle(selRoiMaxMagAbsLoc - (Size)ctrRoiMaxMagLoc,
												 ctrRoi.Size);

			//finally, copy ltdSelRoi in T to ctrRoi in foT
			using (UMat magLtdSelT = new UMat(magT, ltdSelRoi),
					   phLtdSelT = new UMat(phT, ltdSelRoi),
					   magCtrFoT = new UMat(magFoT, ctrRoi),
					   phCtrFoT = new UMat(phFoT, ctrRoi)) {
				magLtdSelT.CopyTo(magCtrFoT);
				phLtdSelT.CopyTo(phCtrFoT);
			}

		}
		public void ProcessAddPhDiffNInverseT(
			double rDist,
			double sensorPxWidth,
			double sensorPxHeight,
			double wavelength,
			UMat magFoT,
			UMat phFoT,
			Rectangle ctrRoi,
			Point ctrRoiMaxMagLoc,
			out UMat magFo,
			out UMat phFo,
			ref Image<Gray, double> unitPhDiffM
		) {
			///Based on a bunch of parameters, calculates the phase difference at each point
			///between the aperture(hologram) plane and the image plane. It adds this phase
			///difference to each plane wave(point on the fourier spectrum), turning the fourier
			///spectrum to one at the image plane, and inverse DFTs to get the predicted magnitude
			///and phase of light at the image plane. It returns them as magFo and phFo.

			//if unassigned, generate unit phase difference matrix
			if (unitPhDiffM == null) {
				unitPhDiffM = getUnitPhDiffM(phFoT.Size, phFoT.Size, sensorPxWidth, sensorPxHeight, wavelength);
			}
			else {
				//if new phase transform image is wider or higher than current 
				//(giant) unitPhDiff matrix, dispose current giant matrix and 
				//generate a bigger one
				if (unitPhDiffM.IsROISet) {
					//clear ROI before checking dimensions
					unitPhDiffM.ROI = Rectangle.Empty;
				}
				if (phFoT.Rows > unitPhDiffM.Rows || phFoT.Cols > unitPhDiffM.Cols) {
					int biggerRows = Math.Max(phFoT.Rows, unitPhDiffM.Rows);
					int biggerCols = Math.Max(phFoT.Cols, unitPhDiffM.Cols);
					unitPhDiffM.Dispose();
					unitPhDiffM = getUnitPhDiffM(new Size(biggerCols, biggerRows), phFoT.Size,
												 sensorPxWidth, sensorPxHeight, wavelength);
				}
			}

			//Calculate phase difference matrix to add to sinewaves at particular (x,y)s.
			//phDiffM = rDist * unitPhDiffM, but first select the right region on unitPhDiffM.
			Point unitPhDiff0HzLoc = new Point(unitPhDiffM.Cols / 2, unitPhDiffM.Rows / 2);
			Rectangle unitPhDiffCtrRoi = new Rectangle(unitPhDiff0HzLoc - (Size)ctrRoiMaxMagLoc, ctrRoi.Size);

			//set ROI on the (giant) unitPhDiffM, to multiply rDist by only that part.
			unitPhDiffM.ROI = unitPhDiffCtrRoi;
			Image<Gray, float> phDiffFM;//convert phDiffM to float to make same type as
										//phFoT, we add them together later
			using (Image<Gray, double> phDiffDM = rDist * unitPhDiffM) {//phDiff 'double' matrix
				phDiffFM = phDiffDM.Convert<Gray, float>();
			}
			//reset ROI
			unitPhDiffM.ROI = Rectangle.Empty;


			//if video mode is off, add phase difference to a copy of the first-order-only image,
			//because we don't want to modify the input and feed the output back to the input as 
			//this function is called again when one of the parameters changes
			UMat phFoT2Mod = videoMode ? phFoT : phFoT.Clone(); //phFoT to modify
			using (UMat phCtrFoT2Mod = new UMat(phFoT2Mod, ctrRoi)) {
				CvInvoke.Add(phCtrFoT2Mod, phDiffFM, phCtrFoT2Mod, dtype: DepthType.Cv32F);
			}
			phDiffFM.Dispose();

			//convert magnitude and phase Transform parts at the image plane 
			//to real and imaginary parts and Inverse (fourier) Transform to 
			//get magnitude and phase at the image plane (which is what we want!)
			using (UMat reFoT = new UMat(), imFoT = new UMat()) {
				CvInvoke.PolarToCart(magFoT, phFoT2Mod, reFoT, imFoT);
				if (videoMode == false) {
					//dispose the copy
					phFoT2Mod.Dispose();
				}
				InverseT(reFoT, imFoT, out magFo, out phFo);
			}

		}

		public void SwitchQuadrants(UMat img) {
			//crop the spectrum, if it has an odd number of rows or columns, which
			//leaves the right and bottom edges the wrong value...
			//does nothing to 1d matrixes as region of interest is
			//set with 0 width and height.
			using (img = new UMat(img, new Rectangle(0, 0, img.Cols & -2, img.Rows & -2))) {
				// rearrange the quadrants of Fourier image  so that the origin is at the image center
				int cx = img.Cols / 2;//centre x and y
				int cy = img.Rows / 2;

				using (UMat q0 = new UMat(img, new Rectangle(0, 0, cx, cy)),  // Top-Left - Create a ROI per quadrant
							q1 = new UMat(img, new Rectangle(cx, 0, cx, cy)),  // Top-Right
							q2 = new UMat(img, new Rectangle(0, cy, cx, cy)),  // Bottom-Left
							q3 = new UMat(img, new Rectangle(cx, cy, cx, cy)), // Bottom-Right
							tmp = new UMat()) {

					q0.CopyTo(tmp);// swap quadrants (Top-Left with Bottom-Right)
					q3.CopyTo(q0);
					tmp.CopyTo(q3);
					q1.CopyTo(tmp);// swap quadrant (Top-Right with Bottom-Left)                    
					q2.CopyTo(q1);
					tmp.CopyTo(q2);
				}
			}
		}
		public void InverseT(UMat reIn, UMat imIn, out UMat magOut, out UMat phOut) {
			///accepts real and imaginary parts, inverse (fourier) Transforms
			///converts real and imaginary output parts to magnitude and
			///phase, returns magnitude and phase parts.
			///Reference: https://stackoverflow.com/questions/16812950/how-do-i-compute-dft-and-its-reverse-with-emgu
			using (VectorOfUMat vec = new VectorOfUMat(reIn, imIn)) {
				using (UMat cImg = new UMat()) {
					CvInvoke.Merge(vec, cImg);//because Dft method accepts and outputs 2-channel image
					CvInvoke.Dft(cImg, cImg, DxtType.Inverse, 0);
					//new objects put into vec, vec[0] is reOut, vec[1] is imOut.
					CvInvoke.Split(cImg, vec);
				}
				//convert output of inverse Transform to magnitude and polar
				magOut = new UMat();
				phOut = new UMat();
				CvInvoke.CartToPolar(vec[0], vec[1], magOut, phOut);
			}
		}
		public Image<Gray, double> getUnitPhDiffM(Size size, Size fTSize, double sensorPixelWidth, double sensorPixelHeight, double wavelength) {
			///This function is called whenever rDist, TODO: wavelength or sensor pixel width/height changes.
			///returns the unit phase difference for specified parameters
			///to "propagate" the sinewaves. The unit phase difference is calculated
			///once because most of the calculation for the phase difference matrix
			///stays the same as when the program is use, most likely reconstruction distance
			///is changed and the others are set once and forgotten. So calculation of 
			///phase difference matrix becomes  rDist * (selected part of )unitPhDiffM.
			///It is wise to recalculate unitPhDiffM whenever the image size gets larger,
			///as when the image size gets smaller or the required area changes
			///the same matrix can be used.

			//I use the image class because it is faster than Matrix<type> class. Only difference I found 
			//between image class and umat/mat with byte[] array (for reconstruction) is at CvInvoke.Add,
			//where umat/mat takes 100ms less time in 17s. Not worth the extra carefulness (yet).
			Image<Gray, double> unitPhDiffM = new Image<Gray, double>(size);

			//As much calculation is taken out of the loop as possible.
			double sensorWidth = fTSize.Width * sensorPixelWidth;//must be size of the forward (fourier) Transform. TODO: Experiment with zero-padding
			double sensorHeight = fTSize.Height * sensorPixelHeight;
			double xFreqInt = 1 / sensorWidth;//frequency intervals in the fourier transform
			double yFreqInt = 1 / sensorHeight;
			double xFreqIntTimesWavelength = xFreqInt * wavelength;
			double yFreqIntTimesWavelength = yFreqInt * wavelength;
			double k = 2 * Math.PI / wavelength; // wavenumber
			int rows = size.Height;
			int cols = size.Width;
			int zeroHzX = cols / 2;
			int zeroHzY = rows / 2;
			for (int y = 0; y < rows; ++y) {
				for (int x = 0; x < cols; ++x) {
					int xTFreq = x - zeroHzX;//(fourier) Transform frequencies
					int yTFreq = y - zeroHzY;
					double gammaX = xTFreq * xFreqIntTimesWavelength;
					double gammaY = yTFreq * yFreqIntTimesWavelength;
					unitPhDiffM.Data[y, x, 0] = k * Math.Sqrt(1 - gammaX * gammaX - gammaY * gammaY);
				}
			}
			return unitPhDiffM;
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
		public Point[] GetPointsFromRoi(Rectangle roi) {
			//returns top-left and bottom-right point of the roi.
			//Width and height of an ROI represents the cols and rows
			//of the ROI, so in calculation of the bottom-right point
			//we minus 1 to exclude the starting point.
			return new Point[] {
				roi.Location,
				roi.Location + roi.Size - new Size(1,1)
			};
		}
		public void LimitRoiToImage2(IImage img, ref Rectangle roi) {
			///built-in function assumes that input rectangles overlap even a bit and have
			///positive sizes, and treats negatively-sized rectangles as empty rectangles, so
			///this function is faster than GetImageLimitedRoi.
			Rectangle imgMaxRoi = new Rectangle(new Point(0, 0), img.Size);
			roi = Rectangle.Intersect(roi, imgMaxRoi);
		}
		public void DisposeIfExists(IDisposable obj) {
			if (obj != null) {
				obj.Dispose();
			}
		}

		private void RDistTB_ValueChange(object sender, EventArgs e) {
			this.rDist = ReconDistSliderHost.tb.Value / 1000d;
			if (videoMode == false) {
				InputParametersToEnd(
					this.rDist, this.sensorPxWidth, this.sensorPxHeight, this.wavelength,
					this.magFoT, this.phFoT, this.ctrRoi, this.ctrRoiMaxMagLoc, ref this.unitPhDiffM);
			}
			toolStripLabel1.Text = String.Format("Reconstruction Distance: {0}mm", this.rDist * 1000);
		}

		public Point GetImageLimitedPoint(IImage img, Point p) {
			return new Point {
				X = p.X.LimitToRange(0, img.Size.Width - 1),
				Y = p.Y.LimitToRange(0, img.Size.Height - 1)
			};
		}
	}
}

