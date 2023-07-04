using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Lima;
using AForge.Video;
using AForge.Video.DirectShow;

public class WebCamManager
{
    public WebCamManager(object lockObject = null)
    {
        if (lockObject is null)
            lockObject = new object();
        this.obj = lockObject;
    }

    private VideoCaptureDevice? cam;
    private Bitmap im = null;
    private object obj = null;
    private List<Action<Bitmap>> requests = new List<Action<Bitmap>>();

    public Bitmap Image => im;

    public void Load()
    {
        var webcam = new FilterInfoCollection(FilterCategory.VideoInputDevice);
        if (webcam != null && webcam.Count > 0)
        {
            cam = new VideoCaptureDevice(webcam[0].MonikerString);
            cam.NewFrame += onFrame;
        }
    }

    public void Start()
    {
        cam.Start();
    }

    public void Stop()
    {
        if (cam != null && cam.IsRunning)
        {
            cam.SignalToStop();
            cam.WaitForStop();
            cam.NewFrame -= onFrame;
            cam = null;
        }
    }

    public ImageProvider CreateProvider()
    {
        ImageProvider provider = new ImageProvider(this, obj);
        return provider;
    } 

    public void AddHandler(int interval, Action<Bitmap> func)
    {
        var provider = CreateProvider();
        provider.Interval = interval;
        provider.OnFrame += func;
    }

    public void RequestScreenshot(Action<Bitmap> callback)
        => this.requests.Add(callback);

    private void onFrame(object sender, NewFrameEventArgs eventArgs)
    {
        lock (obj)
        {
            if (im is not null)
                im.Dispose();
            this.im = (Bitmap)eventArgs.Frame
                .GetThumbnailImage(480, 320, null, IntPtr.Zero);

            if (requests.Count == 0)
                return;

            foreach (var request in this.requests)
                request((Bitmap)this.im.Clone());

            requests.Clear();
        }
    }
}