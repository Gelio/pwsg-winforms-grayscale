using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdaptiveGrayscaleHistogram
{
    public partial class Histogram : Form
    {
        public event EventHandler<Tuple<int, int>> ValueChanged;

        public Histogram()
        {
            InitializeComponent();
        }

        private void anyTrackbar_ValueChanged(object sender, EventArgs e)
        {
            this.ValueChanged?.Invoke(this, new Tuple<int, int>(trackBarBottomValue.Value, trackBarTopValue.Value));
        }
    }
}
