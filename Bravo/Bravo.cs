using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Bravo;

class Blur
{
    public unsafe Bitmap UseBlur(Bitmap input, int n)
    {
        long[] r, g, b;
        (r, g, b) = integral(input);

        int side = 2 * n + 1;
        int area = side * side;

        var result = input.Clone() as Bitmap;
        var data = result.LockBits(
            new Rectangle(0, 0, result.Width, result.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );
        byte* p = (byte*)data.Scan0.ToPointer();

        for (int j = n + 1; j < input.Height - n - 1; j++)
        {
            for (int i = n + 1; i < input.Width - n - 1; i++)
            {
                int index = j * input.Width + i;
                int imgIndex = j * data.Stride + 3 * i;

                long lfcf = index + n + n * input.Width;
                long li1cf = index + n - (n + 1) * input.Width;
                long lfci1 = index - (n + 1) + n * input.Width;
                long li1ci1 = index - (n + 1) - (n + 1) * input.Width;

                int bd = (int)((b[lfcf] - b[li1cf] - b[lfci1] + b[li1ci1]) / area);
                int gd = (int)((g[lfcf] - g[li1cf] - g[lfci1] + g[li1ci1]) / area);
                int rd = (int)((r[lfcf] - r[li1cf] - r[lfci1] + r[li1ci1]) / area);

                p[imgIndex] = (byte)bd;
                p[imgIndex + 1] = (byte)gd;
                p[imgIndex + 2] = (byte)rd;
            }
        }

        result.UnlockBits(data);

        return result;
    }

    private unsafe (long[] r, long[] g, long[] b) integral(Bitmap input)
    {
        var data = input.LockBits(
            new Rectangle(0, 0, input.Width, input.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        long[] r = new long[input.Width * input.Height];
        long[] g = new long[input.Width * input.Height];
        long[] b = new long[input.Width * input.Height];

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

    unsafe void setInitial(byte* im, long* r, long* g, long* b)
    {
        *b = *(im + 0);
        *g = *(im + 1);
        *r = *(im + 2);
    }

    unsafe void setLine(byte* im, long* r, long* g, long* b, int width)
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

    unsafe void setColumn(byte* im, long* r, long* g, long* b, int height, int width, int stride)
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

    unsafe void fill(byte* im, long* r, long* g, long* b, int height, int width, int stride)
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
        }
    }
}