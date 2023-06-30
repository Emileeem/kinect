using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Bravo;

static class Blur
{
    static public unsafe Bitmap UseBlur(Bitmap input, int n)
    {
        long[] r, g, b;
        (r, g, b) = integral(input);

        var result = input.Clone() as Bitmap;
        var data = result.LockBits(
            new Rectangle(0, 0, result.Width, result.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        fixed (long* rPointer = r, gPointer = g, bPointer = b)
        {
            byte* p = (byte*)data.Scan0.ToPointer();
            long* rp = rPointer, gp = gPointer, bp = bPointer;
            SetImage(p, rp, gp, bp, input.Width, input.Height, data.Stride, n);
        }
        result.UnlockBits(data);

        return result;
    }

    static private unsafe void SetImage(byte* im, long* r, long* g, long* b, int width, int height, int stride, int n)
    {
        int side = 2 * n + 1;
        float area = side * side;

        int cantoA = -(n + 1) - (n + 1) * width; // nao ta aqui
        int cantoB = n - (n + 1) * width; // nao ta aqui
        int cantoC = -(n + 1) + n * width; // nao ta aqui
        int cantoD = n + n * width; // nao ta aqui

        int desloc = (n + 1) * (width + 1); // nao ta aqui
        im += (n + 1) * (stride + 3); // nao ta aqui
        b += desloc;
        g += desloc;
        r += desloc;

        int subDesloc = 2 * n + 1;

        for (int j = n + 1; j < height - n; j++)
        {
            for (int i = n + 1; i < width - n; i++, im += 3, b++, g++, r++)
            {
                *(im + 0) = (byte)((*(b + cantoD) + *(b + cantoA) - *(b + cantoB) - *(b + cantoC)) / area);
                *(im + 1) = (byte)((*(g + cantoD) + *(g + cantoA) - *(g + cantoB) - *(g + cantoC)) / area);
                *(im + 2) = (byte)((*(r + cantoD) + *(r + cantoA) - *(r + cantoB) - *(r + cantoC)) / area);
            }

            r += subDesloc;
            g += subDesloc;
            b += subDesloc;
            im += 6 * n + 3 + stride - 3 * width; // nao ta aqui
        }
    }

    static private unsafe (long[] r, long[] g, long[] b) integral(Bitmap input)
    {
        var data = input.LockBits(
            new Rectangle(0, 0, input.Width, input.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        long[] g = new long[input.Width * input.Height];
        long[] b = new long[input.Width * input.Height];
        long[] r = new long[input.Width * input.Height];

        fixed (long* rPointer = r, gPointer = g, bPointer = b)
        {
            var img = (byte*)data.Scan0;
            long* rp = rPointer, gp = gPointer, bp = bPointer;

            setInitial(img, rp, gp, bp);
            setLine(img, rp, gp, bp, input.Width);
            setColumn(img, rp, gp, bp, input.Height, input.Width, data.Stride);
            fill(img, rp, gp, bp, input.Height, input.Width, data.Stride);
        }

        input.UnlockBits(data);

        return (r, g, b);
    }

    static unsafe void setInitial(byte* im, long* r, long* g, long* b)
    {
        *b = *(im + 0);
        *g = *(im + 1);
        *r = *(im + 2);
    }

    static unsafe void setLine(byte* im, long* r, long* g, long* b, int width)
    {
        b++;
        g++;
        r++;
        for (int i = 1; i < width; i++, r++, g++, b++, im += 3)
        {
            *b = *(im + 0) + *(b - 1);
            *g = *(im + 1) + *(g - 1);
            *r = *(im + 2) + *(r - 1);
        }
    }

    static unsafe void setColumn(byte* im, long* r, long* g, long* b, int height, int width, int stride)
    {
        b += width;
        g += width;
        r += width;

        for (int i = 1; i < height; i++, r += width, g += width, b += width, im += stride)
        {
            *b = *(im + 0) + *(b - width);
            *g = *(im + 1) + *(g - width);
            *r = *(im + 2) + *(r - width);
        }
    }

    static unsafe void fill(byte* im, long* r, long* g, long* b, int height, int width, int stride)
    {
        int intWidth = width + 1;

        b += intWidth;
        g += intWidth;
        r += intWidth;
        im += stride;

        var endj = r + (height - 1) * (width - 1);
        for (; r < endj; b++, g++, r++, im += 3)
        {
            var endi = r + width - 1;
            while (r < endi)
            {
                *b = *(im + 0) + *(b - 1) + *(b - width) - *(b - intWidth);
                *g = *(im + 1) + *(g - 1) + *(g - width) - *(g - intWidth);
                *r = *(im + 2) + *(r - 1) + *(r - width) - *(r - intWidth);
                b++;
                g++;
                r++;
                im += 3;
            }
            im += stride - 3 * width;
        }
    }
}