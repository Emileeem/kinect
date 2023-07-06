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
using Bravo;
using Eco;

namespace Webcam
{
    public partial class Form1 : Form
    {
        WebCamManager cam;
        Bitmap bg = null;
        float bgMedia = 0;

        bool useBlur = true;
        bool useEsq = false;
        int blur = 10;
        int bgBlur = 0;

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
                        if (this.bg is null)
                        {
                            cam.RequestScreenshot(im =>
                            {
                                this.bg = Blur.Apply(im, blur);
                                this.bgMedia = mediaBg(histogram(this.bg));
                                this.bgBlur = blur;
                            });
                        }
                        else
                        {
                            this.bg = null;
                            this.bgBlur = 0;
                        }
                        break;
                    case Keys.Enter:
                        if (this.useEsq)
                            this.useEsq = false;
                        else this.useEsq = true;
                        break;
                    case Keys.Tab:
                        if (this.useBlur)
                            this.useBlur = false;
                        else this.useBlur = true;
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
                label1.Text =
                    "Blur/Quant: \n" + (this.useBlur ? "off/" : "on/") + this.blur.ToString() + "\n\n" +
                    "BlurBg: " + this.bgBlur.ToString();

                lock (cam)
                {
                    if (!this.useBlur)
                        im = Blur.Apply(im, blur);
                    im = flame(bgMedia, bg, im);
                    if (this.useEsq)
                        try
                        {
                            im = Esqueleto.genEsqueleto(im);
                        }
                        catch (Exception)
                        {
                            
                        }

                    pbWebcam.Image = im;
                }
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
