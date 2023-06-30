namespace Lima;

using System;
using System.Drawing;
using System.Drawing.Imaging;
using Oscar;

class Lima
{
    static Bitmap flame(Bitmap img1, Bitmap img2)
    {
        int[] hist1 = histogram(img1);
        int[] hist2 = histogram(img2);

        correcaoLuminosidade(img1, hist1, img2, hist2);

        int[] hist3 = histSub(img1, img2);
        int n = img2.Height * img2.Width;
        int threshold = Otsu.genOtsu(hist3, n);

        binarize(threshold, img1, img2);

        return img2;
    }


    static unsafe int[] histogram(Bitmap bmp)
    {
        int[] hist = new int[256];

        var data = bmp.LockBits(
            new Rectangle(0, 0, bmp.Width, bmp.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        int limitX = bmp.Width * 5 / 100 - 1;
        for (int i = 0, k = bmp.Width - 1; i < limitX; i++, k--)
        {
            for (int j = 0; j < bmp.Height; j++)
            {
                int index1 = 3 * i + j * data.Stride;
                byte b1 = im[index1];
                byte g1 = im[index1 + 1];
                byte r1 = im[index1 + 2];
                int intensidade1 = (299 * r1 + 587 * g1 + 114 * b1) / 1000;
                hist[intensidade1]++;

                int index2 = 3 * k + j * data.Stride;
                byte b2 = im[index2];
                byte g2 = im[index2 + 1];
                byte r2 = im[index2 + 2];
                int intensidade2 = (299 * r2 + 587 * g2 + 114 * b2) / 1000;
                hist[intensidade2]++;
            }
        }

        int limitJ = bmp.Height * 5 / 100;
        for (int i = 0; i < bmp.Width; i++)
        {
            for (int j = 0; j < limitJ; j++)
            {
                int index1 = 3 * i + j * data.Stride;
                byte b1 = im[index1];
                byte g1 = im[index1 + 1];
                byte r1 = im[index1 + 2];
                int intensidade1 = (int)(r1 * 0.299 + g1 * 0.587 + b1 * 0.114);
                hist[intensidade1]++;
            }
        }

        bmp.UnlockBits(data);

        return hist;
    }

    static unsafe int[] histSub(Bitmap bmp1, Bitmap bmp2)
    {
        int[] sub = new int[256 * 256];

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

                float db = img1[index + 0] - img2[index + 0];
                float dg = img1[index + 1] - img2[index + 1];
                float dr = img1[index + 2] - img2[index + 2];

                if (db < 0)
                    db = -db;

                if (dg < 0)
                    dg = -dg;

                if (dr < 0)
                    dr = -dr;

                int diff = (int)(db + dg + dr) / 3;
                sub[diff]++;
            }
        }

        bmp1.UnlockBits(data);
        bmp2.UnlockBits(data2);

        return sub;
    }

    static unsafe void binarize(int threshold, Bitmap bmp1, Bitmap bmp2)
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

        for (int j = 0; j < data1.Height; j++)
        {
            for (int i = 0; i < data1.Width; i++)
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
        }

        bmp1.UnlockBits(data1);
        bmp2.UnlockBits(data2);
    }

    static unsafe void correcaoLuminosidade(Bitmap bmp1, int[] hist1, Bitmap bmp2, int[] hist2)
    {
        float sum = 0;
        float count = 0;
        for (int k = 0; k < hist1.Length; k++)
        {
            count += hist1[k];
            sum += k * hist1[k];
        }
        float media1 = sum / count;

        sum = 0;
        count = 0;
        for (int k = 0; k < hist2.Length; k++)
        {
            count += hist2[k];
            sum += k * hist2[k];
        }
        float media2 = sum / count;

        var data = bmp1.LockBits(
            new Rectangle(0, 0, bmp1.Width, bmp1.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb
        );
        var data2 = bmp2.LockBits(
            new Rectangle(0, 0, bmp2.Width, bmp2.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data2.Scan0.ToPointer();
        float alfa = media1 / media2;

        for (int j = 0; j < bmp2.Height; j++)
        {
            for (int i = 0; i < bmp2.Width; i++)
            {
                int index = 3 * i + j * data2.Stride;

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
        }

        bmp1.UnlockBits(data);
        bmp2.UnlockBits(data2);
    }
}



