using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;

namespace AdaptiveGrayscaleHistogram.Filters
{
    class AdaptiveFilter : IFilter
    {
        public int Delay => 125;

        private HistogramValues _histogramValues;
        private byte[] _bitmapBytes = null;
        private int _bytesPerPixel = 0;
        private int _width = 0;
        private int _height = 0;


        public AdaptiveFilter(HistogramValues histogramValues)
        {
            _histogramValues = histogramValues;
        }

        public void Apply(Bitmap[] buffers, byte[] bitmapBytes, int bytesPerPixel, int width, int height, Action<int> reportProgress, Func<bool> shouldCancel)
        {
            _bitmapBytes = bitmapBytes;
            _bytesPerPixel = bytesPerPixel;
            _width = width;
            _height = height;

            byte[] initialBitmapBytes = bitmapBytes.Clone() as byte[];
            int totalWidthChanges = (int)(Math.Log(width) / Math.Log(2)),
                totalHeightChanges = (int)(Math.Log(height) / Math.Log(2));
            int widthChanges = 0,
                heightChanges = 0;

            Rectangle rect = new Rectangle(0, 0, width, height);

            // Check if the rectangle is not a single pixel
            while (rect.Width > 1 || rect.Height > 1)
            {
                for (rect.X = 0; rect.X < width; rect.X += rect.Width)
                {
                    int xTo = rect.X + rect.Width;
                    if (xTo > width)
                        xTo = width;

                    for (rect.Y = 0; rect.Y < height; rect.Y += rect.Height)
                    {
                        if (shouldCancel())
                            return;

                        int yTo = rect.Y + rect.Height;
                        if (yTo > height)
                            yTo = height;

                        // For each rectangle calculate the color
                        int currentPixelPosition = IndexAt(rect.X, rect.Y);
                        byte grayscaleColor = GrayscaleHelpers.GetGrayscaleColor(initialBitmapBytes[currentPixelPosition], initialBitmapBytes[currentPixelPosition + 1], initialBitmapBytes[currentPixelPosition + 2]);

                        // Apply histogram bottom/top values
                        if (grayscaleColor > _histogramValues.MaxValue)
                            grayscaleColor = _histogramValues.MaxValue;
                        if (grayscaleColor < _histogramValues.MinValue)
                            grayscaleColor = _histogramValues.MinValue;

                        // And fill it with said color
                        for (int currentX = rect.X; currentX < xTo; currentX++)
                        {
                            for (int currentY = rect.Y; currentY < yTo; currentY++)
                            {
                                int currentPixelRectPosition = IndexAt(currentX, currentY);
                                bitmapBytes[currentPixelRectPosition] = grayscaleColor;
                                bitmapBytes[currentPixelRectPosition + 1] = grayscaleColor;
                                bitmapBytes[currentPixelRectPosition + 2] = grayscaleColor;
                            }
                        }
                    }
                }

                if (rect.Width > 1)
                {
                    rect.Width /= 2;
                    widthChanges++;
                }
                if (rect.Height > 1)
                {
                    rect.Height /= 2;
                    heightChanges++;
                }

                double progress = ((double)widthChanges / totalWidthChanges) + ((double)heightChanges / totalHeightChanges);
                reportProgress((int)(progress * 50));
            }
        }

        /// <summary>
        /// Converts two dimensional coordinates on the picture into single dimentional one
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private int IndexAt(int x, int y)
        {
            return (y * _width + x) * _bytesPerPixel;
        }
    }
}
