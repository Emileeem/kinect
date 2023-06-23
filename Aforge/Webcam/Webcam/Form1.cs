using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.DirectShow;

namespace Webcam
{
    public partial class Form1 : Form
    {
        private VideoCaptureDevice? camera;
        public Form1()
        {
            InitializeComponent();

            KeyPreview = true;
            this.KeyDown += (o, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        closeApp();
                        break;
                    case Keys.Enter:
                        ligar();
                        break;
                    case Keys.Space:
                        foto();
                        break;
                }
            };
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            var webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (webcam != null && webcam.Count > 0)
            {
                camera = new VideoCaptureDevice(webcam[0].MonikerString);
                camera.NewFrame += Camera_NewFrame;
            }
        }
        

        private void Camera_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (pbWebcam.Image != null)
            {
                pbWebcam.Image.Dispose();
            }
            pbWebcam.Image = (Bitmap)eventArgs.Frame.Clone();
        }


        void ligar()
        {
            camera.Start();
        }
        void foto()
        {
            if (pbWebcam.Image != null && camera.IsRunning)
            {
                try
                {
                    camera.NewFrame -= Camera_NewFrame;

                    using (var dialog = new SaveFileDialog())
                    {
                        dialog.DefaultExt = "png";
                        dialog.AddExtension = true;

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            pbWebcam.Image.Save(dialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                            pbWebcam.Image.Clone();
                        }
                    }
                }
                finally
                {
                    camera.NewFrame += Camera_NewFrame;
                }
            }
        }

        void closeApp()
        {
            if (camera != null && camera.IsRunning)
            {
                camera.SignalToStop();
                camera.WaitForStop();
                camera.NewFrame -= Camera_NewFrame;
                camera = null;
            }
            Close();
        }
    }
}
