using System;

namespace Oscar;

class Otsu
{
    public void otsu(int[] hist)
    {
        float minSigma = float.PositiveInfinity;
        float sigma = 0;
        int maxT = 0;
        float desv1;
        float desv2;

        float s0 = 0;
        float s1 = 0;

        float c0 = 0;
        float c1 = 0;

        float m0 = 0;
        float m1 = 0;

        float p0 = 0;
        float p1 = 0;

        float totalpx;

        for (int i = 0; i < hist.Length; i++)
        {
            s1 += i * hist[i];
            c1 += hist[i];
        }

        totalpx = c1;

        for (int i = 0; i < hist.Length; i++)
        {
            var qtd = hist[i];
            var sum = i * hist[i];


            s1 -= sum;
            c1 -= qtd;
            s0 += sum;
            c0 += qtd;

            m0 = s0 / c0;
            m1 = s1 / c1;

            p0 = c0 / totalpx;
            p1 = c1 / totalpx;

            desv1 = this.varstddev(hist, 0, i, m0);
            desv2 = this.varstddev(hist, i, hist.Length, m1);

            sigma = p0 * desv1 + p1 * desv2;
            if (minSigma > sigma)
            {
                minSigma = sigma;
                maxT = i;
            }

        }
    }

    public float varstddev(int[] hist, int start, int end, float med)
    {
        float sum = 0;
        int count = 0;
        for (int i = start; i < end; i++)
        {
            var diff = i - med;
            var std = diff * diff;
            sum += hist[i] * std;
            count += hist[i];
        }
        return sum / count;

    }

}
