using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TIS.Imaging;
using Emgu.CV;
using System.Windows.Forms.Design;

namespace DHM_Main {
	public partial class DHM_Main : Form {
		CamView camView;
		FTView fTView;
		PhaseView phaseView;
		IntensityView intensityView;
		Parameters_Input window = new Parameters_Input();
		TIS.Imaging.VCDHelpers.VCDSimpleProperty vCDProp;
		myFilter filter = new myFilter();
		UMat rawData = new UMat();
		bool ready_to_Close = false;

		/// <summary>
		/// Adds trackbar to toolstrip stuff
		/// </summary>

		public DHM_Main() {
			InitializeComponent();
			Init_ReconSlider();
			filter.NewFrameHandler += filter_NewFrameHandler;
		}
		private void Init_ReconSlider(){
			ReconDistSliderHost.tb.Value = 0;
			ReconDistSliderHost.tb.Minimum = Int32.Parse(ReconMinTxt.Text);
			ReconDistSliderHost.tb.Maximum = Int32.Parse(ReconMaxTxt.Text);
			ReconDistSliderHost.tb.ValueChanged += RDistTB_ValueChange;
		}
		private void Camera_View_button_Click(object sender, EventArgs e) {
			if (camView == null) {
				camView = new CamView {
					MdiParent = this
				};
				camView.FormClosing += subFormClosing;
				camView.FormClosed += camView_FormClosed;
				camView.Show();
				AddCamViewEvents();
				UpdateControls();
			}
			else {
				camView.Activate();
			}
		}

		private void camView_FormClosed(object sender, FormClosedEventArgs e) {
			camView = null;
		}
		private void AddCamViewEvents() {
			camView.Add_event(CamView.ControlsE.AutoC, AutoCheck_CheckedChanged);
			camView.Add_event(CamView.ControlsE.PlayB, PlayButton_Click);
			camView.Add_event(CamView.ControlsE.StopB, StopButton_Click);
			camView.Add_event(CamView.ControlsE.CameraB, CameraButton_Click);
			camView.Add_event(CamView.ControlsE.SettingB, SettingButton_Click);
			camView.Add_event(CamView.ControlsE.SaveB, SaveButton_Click);
			camView.Add_event(CamView.ControlsE.LoadB, LoadButton_Click);
			//Added AND Removed in UpdateSliders
			//camView.Add_event(CamView.ControlsE.GainS, GainSlider_ValueChanged);
			//camView.Add_event(CamView.ControlsE.ExposureS, ExposureSlider_ValueChanged);
		}
		private void ToolStripButton1_Click(object sender, EventArgs e) {
			if (fTView == null) {
				fTView = new FTView {
					MdiParent = this
				};
				fTView.FormClosing += subFormClosing;
				fTView.FormClosed += fTView_FormClosed;
				fTView.Rect_Changed_Hdl += On_Rect_Chg;
				fTView.Show();
			}
			else {
				fTView.Activate();
			}
		}

		private void On_Rect_Chg(object sender, Rect_Changed e) {
			fTSelRoi = e.rectangle;
			if(!videoMode){
				SelectAreaToEnd(fTSelRoi, magT, phT);
				fTView.fTLtdSelRoi = fTLtdSelRoi;
				fTView.fTSelRoiMaxMagAbsLoc = fTSelRoiMaxMagAbsLoc;
			}
		}

		private void fTView_FormClosed(object sender, FormClosedEventArgs e) {
			fTView = null;
		}

		private void ToolStripButton2_Click(object sender, EventArgs e) {
			if (phaseView == null) {
				phaseView = new PhaseView {
					MdiParent = this
				};
				phaseView.FormClosing += subFormClosing;
				phaseView.FormClosed += phaseView_FormClosed;
				phaseView.Show();
			}
			else {
				phaseView.Activate();
			}
		}

		private void phaseView_FormClosed(object sender, FormClosedEventArgs e) {
			phaseView = null;
		}

		private void ToolStripButton3_Click(object sender, EventArgs e) {
			if (intensityView == null) {
				intensityView = new IntensityView {
					MdiParent = this
				};
				intensityView.FormClosing += subFormClosing;
				intensityView.FormClosed += intensityView_FormClosed;
				intensityView.Show();
			}
			else {
				intensityView.Activate();
			}
		}

		private void intensityView_FormClosed(object sender, FormClosedEventArgs e) {
			intensityView = null;
		}

