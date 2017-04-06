namespace AdaptiveGrayscaleHistogram
{
    partial class Histogram
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.trackBarBottomValue = new System.Windows.Forms.TrackBar();
            this.trackBarTopValue = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBottomValue)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTopValue)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBarBottomValue
            // 
            this.trackBarBottomValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarBottomValue.Location = new System.Drawing.Point(12, 17);
            this.trackBarBottomValue.Maximum = 255;
            this.trackBarBottomValue.Name = "trackBarBottomValue";
            this.trackBarBottomValue.Size = new System.Drawing.Size(309, 45);
            this.trackBarBottomValue.TabIndex = 0;
            this.trackBarBottomValue.ValueChanged += new System.EventHandler(this.anyTrackbar_ValueChanged);
            // 
            // trackBarTopValue
            // 
            this.trackBarTopValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarTopValue.Location = new System.Drawing.Point(12, 64);
            this.trackBarTopValue.Maximum = 255;
            this.trackBarTopValue.Name = "trackBarTopValue";
            this.trackBarTopValue.Size = new System.Drawing.Size(309, 45);
            this.trackBarTopValue.TabIndex = 1;
            this.trackBarTopValue.Value = 255;
            this.trackBarTopValue.ValueChanged += new System.EventHandler(this.anyTrackbar_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Bottom value";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Top value";
            // 
            // Histogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(333, 133);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarTopValue);
            this.Controls.Add(this.trackBarBottomValue);
            this.Name = "Histogram";
            this.Text = "Histogram";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBottomValue)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarTopValue)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBarBottomValue;
        private System.Windows.Forms.TrackBar trackBarTopValue;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}