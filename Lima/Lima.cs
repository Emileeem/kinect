namespace Lima;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Oscar;

class Lima
{
    static void flame(Bitmap bg, Bitmap frame)
    {
        //Gera o histograma e equaliza o fundo;
        int[] hist1 = histogram(bg);
        equalize(hist1, bg);

        //Gera o histograma e equaliza a imagem com algo na frente;
        int[] hist2 = histogram(frame);
        equalize(hist2, frame);

        //Define o threshold
        int[] hist3 = histSub(bg, frame);
        int n = frame.Height * frame.Width;
        int threshold = Otsu.genOtsu(hist3, n);

        //Binariza a imagem
        binarize(threshold, bg, frame);

        // MessageBox.Show((DateTime.Now - dt).TotalMilliseconds.ToString());
        // img2.Save("./imagensOFC/flameImage2.png");

    }

    unsafe static int[] histogram(Bitmap bmp)
    {
        int[] hist = new int[256];

        var data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                int index = 3 * i + j * data.Stride;
                byte b = im[index];
                byte g = im[index + 1];
                byte r = im[index + 2];
                int intensidade = (int)(r * 0.299 + g * 0.587 + b * 0.114);
                hist[intensidade]++;
            }
        }

        bmp.UnlockBits(data);

        return hist;
    }

    unsafe static void equalize(int[] vetor, Bitmap bmp)
    {
        int max = 0;
        int min = 0;

        for (int i = 0; i < 255; i++)
        {
            if (vetor[i] != 0)
            {
                min = i;
                break;
            }
        }
        for (int j = 255; j > 0; j--)
        {
            if (vetor[j] != 0)
            {
                max = j;
                break;
            }
        }

        int[] equalize = new int[256];

        var data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        for (int i = 0; i < data.Width; i++)
        {
            for (int j = 0; j < data.Height; j++)
            {
                int index = 3 * i + j * data.Stride;
                int b = im[index];
                int g = im[index + 1];
                int r = im[index + 2];

                r = (255 * (r - min)) / (max - min);
                g = (255 * (g - min)) / (max - min);
                b = (255 * (b - min)) / (max - min);

                if (r > 255)
                    r = 255;
                if (r < 0)
                    r = 0;
                if (g > 255)
                    g = 255;
                if (g < 0)
                    g = 0;
                if (b > 255)
                    b = 255;
                if (b < 0)
                    b = 0;

                im[index] = (byte)(b);
                im[index + 1] = (byte)(g);
                im[index + 2] = (byte)(r);
            }
        }

        bmp.UnlockBits(data);
    }

    unsafe static int[] histSub(Bitmap bmp1, Bitmap bmp2)
    {
        int[] sub = new int[256];

        var data = bmp1.LockBits(
            new Rectangle(0, 0, bmp1.Width, bmp1.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        var data2 = bmp2.LockBits(
            new Rectangle(0, 0, bmp2.Width, bmp2.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* img1 = (byte*)data.Scan0.ToPointer();
        byte* img2 = (byte*)data2.Scan0.ToPointer();


        for (int j = 0; j < data.Height; j++)
        {
            for (int i = 0; i < data.Width; i++)
            {
                int index = 3 * i + j * data.Stride;

                float db = MathF.Abs(img1[index + 0] - img2[index + 0]);
                float dg = MathF.Abs(img1[index + 1] - img2[index + 1]);
                float dr = MathF.Abs(img1[index + 2] - img2[index + 2]);

                int diff = (int)(db + dg + dr) / 3;
                sub[diff]++;
            }
        }

        bmp1.UnlockBits(data);
        bmp2.UnlockBits(data2);

        return sub;
    }

    unsafe static void binarize(int threshold, Bitmap bmp1, Bitmap bmp2)
    {
        var data1 = bmp1.LockBits(
            new Rectangle(0, 0, bmp1.Width, bmp1.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );
        byte* im1 = (byte*)data1.Scan0.ToPointer();

        var data2 = bmp2.LockBits(
            new Rectangle(0, 0, bmp2.Width, bmp2.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );
        byte* im2 = (byte*)data2.Scan0.ToPointer();

        for (int i = 0; i < data1.Width; i++)
        {
            for (int j = 0; j < data1.Height; j++)
            {
                int index = 3 * i + j * data1.Stride;

                int db = im1[index + 0] - im2[index + 0];
                int dg = im1[index + 1] - im2[index + 1];
                int dr = im1[index + 2] - im2[index + 2];

                if (db < 0)
                    db = -db;

                if (dg < 0)
                    dg = -dg;

                if (dr < 0)
                    dr = -dr;


                int diff = (dr + dg + db) / 3;

                if (diff < threshold)
                {
                    im2[index] = (byte)(255);
                    im2[index + 1] = (byte)(255);
                    im2[index + 2] = (byte)(255);
                }
                else
                {
                    im2[index] = (byte)(0);
                    im2[index + 1] = (byte)(0);
                    im2[index + 2] = (byte)(0);
                }
            }
        }

        bmp1.UnlockBits(data1);
        bmp2.UnlockBits(data2);
    }
}



