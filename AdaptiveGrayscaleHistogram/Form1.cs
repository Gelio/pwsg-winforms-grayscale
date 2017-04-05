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
    enum GrayscaleMode
    {
        Async,
        Sync
    }

    public partial class Form1 : Form
    {
        private Bitmap initialBitmap = null;
        private bool shouldReset = false;
        private GrayscaleMode grayscaleMode = GrayscaleMode.Async;
        

        public Form1()
        {
            InitializeComponent();
            comboBoxWorker.SelectedIndex = 0;
        }

        private void loadImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog selectFile = new OpenFileDialog();
            if (selectFile.ShowDialog() == DialogResult.Cancel)
                return;

            using (Stream imageStream = selectFile.OpenFile())
            {
                try
                {
                    initialBitmap = new Bitmap(imageStream);
                } catch
                {
                    MessageBox.Show("Cannot load image");
                    return;
                }
            }


            pictureBox.Image = initialBitmap;
            resetToolStripMenuItem.Enabled = true;
            grayscaleToolStripMenuItem.Enabled = true;
            this.MaximumSize = initialBitmap.Size;
            this.Size = initialBitmap.Size;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grayscaleToolStripMenuItem.Enabled = true;
            if (grayscaleMode == GrayscaleMode.Sync)
            {
                shouldReset = true;
                pictureBox.Image = initialBitmap;
            }
            else if (grayscaleMode == GrayscaleMode.Async)
            {
                if (backgroundWorker1.CancellationPending)
                    return;

                backgroundWorker1.CancelAsync();
                progressBar.Value = 0;

                if (!backgroundWorker1.IsBusy)
                {
                    pictureBox.Image = initialBitmap;
                    return;
                }
            }
            
        }

        private GrayscaleMode ParseGrayscaleMode()
        {
            if (comboBoxWorker.SelectedIndex == 0)
                return GrayscaleMode.Sync;
            else
                return GrayscaleMode.Async;
        }

        private void grayscaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            grayscaleToolStripMenuItem.Enabled = false;
            grayscaleMode = ParseGrayscaleMode();
            
            if (grayscaleMode == GrayscaleMode.Sync)
            {
                StartGrayscaleSync();
            }
            else if (grayscaleMode == GrayscaleMode.Async)
            {
                if (backgroundWorker1.IsBusy)
                    return;

                pictureBox.Image = new Bitmap(initialBitmap);
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void StartGrayscaleSync()
        {
            shouldReset = false;
            pictureBox.Image = new Bitmap(initialBitmap);
            Bitmap bm = pictureBox.Image as Bitmap;

            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    if (shouldReset)
                    {
                        shouldReset = false;
                        return;
                    }

                    Color baseColor = bm.GetPixel(x, y);
                    bm.SetPixel(x, y, grayscaleColor(baseColor));
                }
            }
        }

        private Color grayscaleColor(Color baseColor)
        {
            int endColor = (int)(baseColor.R * 0.3 + baseColor.G * 0.59 + baseColor.B * 0.11);
            return Color.FromArgb(endColor, endColor, endColor);
        }

        private void backgroundWorker1_DoWork_1(object sender, DoWorkEventArgs e)
        {
            Bitmap bm = new Bitmap(initialBitmap);
            long totalPixels = bm.Width * bm.Height;
            long currentPixel = 0;

            for (int x = 0; x < bm.Width; x++)
            {
                for (int y = 0; y < bm.Height; y++)
                {
                    if (backgroundWorker1.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }

                    currentPixel++;
                    Color baseColor = bm.GetPixel(x, y);
                    bm.SetPixel(x, y, grayscaleColor(baseColor));
                }
                int progress = (int)((double)(currentPixel) / (double)(totalPixels) * 100);
                backgroundWorker1.ReportProgress(progress, bm);
                Thread.Sleep(10);
            }

            pictureBox.Image = bm;
        }

        private void backgroundWorker1_ProgressChanged_1(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            pictureBox.Image = e.UserState as Image;
        }

        private void backgroundWorker1_RunWorkerCompleted_1(object sender, RunWorkerCompletedEventArgs e)
        {
            progressBar.Value = 100;
            grayscaleToolStripMenuItem.Enabled = true;
        }
    }
}
