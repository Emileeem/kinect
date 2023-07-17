using System.Drawing;
using System.Drawing.Imaging;

namespace Bravo;

/// <summary>
/// É a classe que guarda todas os metódos para construir a imagem borrada.
/// </summary>

public static class Blur
{
    /// <summary>
    /// Aplica blur em uma imagem.
    /// </summary>
    /// <param name="input">Imagem na qual o efeito de blur será aplicado.</param>
    /// <param name="n">Intensidade do blur, se o valor sobrepor o tamanho da imagem nada irá acontecer.</param>
    /// <returns>Imagem Bitmap com o efeito de blur aplicado.</returns>
    public static unsafe Bitmap Apply(Bitmap input, int n)
    {
        long[] r, g, b;
        (r, g, b) = integral(input);

        var result = input.Clone() as Bitmap;
        var data = result.LockBits(
            new Rectangle(0, 0, result.Width, result.Height),
            ImageLockMode.ReadWrite,
            PixelFormat.Format24bppRgb
        );

        applyBlurInBitmapData(data, n, r, g, b);

        result.UnlockBits(data);

        return result;
    }

    /// <summary>
    /// Cria uma imagem integral a partir da imagem original.
    /// </summary>
    /// <param name="input">Imagem que originará a imagem integral</param>
    /// <returns>Retorna separadamente a integral dos bytes R G B de cada pixel da imagem referenciada.</returns>
    private static unsafe (long[] r, long[] g, long[] b) integral(Bitmap input)
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

            int widthint = input.Width;
            int heightint = input.Height;
            int strideint = data.Stride;

