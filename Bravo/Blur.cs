using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Bravo;

class Blur
{
    public unsafe Bitmap UseBlur(Bitmap input, int n)
    {
        long[] r, g, b;
        (r, g, b) = integral(input);

        // LockBits no clone do input (que vai ser o resultado)
        var result = input.Clone() as Bitmap;
        var data = result.LockBits(
            new Rectangle(0, 0, result.Width, result.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        // Cria um ponteiro para o data (clone do input lockado)
        byte* p = (byte*)data.Scan0.ToPointer();

        // Ponteiros pras cores
        fixed (long* rPointer = r, gPointer = g, bPointer = b)
        {
            var img = (byte*)data.Scan0;
            long* rp = rPointer, gp = gPointer, bp = bPointer;
            SetImage(p, rp, gp, bp, input.Width, input.Height, data.Stride, n);
        }

        void SetImage(byte* im, long* r, long* g, long* b, int width, int height, int stride, int n) {
            int side = 2 * n + 1;
            int area = side * side;

            // Códigos que vão ser inutilizados
            // long cantoD = index + n + n * width;
            // long cantoB = index + n - (n + 1) * width;
            // long cantoC = index - (n + 1) + n * width;
            // long cantoA = index - (n + 1) - (n + 1) * width;

            // int bd = (int)((b[lfcf] - b[li1cf] - b[lfci1] + b[li1ci1]) / area);
            // int gd = (int)((g[lfcf] - g[li1cf] - g[lfci1] + g[li1ci1]) / area);
            // int rd = (int)((r[lfcf] - r[li1cf] - r[lfci1] + r[li1ci1]) / area);

            // long cantoD = n + n * width;
            // long cantoB = n - (n + 1) * width;
            // long cantoC = (n + 1) + n * width;
            // long cantoA = (n + 1) - (n + 1) * width;

            for (int j = n + 1; j < height - n - 1; j++)
            {
                for (int i = n + 1; i < width - n - 1; i++, b++, g++, r++, im += 3)
                {
                    int index = j * width + i;
                    int imgIndex = j * stride + 3 * i;



                    long cantoA = ((n + 1) + ((n + 1) * width)) * -1;
                    long cantoB = ((n + 1) * width) * -1 + n;
                    long cantoC = n * width + n;
                    long cantoD = (n + 1) * -1 + (n * width);

                    *(im + 0) = (byte)((*(b + cantoA) + *(b + cantoC) - (*(b + cantoB) + *(b + cantoD))) / area);
                    *(im + 1) = (byte)((*(g + cantoA) + *(g + cantoC) - (*(g + cantoB) + *(g + cantoD))) / area);
                }
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