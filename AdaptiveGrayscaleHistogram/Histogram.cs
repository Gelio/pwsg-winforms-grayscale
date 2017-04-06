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
    public struct HistogramValues
    {
        public int MinValue { get; private set; }
        public int MaxValue { get; private set; }

        public HistogramValues(int minValue, int maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;
        }
    }

    public partial class Histogram : Form
    {
        public event EventHandler<HistogramValues> ValueChanged;

        public Histogram()
        {
            InitializeComponent();
        }

        private void anyTrackbar_ValueChanged(object sender, EventArgs e)
        {
            this.ValueChanged?.Invoke(this, new HistogramValues(trackBarBottomValue.Value, trackBarTopValue.Value));
        }
    }
}
