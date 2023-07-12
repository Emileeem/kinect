using System;
using System.Collections.Generic;

namespace Kilo;

public static class Threshold
{
    /// <summary>
    /// Encontra um ponto ideal de separação entre duas classes via método de Kmeans de 2 dimensões
    /// </summary>
    /// <param name="hist">Histograma</param>
    /// <param name="tolerance">Tolerância de erro desejada, para calibragem</param>
    /// <returns>Retorna um Threshold</returns>
    public static int Kmeans2D(int[] hist, float tolerance = 0.20f)
    {
        var rand = new Random();

        var class1 = rand.Next(0, hist.Length / 2);
        var class2 = rand.Next(hist.Length / 2 + 1, hist.Length);

        var sum = 0;

        var menor = int.MaxValue;

        List<int> Varredura = new List<int>();
        var oldClass = class1;

        var n1 = 0;
        var n2 = 0;

        var count1 = 0;
        var count2 = 0;

        for (int k = 0; k < 10; k++)
        {
            n1 = n2 = 0;
            count1 = count2 = 0;

            int distDiv = class2 - class1;
            int mid = class1 + distDiv / 2;

            for (int i = 0; i < mid; i++)
            {
                n1 += i * hist[i];
                count1 += hist[i];
            }

            for (int i = mid; i < hist.Length; i++)
            {
                n2 += i * hist[i];
                count2 += hist[i];
            }

            if (count1 == 0)
                class1 = 0;
            else
                class1 = (int)(n1 / count1);

            if (count2 == 0)
                class2 = 0;
            else
                class2 = (int)(n2 / count2);

            var diff = class1 - oldClass;
            if (diff > -5 && diff < 5)
                break;

            oldClass = class1;
        }

        for (int k = class1; k < class2 - 10; k++)
        {
            sum = 0;
            for (int c = 0; c < 10; c++)
            {
                var aux = hist[k + c];
                sum += aux;
            }

            var m = sum / 10;
            Varredura.Add(m);

            if (menor > m)
                menor = m;
        }

        int max = (int)((tolerance + 1) * menor);
        int? first = null;
        int? last = null;

        for (int k = 0; k < Varredura.Count; k++)
        {
            bool isBiggerThanMax = Varredura[k] > max;

            if (isBiggerThanMax && last is not null && first is not null)
                break;

            if (isBiggerThanMax)
                continue;

            last = class1 + k;
            if (first is null)
                first = class1 + k;
        }

        return (first.Value + last.Value) / 2 + 5;
    }
    /// <summary>
    /// Encontra um ponto ideal de separação entre duas classes via método de Otsu
    /// </summary>
    /// <param name="hist">Histograma</param>
    /// <param name="N">Soma total dos elementos do histograma</param>
    /// <returns>Retorna um Threshold</returns>
    public static byte[] Kmeans3D(float[] imgvector)
    {
        var rand = new Random();

        byte[] centroides = new byte [6];

        int r1;
        int g1;
        int b1;

        int r2;
        int g2;
        int b2;

        var count1 = 1;
        var count2 = 1;

        int ir;
        int ig;
        int ib;

        rand.NextBytes (centroides);

        for (int k = 0; k < 1000; k++)
        {
            count1 = count2 = 1;
            r1 = r2 = 0;
            g1 = g2 = 0;
            b1 = b2 = 0;


            for (int i = 0; i < imgvector.Length; i += 3)
            {
                ir = (int)imgvector[i + 0];
                ig = (int)imgvector[i + 1];
                ib = (int)imgvector[i + 2];

                var d1 = (centroides[0] - ir) * (centroides[0] - ir)
                    + (centroides[1] - ig) * (centroides[1] - ig)
                    + (centroides[2] - ib) * (centroides[2] - ib);

                var d2 = (centroides[3] - ir) * (centroides[3] - ir)
                    + (centroides[4] - ig) * (centroides[4] - ig)
                    + (centroides[5] - ib) * (centroides[5] - ib);

                if (d1 < d2)
                {
                    r1 += ir;
                    g1 += ig;
                    b1 += ib;
                    count1++;
                }
                else
                {
                    r2 += ir;
                    g2 += ig;
                    b2 += ib;
                    count2++;
                }
            }

            centroides[0] = (byte)(r1 / count1);
            centroides[1] = (byte)(g1 / count1);
            centroides[2] = (byte)(b1 / count1);

            centroides[3] = (byte)(r2 / count2);
            centroides[4] = (byte)(g2 / count2);
            centroides[5] = (byte)(b2 / count2);

        }

        return centroides;
    }
    /// <summary>
    /// Encontra um ponto ideal de separação entre duas classes via método de Otsu
    /// </summary>
    /// <param name="hist">Histograma</param>
    /// <param name="N">Soma total dos elementos do histograma</param>
    /// <returns>Retorna um Threshold</returns>
    public static int Otsu(int[] hist, int N)
    {
        float minSigma = float.MaxValue;
        float sW = 0;
        int best = 0;

        float wk = 0;

        float uk = 0;

        float sk = 0;

        float uT = 0;
        float sT = 0;

        for (int i = 0; i < hist.Length; i++)
        {
            var aux = i * hist[i] / (float)N;
            uT += aux;
            sT += i * aux;
        }

        for (int i = 0; i < hist.Length; i++)
        {
            var ni = (float)hist[i];
            var pi = ni / N;

            wk += pi;
            uk += i * pi;
            sk += i * i * pi;

            var u0 = uk / wk;
            var u1 = (uT - uk) / (1 - wk);

            var v0 = sk / wk - u0 * u0;
            var v1 = (sT - sk) / (1 - wk) - u1 * u1;

            sW = wk * v0 + (1 - wk) * v1;

            if (sW < minSigma)
            {
                minSigma = sW;
                best = i;
            }
        }
        return best;
    }
}
