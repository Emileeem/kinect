using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Bravo;

// Blur Resumo:
// É a classe que guarda todas os metódos para construir a imagem borrada.

class Blur
{
    // UseBlur Resumo:
    // Aplica blur em uma imagem.
    //
    // Parâmetros:
    //     (Bitmap) input    imagem na qual o efeito de blur será aplicado
    //     (int) n           intensidade do blur, se o valor sobrepor o tamanho da imagem nada irá acontecer
    //
    // Retorna:
    //     Imagem Bitmap com o efeito de blur aplicado.

    public unsafe Bitmap UseBlur(Bitmap input, int n)
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

    // SetImage Resumo:
    // Calcula a média dos pixeis e aplica o blur na imagem resultante.
    //
    // Parâmetros:
    //     (byte*) im     imagem na qual o efeito de blur será aplicado
    //     (long*) r      bytes de cor vermelha dos pixeis da imagem
    //     (long*) g      bytes de cor verde dos pixeis da imagem
    //     (long*) b      bytes de cor azul dos pixeis da imagem
    //     (int) width    largura da imagem
    //     (int) height   altura da imagem
    //     (int) stride   largura da imagem mais os possíveis pixeis que podem sobrepoer essa largura
    //     (int) n        intensidade do blur, se o valor sobrepor o tamanho da imagem nada irá acontecer

    private unsafe void SetImage(byte* im, long* r, long* g, long* b, int width, int height, int stride, int n)
    {
        int side = 2 * n + 1;
        float area = side * side;

        int desloc = n + 1;
        im += (n + 1) * 3;
        b += desloc;
        g += desloc;
        r += desloc;

        Parallel.For(n + 1, height - n, j =>
        {
            int cantoA = -(n + 1) - (n + 1) * width;
            int cantoB = n - (n + 1) * width;
            int cantoC = -(n + 1) + n * width;
            int cantoD = n + n * width;

            var larea = area;
            var lwidth = width;
            var lstride = stride;
            int subDesloc = 2 * n + 1;

            var lb = b;
            var lg = g;
            var lr = r;
            var lim = im;

            lb += j * lwidth;
            lg += j * lwidth;
            lr += j * lwidth;
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

    // Integral Resumo:
    // Cria uma imagem integral a partir da imagem original.
    //
    // Parâmetros:
    //     (Bitmap) input  imagem que originará a imagem integral
    //
    // Retorna:
    //     Retorna separadamente a integral dos bytes R G B de cada pixel da imagem referenciada.

    private unsafe (long[] r, long[] g, long[] b) integral(Bitmap input)
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

            Parallel.Invoke(
                () =>
                {
                    setInitial(img, bp, 0);
                    setLine(img, bp, 0, input.Width);
                    setColumn(img, bp, 0, input.Height, input.Width, data.Stride);
                    fill(img, bp, 0, input.Height, input.Width, data.Stride);
                },

                () =>
                {
                    setInitial(img, gp, 1);
                    setLine(img, gp, 1, input.Width);
                    setColumn(img, gp, 1, input.Height, input.Width, data.Stride);
                    fill(img, gp, 1, input.Height, input.Width, data.Stride);
                },

                () =>
                {
                    setInitial(img, rp, 2);
                    setLine(img, rp, 2, input.Width);
                    setColumn(img, rp, 2, input.Height, input.Width, data.Stride);
                    fill(img, rp, 2, input.Height, input.Width, data.Stride);
                }
            );
        }

        input.UnlockBits(data);

        return (r, g, b);
    }

    // setInitial Resumo:
    // Cálculo do canal R G B que incrementa na imagem
    // 
    // Parâmetros:
    //     (*byte) im     imagem que será usada de base para fazer os cáculos
    //     (*long) c      define em qual canal de cor que será aplicado o metódo 
    //     (int) channel  define o canal de cor na imagem (0, 1 e 2)       

    unsafe void setInitial(byte* im, long* c, int channel)
        => *c = *(im + channel);

    // setLine Resumo:
    // Incrementação do efeito de blur por canais separados nos pixeis da primeira linha da imagem.
    //
    // Parâmetros:
    //     (*byte) im     imagem que será usada de base para fazer os cáculos
    //     (*long) c      define em qual canal de cor que será aplicado o metódo 
    //     (int) channel  define o canal de cor na imagem (0, 1 e 2)  
    //     (int) width    largura da imagem   

    unsafe void setLine(byte* im, long* c, int channel, int width)
    {
        c++;
        for (int i = 1; i < width; i++, c++, im += 3)
        {
            *c = *(im + channel) + *(c - 1);
        }
    }

    // setColumn Resumo:
    // Incrementação do efeito de blur por canais separados nos pixeis da primeira coluna da imagem.
    //
    // Parâmetros:
    //     (*byte) im     imagem que será usada de base para fazer os cáculos
    //     (*long) c      define em qual canal de cor que será aplicado o metódo 
    //     (int) channel  define o canal de cor na imagem (0, 1 e 2)
    //     (int) width    largura da imagem
    //     (int) height   altura da imagem
    //     (int) stride   largura da imagem mais os possíveis pixeis que podem sobrepoer essa largura

    unsafe void setColumn(byte* im, long* c, int channel, int height, int width, int stride)
    {
        c += width;

        for (int i = 1; i < height; i++, c += width, im += stride)
        {
            *c = *(im + channel) + *(c - width);
        }
    }

    // Fill Resumo:
    // Incrementação do efeito de blur por canais separados nos pixeis de todo o resto da imagem.
    //
    // Parâmetros:
    //     (*byte) im     imagem que será usada de base para fazer os cáculos
    //     (*long) c      define em qual canal de cor que será aplicado o metódo 
    //     (int) channel  define o canal de cor na imagem (0, 1 e 2)
    //     (int) width    largura da imagem
    //     (int) height   altura da imagem
    //     (int) stride   largura da imagem mais os possíveis pixeis que podem sobrepoer essa largura

    unsafe void fill(byte* im, long* c, int channel, int height, int width, int stride)
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
}