            Parallel.Invoke(
                () =>
                {
                    var b = bp;
                    var im = img;
                    setInitial(im, b, 0);
                    setLine(im, b, 0, widthint);
                    setColumn(im, b, 0, heightint, widthint, strideint);
                    fill(im, b, 0, heightint, widthint, strideint);
                },

                () =>
                {
                    var g = gp;
                    var im = img;
                    setInitial(im, g, 1);
                    setLine(im, g, 1, widthint);
                    setColumn(im, g, 1, heightint, widthint, strideint);
                    fill(im, g, 1, heightint, widthint, strideint);
                },

                () =>
                {
                    var r = rp;
                    var im = img;
                    setInitial(im, r, 2);
                    setLine(im, r, 2, widthint);
                    setColumn(im, r, 2, heightint, widthint, strideint);
                    fill(im, r, 2, heightint, widthint, strideint);
                }
            );
        }

        input.UnlockBits(data);

        return (r, g, b);
    }

    /// <summary>
    /// Cálculo do canal R G B que incrementa na imagem
    /// </summary>
    /// <param name="im">Imagem que será usada de base para fazer os cáculos</param>
    /// <param name="c">Define em qual canal de cor que será aplicado o metódo</param> 
    /// <param name="channel">Channel  define o canal de cor na imagem (0, 1 e 2)</param>   
    private unsafe static void setInitial(byte* im, long* c, int channel)
        => *c = *(im + channel);

    /// <summary>
    /// Incrementação do efeito de blur por canais separados nos pixeis da primeira linha da imagem.
    /// </summary>
    /// <param name="im">Imagem que será usada como base para fazer os cáculos</param>
    /// <param name="c">Define em qual canal de cor que será aplicado o metódo</param>
    /// <param name="channel">Define o canal de cor na imagem (0, 1 e 2)</param>
    /// <param name="width">Largura da imagem</param>
    private unsafe static void setLine(byte* im, long* c, int channel, int width)
    {
        c++;
        for (int i = 1; i < width; i++, c++, im += 3)
        {
            *c = *(im + channel) + *(c - 1);
        }
    }

    /// <summary>
    /// Incrementação do efeito de blur por canais separados nos pixeis da primeira coluna da imagem.
    /// </summary>
    /// <param name="im">Imagem que será usada de base para fazer os cáculos</param>
    /// <param name="c">Define em qual canal de cor que será aplicado o metódo</param>
    /// <param name="channel">Define o canal de cor na imagem (0, 1 e 2)</param>
    /// <param name="width">Largura da imagem</param>
    /// <param name="height">Altura da imagem</param>
    /// <param name="stride">Largura da imagem mais os possíveis pixeis que podem sobrepoer essa largura</param>   
    private unsafe static void setColumn(byte* im, long* c, int channel, int height, int width, int stride)
    {
        c += width;

        for (int i = 1; i < height; i++, c += width, im += stride)
        {
            *c = *(im + channel) + *(c - width);
        }
    }

    /// <summary>
    /// Incrementação do efeito de blur por canais separados nos pixeis de todo o resto da imagem.
    /// </summary>
    /// <param name="im">Imagem que será usada de base para fazer os cáculos</param>
    /// <param name="c">Define em qual canal de cor que será aplicado o metódo</param>
    /// <param name="channel">Define o canal de cor na imagem (0, 1 e 2)</param>
    /// <param name="width">Largura da imagem</param>
    /// <param name="height">Altura da imagem</param>
    /// <param name="stride">Largura da imagem mais os possíveis pixeis que podem sobrepoer essa largura</param>
    private unsafe static void fill(byte* im, long* c, int channel, int height, int width, int stride)
    {
        int intWidth = width + 1;

        c += intWidth;
        im += stride;

        var endj = c + (height - 1) * (width - 1);
        for (; c < endj; c++, im += 3)
        {
            var endi = c + width - 1;
            while (c < endi)
            {
                *c = *(im + channel) + *(c - 1) + *(c - width) - *(c - intWidth);
                c++;
                im += 3;
            }
            im += stride - 3 * width;
        }
    }

    /// <summary>
    /// Aplica o Blur por toda a imagem de acordo com a intensidade instanciada. 
    /// </summary>
    /// <param name="input">Imagem que originará a imagem integral</param>
    private static unsafe void applyBlurInBitmapData(BitmapData data, int n, long[] r, long[] g, long[] b)
    {
        fixed (long* rPointer = r, gPointer = g, bPointer = b)
        {
            byte* p = (byte*)data.Scan0.ToPointer();
            long* rp = rPointer, gp = gPointer, bp = bPointer;
            setImage(p, rp, gp, bp, data.Width, data.Height, data.Stride, n);
        }
    }

    /// <summary>
    /// Calcula a média dos pixeis e aplica o blur na imagem resultante.
    /// </summary>
    /// <param name="im">Imagem na qual o efeito de blur será aplicado</param>
    /// <param name="r">Bytes de cor vermelha dos pixeis da imagem</param>
    /// <param name="g">Bytes de cor verde dos pixeis da imagem</param>
    /// <param name="b">Bytes de cor azul dos pixeis da imagem</param>
    /// <param name="width">Largura da imagem</param>
    /// <param name="height">Altura da imagem</param>
    /// <param name="stride">Largura da imagem mais os possíveis pixeis que podem sobrepoer essa largura</param>
    /// <param name="n">Intensidade do blur, se o valor sobrepor o tamanho da imagem nada irá acontecer</param>
    private static unsafe void setImage(byte* im, long* r, long* g, long* b, int width, int height, int stride, int n)
    {
        int side = 2 * n + 1;
        float area = side * side;
        int nplus = n + 1;

        int desloc = n + 1;
        im += (n + 1) * 3;
        b += desloc;
        g += desloc;
        r += desloc;

        Parallel.For(n + 1, height - n, j =>
        {
            int cantoA = -(nplus) - (nplus) * width;
            int cantoB = n - (nplus) * width;
            int cantoC = -(nplus) + n * width;
            int cantoD = n + n * width;

            var larea = area;
            var lwidth = width;
            var lstride = stride;
            int subDesloc = 2 * n + 1;

            var lb = b;
            var lg = g;
            var lr = r;
            var lim = im;

            var jlwidth = j * lwidth;

            lb += jlwidth;
            lg += jlwidth;
            lr += jlwidth;
            lim += j * lstride; 

            for (int i = n + 1; i < lwidth - n; i++, lim += 3, lb++, lg++, lr++)
            {
                *(lim + 0) = (byte)((*(lb + cantoD) + *(lb + cantoA) - *(lb + cantoB) - *(lb + cantoC)) / larea);
                *(lim + 1) = (byte)((*(lg + cantoD) + *(lg + cantoA) - *(lg + cantoB) - *(lg + cantoC)) / larea);
                *(lim + 2) = (byte)((*(lr + cantoD) + *(lr + cantoA) - *(lr + cantoB) - *(lr + cantoC)) / larea);
            }

            lr += subDesloc;
            lg += subDesloc;
            lb += subDesloc;
            lim += 6 * n + 3 + lstride - 3 * lwidth;
        }); 
    }
}