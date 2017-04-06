using System;
using System.Drawing;

namespace AdaptiveGrayscaleHistogram.Filters
{
    interface IFilter
    {
        void Apply(Bitmap[] buffers, byte[] bitmapBytes, int bytesPerPixel, int width, int height, Action<int> reportProgress, Func<bool> shouldCancel);
    }
}
