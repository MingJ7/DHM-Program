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

namespace DHM_Main {
	public partial class FTView2 : DisplayForm {
		UMat a = new UMat();
		public FTView2() {
			InitializeComponent();
		}
	}
}
