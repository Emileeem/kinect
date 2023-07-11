namespace Lima;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Kilo;
using Papa;

static class Lima
{
    static public Bitmap flame(float mediaBg, Bitmap img1, Bitmap img2)
    {
        if (img1 is null)
            return img2;

        int[] hist2 = histogram(img2);

        correcaoLuminosidade(mediaBg, img2, hist2);

        var subTransformed = histSubT(img1, img2);

        byte[] kmeans = Threshold.Kmeans3D(subTransformed);
        var p = ((float) kmeans[0], (float) kmeans[1], (float) kmeans[2]);
        var q = ((float) kmeans[3], (float) kmeans[4], (float) kmeans[5]);

        var abcd = Plano.CalcularPlano(p,q);
        binarize(img1, img2, ((float)abcd.a, (float)abcd.b, (float)abcd.c, (float)abcd.d));

        return img2;
    }


    static public unsafe int[] histogram(Bitmap bmp)
    {
        int[] hist = new int[256];

        var data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        int limitX = bmp.Width * 5 / 100 - 1;
        int limitX2 = bmp.Width * 95 / 100 - 1;
        int height = bmp.Height;
        int width = bmp.Width;

        Parallel.Invoke(
            () =>
            {
                Parallel.For(0, limitX, i =>
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index1 = 3 * i + j * data.Stride;
                        byte b1 = im[index1];
                        byte g1 = im[index1 + 1];
                        byte r1 = im[index1 + 2];
                        int intensity1 = (299 * r1 + 587 * g1 + 114 * b1) / 1000;
                        Interlocked.Increment(ref hist[intensity1]);
                    }
                });
            },

            () =>
            {
                Parallel.For(limitX2, width, i =>
                {
                    for (int j = 0; j < height; j++)
                    {
                        int index2 = 3 * i + j * data.Stride;
                        byte b2 = im[index2];
                        byte g2 = im[index2 + 1];
                        byte r2 = im[index2 + 2];
                        int intensity2 = (299 * r2 + 587 * g2 + 114 * b2) / 1000;
                        Interlocked.Increment(ref hist[intensity2]);
                    }
                });
            }
        );

        int limitJ = bmp.Height * 5 / 100;
        Parallel.For(0, bmp.Width, i =>
        {
            for (int j = 0; j < limitJ; j++)
            {
                int index1 = 3 * i + j * data.Stride;
                byte b1 = im[index1];
                byte g1 = im[index1 + 1];
                byte r1 = im[index1 + 2];
                int intensity1 = (int)(r1 * 0.299 + g1 * 0.587 + b1 * 0.114);
                Interlocked.Increment(ref hist[intensity1]);
            }
        });

        bmp.UnlockBits(data);

        return hist;
    }

    static public unsafe float[] histSubT(Bitmap bmp1, Bitmap bmp2)
    {

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

        int tamanho = (data.Height * data.Width * 3) / 25;

        float[] sub = new float[tamanho];

        byte* img1 = (byte*)data.Scan0.ToPointer();
        byte* img2 = (byte*)data2.Scan0.ToPointer();

        int k = 0;

        for (int j = 0; j < data.Height - 1; j += 5)
        {
            for (int i = 0; i < data.Width - 1; i += 5)
            {
                int index = 3 * i + j * data.Stride;

                float dr = img1[index + 2] - img2[index + 2];
                float dg = img1[index + 1] - img2[index + 1];
                float db = img1[index + 0] - img2[index + 0];

                if (dr < 0)
                    dr = -dr;

                if (dg < 0)
                    dg = -dg;

                if (db < 0)
                    db = -db;

                sub[k + 0] = dr;
                sub[k + 1] = dg;
                sub[k + 2] = db;

                k += 3;
            }
        }

        bmp1.UnlockBits(data);
        bmp2.UnlockBits(data2);

        return sub;
    }

    static public unsafe void binarize(Bitmap bmp1, Bitmap bmp2, (float a, float b, float c, float d) tupla)
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

        int width = data1.Width;
        int stride = data1.Stride;

        Parallel.For(0, data1.Height, j =>
        {
            for (int i = 0; i < width; i++)
            {
                int index = 3 * i + j * stride;

                float db = im1[index + 0] - im2[index + 0];
                float dg = im1[index + 1] - im2[index + 1];
                float dr = im1[index + 2] - im2[index + 2];

                dr = dr * dr;
                dg = dg * dg;
                db = db * db;

                float diff = (dr * tupla.a + dg * tupla.b + db * tupla.c + tupla.d);

                if (diff > 0)
                {
                    im2[index + 0] = (byte)(255);
                    im2[index + 1] = (byte)(255);
                    im2[index + 2] = (byte)(255);
                }
                else
                {
                    im2[index + 0] = (byte)(0);
                    im2[index + 1] = (byte)(0);
                    im2[index + 2] = (byte)(0);
                }
            }
        });

        bmp1.UnlockBits(data1);
        bmp2.UnlockBits(data2);
    }


    static public unsafe void correcaoLuminosidade(float mediaBg, Bitmap bmp2, int[] hist2)
    {
        float sum = 0;
        float count = 0;
        for (int k = 0; k < hist2.Length; k++)
        {
            count += hist2[k];
            sum += k * hist2[k];
        }
        float media2 = sum / count;

        var data2 = bmp2.LockBits(
            new Rectangle(0, 0, bmp2.Width, bmp2.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data2.Scan0.ToPointer();
        float alfa = mediaBg / media2;

        int width = bmp2.Width;
        int stride = data2.Stride;

        Parallel.For(0, bmp2.Height, j =>
        {
            for (int i = 0; i < width; i++)
            {
                int index = 3 * i + j * stride;

                im[index + 0] = (byte)(im[index + 0] * alfa);
                im[index + 1] = (byte)(im[index + 1] * alfa);
                im[index + 2] = (byte)(im[index + 2] * alfa);

                if (im[index + 0] > 255)
                    im[index + 0] = 255;
                if (im[index + 1] > 255)
                    im[index + 1] = 255;
                if (im[index + 2] > 255)
                    im[index + 2] = 255;
            }
        });

        bmp2.UnlockBits(data2);
    }


    static public float mediaBg(int[] histBg)
    {
        float sum = 0;
        float count = 0;
        for (int k = 0; k < histBg.Length; k++)
        {
            count += histBg[k];
            sum += k * histBg[k];
        }
        return sum / count;
    }
}



