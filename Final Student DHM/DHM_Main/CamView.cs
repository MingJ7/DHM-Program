using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;


using TIS.Imaging;
using Emgu.CV.UI;
using Emgu.CV;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
namespace DHM_Main
{
	public enum SliderControls { Maximum, Minimum, Value, TickFrequency };

	public partial class CamView : Form {
		bool showOverExposure = false;
		bool showColorMap = false;
		UMat mat252,mat255;
		UMat mat65279,mat65535;
		UMat col_8bit,col_16bit;
		public enum ControlsE { CameraB, PlayB, StopB, SettingB, SaveB, LoadB, GainS, ExposureS, AutoC };
		public CamView() {
			InitializeComponent();
			Initialise_Sliders();
		}
		void Initialise_Sliders() {
			this.ControlStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.ToolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
			this.GainSlider = new System.Windows.Forms.TrackBar();
			this.ExposureSlider = new System.Windows.Forms.TrackBar();
			this.GainSliderHost = new System.Windows.Forms.ToolStripControlHost(GainSlider);
			this.ExposureSliderHost = new System.Windows.Forms.ToolStripControlHost(ExposureSlider);
			this.AutoCheck = new System.Windows.Forms.CheckBox();
			this.AutoCheckHost = new System.Windows.Forms.ToolStripControlHost(AutoCheck);
			this.ControlStrip.SuspendLayout();

			////
			//SELF GENERATED CODE
			////
			this.GainSlider.Name = "gain slider";
			this.GainSlider.AutoSize = false;
			this.GainSlider.Size = new System.Drawing.Size(104, 22);

			this.ExposureSlider.Name = "exposure";
			this.ExposureSlider.AutoSize = false;
			this.ExposureSlider.Size = new System.Drawing.Size(104, 22);

			// 
			// ControlStrip
			// 
			this.ControlStrip.Dock = System.Windows.Forms.DockStyle.None;
			this.ControlStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.toolStripLabel1,
			this.GainSliderHost,
			this.ToolStripLabel2,
			this.ExposureSliderHost,
			this.AutoCheckHost});
			this.ControlStrip.Location = new System.Drawing.Point(251, 0);
			this.ControlStrip.Name = "ControlStrip";
			this.ControlStrip.Size = new System.Drawing.Size(306, 25);
			this.ControlStrip.TabIndex = 2;
			this.ControlStrip.Text = "ControlStrip";
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(31, 22);
			this.toolStripLabel1.Text = "Gain";
			// 
			// GainSliderHost
			// 
			this.GainSliderHost.Name = "GainSliderHost";
			this.GainSliderHost.Size = new System.Drawing.Size(104, 22);
			this.GainSliderHost.Text = "GainSliderHost";
			// 
			// ToolStripLabel2
			// 
			this.ToolStripLabel2.Name = "ToolStripLabel2";
			this.ToolStripLabel2.Size = new System.Drawing.Size(55, 22);
			this.ToolStripLabel2.Text = "Exposure";
			// 
			// ExposureSliderHost
			// 
			this.ExposureSliderHost.Name = "ExposureSliderHost";
			this.ExposureSliderHost.Size = new System.Drawing.Size(104, 22);
			this.ExposureSliderHost.Text = "ExposureSliderHost";
			//
			//AutoCheckHost
			//
			this.AutoCheckHost.Name = "AutoCheckHost";
			this.AutoCheckHost.Size = new System.Drawing.Size(55, 22);
			this.AutoCheckHost.Text = "AutoCheckHost";
			//
			//AutoCheck
			//
			this.AutoCheck.Name = "AutoChecked";
			this.AutoCheck.Size = new System.Drawing.Size(55, 22);
			this.AutoCheck.Text = "Auto";
			this.AutoCheck.Checked = false;
			this.AutoCheck.Enabled = false;

