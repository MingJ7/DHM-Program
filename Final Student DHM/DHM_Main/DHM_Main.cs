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
		CamView2 camView;
		FTView2 fTView;
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
		private void Init_ReconSlider() {
			ReconDistSliderHost.tb.Value = 0;
			ReconDistSliderHost.tb.Minimum = Int32.Parse(ReconMinTxt.Text);
			ReconDistSliderHost.tb.Maximum = Int32.Parse(ReconMaxTxt.Text);
			ReconDistSliderHost.tb.ValueChanged += RDistTB_ValueChange;
		}
		private void Camera_View_button_Click(object sender, EventArgs e) {
			if (camView == null) {
				camView = new CamView2 {
					MdiParent = this
				};
				camView.FormClosing += subFormClosing;
				camView.FormClosed += camView_FormClosed;
				camView.ReloadRequiredHandler += Reload_Image;
				camView.AddSaveRawEH(SaveRaw);
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
			camView.Add_event(CamView2.ControlsE.AutoC, AutoCheck_CheckedChanged);
			camView.Add_event(CamView2.ControlsE.PlayB, PlayStopButton_Click);
			camView.Add_event(CamView2.ControlsE.CameraB, CameraButton_Click);
			camView.Add_event(CamView2.ControlsE.SettingB, SettingButton_Click);
			camView.Add_event(CamView2.ControlsE.LoadB, LoadButton_Click);
			//Added AND Removed in UpdateSliders
			//camView.Add_event(CamView2.ControlsE.GainS, GainSlider_ValueChanged);
			//camView.Add_event(CamView2.ControlsE.ExposureS, ExposureSlider_ValueChanged);
		}
		private void ToolStripButton1_Click(object sender, EventArgs e) {
			if (fTView == null) {
				fTView = new FTView2 {
					MdiParent = this
				};
				fTView.FormClosing += subFormClosing;
				fTView.FormClosed += fTView_FormClosed;
				fTView.Rect_Changed_Hdl += On_Rect_Chg;
				fTView.ReloadRequiredHandler += Reload_Image;
				fTView.AddSaveRawEH(SaveRaw);
				fTView.AddWeightCentEH(Weight_Cent_Button_Click);
				fTView.AddManualCentEH(Manual_Cent_Button_Click);
				fTView.Show();
			}
			else {
				fTView.Activate();
			}
		}

		private void On_Rect_Chg(object sender, Rect_Changed e) {
			fTSelRoi = e.rectangle;
			if (!videoMode) {
				SelectAreaToEnd(fTSelRoi, magT, phT);
				fTView.fTLtdSelRoi = fTLtdSelRoi;
				fTView.fTSelRoiMaxMagAbsLoc = fTSelRoiMaxMagAbsLoc;
				ProcessNDispMagT(magT);
				fTView.Refresh();
			}
		}

		private void fTView_FormClosed(object sender, FormClosedEventArgs e) {
			_ManualCentering = _WeightCentering = false;
			fTView = null;
		}

		private void ToolStripButton2_Click(object sender, EventArgs e) {
			if (phaseView == null) {
				phaseView = new PhaseView {
					MdiParent = this
				};
				phaseView.FormClosing += subFormClosing;
				phaseView.FormClosed += phaseView_FormClosed;
				phaseView.ReloadRequiredHandler += Reload_Image;
				phaseView.AddSaveRawEH(SaveRaw);
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
				intensityView.ReloadRequiredHandler += Reload_Image;
				intensityView.AddSaveRawEH(SaveRaw);
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
					camView.Slider_Set(CamView2.ControlsE.GainS, SliderControls.Value, vCDProp.RangeValue[TIS.Imaging.VCDIDs.VCDID_Gain]);
					camView.Set_Enabled(CamView2.ControlsE.GainS, !camView.Auto_Checked());
				}
				if (vCDProp.AutoAvailable(TIS.Imaging.VCDIDs.VCDID_Exposure)) {
					vCDProp.Automation[TIS.Imaging.VCDIDs.VCDID_Exposure] = camView.Auto_Checked();
					camView.Slider_Set(CamView2.ControlsE.ExposureS, SliderControls.Value, vCDProp.RangeValue[TIS.Imaging.VCDIDs.VCDID_Exposure]);
					camView.Set_Enabled(CamView2.ControlsE.ExposureS, !camView.Auto_Checked());
				}
			}
		}
		private void CameraButton_Click(object sender, EventArgs e) {
			SelectDevice();
		}

		private void UpdateControls() {
			bool IsDeviceValid = icImagingControl1.DeviceValid;
			if (camView != null) {
				camView.Set_Enabled(CamView2.ControlsE.PlayB, IsDeviceValid);

				camView.Set_Enabled(CamView2.ControlsE.PlayB, IsDeviceValid);
				camView.Set_Enabled(CamView2.ControlsE.SaveB, IsDeviceValid);
				camView.Set_Enabled(CamView2.ControlsE.SettingB, IsDeviceValid);
				camView.Set_Enabled(CamView2.ControlsE.AutoC, IsDeviceValid);
				camView.UpdatePlayStopButton(ref videoMode);
			}
			playStopButton.Enabled = IsDeviceValid;
		}
		private void StartLiveVideo() {
			videoMode = true;
			if (icImagingControl1.DeviceValid) {
				icImagingControl1.LiveStart();
				videoMode = true;
				playStopButton.Image = Properties.Resources.STOP;
				camView?.UpdatePlayStopButton(ref videoMode);
				camSelButton.Enabled = false;
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
				UpdateDispImgSizes(icImagingControl1.ImageSize);
				if (camView != null) {
					Update_Sliders();
				}
			}
		}

		private void UpdateDispImgSizes(Size imageSize) {
			camView?.updateImgSize(imageSize);
			fTView?.updateImgSize(imageSize);
			intensityView?.updateImgSize(imageSize);
			phaseView?.updateImgSize(imageSize);
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

		private void SettingButton_Click(object sender, EventArgs e) {
			ShowProperties();
		}
		async private void LoadButton_Click(object sender, EventArgs e) {
			OpenFileDialog openFileDialog = new OpenFileDialog() {
				Title = "Open Image File",
				Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png *.bmp) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png; *.bmp|All files|*.*"
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK) {
				try {
					rawData = new UMat(openFileDialog.FileName, Emgu.CV.CvEnum.ImreadModes.Grayscale);
				}
				catch (ArgumentException) {
					MessageBox.Show("The selected file could not be loaded");
				}
				UpdateDispImgSizes(rawData.Size);
				await Task.Run(() => LoadImageToEnd(rawData));
				//Task.Run(()=> LoadImageToEnd(rawData));
				//LoadImageToEnd(rawData);
			}
		}

		private void Update_Sliders() {
			if (icImagingControl1.DeviceValid) {
				vCDProp = TIS.Imaging.VCDHelpers.VCDSimpleModule.GetSimplePropertyContainer(icImagingControl1.VCDPropertyItems);
				//Loading Values into sliders
				camView.Slider_Set(CamView2.ControlsE.GainS, SliderControls.Minimum, vCDProp.RangeMin(VCDIDs.VCDID_Gain));
				camView.Slider_Set(CamView2.ControlsE.GainS, SliderControls.Maximum, vCDProp.RangeMax(VCDIDs.VCDID_Gain));
				camView.Slider_Set(CamView2.ControlsE.GainS, SliderControls.Value, vCDProp.RangeValue[VCDIDs.VCDID_Gain]);
				camView.Slider_Set(CamView2.ControlsE.GainS, SliderControls.TickFrequency, (camView.Slider_Get(CamView2.ControlsE.GainS, SliderControls.Maximum) - camView.Slider_Get(CamView2.ControlsE.GainS, SliderControls.Minimum) / 10));
				camView.Add_event(CamView2.ControlsE.GainS, GainSlider_ValueChanged);

				camView.Slider_Set(CamView2.ControlsE.ExposureS, SliderControls.Minimum, vCDProp.RangeMin(VCDIDs.VCDID_Exposure));//on our cam is 0.1ms
																																  //this.ExposureSlider.Maximum = VCDProp.RangeMax(TIS.Imaging.VCDIDs.VCDID_Exposure);
																																  //max value on cam is 300s which is bad
				camView.Slider_Set(CamView2.ControlsE.ExposureS, SliderControls.Maximum, 1000); ;//max at 0.1s
				camView.Slider_Set(CamView2.ControlsE.ExposureS, SliderControls.Value, vCDProp.RangeValue[VCDIDs.VCDID_Exposure]);
				camView.Slider_Set(CamView2.ControlsE.ExposureS, SliderControls.TickFrequency, (camView.Slider_Get(CamView2.ControlsE.ExposureS, SliderControls.Maximum) - camView.Slider_Get(CamView2.ControlsE.ExposureS, SliderControls.Minimum)) / 10);
				camView.Add_event(CamView2.ControlsE.ExposureS, ExposureSlider_ValueChanged);
				if (vCDProp.AutoAvailable(VCDIDs.VCDID_Gain)) {
					vCDProp.Automation[VCDIDs.VCDID_Gain] = false;
				}
				if (vCDProp.AutoAvailable(VCDIDs.VCDID_Exposure)) {
					vCDProp.Automation[VCDIDs.VCDID_Exposure] = false;
				}
			}
			else {
				camView.Set_Enabled(CamView2.ControlsE.GainS, false);
				camView.Remove_Event(CamView2.ControlsE.GainS, GainSlider_ValueChanged);
				camView.Set_Enabled(CamView2.ControlsE.ExposureS, false);
				camView.Remove_Event(CamView2.ControlsE.ExposureS, ExposureSlider_ValueChanged);
			}
		}

		private void ExposureSlider_ValueChanged(object sender, EventArgs e) {
			vCDProp.RangeValue[VCDIDs.VCDID_Exposure] = 10 ^ camView.Slider_Get(CamView2.ControlsE.ExposureS, SliderControls.Value);
		}

		private void GainSlider_ValueChanged(object sender, EventArgs e) {
			vCDProp.RangeValue[VCDIDs.VCDID_Gain] = camView.Slider_Get(CamView2.ControlsE.GainS, SliderControls.Value);
		}
		private void filter_NewFrameHandler(object sender, NewFrameEvent e) {
			//update rawData
			rawData = e.transfer;
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
				if (phaseView != null) phaseView.Close();
				if (intensityView != null) intensityView.Close();
				if (fTView != null) fTView.Close();

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
			} catch (FormatException) {
				//If fail, set it to previous value
				ReconMinTxt.Text = ReconDistSliderHost.tb.Minimum.ToString();
			} finally {
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
			if (icImagingControl1.LiveVideoRunning) {
				await Task.Run(() => StopLiveVideo());
				playStopButton.Image = Properties.Resources.NEXT;
				camSelButton.Enabled = true;
				videoMode = false;
				camView?.UpdatePlayStopButton(ref videoMode);
				camView?.Set_Enabled(CamView2.ControlsE.CameraB, true);
			} else {
				StartLiveVideo();
			}
		}
		private async void subFormClosing(object sender, FormClosingEventArgs e) {
			Form toClose = (Form)sender;
			bool running = false;
			if (e.CloseReason == CloseReason.UserClosing) {
				running = icImagingControl1.LiveVideoRunning;
			}
			if (running) {
				e.Cancel = true;
				await Task.Run(() => StopLiveVideo());
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
		private void SaveRaw(object sender, EventArgs e) {
			ToolStripButton toolStripButton = (ToolStripButton)sender;
			Form form = toolStripButton.GetCurrentParent().FindForm();
			// Call the save file dialog to enter the file name of the image
			SaveFileDialog saveFileDialog1 = new SaveFileDialog();
			saveFileDialog1.Filter = "XML files(*.xml) | *.xml";
			saveFileDialog1.FilterIndex = 1;
			saveFileDialog1.RestoreDirectory = true;

			if (saveFileDialog1.ShowDialog() == DialogResult.OK) {
				using (Mat mat = new Mat()) {
					switch (form.Text) {
						case "CamView2": //rawData.Save(saveFileDialog1.FileName);
							rawData.CopyTo(mat);
							break;
						case "PhaseView": //phFo.Save(saveFileDialog1.FileName);
							phFo.CopyTo(mat);
							break;
						case "FTView1": //magT.Save(saveFileDialog1.FileName);
							magT.CopyTo(mat);
							break;
						case "IntensityView": //magFo.Save(saveFileDialog1.FileName);
							magFo.CopyTo(mat);
							break;
						default:
							MessageBox.Show("A Error has occur");
							break;
					}
					//XML saving supported only by this
					using (FileStorage fs = new FileStorage(saveFileDialog1.FileName, FileStorage.Mode.Write)) {
						fs.Write(mat, "top");
					}
				}
			}
		}
		private void Weight_Cent_Button_Click(object sender, EventArgs e){
			_WeightCentering = !_WeightCentering;
			ToolStripButton button = (ToolStripButton)sender;
			if (_WeightCentering) {
				button.BackColor = Color.Chartreuse;
			}
			else {
				button.BackColor = SystemColors.Control; //used to be GhostWhite
			}
			if (!videoMode){
				LoadImageToEnd(rawData);
			}
		}
		private void Manual_Cent_Button_Click(object sender, EventArgs e) {
			_ManualCentering = !_ManualCentering;
			ToolStripButton button = (ToolStripButton)sender;
			if (_ManualCentering) {
				button.BackColor = Color.Chartreuse;
			}
			else {
				button.BackColor = SystemColors.Control; //used to be GhostWhite
			}
			if (!videoMode) {
				LoadImageToEnd(rawData);
			}
		}
		private void Reload_Image(object sender, EventArgs e) {
			if (videoMode) return;
			Form form = (Form)sender;
			using (Mat mat = new Mat()) {
				switch (form.Text) {
					case "CamView2":
						ProcessNDispGray(rawData);
						break;
					case "PhaseView":
						ProcessNDispPhFo(phFo);
						break;
					case "FTView1":
						ProcessNDispMagT(magT);
						break;
					case "IntensityView":
						ProcessNDispMagFo(magFo);
						break;
					default:
						MessageBox.Show("A Error has occur");
						break;
				}
			}

		}
	}
}
