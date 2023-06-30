using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using static Lima.Lima;
using static Bravo.Blur;

namespace Webcam
{
    public partial class Form1 : Form
    {
        WebCamManager cam;
        Bitmap bg = null;
        bool seeBg = true;
        int blur = 5;

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
                    case Keys.Space:
                         cam.RequestScreenshot(im => this.bg = UseBlur(im, blur));
                        break;
                    case Keys.Enter:
                        this.bg = null;
                        break;
                    case Keys.Tab:
                        if (this.seeBg)
                            this.seeBg = false;
                        else this.seeBg = true;
                        break;
                    case Keys.Add:
                        if (this.blur < 50)
                            this.blur++;
                        break;
                    case Keys.Subtract:
                        if (this.blur > 0)
                            this.blur--;
                        break;

                        
                }
            };
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            cam = new WebCamManager();
            cam.Load();
            cam.AddHandler(25, im =>
            {
                label1.Text = "Blur: " + blur.ToString();
                label2.Text = "Blur: " + blur.ToString();
                label3.Text = "Blur: " + blur.ToString();
                if (!this.seeBg)
                    im = UseBlur(im, blur);
                im = flame(bg, im);
                pbWebcam.Image = im;
            });



        void closeApp()
        {
            cam.Stop();
            Close();
        }
            Load += delegate
            {
                cam.Start();
            };
        }


    }
}