		private void AutoCheck_CheckedChanged(object sender, EventArgs e) {
			if (icImagingControl1.DeviceValid) {
				if (vCDProp.AutoAvailable(TIS.Imaging.VCDIDs.VCDID_Gain)) {
					vCDProp.Automation[TIS.Imaging.VCDIDs.VCDID_Gain] = camView.Auto_Checked();
					camView.Slider_Set(CamView.ControlsE.GainS, SliderControls.Value, vCDProp.RangeValue[TIS.Imaging.VCDIDs.VCDID_Gain]);
					camView.Set_Enabled(CamView.ControlsE.GainS, !camView.Auto_Checked());
				}
				if (vCDProp.AutoAvailable(TIS.Imaging.VCDIDs.VCDID_Exposure)) {
					vCDProp.Automation[TIS.Imaging.VCDIDs.VCDID_Exposure] = camView.Auto_Checked();
					camView.Slider_Set(CamView.ControlsE.ExposureS, SliderControls.Value, vCDProp.RangeValue[TIS.Imaging.VCDIDs.VCDID_Exposure]);
					camView.Set_Enabled(CamView.ControlsE.ExposureS, !camView.Auto_Checked());
				}
			}
		}
		private void PlayButton_Click(object sender, EventArgs e) {
			StartLiveVideo();
		}
		private void CameraButton_Click(object sender, EventArgs e) {
			SelectDevice();
		}

		private void UpdateControls() {
			bool IsDeviceValid = icImagingControl1.DeviceValid;
			if (camView != null) {
				camView.Set_Enabled(CamView.ControlsE.PlayB, IsDeviceValid);
				camView.Set_Enabled(CamView.ControlsE.StopB, IsDeviceValid);

				if (IsDeviceValid) {
					camView.Set_Enabled(CamView.ControlsE.PlayB, !icImagingControl1.LiveVideoRunning);
					camView.Set_Enabled(CamView.ControlsE.StopB, icImagingControl1.LiveVideoRunning);
					//AspectRatio = CalAspectRatio(icImagingControl1.LiveDisplayWidth, icImagingControl1.LiveDisplayHeight);
				}
				camView.Set_Enabled(CamView.ControlsE.SaveB, IsDeviceValid);
				camView.Set_Enabled(CamView.ControlsE.SettingB, IsDeviceValid);
				camView.Set_Enabled(CamView.ControlsE.AutoC, IsDeviceValid);
			}
			playStopButton.Enabled = IsDeviceValid;
		}
		private void StartLiveVideo() {
			videoMode = true;
			if (icImagingControl1.DeviceValid) {
				icImagingControl1.LiveStart();
				videoMode = true;
				playStopButton.Image = Properties.Resources.STOP;
				camSelButton.Enabled = false;
				if (camView != null) {
					camView.Set_Enabled(CamView.ControlsE.PlayB, false);
					camView.Set_Enabled(CamView.ControlsE.StopB, true);
					camView.Set_Enabled(CamView.ControlsE.CameraB, false);
				}
			}
		}

		/// <summary>
		/// StopLiveVideo
		///
		/// Stop the live video display and change the button states of the
		/// play and stop button.
		/// </summary>
		private void StopLiveVideo() {
			if (icImagingControl1.DeviceValid) {
				icImagingControl1.LiveStop();
			}
		}

		/// <summary>
		/// SelectDevice
		///
		/// Show the device selection dialog.
		/// </summary>
		private void SelectDevice() {
			if (icImagingControl1.DeviceValid) icImagingControl1.DeviceFrameFilters.Clear();
			icImagingControl1.ShowDeviceSettingsDialog();
			if (icImagingControl1.DeviceValid) {
				filter.updateCamSize(icImagingControl1.ImageSize, icImagingControl1.VideoFormatCurrent);
				icImagingControl1.DeviceFrameFilters.Add(icImagingControl1.FrameFilterCreate(filter));
				UpdateControls();
				if (camView != null) {
					Update_Sliders();
					camView.Update_Image_Size(icImagingControl1.ImageSize);
				}
			}
		}

		/// <summary>
		/// ShowProperties
		///
		/// Show the property dialog of the current video capture device.
		/// </summary>
		private void ShowProperties() {
			if (icImagingControl1.DeviceValid) {
				icImagingControl1.ShowPropertyDialog();
				UpdateControls();
			}
		}

		/// <summary>
		/// SaveImage
		///
		/// Snap (capture) an image from the video stream and save it to harddisk.
		/// </summary>
		private void SaveImage() {
			// Call the save file dialog to enter the file name of the image
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "Image files(*.jpg, *.jpeg, *.jpe, *.jfif, *.png *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp | All files | *.* ";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
				// Save the image.
				camView.SaveImage(saveFileDialog1.FileName);
			}

		}

