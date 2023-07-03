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
            this.label1 = new System.Windows.Forms.Label();
            //this.label2 = new System.Windows.Forms.Label();
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Blur: ";
            this.label1.Font = new Font("Arial", 25,FontStyle.Bold);
            // 
            // label2
            // 
            //this.label2.AutoSize = true;
            //this.label2.Location = new System.Drawing.Point(10, 70);
            //this.label2.Name = "label2";
            //this.label2.Size = new System.Drawing.Size(38, 15);
            //this.label2.TabIndex = 2;
            //this.label2.Text = "Bg: ";
            //this.label2.Font = new Font("Arial", 40, FontStyle.Bold);
            //// 
            //// label3
            //// 
            //this.label3.AutoSize = true;
            //this.label3.Location = new System.Drawing.Point(10, 50);
            //this.label3.Name = "label3";
            //this.label3.Size = new System.Drawing.Size(38, 15);
            //this.label3.TabIndex = 4;
            //this.label3.Text = "Blur: ";
            //this.label3.Font = new Font("Arial", 40,FontStyle.Bold);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 483);
            this.Controls.Add(this.label1);
            //this.Controls.Add(this.label2);
            this.Controls.Add(this.pbWebcam);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pbWebcam)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PictureBox pbWebcam;
        private Label label1;
        //private Label label2;
        //private Label label3;
    }
}