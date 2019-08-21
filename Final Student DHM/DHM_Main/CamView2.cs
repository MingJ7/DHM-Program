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
using Emgu.CV.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
namespace DHM_Main {
	public partial class CamView2 : DisplayForm {
		bool showOverExposure = false;
		UMat mat252, mat255;
		UMat mat65279, mat65535;
		UMat col_8bit, col_16bit;
		public enum ControlsE { CameraB, PlayB, StopB, SettingB, SaveB, LoadB, GainS, ExposureS, AutoC };
		public CamView2() {
			InitializeComponent();
		}

		public virtual void Add_event(ControlsE con, EventHandler @event) {
			switch (con) {
				case ControlsE.CameraB:
					CameraButton.Click += @event;
					break;
				case ControlsE.PlayB:
					PlayStopButton.Click += @event;
					break;
				case ControlsE.SettingB:
					SettingButton.Click += @event;
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
						PlayStopButton.Click -= eh;
						break;
					case ControlsE.SettingB:
						SettingButton.Click -= eh;
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
			}
			catch (NullReferenceException e) {
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
					PlayStopButton.Enabled = enable;
					break;
				case ControlsE.SettingB:
					SettingButton.Enabled = enable;
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
		public override void dispImg(UMat inImg) {
			if (!showOverExposure) base.dispImg(inImg);
			else{
				if (inImg.Depth == DepthType.Cv8U) {
					//Create Mask which shows locations of overexposed pixels 
					UMat mask = new UMat();
					CvInvoke.InRange(inImg, mat252, mat255, mask);
					//Clone inImage with 3 channels (Color) to dispImg to allow color red to be displayed
					CvInvoke.CvtColor(inImg, col_8bit, ColorConversion.Gray2Bgr);
					//set location of overexposed pixels to red
					col_8bit.SetTo(new MCvScalar(0, 0, 255), mask);
					mask.Dispose();
				}
				else {
					//Create Mask which shows locations of overexposed pixels
					UMat mask = new UMat();
					CvInvoke.InRange(inImg, mat65279, mat65535, mask);
					//Clone inImage with 3 channels (Color) to dispImg to allow color red to be displayed
					CvInvoke.CvtColor(inImg, col_16bit, ColorConversion.Gray2Bgr);
					//set location of overexposed pixels to red
					col_16bit.SetTo(new MCvScalar(0, 0, 65535), mask);
					mask.Dispose();
				}
			}
		}
		public override void updateImgSize(Size size) {
			base.updateImgSize(size);
			mat252.Create(size.Height, size.Width, DepthType.Cv8U, 1);
			mat252.SetTo(new MCvScalar(252));
			mat255.Create(size.Height, size.Width, DepthType.Cv8U, 1);
			mat255.SetTo(new MCvScalar(255));
			mat65279.Create(size.Height, size.Width, DepthType.Cv16U, 1);
			mat65279.SetTo(new MCvScalar(65279));
			mat65535.Create(size.Height, size.Width, DepthType.Cv16U, 1);
			mat65535.SetTo(new MCvScalar(65535));
		}
		protected override void ColEnabledChanged() {
			if (showColorMap||showOverExposure) {
				disp_1.Create(disp_1.Rows, disp_1.Cols, DepthType.Cv8U, 3);
				disp_2.Create(disp_2.Rows, disp_2.Cols, DepthType.Cv8U, 3);
			}
			else {
				disp_1.Create(disp_1.Rows, disp_1.Cols, DepthType.Cv8U, 1);
				disp_2.Create(disp_2.Rows, disp_2.Cols, DepthType.Cv8U, 1);
			}
		}
		public bool Auto_Checked() {
			return AutoCheck.Checked;
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
		private void OverExposureButton_Click(object sender, EventArgs e) {
			//TODO: there must be a neater way of doing this
			//toggle between rectangle selection mode and navigating the image
			ToolStripButton button = (ToolStripButton)sender;
			showOverExposure = !showOverExposure;
			ColEnabledChanged();
			chgToolStripbuttonBG(button, showOverExposure);
		}
	}
}
