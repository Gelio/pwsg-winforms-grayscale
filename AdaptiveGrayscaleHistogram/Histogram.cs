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
        public byte MinValue { get; private set; }
        public byte MaxValue { get; private set; }

        public HistogramValues(byte minValue, byte maxValue)
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
            byte minValue = Convert.ToByte(trackBarBottomValue.Value),
                maxValue = Convert.ToByte(trackBarTopValue.Value);

            if (sender == trackBarBottomValue && minValue > maxValue)
            {
                trackBarBottomValue.Value = maxValue;
                return;
            }
            else if (sender == trackBarTopValue && minValue > maxValue)
            {
                trackBarTopValue.Value = minValue;
                return;
            }

            this.ValueChanged?.Invoke(this, new HistogramValues(minValue, maxValue));
        }
    }
}
