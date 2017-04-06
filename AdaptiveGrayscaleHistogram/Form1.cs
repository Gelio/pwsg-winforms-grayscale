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

        private byte GetGrayscaleColor(byte r, byte g, byte b)
        {
            return (byte)(r * 0.3 + g * 0.59 + b * 0.11);
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            int buffersCount = 5;
            Bitmap[] buffers = new Bitmap[buffersCount];
            for (int i=0; i < buffersCount; i++)
                buffers[i] = new Bitmap(initialBitmap);
            int currentBuffer = -1;

            int width = buffers[0].Width,
                height = buffers[0].Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            byte[] bitmapBytes = Array1DFromBitmap(buffers[0]);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(buffers[0].PixelFormat) / 8;
            int currentPixelPosition = 0;

            for (int y = 0; y < height; y++)
            {
                // Move to the next bitmap
                currentBuffer = (currentBuffer + 1) % buffersCount;

                // Lock bits
                BitmapData currentBitmapData = buffers[currentBuffer].LockBits(rect, ImageLockMode.ReadWrite, buffers[currentBuffer].PixelFormat);

                for (int x = 0; x < width; x++, currentPixelPosition += bytesPerPixel)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    
                    byte grayscaleColor = GetGrayscaleColor(bitmapBytes[currentPixelPosition], bitmapBytes[currentPixelPosition + 1], bitmapBytes[currentPixelPosition + 2]);
                    bitmapBytes[currentPixelPosition] = grayscaleColor;
                    bitmapBytes[currentPixelPosition + 1] = grayscaleColor;
                    bitmapBytes[currentPixelPosition + 2] = grayscaleColor;
                }

                System.Runtime.InteropServices.Marshal.Copy(bitmapBytes, 0, currentBitmapData.Scan0, bitmapBytes.Length);

                // Unlock bits
                buffers[currentBuffer].UnlockBits(currentBitmapData);
                

                int progress = Convert.ToInt32((double)((double)(y + 1) * 100 / (double)height));
                backgroundWorker1.ReportProgress(progress, buffers[currentBuffer]);
                Thread.Sleep(1);
            }

            e.Result = buffers[currentBuffer];
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

        private void backgroundWorker1_ProgressChanged_1(object sender, ProgressChangedEventArgs e)
        {
            pictureBox.Image = e.UserState as Bitmap;

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
