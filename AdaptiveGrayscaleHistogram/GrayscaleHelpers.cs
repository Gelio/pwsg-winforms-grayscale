using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace AdaptiveGrayscaleHistogram
{
    static class GrayscaleHelpers
    {
        public static Color GetGrayscaleColor(Color baseColor)
        {
            int endColor = (int)(baseColor.R * 0.3 + baseColor.G * 0.59 + baseColor.B * 0.11);
            return Color.FromArgb(endColor, endColor, endColor);
        }

        public static byte GetGrayscaleColor(byte r, byte g, byte b)
        {
            return (byte)(r * 0.3 + g * 0.59 + b * 0.11);
        }

        public static byte[] Array1DFromBitmap(Bitmap bmp)
        {
            if (bmp == null) throw new NullReferenceException("Bitmap is null");

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            IntPtr ptr = data.Scan0;

            //declare an array to hold the bytes of the bitmap
            int numBytes = data.Stride * bmp.Height;
            byte[] bytes = new byte[numBytes];

            //copy the RGB values into the array
            System.Runtime.InteropServices.Marshal.Copy(ptr, bytes, 0, numBytes);

            bmp.UnlockBits(data);

            return bytes;
        }
    }
}
