using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            public Bitmap bm;
            public int x;
            public object bgLock = new object();
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            Bitmap bm = new Bitmap(initialBitmap);
            int width = bm.Width,
                height = bm.Height;
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

                    lock (bgLock)
                    {
                        Color baseColor = bm.GetPixel(x, y);
                        Color grayscaleColor = GetGrayscaleColor(baseColor);
                        bm.SetPixel(x, y, GetGrayscaleColor(baseColor));
                    }
                }

                int progress = Convert.ToInt32((double)((double)(x + 1) * 100 / (double)width));
                backgroundWorker1.ReportProgress(progress, new ProgressInfo() { bm = bm, x = x, bgLock = bgLock });
                Thread.Sleep(2);
            }

            e.Result = bm;
        }

        private void backgroundWorker1_ProgressChanged_1(object sender, ProgressChangedEventArgs e)
        {
            ProgressInfo info = e.UserState as ProgressInfo;
            int x = info.x;
            Bitmap bm = pictureBox.Image as Bitmap;
            lock (info.bgLock)
            {
                for (int y = 0; y < bm.Height; y++)
                    bm.SetPixel(x, y, info.bm.GetPixel(x, y));
            }
            pictureBox.Image = bm;

            // Weird stuff with synchronizing the progress bar
            if (e.ProgressPercentage == progressBar.Maximum)
            {
                progressBar.Maximum = e.ProgressPercentage + 1;
                progressBar.Value = e.ProgressPercentage + 1;
                progressBar.Maximum = e.ProgressPercentage;
            }
            else
                progressBar.Value = e.ProgressPercentage + 1;
                
            progressBar.Value = e.ProgressPercentage;
        }

        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                pictureBox.Image = e.Result as Bitmap;
                progressBar.Value = 100;
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
                progressBar.Value = 0;
            }
            else
            {
                if (backgroundWorker1.CancellationPending)
                    return;

                pictureBox.Image = new Bitmap(initialBitmap);
                progressBar.Value = 0;
            }
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grayscaleToolStripMenuItem.Enabled = false;
            adaptiveToolStripMenuItem.Enabled = false;
            // histogramToolStripMenuItem.Enabled = false;

            if (backgroundWorker1.IsBusy)
                return;

            pictureBox.Image = new Bitmap(initialBitmap);
            backgroundWorker1.RunWorkerAsync();
        }
    }
}
