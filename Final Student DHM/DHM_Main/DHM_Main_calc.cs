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
		bool _WeightCentering = false;
		bool _ManualCentering = false;

		double rDist,
			   wavelength = Properties.Settings.Default.Wavelength/1000000000.0,
			   sensorPxWidth = Properties.Settings.Default.Pixel_w/ 1000000000.0, 
			   sensorPxHeight = Properties.Settings.Default.Pixel_h/ 1000000000.0;

		UMat unitPhDiffDM,
			phDiffFM = new UMat();
		UMat magT = new UMat(), phT = new UMat();

		Size oldMagTSize;
		Rectangle fTSelRoi;

		Point fTSelRoiMaxMagAbsLoc;
		Rectangle fTLtdSelRoi;

		Rectangle ctrRoi;
		Point ctrRoiMaxMagLoc;
		UMat magFoT = new UMat(), phFoT = new UMat();

		UMat magFo = new UMat(), phFo = new UMat();

		UMat img8or16u1c = new UMat(),//memory that is the size of image, 8/16-bit unsigned, 1 channel.
			 img32f1c = new UMat(),//the image size stays the same throughout the whole program.
			 quarterImg32f2c = new UMat(),
			 img32f2c = new UMat();
		VectorOfUMat vec = new VectorOfUMat();//stores the inputs and outputs of CvInvoke.Merge/Split, which are 2 2-channel 32f images.
		UMat dispGray = new UMat(),
			dispMagT = new UMat(),
			dispMagFo = new UMat(),
			dispPhFo = new UMat();
		public void UpdateParameters() {
			wavelength = Properties.Settings.Default.Wavelength / 1000000000.0;
			sensorPxWidth = Properties.Settings.Default.Pixel_w / 1000000000.0;
			sensorPxHeight = Properties.Settings.Default.Pixel_h / 1000000000.0;
		}


		public void LoadImageToEnd(UMat srcImg) {

			///srcImg should be real-valued and either grayscale or BGR
			//convert to grayscale in case source image is BGR
			UMat grayImg = this.img8or16u1c;
			Bgr2Gray(srcImg, grayImg);
			ProcessNDispGray(grayImg);
			ProcessForwardT(grayImg, this.magT, this.phT, switchQuadrants: true);
			ProcessNDispMagT(this.magT);

			//on first image load, set default selected area to whole image
			if (this.fTSelRoi == Rectangle.Empty) {
				this.fTSelRoi = new Rectangle(new Point(0, 0), this.magT.Size);
			}//if the new image is smaller than the previous image, fTSelRoi might go out of bounds on the new image so limit it
			else if (this.magT.Size.Width < oldMagTSize.Width || this.magT.Size.Height < oldMagTSize.Height) {
				LimitRoiToImage(magT, ref this.fTSelRoi);
			}
			this.oldMagTSize = magT.Size;

			//continue the chain
			SelectAreaToEnd(this.fTSelRoi, this.magT, this.phT);
		}
		public void SelectAreaToEnd(Rectangle selRoi, UMat magT, UMat phT) {
			ProcessCopyOverNCenter(magT, phT, selRoi,
								   out _, out _,
								   out _, out Point selRoiMaxMagLoc,
								   out this.ctrRoi, out this.ctrRoiMaxMagLoc,
								   out this.fTLtdSelRoi, this.magFoT, this.phFoT);
			this.fTSelRoiMaxMagAbsLoc = selRoi.Location + (Size)selRoiMaxMagLoc;


			//continue the chain
			InputParametersToEnd(this.rDist, this.sensorPxWidth, this.sensorPxHeight, this.wavelength,
								 this.magFoT, this.phFoT, this.ctrRoi, this.ctrRoiMaxMagLoc, ref this.unitPhDiffDM);
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
			ref UMat unitPhDiffDM
		) {
			ProcessAddPhDiffNInverseT(rDist, sensorPxWidth, sensorPxHeight, wavelength, magFoT,
									  phFoT, ctrRoi, ctrRoiMaxMagLoc,
									  this.magFo, this.phFo,
									  ref unitPhDiffDM);

			ProcessNDispMagFo(this.magFo);
			ProcessNDispPhFo(this.phFo);
			//todo: delete line below when done testing this section, uncomment the 2 above lines.
			//InputParametersToEnd(rDist, sensorPxWidth, sensorPxHeight, wavelength, magFoT, phFoT, ctrRoi, ctrRoiMaxMagLoc, ref unitPhDiffDM);
		}
		public void Bgr2Gray(UMat i, UMat o) {
			//if not grayscale, convert bgr image to grayscale
			if (i.NumberOfChannels != 1) {
				CvInvoke.CvtColor(i, o, ColorConversion.Bgr2Gray);
			}
			else {
				i.CopyTo(o);
			}
		}
		public void ZeroPadImage(UMat img) {
			//Pad image for more optimal DFT speed
			int m = CvInvoke.GetOptimalDFTSize(img.Rows);
			int n = CvInvoke.GetOptimalDFTSize(img.Cols);
			CvInvoke.CopyMakeBorder(img, img, 0, m - img.Rows, 0, n - img.Cols, BorderType.Constant, new MCvScalar(0));//pad with '0's
		}
		public void ProcessNDispGray(UMat grayImg) {
			if (camView == null) return;
			Norm4Disp(grayImg, this.dispGray);
			//TODO: check whether this updates the imageBox's image
			camView.dispImg(this.dispGray);
		}
		public void ProcessNDispMagT(UMat magT) {
			if (fTView == null) return;
			Norm4Disp(magT, this.dispMagT, log: true);
			//magTImageBox.Image = this.dispMagT;
			fTView.dispImg(this.dispMagT);
		}
		public void ProcessNDispMagFo(UMat magFo) {
			if (intensityView == null) return;
			Norm4Disp(magFo, this.dispMagFo, log: true);
			//magFoImageBox.Image = this.dispMagFo;
			intensityView.dispImg(this.dispMagFo);
		}
		public void ProcessNDispPhFo(UMat phFo) {
			if (phaseView == null) return;
			Norm4Disp(phFo, this.dispPhFo);
			//phFoImageBox.Image = this.dispPhFo;
			phaseView.dispImg(this.dispPhFo);
		}
		public void ProcessForwardT(UMat inImg, UMat outMagT, UMat outPhT, bool zeroPad = false, bool switchQuadrants = true) {
			///Accepts a 1-channel image, updates outMagT and outPhT.
			///magnitude and phase, cause I can't think of why you 
			///would wanna look at real and imaginary Transforms.
			///Also can't think of how you can get complex-valued images. 
			///Quadrant rearranging doesn't support odd rows or cols.
			///T stuff, reference: https://docs.opencv.org/master/d8/d01/tutorial_discrete_fourier_transform.html
			//convert image to 32bit float because spacial frequency
			//domain values are way bigger than spatial domain values.
			UMat re = outMagT;//32-bit float real image, use memory from
							  //outMagT cause it's gonna be updated anyway
			inImg.ConvertTo(re, DepthType.Cv32F);
			if (zeroPad) {
				//zero pad for faster dft
				ZeroPadImage(re);
			}
			//create imaginary image of zeros of same depthType
			//and size as image representing real plane
			UMat im = outPhT;//imaginary
			im.Create(re.Rows, re.Cols, re.Depth, re.NumberOfChannels);//allocate memory so you can set it to zero array
																	   //if memory hasn't already been allocated for it
			im.SetTo(new MCvScalar(0));

			/// Quick exerpt about VectorOfUMat:
			/// if you create a VectorOfUMat vec, only if the first UMat variable
			/// to store the object is the vector node, like
			/// VectorOfUMat vec = new VectorOfUmat(new Umat(), new Umat());
			/// vec.Push(someUMat.Clone());
			/// then vec.Dispose/Clear actually disposes all the objects referenced
			/// to by the UMats in vec. In this case, if you did:
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
			/// The situation is the same for vec.Clear, except Clear doesn't
			/// dispose of vec itself, it just disposes the objects the UMats in
			/// it reference to.
			//Dft accepts 2-channel images, so we use Merge to merge
			//our 2 1-channel images into a single 2-channel image.
			//Merge accepts object arrays, so we create a VectorOfUMat
			//of our 2 images to feed into Merge.

			VectorOfUMat vec = this.vec;
			UMat cImg = this.img32f2c;
			vec.Push(re);
			vec.Push(im);//vec[0] = re, vec[1] = im
			;
			CvInvoke.Merge(vec, cImg);
			CvInvoke.Dft(cImg, cImg, DxtType.Forward, 0);//use back the same memory
														 //switch quadrants while images are still combined
			if (switchQuadrants) {
				SwitchQuadrants(cImg);
			}
			//make the 2-channel array into 2 1-channel arrays
			CvInvoke.Split(cImg, vec);//vec[0] is reT, vec[1] is imT, they are new objects.
			CvInvoke.CartToPolar(vec[0], vec[1], outMagT, outPhT);
			vec.Clear();//dispose reT and imT.TODO: find a way to get rid of allocating memory for reT and imT.
		}
		public void ProcessCopyOverNCenter(UMat magT, UMat phT, Rectangle selRoi,
											out double selRoiMinMagVal, out double selRoiMaxMagVal,
											out Point selRoiMinMagLoc, out Point selRoiMaxMagLoc,
											out Rectangle ctrRoi, out Point ctrRoiMaxMagLoc,
											out Rectangle ltdSelRoi,
											UMat magFoT, UMat phFoT) {
			///Takes the selected roi, copies and pastes it
			///onto a zero image with its maximum value at the bottom-right
			///of the centre, updates magFoT and phFoT.

			//create UMats filled with '0's to "paste" the first-order's Transform into
			magFoT.Create(magT.Rows, magT.Cols, DepthType.Cv32F, 1);
			magFoT.SetTo(new MCvScalar(0));
			magFoT.CopyTo(phFoT);
			if (_ManualCentering) {
				selRoiMinMagVal = 0;
				selRoiMaxMagVal = 0;
				selRoiMinMagLoc = new Point(0, 0);
				selRoiMaxMagLoc = fTView.GetManualCent() - (Size)selRoi.Location;
			}
			else {
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
					if (_WeightCentering) {
						using (UMat Mask = magSelT.Clone()) {
							UMat aaaaa = new UMat();
							Mask.SetTo(new MCvScalar((long)(selRoiMaxMagVal * 0.3)));
							CvInvoke.Compare(magSelT, Mask, aaaaa, CmpType.GreaterEqual);
							Moments m = CvInvoke.Moments(aaaaa, true);
							selRoiMaxMagLoc = new Point((int)(m.M10 / m.M00), (int)(m.M01 / m.M00));
						}
					}
				}
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
			;
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
			UMat magFo,
			UMat phFo,
			ref UMat unitPhDiffDM
		) {
			///Based on a bunch of parameters, calculates the phase difference at each point
			///between the aperture(hologram) plane and the image plane. It adds this phase
			///difference to each plane wave(point on the fourier spectrum), turning the fourier
			///spectrum to one at the image plane, and inverse DFTs to get the predicted magnitude
			///and phase of light at the image plane, as magFo and phFo.

			//if unassigned, generate unit phase difference matrix
			if (unitPhDiffDM == null) {
				unitPhDiffDM = GetUnitPhDiffDoubleM(phFoT.Size, phFoT.Size, sensorPxWidth, sensorPxHeight, wavelength);
			}
			else {
				//if new phase transform image is wider or higher than current 
				//(giant) unitPhDiff matrix, dispose current giant matrix and 
				//generate a bigger one
				if (phFoT.Rows > unitPhDiffDM.Rows || phFoT.Cols > unitPhDiffDM.Cols) {
					int biggerRows = Math.Max(phFoT.Rows, unitPhDiffDM.Rows);
					int biggerCols = Math.Max(phFoT.Cols, unitPhDiffDM.Cols);
					unitPhDiffDM.Dispose();
					unitPhDiffDM = GetUnitPhDiffDoubleM(new Size(biggerCols, biggerRows), phFoT.Size,
												 sensorPxWidth, sensorPxHeight, wavelength);
				}
			}

			//Calculate phase difference matrix to add to sinewaves at particular (x,y)s.
			//phDiffM = rDist * unitPhDiffM, but first select the right region on unitPhDiffM.
			Point unitPhDiff0HzLoc = new Point(unitPhDiffDM.Cols / 2, unitPhDiffDM.Rows / 2);
			Rectangle unitPhDiffCtrRoi = new Rectangle(unitPhDiff0HzLoc - (Size)ctrRoiMaxMagLoc, ctrRoi.Size);

			//get ROI on the (giant) unitPhDiffM and multiply rDist by only that part.
			//the output goes into this.phDiffFM.
			UMat phDiffFM = this.phDiffFM;
			using (UMat unitPhDiffDMCtr = new UMat(unitPhDiffDM, unitPhDiffCtrRoi)) {
				unitPhDiffDMCtr.ConvertTo(phDiffFM, DepthType.Cv32F, alpha: rDist);//convert phDiffDM to float to make same type as
																				   //phFoT, we add them together later
			}

			//if video mode is off, add phase difference to a copy of the first-order-only image,
			//because we don't want to modify the input and feed the output back to the input as 
			//this function is called again when one of the parameters changes
			UMat phFoT2Mod; //phFoT to modify
			if (videoMode) {
				phFoT2Mod = phFoT;
			}
			else {
				phFoT2Mod = magFo;//we're gonna update it later anyway
				phFoT.CopyTo(phFoT2Mod);
			}
			using (UMat phFoT2ModCtr = new UMat(phFoT2Mod, ctrRoi)) {
				CvInvoke.Add(phFoT2ModCtr, phDiffFM, phFoT2ModCtr, dtype: DepthType.Cv32F);
			}

			//convert magnitude and phase Transform parts at the image plane 
			//to real and imaginary parts and Inverse (fourier) Transform to 
			//get magnitude and phase at the image plane (which is what we want!)
			UMat reFoT = magFo, imFoT = phFo; //we're gonna update it later anyway
			CvInvoke.PolarToCart(magFoT, phFoT2Mod, reFoT, imFoT);//reusing memory here.
			InverseT(reFoT, imFoT, magFo, phFo);//reusing memory here as well.

		}

		public void SwitchQuadrants(UMat img) {
			///accepts 2-channel images.
			//select even-sized ROI, if it has an odd number of rows or columns, which
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
							q3 = new UMat(img, new Rectangle(cx, cy, cx, cy))) { // Bottom-Right
					UMat tmp = quarterImg32f2c;

					q0.CopyTo(tmp);// swap quadrants (Top-Left with Bottom-Right)
					q3.CopyTo(q0);
					tmp.CopyTo(q3);
					q1.CopyTo(tmp);// swap quadrant (Top-Right with Bottom-Left)                    
					q2.CopyTo(q1);
					tmp.CopyTo(q2);
				}
			}
		}
		public void InverseT(UMat reIn, UMat imIn, UMat magOut, UMat phOut) {
			///accepts real and imaginary parts, inverse (fourier) Transforms
			///converts real and imaginary output parts to magnitude and
			///phase, returns magnitude and phase parts.
			///Refer to ForwardT() for more info on why I code like this.
			///Reference: https://stackoverflow.com/questions/16812950/how-do-i-compute-dft-and-its-reverse-with-emgu

			VectorOfUMat vec = this.vec;
			vec.Push(reIn);
			vec.Push(imIn);
			UMat cImg = this.img32f2c;
			CvInvoke.Merge(vec, cImg);//because Dft method accepts and outputs 2-channel image
			CvInvoke.Dft(cImg, cImg, DxtType.Inverse, 0);
			//new objects put into vec, vec[0] is reOut, vec[1] is imOut.
			CvInvoke.Split(cImg, vec);
			//convert output of inverse Transform to magnitude and polar
			CvInvoke.CartToPolar(vec[0], vec[1], magOut, phOut);
			vec.Clear();

		}
		public UMat GetUnitPhDiffDoubleM(Size size, Size fTSize, double sensorPixelWidth, double sensorPixelHeight, double wavelength) {
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
			using (Image<Gray, double> unitPhDiffM = new Image<Gray, double>(size)) {

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
				return unitPhDiffM.ToUMat();
			}
		}
		public void Norm4Disp(UMat i, UMat o, bool log = false) {
			if (log) {//we add 1 to the image with only positive values to
					  //prevent 0s being turned into -infinities by logging
					  //set temporary variable to a 1 array
				this.img32f1c.Create(i.Rows, i.Cols, i.Depth, i.NumberOfChannels);
				this.img32f1c.SetTo(new MCvScalar(1));
				//elementwise add 1 to the input and store it in tmp
				//not in input because we don't wanna change the input,
				//not in output because output is 8-bit depth, we don't want 
				//conversion as it takes too long.
				CvInvoke.Add(i, this.img32f1c, this.img32f1c);
				//log the tmp
				CvInvoke.Log(this.img32f1c, this.img32f1c);
				//normalise tmp and store in output.
				CvInvoke.Normalize(this.img32f1c, o, 0, 255, normType: NormType.MinMax, dType: DepthType.Cv8U);
			}
			else {
				CvInvoke.Normalize(i, o, 0, 255, normType: NormType.MinMax, dType: DepthType.Cv8U);
			}
		}

		public void LimitRoiToImage(IImage img, ref Rectangle roi) {
			///This function supports negative rectangle sizes,
			///and hence is slower than LimitRoiToImage2.
			//get the top-left and bottom-right points of the roi
			Point[] points = GetPointsFromRoi(roi);

			//limit points to within image
			points[0] = GetImageLimitedPoint(img, points[0]);
			points[1] = GetImageLimitedPoint(img, points[1]);

			//get an roi from image-limited points
			roi = GetRoiFromRect(GetRectFromPoints(points[0], points[1]));
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
		private Rectangle GetRectFromPoints(Point p1, Point p2) {
			//rectangle calculation that supports creating rectangle with cursor drags from bottom-right to top-left,
			//as graphics.paint apparently doesn't support negatively-sized rectangles
			return new Rectangle {
				X = Math.Min(p1.X, p2.X),
				Y = Math.Min(p1.Y, p2.Y),
				Height = Math.Abs(p1.Y - p2.Y),
				Width = Math.Abs(p1.X - p2.X)
			};
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
		public Point GetImgBoxImgCoords(ImageBox imgBox, Point controlCoords) {
			///When you mouse over an (EMGU CV) ImageBox, the location you receive
			///from MouseEventArgs is not the actual location on the image because
			///there is zoom scaling. This function takes the MouseEventArgs coords
			///and translates them into image coords. 
			int offsetX = (int)(controlCoords.X / imgBox.ZoomScale);
			int offsetY = (int)(controlCoords.Y / imgBox.ZoomScale);
			int horizontalScrollBarValue = imgBox.HorizontalScrollBar.Visible ? imgBox.HorizontalScrollBar.Value : 0;
			int verticalScrollBarValue = imgBox.VerticalScrollBar.Visible ? imgBox.VerticalScrollBar.Value : 0;
			Point nonImageLimitedPoint = new Point(offsetX + horizontalScrollBarValue, offsetY + verticalScrollBarValue);
			return GetImageLimitedPoint(imgBox.DisplayedImage, nonImageLimitedPoint);
		}
		public Point GetImageLimitedPoint(IImage img, Point p) {
			return new Point {
				X = p.X.LimitToRange(0, img.Size.Width - 1),
				Y = p.Y.LimitToRange(0, img.Size.Height - 1)
			};
		}

		private void RDistTB_ValueChange(object sender, EventArgs e) {
			this.rDist = ReconDistSliderHost.tb.Value / 1000d;
			if (videoMode == false) {
				InputParametersToEnd(
					this.rDist, this.sensorPxWidth, this.sensorPxHeight, this.wavelength,
					this.magFoT, this.phFoT, this.ctrRoi, this.ctrRoiMaxMagLoc, ref this.unitPhDiffDM);
			}
			toolStripLabel1.Text = String.Format("Reconstruction Distance: {0}mm", this.rDist * 1000);
		}
	}
}

