using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace AdaptiveGrayscaleHistogram.Filters
{
    class AdaptiveFilter : IFilter
    {
        private HistogramValues histogramValues;

        public AdaptiveFilter(HistogramValues histogramValues)
        {
            this.histogramValues = histogramValues;
        }

        public void Apply(Bitmap[] buffers, byte[] bitmapBytes, int bytesPerPixel, int width, int height, Action<int> reportProgress, Func<bool> shouldCancel)
        {
            MessageBox.Show("Adaptive filter");
            reportProgress(100);
        }
    }
}
