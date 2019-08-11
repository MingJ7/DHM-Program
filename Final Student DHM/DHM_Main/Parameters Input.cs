using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DHM_Main {
	public partial class Parameters_Input : Form {
		public Parameters_Input() {
			InitializeComponent();
		}

		private void Parameters_Input_Load(object sender, EventArgs e) {
			textBox1.Text = Properties.Settings.Default.Wavelength.ToString();
			textBox2.Text = Properties.Settings.Default.Pixel_h.ToString();
			textBox3.Text = Properties.Settings.Default.Pixel_w.ToString();
		}

		private void Button1_Click(object sender, EventArgs e) {
			bool allclear = true;
			try {
				//try to convert the string to value for slider
				Properties.Settings.Default.Wavelength = Int32.Parse(textBox1.Text);
			}
			catch (FormatException) {
				//If fail, set it to previous value
				textBox1.Text = Properties.Settings.Default.Wavelength.ToString();
				allclear = false;
				MessageBox.Show("Please input valid interger for wavelength","Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			try {
				//try to convert the string to value for slider
				Properties.Settings.Default.Pixel_h = Int32.Parse(textBox2.Text);
			}
			catch (FormatException) {
				//If fail, set it to previous value
				textBox2.Text = Properties.Settings.Default.Pixel_h.ToString();
				allclear = false;
				MessageBox.Show("Please input valid interger for pixel height", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			try {
				//try to convert the string to value for slider
				Properties.Settings.Default.Pixel_w = Int32.Parse(textBox3.Text);
			}
			catch (FormatException) {
				//If fail, set it to previous value
				textBox3.Text = Properties.Settings.Default.Pixel_w.ToString();
				allclear = false;
				MessageBox.Show("Please input valid interger for pixel width", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			if (allclear) this.Close();
		}
	}
}
