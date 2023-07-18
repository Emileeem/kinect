using System.Drawing.Imaging;
using System.Drawing;

namespace Sierra;

static public class Silhueta
{
    public static unsafe (int, int, int, int, int, int) Polos(Bitmap input)
    {
        var data = input.LockBits(
            new Rectangle(0, 0, input.Width, input.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        int x1 = -1, x2 = 0;
        int y1 = -1, y2 = 0;
        int ymin = -1, ymax = 0;

        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                int index = 3 * i + j * data.Stride;

                if (im[index + 0] >= 230 && im[index + 1] >= 230 && im[index + 2] >= 230)
                {
                    if (im[index + data.Stride + 0] < 230)
                    {
                        x2 = i;
                        if (x1 == -1)
                            x1 = i;
                    }
                }
            }
        }

        for (int j = 1; j < input.Height - 1; j++)
        {
            for (int i = 1; i < input.Width - 1; i++)
            {
                int index = 3 * i + j * data.Stride;

                if (im[index + 0] >= 230 && im[index + 1] >= 230 && im[index + 2] >= 230)
                {
                    if (im[index + 3 + 0] < 230)
                    {
                        ymax = j;
                        if (ymin == -1)
                            ymin = j;
                    }
                }
            }
        }

        int mx = (x1 + x2) / 2;

        for (int j = 1; j < input.Height - 1; j++)
        {
            int index = 3 * mx + j * data.Stride;
            if (im[index + 0] < 230)
            {
                y2 = j;
                if (y1 == -1)
                    y1 = j;
            }
        }

        input.UnlockBits(data);

        return (x1, x2, y1, y2, ymin, ymax);
    }

    public static unsafe Bitmap CropImg((int, int, int, int, int, int) xymax, (long[,], long[,]) contorno, Bitmap input)
    {
        int Width = (xymax.Item2 - xymax.Item1) + 1;
        int Height = (xymax.Item6 - xymax.Item5) + 1;

        Bitmap img = new Bitmap(Width, Height);

        for (int j = 0; j < Height; j++)
        {
            for (int i = 0; i < Width; i++)
            {
                img.SetPixel(i, j, Color.White);
            }
        }

        for (int j = 0; j < input.Height; j++)
        {
            for (int i = 0; i < input.Width; i++)
            {
                if (contorno.Item1[j, i] != 0)
                {
                    var iv2 = i - xymax.Item1;
                    var jv2 = j - xymax.Item5;
                    img.SetPixel(iv2, jv2, Color.Black);
                }
            }
        }

        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                if (contorno.Item2[j, i] != 0)
                {
                    var iv2 = i - xymax.Item1;
                    var jv2 = j - xymax.Item5;
                    img.SetPixel(iv2, jv2, Color.Black);
                }
            }
        }

        return img;
    }

    public static unsafe (long[,], long[,]) Contorno(Bitmap input)
    {
        long[,] LeftToRight = new long[input.Height, input.Width];
        long[,] TopToBottom = new long[input.Height, input.Width];

        var data = input.LockBits(
            new Rectangle(0, 0, input.Width, input.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        // Top to Bottom
        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                int index = 3 * i + j * data.Stride;

                if (im[index + 0] >= 230 && im[index + 1] >= 230 && im[index + 2] >= 230)
                {
                    if (im[index - data.Stride + 0] < 230)
                    {
                        TopToBottom[j - 1, i] = i;
                    }

                    if (im[index + data.Stride + 0] < 230)
                    {
                        TopToBottom[j + 1, i] = i;
                    }
                }
            }
        }

        // Left to Right
        for (int j = 1; j < input.Height - 1; j++)
        {
            for (int i = 1; i < input.Width - 1; i++)
            {
                int index = 3 * i + j * data.Stride;

                if (im[index + 0] >= 230 && im[index + 1] >= 230 && im[index + 2] >= 230)
                {
                    if (im[index - 3 + 0] < 230)
                    {
                        LeftToRight[j, i - 1] = j;
                    }

                    if (im[index + 3 + 0] < 230)
                    {
                        LeftToRight[j, i + 1] = j;
                    }
                }
            }
        }

        input.UnlockBits(data);

        return (LeftToRight, TopToBottom);
    }

    public static unsafe Bitmap JuntarContornos((long[,], long[,]) vetor, Bitmap input)
    {
        for (int j = 1; j < input.Height - 1; j++)
        {
            for (int i = 1; i < input.Width - 1; i++)
            {
                if (vetor.Item1[j, i] != 0)
                {
                    input.SetPixel(i, j, Color.Black);
                }
            }
        }

        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                if (vetor.Item2[j, i] != 0)
                {
                    input.SetPixel(i, j, Color.Black);
                }
            }
        }

        return input;
    }

    public static unsafe (long[,], long[,]) ContornoForSecondImg(Bitmap input)
    {
        long[,] LeftToRight = new long[input.Height, input.Width];
        long[,] TopToBottom = new long[input.Height, input.Width];

        var data = input.LockBits(
            new Rectangle(0, 0, input.Width, input.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        byte* im = (byte*)data.Scan0.ToPointer();

        // Top to Bottom
        for (int i = 1; i < input.Width - 1; i++)
        {
            for (int j = 1; j < input.Height - 1; j++)
            {
                int index = 3 * i + j * data.Stride;

                if (im[index - data.Stride + 0] != 255)
                {
                    TopToBottom[j, i] = i;
                }
            }
        }

        // Left to Right
        for (int j = 1; j < input.Height - 1; j++)
        {
            for (int i = 1; i < input.Width - 1; i++)
            {
                int index = 3 * i + j * data.Stride;

                if (im[index - 3 + 0] != 255)
                {
                    LeftToRight[j, i - 1] = j;
                }
            }
        }

        input.UnlockBits(data);

        return (LeftToRight, TopToBottom);
    }

}