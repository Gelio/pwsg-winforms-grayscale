using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace AdaptiveGrayscaleHistogram
{
    public partial class Form1 : Form
    {
        private Bitmap initialBitmap = null;
        

        public Form1()
        {
            InitializeComponent();
        }

        private Color GetGrayscaleColor(Color baseColor)
        {
            int endColor = (int)(baseColor.R * 0.3 + baseColor.G * 0.59 + baseColor.B * 0.11);
            return Color.FromArgb(endColor, endColor, endColor);
        }

        private class ProgressInfo
        {
            public Bitmap bmp;
            public int x;
            public object bgLock = new object();
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            Bitmap bmp = new Bitmap(initialBitmap);
            int width = bmp.Width,
                height = bmp.Height;
            byte[] pixelArray = Array1DFromBitmap(bmp);
            object bgLock = new object();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    //lock (bgLock)
                    //{
                        Color baseColor = bmp.GetPixel(x, y);
                        Color grayscaleColor = GetGrayscaleColor(baseColor);
                        bmp.SetPixel(x, y, GetGrayscaleColor(baseColor));
                    //}
                }

                int progress = Convert.ToInt32((double)((double)(x + 1) * 100 / (double)width));
                backgroundWorker1.ReportProgress(progress, new ProgressInfo() { bmp = BitmapFromArray1D(pixelArray, width, height, bmp.PixelFormat), x = x, bgLock = bgLock });
                Thread.Sleep(2);
            }

            e.Result = bmp;
        }

        private static byte[] Array1DFromBitmap(Bitmap bmp)
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

        private static Bitmap BitmapFromArray1D(byte[] bytes, int width, int height, PixelFormat pixelFormat)
        {
            Bitmap grayBmp = new Bitmap(width, height, pixelFormat);
            Rectangle grayRect = new Rectangle(0, 0, grayBmp.Width, grayBmp.Height);
            BitmapData grayData = grayBmp.LockBits(grayRect, ImageLockMode.ReadWrite, grayBmp.PixelFormat);
            IntPtr grayPtr = grayData.Scan0;

            int grayBytes = grayData.Stride * grayBmp.Height;

            System.Runtime.InteropServices.Marshal.Copy(bytes, 0, grayPtr, grayBytes);

            grayBmp.UnlockBits(grayData);
            return grayBmp;
        }

        private void backgroundWorker1_ProgressChanged_1(object sender, ProgressChangedEventArgs e)
        {
            ProgressInfo info = e.UserState as ProgressInfo;
            int x = info.x;
            //Bitmap bm = pictureBox.Image as Bitmap;
            //lock (info.bgLock)
            //{
            //    for (int y = 0; y < bm.Height; y++)
            //        bm.SetPixel(x, y, info.bm.GetPixel(x, y));
            //}
            pictureBox.Image = info.bmp;

            SetProgressValue(e.ProgressPercentage);
        }

        private void SetProgressValue(int value)
        {
            // Weird stuff with synchronizing the progress bar
            if (value == progressBar.Maximum)
            {
                progressBar.Maximum = value + 1;
                progressBar.Value = value + 1;
                progressBar.Maximum = value;
            }
            else
                progressBar.Value = value + 1;

            progressBar.Value = value;
        }

        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox.Image = e.Result as Bitmap;
                SetProgressValue(100);
            }

            grayscaleToolStripMenuItem.Enabled = true;
            adaptiveToolStripMenuItem.Enabled = true;
            histogramToolStripMenuItem.Enabled = true;
        }

        private void loadImageToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
                backgroundWorker1.CancelAsync();

            OpenFileDialog selectFile = new OpenFileDialog();
            if (selectFile.ShowDialog() == DialogResult.Cancel)
                return;

            using (Stream imageStream = selectFile.OpenFile())
            {
                try
                {
                    initialBitmap = new Bitmap(imageStream);
                }
                catch
                {
                    MessageBox.Show("Cannot load image");
                    return;
                }
            }


            pictureBox.Image = new Bitmap(initialBitmap);
            resetToolStripMenuItem.Enabled = true;
            grayscaleToolStripMenuItem.Enabled = true;
            adaptiveToolStripMenuItem.Enabled = true;
            histogramToolStripMenuItem.Enabled = true;
            this.MaximumSize = initialBitmap.Size;
            this.Size = initialBitmap.Size;
        }

        private void resetToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                backgroundWorker1.CancelAsync();
                SetProgressValue(0);
            }
            else
            {
                if (backgroundWorker1.CancellationPending)
                    return;

                pictureBox.Image = new Bitmap(initialBitmap);
                SetProgressValue(0);
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grayscaleToolStripMenuItem.Enabled = false;
            adaptiveToolStripMenuItem.Enabled = false;

            if (backgroundWorker1.IsBusy)
                return;

            pictureBox.Image = new Bitmap(initialBitmap);
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
