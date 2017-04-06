using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AdaptiveGrayscaleHistogram.Filters
{
    class GrayscaleFilter : IFilter
    {
        public int Delay => 5;

        public void Apply(Bitmap[] buffers, byte[] bitmapBytes, int bytesPerPixel, int width, int height, Action<int> reportProgress, Func<bool> shouldCancel)
        {
            int currentPixelPosition = 0,
                buffersCount = buffers.Length;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++, currentPixelPosition += bytesPerPixel)
                {
                    if (shouldCancel())
                        return;

                    byte grayscaleColor = GrayscaleHelpers.GetGrayscaleColor(bitmapBytes[currentPixelPosition], bitmapBytes[currentPixelPosition + 1], bitmapBytes[currentPixelPosition + 2]);
                    bitmapBytes[currentPixelPosition] = grayscaleColor;
                    bitmapBytes[currentPixelPosition + 1] = grayscaleColor;
                    bitmapBytes[currentPixelPosition + 2] = grayscaleColor;
                }

                int progress = Convert.ToInt32((double)((double)(y + 1) * 100 / (double)height));
                reportProgress(progress);
            }
        }
    }
}