		private async void StopButton_Click(object sender, EventArgs e) {
			await Task.Run(() => StopLiveVideo());
			playStopButton.Image = Properties.Resources.NEXT;
			camSelButton.Enabled = true;
			videoMode = false;
			if (camView != null) {
				camView.Set_Enabled(CamView.ControlsE.PlayB, true);
				camView.Set_Enabled(CamView.ControlsE.StopB, false);
				camView.Set_Enabled(CamView.ControlsE.CameraB, true);
			}
		}

		private void SettingButton_Click(object sender, EventArgs e) {
			ShowProperties();
		}

		private void SaveButton_Click(object sender, EventArgs e) {
			SaveImage();
		}
		private void LoadButton_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog() {
				Title = "Open Image File",
				Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp|All files|*.*"
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				try {
					rawData = new UMat(openFileDialog.FileName, Emgu.CV.CvEnum.ImreadModes.Grayscale);
					camView.Update_Image_Size(rawData.Size);
					LoadImageToEnd(rawData);
				}
				catch (ArgumentException) {
					MessageBox.Show("The selected file could not be loaded");
				}
			}

		}

		private void Update_Sliders() {
			if (icImagingControl1.DeviceValid) {
				vCDProp = TIS.Imaging.VCDHelpers.VCDSimpleModule.GetSimplePropertyContainer(icImagingControl1.VCDPropertyItems);
				//Loading Values into sliders
				camView.Slider_Set(CamView.ControlsE.GainS, SliderControls.Minimum, vCDProp.RangeMin(VCDIDs.VCDID_Gain));
				camView.Slider_Set(CamView.ControlsE.GainS, SliderControls.Maximum, vCDProp.RangeMax(VCDIDs.VCDID_Gain));
				camView.Slider_Set(CamView.ControlsE.GainS, SliderControls.Value, vCDProp.RangeValue[VCDIDs.VCDID_Gain]);
				camView.Slider_Set(CamView.ControlsE.GainS, SliderControls.TickFrequency, (camView.Slider_Get(CamView.ControlsE.GainS, SliderControls.Maximum) - camView.Slider_Get(CamView.ControlsE.GainS, SliderControls.Minimum) / 10));
				camView.Add_event(CamView.ControlsE.GainS, GainSlider_ValueChanged);

				camView.Slider_Set(CamView.ControlsE.ExposureS, SliderControls.Minimum, vCDProp.RangeMin(VCDIDs.VCDID_Exposure));//on our cam is 0.1ms
																																 //this.ExposureSlider.Maximum = VCDProp.RangeMax(TIS.Imaging.VCDIDs.VCDID_Exposure);
																																 //max value on cam is 300s which is bad
				camView.Slider_Set(CamView.ControlsE.ExposureS, SliderControls.Maximum, 1000); ;//max at 0.1s
				camView.Slider_Set(CamView.ControlsE.ExposureS, SliderControls.Value, vCDProp.RangeValue[VCDIDs.VCDID_Exposure]);
				camView.Slider_Set(CamView.ControlsE.ExposureS, SliderControls.TickFrequency, (camView.Slider_Get(CamView.ControlsE.ExposureS, SliderControls.Maximum) - camView.Slider_Get(CamView.ControlsE.ExposureS, SliderControls.Minimum)) / 10);
				camView.Add_event(CamView.ControlsE.ExposureS, ExposureSlider_ValueChanged);
				if (vCDProp.AutoAvailable(VCDIDs.VCDID_Gain)) {
					vCDProp.Automation[VCDIDs.VCDID_Gain] = false;
				}
				if (vCDProp.AutoAvailable(VCDIDs.VCDID_Exposure)) {
					vCDProp.Automation[VCDIDs.VCDID_Exposure] = false;
				}
			}
			else {
				camView.Set_Enabled(CamView.ControlsE.GainS, false);
				camView.Remove_Event(CamView.ControlsE.GainS, GainSlider_ValueChanged);
				camView.Set_Enabled(CamView.ControlsE.ExposureS, false);
				camView.Remove_Event(CamView.ControlsE.ExposureS, ExposureSlider_ValueChanged);
			}
		}

		private void ExposureSlider_ValueChanged(object sender, EventArgs e) {
			vCDProp.RangeValue[VCDIDs.VCDID_Exposure] = 10 ^ camView.Slider_Get(CamView.ControlsE.ExposureS, SliderControls.Value);
		}

		private void GainSlider_ValueChanged(object sender, EventArgs e) {
			vCDProp.RangeValue[VCDIDs.VCDID_Gain] = camView.Slider_Get(CamView.ControlsE.GainS, SliderControls.Value);
		}
		private void filter_NewFrameHandler(object sender, NewFrameEvent e) {
			//update rawData in such a way that at no point in time is it referencing a disposed object
			UMat old = rawData;
			rawData = e.transfer;
			if (old != null) old.Dispose();
			//process image and display processed data in windows
			this.LoadImageToEnd(rawData);
		}

		private async void DHM_Main_FormClosing(object sender, FormClosingEventArgs e) {
			if (!ready_to_Close) {
				e.Cancel = true;
				if (icImagingControl1.LiveVideoRunning) {
					await Task.Run(() => StopLiveVideo());
				}
				icImagingControl1.DeviceFrameFilters.Clear();

				if (camView != null) camView.Close();
				if(phaseView!=null) phaseView.Close();
				if(intensityView!=null) intensityView.Close();
				if(fTView!=null) fTView.Close();

				rawData.Dispose();

				Properties.Settings.Default.Save();
				try {
					icImagingControl1.Dispose();
				}
				finally {
					ready_to_Close = true;
					this.Close();
				}
			}
			else {

			}
		}


		[ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.StatusStrip)]
		public class ToolStripTraceBarItem : ToolStripControlHost {
			public TrackBar tb;
			public ToolStripTraceBarItem() : base(new TrackBar()) {
				tb = (TrackBar)this.Control;
				tb.AutoSize = false;
				tb.Size = new System.Drawing.Size(104, 25);
			}
		}

		private void ReconMinTxt_Leave(object sender, EventArgs e) {
			try {
				//try to convert the string to value for slider
				ReconDistSliderHost.tb.Minimum = Int32.Parse(ReconMinTxt.Text);
			}catch(FormatException){
				//If fail, set it to previous value
				ReconMinTxt.Text = ReconDistSliderHost.tb.Minimum.ToString();
			}finally{
				//if value is below new minimum, set to minimum
				if (ReconDistSliderHost.tb.Value < ReconDistSliderHost.tb.Minimum) {
					ReconDistSliderHost.tb.Value = ReconDistSliderHost.tb.Minimum;
					//to increase slider accuracy
					ReconDistSliderHost.tb.TickFrequency = (ReconDistSliderHost.tb.Maximum - ReconDistSliderHost.tb.Minimum) / 10;
				}
			}
		}

		private void ReconMaxTxt_Leave(object sender, EventArgs e) {
			try {
				//try to convert the string to value for slider
				ReconDistSliderHost.tb.Maximum = Int32.Parse(ReconMaxTxt.Text);
			}
			catch (FormatException) {
				//If fail, set it to previous value
				ReconMaxTxt.Text = ReconDistSliderHost.tb.Maximum.ToString();
			}
			finally {
				//if value is above new max, set to max
				if (ReconDistSliderHost.tb.Value > ReconDistSliderHost.tb.Maximum) {
					ReconDistSliderHost.tb.Value = ReconDistSliderHost.tb.Maximum;
					//to increase slider accuracy
					ReconDistSliderHost.tb.TickFrequency = (ReconDistSliderHost.tb.Maximum - ReconDistSliderHost.tb.Minimum) / 10;
				}
			}
		}

		private async void PlayStopButton_Click(object sender, EventArgs e) {
			if(icImagingControl1.LiveVideoRunning){
				await Task.Run(() => StopLiveVideo());
				playStopButton.Image = Properties.Resources.NEXT;
				camSelButton.Enabled = true;
				videoMode = false;
				if(camView!=null) {
					camView.Set_Enabled(CamView.ControlsE.PlayB, true);
					camView.Set_Enabled(CamView.ControlsE.StopB, false);
					camView.Set_Enabled(CamView.ControlsE.CameraB, true);
				}
			}else{
				StartLiveVideo();
			}
		}
		private async void subFormClosing(object sender, FormClosingEventArgs e){
			Form toClose = (Form)sender;
			bool running = false;
			if (e.CloseReason == CloseReason.UserClosing) {
				running = icImagingControl1.LiveVideoRunning;
			}
			if(running){
				e.Cancel = true;
				await Task.Run(()=>StopLiveVideo());
				toClose.Close();
				StartLiveVideo();
			}
		}

		private void PropButton_Click(object sender, EventArgs e) {
			window.ShowDialog();
		}

		private void DHM_Main_Shown(object sender, EventArgs e) {
			window.ShowDialog();
			UpdateParameters();
		}
	}
}