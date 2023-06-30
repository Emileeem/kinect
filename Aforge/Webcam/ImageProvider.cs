using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ImageProvider
{
    private System.Windows.Forms.Timer timer = new();
    private Bitmap img;

    public int Interval
    {
        get => timer.Interval;
        set => timer.Interval = value;
    }

    public ImageProvider(WebCamManager cam, object lockObj)
    {
        timer.Interval = 25;
        timer.Tick += delegate
        {
            if (cam.Image is null)
                return;

            lock (lockObj)
            {
                var old = img;

                img = (Bitmap)cam.Image.Clone();
                if (OnFrame != null)
                    OnFrame(img);

                if (old is null)
                    return;
                old.Dispose();
            }
        };
        timer.Start();
    }

    public event Action<Bitmap> OnFrame;
}