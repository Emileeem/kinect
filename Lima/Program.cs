using System.Drawing;

namespace Lima; 

class Histogram
{
    private int[] histogram = null;

    public Histogram(Bitmap bmp)
    {
        this.histogram = genHistogram(bmp);
    }

    private int[] genHistogram(Bitmap bmp)
    {
        int[] hist = new int[256];

        for (int y = 0; y < bmp.Height; y++)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                Color pixel = bmp.GetPixel(x, y);
                int intensidade = (int)(pixel.R * 0.299 + pixel.G * 0.587 + pixel.B * 0.114);
                hist[intensidade]++;
            }
        }

        return hist;
    }
}



