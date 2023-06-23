namespace Webcam
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbWebcam = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbWebcam)).BeginInit();
            this.SuspendLayout();
            // 
            // pbWebcam
            // 
            this.pbWebcam.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbWebcam.Location = new System.Drawing.Point(0, 0);
            this.pbWebcam.Name = "pbWebcam";
            this.pbWebcam.Size = new System.Drawing.Size(786, 483);
            this.pbWebcam.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbWebcam.TabIndex = 1;
            this.pbWebcam.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 483);
            this.Controls.Add(this.pbWebcam);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pbWebcam)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private PictureBox pbWebcam;
    }
}