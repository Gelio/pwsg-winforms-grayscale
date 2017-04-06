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
    enum FilterType
    {
        Grayscale,
        Adaptive
    }

    public partial class Form1 : Form
    {
        private Bitmap initialBitmap = null;
        private Histogram histogram = new Histogram();
        private Filters.IFilter filter = null;
        private HistogramValues histogramValues = new HistogramValues(0, 255);


        public Form1()
        {
            InitializeComponent();
            histogram.ValueChanged += Histogram_ValueChanged;
        }

        private void Histogram_ValueChanged(object sender, HistogramValues e)
        {
            if (backgroundWorkerGrayscale.IsBusy)
                backgroundWorkerGrayscale.CancelAsync();

            histogramValues = e;
            filter = new Filters.AdaptiveFilter(histogramValues);
            StartBackgroundWorker();
        }

        private void backgroundWorkerGrayscale_DoWork(object sender, DoWorkEventArgs e)
        {
            int buffersCount = 5;
            Bitmap[] buffers = new Bitmap[buffersCount];
            for (int i = 0; i < buffersCount; i++)
                buffers[i] = new Bitmap(initialBitmap);
            int currentBuffer = 0;

            int width = buffers[0].Width,
                height = buffers[0].Height;
            Rectangle rect = new Rectangle(0, 0, width, height);
            byte[] bitmapBytes = GrayscaleHelpers.Array1DFromBitmap(buffers[0]);
            int bytesPerPixel = Bitmap.GetPixelFormatSize(buffers[0].PixelFormat) / 8;

            Func<bool> shouldCancel = () => backgroundWorkerGrayscale.CancellationPending;

            Action<int> reportWorkerProgress = progress =>
            {
                // Move to the next buffer
                currentBuffer = (currentBuffer + 1) % buffersCount;

                // Lock bits
                BitmapData currentBitmapData = buffers[currentBuffer].LockBits(rect, ImageLockMode.ReadWrite, buffers[currentBuffer].PixelFormat);

                // Copy bytes to current buffer
                System.Runtime.InteropServices.Marshal.Copy(bitmapBytes, 0, currentBitmapData.Scan0, bitmapBytes.Length);

                // Unlock bits
                buffers[currentBuffer].UnlockBits(currentBitmapData);

                backgroundWorkerGrayscale.ReportProgress(progress, buffers[currentBuffer]);
                Thread.Sleep(5);
            };

            filter.Apply(buffers, bitmapBytes, bytesPerPixel, width, height, reportWorkerProgress, shouldCancel);

            if (shouldCancel())
                e.Cancel = true;

            e.Result = buffers[currentBuffer];
        }

        private void backgroundWorkerGrayscale_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pictureBox.Image = e.UserState as Bitmap;

            SetProgressBarValue(e.ProgressPercentage);
        }

        private void SetProgressBarValue(int value)
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

        private void backgroundWorkerGrayscale_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox.Image = e.Result as Bitmap;
                SetProgressBarValue(100);
            }

            grayscaleToolStripMenuItem.Enabled = true;
            adaptiveToolStripMenuItem.Enabled = true;
            histogramToolStripMenuItem.Enabled = true;
        }

        private void loadImageToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (backgroundWorkerGrayscale.IsBusy)
                backgroundWorkerGrayscale.CancelAsync();

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


            pictureBox.Image = initialBitmap;
            resetToolStripMenuItem.Enabled = true;
            grayscaleToolStripMenuItem.Enabled = true;
            adaptiveToolStripMenuItem.Enabled = true;
            histogramToolStripMenuItem.Enabled = true;
            this.MaximumSize = initialBitmap.Size;
            this.Size = initialBitmap.Size;
        }

        private void resetToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (backgroundWorkerGrayscale.IsBusy)
            {
                backgroundWorkerGrayscale.CancelAsync();
                SetProgressBarValue(0);
            }
            else
            {
                if (backgroundWorkerGrayscale.CancellationPending)
                    return;

                pictureBox.Image = initialBitmap;
                SetProgressBarValue(0);
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = new Filters.GrayscaleFilter();
            StartBackgroundWorker();
        }

        private void histogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            histogram.ShowDialog();
        }

        private void adaptiveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filter = new Filters.AdaptiveFilter(histogramValues);
            StartBackgroundWorker();
        }

        private void StartBackgroundWorker()
        {
            grayscaleToolStripMenuItem.Enabled = false;
            adaptiveToolStripMenuItem.Enabled = false;

            if (backgroundWorkerGrayscale.IsBusy)
                return;

            backgroundWorkerGrayscale.RunWorkerAsync();
        }
    }
}