			this.ControlStrip.ResumeLayout(false);
			this.ControlStrip.PerformLayout();
			this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.ControlStrip);

		}

		public virtual void Add_event(ControlsE con, EventHandler @event) {
			switch (con) {
				case ControlsE.CameraB:
					CameraButton.Click += @event;
					break;
				case ControlsE.PlayB:
					PlayButton.Click += @event;
					break;
				case ControlsE.StopB:
					StopButton.Click += @event;
					break;
				case ControlsE.SettingB:
					SettingButton.Click += @event;
					break;
				case ControlsE.SaveB:
					SaveButton.Click += @event;
					break;
				case ControlsE.LoadB:
					LoadButton.Click += @event;
					break;
				case ControlsE.GainS:
					GainSlider.ValueChanged += @event;
					break;
				case ControlsE.ExposureS:
					ExposureSlider.ValueChanged += @event;
					break;
				case ControlsE.AutoC:
					AutoCheck.CheckedChanged += @event;
					break;
			}
		}
		public virtual void Remove_Event(ControlsE con, EventHandler eh) {
			try {
				switch (con) {
					case ControlsE.CameraB:
						CameraButton.Click -= eh;
						break;
					case ControlsE.PlayB:
						PlayButton.Click -= eh;
						break;
					case ControlsE.StopB:
						StopButton.Click -= eh;
						break;
					case ControlsE.SettingB:
						SettingButton.Click -= eh;
						break;
					case ControlsE.SaveB:
						SaveButton.Click -= eh;
						break;
					case ControlsE.LoadB:
						LoadButton.Click -= eh;
						break;
					case ControlsE.GainS:
						GainSlider.ValueChanged -= eh;
						break;
					case ControlsE.ExposureS:
						ExposureSlider.ValueChanged -= eh;
						break;
					case ControlsE.AutoC:
						AutoCheck.CheckedChanged -= eh;
						break;
				}
			} catch (NullReferenceException e) {
				if (MessageBox.Show(e.ToString(), e.StackTrace, MessageBoxButtons.YesNo) == DialogResult.Yes) {
					throw e;
				}
			}
		}
		public virtual void Set_Enabled(ControlsE con, bool enable) {
			switch (con) {
				case ControlsE.CameraB:
					CameraButton.Enabled = enable;
					break;
				case ControlsE.PlayB:
					PlayButton.Enabled = enable;
					break;
				case ControlsE.StopB:
					StopButton.Enabled = enable;
					break;
				case ControlsE.SettingB:
					SettingButton.Enabled = enable;
					break;
				case ControlsE.SaveB:
					SaveButton.Enabled = enable;
					break;
				case ControlsE.LoadB:
					LoadButton.Enabled = enable;
					break;
				case ControlsE.GainS:
					GainSlider.Enabled = enable;
					break;
				case ControlsE.ExposureS:
					ExposureSlider.Enabled = enable;
					break;
				case ControlsE.AutoC:
					AutoCheck.Enabled = enable;
					break;
			}
		}
		public virtual void Slider_Set(ControlsE con, SliderControls part, int value) {
			if (con == ControlsE.GainS) {
				switch (part) {
					case SliderControls.Maximum:
						GainSlider.Maximum = value;
						break;
					case SliderControls.Minimum:
						GainSlider.Minimum = value;
						break;
					case SliderControls.Value:
						GainSlider.Value = value;
						break;
					case SliderControls.TickFrequency:
						GainSlider.TickFrequency = value;
						break;
				}
			}
			else if (con == ControlsE.ExposureS) {
				switch (part) {
					case SliderControls.Maximum:
						ExposureSlider.Maximum = value;
						break;
					case SliderControls.Minimum:
						ExposureSlider.Minimum = value;
						break;
					case SliderControls.Value:
						ExposureSlider.Value = value;
						break;
					case SliderControls.TickFrequency:
						ExposureSlider.TickFrequency = value;
						break;
				}
			}
			else {
				throw new Exception("Not a SLider in CamView");
			}
		}

		public void SaveImage(string fileName) {
			imageBox1.Image.Save(fileName);
		}

		public virtual int Slider_Get(ControlsE con, SliderControls part) {
			if (con == ControlsE.GainS) {
				switch (part) {
					case SliderControls.Maximum:
						return GainSlider.Maximum;
					case SliderControls.Minimum:
						return GainSlider.Minimum;
					case SliderControls.Value:
						return GainSlider.Value;
					case SliderControls.TickFrequency:
						return GainSlider.TickFrequency;
				}
			}
			else if (con == ControlsE.ExposureS) {
				switch (part) {
					case SliderControls.Maximum:
						return ExposureSlider.Maximum;
					case SliderControls.Minimum:
						return ExposureSlider.Minimum;
					case SliderControls.Value:
						return ExposureSlider.Value;
					case SliderControls.TickFrequency:
						return ExposureSlider.TickFrequency;
				}
			}
			else {
				throw new Exception("Not a SLider in CamView");
			}
			return -1;
		}
		public bool Auto_Checked() {
			return AutoCheck.Checked;
		}
		public void Process4Disp(UMat inImage) {
			//if inImage is empty, do nothing
			if (inImage.IsEmpty) return;
			else {
				//If Function is called on non-UI thread
				if (imageBox1.InvokeRequired) {
					if (!showOverExposure && !showColorMap) {
						if (inImage.Depth == DepthType.Cv8U) {
							//send img to UI thred for updating
							imageBox1.Invoke(new MethodInvoker(() => Process4Disp(inImage)));
						}else{
							//normalie and store image
							CvInvoke.CvtColor(inImage, col_16bit, ColorConversion.Gray2Bgr);
							imageBox1.Invoke(new MethodInvoker(() => Process4Disp(col_16bit)));
						}
					}
					else if(showOverExposure){
						if (inImage.Depth == DepthType.Cv8U) {
							//Create Mask which shows locations of overexposed pixels 
							UMat mask = new UMat();
							CvInvoke.InRange(inImage, mat252, mat255, mask);
							//Clone inImage with 3 channels (Color) to dispImg to allow color red to be displayed
							CvInvoke.CvtColor(inImage, col_8bit, ColorConversion.Gray2Bgr);
							//set location of overexposed pixels to red
							col_8bit.SetTo(new MCvScalar(0, 0, 255), mask);
							mask.Dispose();
							imageBox1.Invoke(new MethodInvoker(() => Process4Disp(col_8bit)));
						}
						else {
							//Create Mask which shows locations of overexposed pixels
							UMat mask = new UMat();
							CvInvoke.InRange(inImage, mat65279, mat65535, mask);
							//Clone inImage with 3 channels (Color) to dispImg to allow color red to be displayed
							CvInvoke.CvtColor(inImage, col_16bit, ColorConversion.Gray2Bgr);
							//set location of overexposed pixels to red
							col_16bit.SetTo(new MCvScalar(0, 0, 65535), mask);
							mask.Dispose();
							imageBox1.Invoke(new MethodInvoker(() => Process4Disp(col_16bit)));
						}
					}
					else if(showColorMap){
						//Create holder for colormapped img
						//UMat dispImg = new UMat(inImage.Size, DepthType.Cv8U, 3);
						if(inImage.Depth==DepthType.Cv16U){
							//Convert inImg to 8-bit as colormap can only be done on 8-bit images
							using (UMat temp = new UMat()) {
								//CvInvoke.Normalize(inImage, temp, 0, 255, normType: NormType.MinMax, dType: DepthType.Cv8U);
								inImage.ConvertTo(temp, DepthType.Cv8U, 1 / 256.0, 0);
								CvInvoke.ApplyColorMap(temp, col_8bit, ColorMapType.Jet);
							}
						}else {
							//Make colormap image
							CvInvoke.ApplyColorMap(inImage, col_8bit, ColorMapType.Jet);
						}
						imageBox1.Invoke(new MethodInvoker(() => Process4Disp(col_8bit)));
					}
				}
				else {
					//UI thread only Updates image
					imageBox1.Image = inImage;
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
		private void OverExposureButton_Click(object sender, EventArgs e) {
			//TODO: there must be a neater way of doing this
			//toggle between rectangle selection mode and navigating the image
			ToolStripButton button = (ToolStripButton)sender;
			showOverExposure = !showOverExposure;
			if (showOverExposure) {
				button.BackColor = Color.Chartreuse;
			}
			else {
				button.BackColor = SystemColors.Control; //used to be GhostWhite
			}
		}
		public void Update_Image_Size(Size size){
			if (mat255 != null) { mat255.Dispose(); }
			mat255 = new UMat(size, DepthType.Cv8U, 1);
			mat255.SetTo(new MCvScalar(255));
			if (mat252 != null) { mat252.Dispose(); }
			mat252 = new UMat(size, DepthType.Cv8U, 1);
			mat252.SetTo(new MCvScalar(252));
			if (mat65279!=null) mat65279.Dispose();
			mat65279 = new UMat(size, DepthType.Cv16U, 1);
			mat65279.SetTo(new MCvScalar(65279));
			if (mat65535 != null) { mat65535.Dispose(); }
			mat65535 = new UMat(size, DepthType.Cv8U, 1);
			mat65535.SetTo(new MCvScalar(65535));
			col_8bit?.Dispose();
			col_8bit = new UMat(size, DepthType.Cv8U, 3);
			col_16bit?.Dispose();
			col_16bit = new UMat(size, DepthType.Cv16U, 3);
		}
		private void CamView_FormClosing(object sender, FormClosingEventArgs e) {
			mat252?.Dispose();
			mat255?.Dispose();
			mat65279?.Dispose();
			mat65535?.Dispose();
			col_8bit?.Dispose();
			col_16bit?.Dispose();
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

		private void ResetZoomButton_Click(object sender, EventArgs e) {
			imageBox1.SetZoomScale(1d, new Point(0, 0));

		}
	}
}
