using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Kilo;

public static class Kmeans
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

        int sum;

        var menor = int.MaxValue;

        List<int> Varredura = new List<int>();
        var oldClass = class1;

        int n1;
        int n2;

        int count1;
        int count2;

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
    /// Encontra um ponto ideal de separação entre dois centroides via método de Kmeans de 3 dimensões
    /// </summary>
    /// <param name="imgvector">Vetor da imagem em RGB</param>
    /// <returns>Tupla com dois vetores com as coordenadas dos centroides</returns>
    public static (float[], float[]) Kmeans3D(float[] imgvector)
    {
        var rand = new Random();
        // var Kk = new Kmeans();  
        float[] centroide1 = new float[3];
        float[] centroide2 = new float[3];

        int r1;
        int g1;
        int b1;

        int r2;
        int g2;
        int b2;

        float invc1;
        float invc2;

        float oldr1;
        float oldg1;
        float oldb1;

        int count1;
        int count2;

        for (int k = 0; k < 3; k++)
        {
            centroide1[k] = rand.Next(0, 127);
            centroide2[k] = rand.Next(128, 255);
        }

        for (int k = 0; k < 50; k++)
        {
            count1 = count2 = 1;
            r1 = r2 = 0;
            g1 = g2 = 0;
            b1 = b2 = 0;

            Parallel.Invoke(
                () =>
                {
                    float[] centroide1copy = centroide1;
                    float[] centroide2copy = centroide2;

                    float[] imgvectorCopy = imgvector;

                    for (int i = 0; i < imgvectorCopy.Length / 4; i += 3)
                    {
                        int ir = (int)imgvectorCopy[i + 0];
                        int ig = (int)imgvectorCopy[i + 1];
                        int ib = (int)imgvectorCopy[i + 2];

                        var d1 =
                            (centroide1copy[0] - ir) * (centroide1copy[0] - ir) +
                            (centroide1copy[1] - ig) * (centroide1copy[1] - ig) +
                            (centroide1copy[2] - ib) * (centroide1copy[2] - ib);

                        var d2 =
                            (centroide2copy[0] - ir) * (centroide2copy[0] - ir) +
                            (centroide2copy[1] - ig) * (centroide2copy[1] - ig) +
                            (centroide2copy[2] - ib) * (centroide2copy[2] - ib);

                        if (d1 < d2)
                        {
                            Interlocked.Add(ref r1, ir);
                            Interlocked.Add(ref g1, ig);
                            Interlocked.Add(ref b1, ib);
                            Interlocked.Increment(ref count1);
                        }
                        else
                        {
                            Interlocked.Add(ref r2, ir);
                            Interlocked.Add(ref g2, ig);
                            Interlocked.Add(ref b2, ib);
                            Interlocked.Increment(ref count2);
                        }
                    }
                },
                () =>
                {

                    float[] centroide1copy = centroide1;
                    float[] centroide2copy = centroide2;

                    float[] imgvectorCopy2 = imgvector;

                    for (int i = imgvectorCopy2.Length / 4; i < imgvectorCopy2.Length / 2; i += 3)
                    {
                        int ir = (int)imgvectorCopy2[i + 0];
                        int ig = (int)imgvectorCopy2[i + 1];
                        int ib = (int)imgvectorCopy2[i + 2];

                        var d1 =
                            (centroide1copy[0] - ir) * (centroide1copy[0] - ir) +
                            (centroide1copy[1] - ig) * (centroide1copy[1] - ig) +
                            (centroide1copy[2] - ib) * (centroide1copy[2] - ib);

                        var d2 =
                            (centroide2copy[0] - ir) * (centroide2copy[0] - ir) +
                            (centroide2copy[1] - ig) * (centroide2copy[1] - ig) +
                            (centroide2copy[2] - ib) * (centroide2copy[2] - ib);

                        if (d1 < d2)
                        {
                            Interlocked.Add(ref r1, ir);
                            Interlocked.Add(ref g1, ig);
                            Interlocked.Add(ref b1, ib);
                            Interlocked.Increment(ref count1);
                        }
                        else
                        {
                            Interlocked.Add(ref r2, ir);
                            Interlocked.Add(ref g2, ig);
                            Interlocked.Add(ref b2, ib);
                            Interlocked.Increment(ref count2);
                        }
                    }
                },
                () =>
                {
                    float[] centroide1copy = centroide1;
                    float[] centroide2copy = centroide2;

                    float[] imgvectorCopy3 = imgvector;
                    for (int i = imgvectorCopy3.Length / 2; i < imgvectorCopy3.Length * 3 / 4; i += 3)
                    {
                        int ir = (int)imgvectorCopy3[i + 0];
                        int ig = (int)imgvectorCopy3[i + 1];
                        int ib = (int)imgvectorCopy3[i + 2];

                        var d1 =
                            (centroide1copy[0] - ir) * (centroide1copy[0] - ir) +
                            (centroide1copy[1] - ig) * (centroide1copy[1] - ig) +
                            (centroide1copy[2] - ib) * (centroide1copy[2] - ib);

                        var d2 =
                            (centroide2copy[0] - ir) * (centroide2copy[0] - ir) +
                            (centroide2copy[1] - ig) * (centroide2copy[1] - ig) +
                            (centroide2copy[2] - ib) * (centroide2copy[2] - ib);

                        if (d1 < d2)
                        {
                            Interlocked.Add(ref r1, ir);
                            Interlocked.Add(ref g1, ig);
                            Interlocked.Add(ref b1, ib);
                            Interlocked.Increment(ref count1);
                        }
                        else
                        {
                            Interlocked.Add(ref r2, ir);
                            Interlocked.Add(ref g2, ig);
                            Interlocked.Add(ref b2, ib);
                            Interlocked.Increment(ref count2);
                        }
                    }
                },
                () =>
                {
                    float[] centroide1copy = centroide1;
                    float[] centroide2copy = centroide2;

                    float[] imgvectorCopy4 = imgvector;
                    for (int i = imgvectorCopy4.Length * 3 / 4; i < imgvectorCopy4.Length; i += 3)
                    {
                        int ir = (int)imgvectorCopy4[i + 0];
                        int ig = (int)imgvectorCopy4[i + 1];
                        int ib = (int)imgvectorCopy4[i + 2];

                        var d1 =
                            (centroide1copy[0] - ir) * (centroide1copy[0] - ir) +
                            (centroide1copy[1] - ig) * (centroide1copy[1] - ig) +
                            (centroide1copy[2] - ib) * (centroide1copy[2] - ib);

                        var d2 =
                            (centroide2copy[0] - ir) * (centroide2copy[0] - ir) +
                            (centroide2copy[1] - ig) * (centroide2copy[1] - ig) +
                            (centroide2copy[2] - ib) * (centroide2copy[2] - ib);

                        if (d1 < d2)
                        {
                            Interlocked.Add(ref r1, ir);
                            Interlocked.Add(ref g1, ig);
                            Interlocked.Add(ref b1, ib);
                            Interlocked.Increment(ref count1);
                        }
                        else
                        {
                            Interlocked.Add(ref r2, ir);
                            Interlocked.Add(ref g2, ig);
                            Interlocked.Add(ref b2, ib);
                            Interlocked.Increment(ref count2);
                        }
                        // Interlocked.Add(ref r1, tr1); não entendi, elp kk
                    }
                }
            );

            invc1 = 1.0f / count1;
            invc2 = 1.0f / count2;

            oldr1 = centroide1[0];
            oldg1 = centroide1[1];
            oldb1 = centroide1[2];

            centroide1[0] = (byte)(r1 * invc1);
            centroide1[1] = (byte)(g1 * invc1);
            centroide1[2] = (byte)(b1 * invc1);

            centroide2[0] = (byte)(r2 * invc2);
            centroide2[1] = (byte)(g2 * invc2);
            centroide2[2] = (byte)(b2 * invc2);


            if (distance(oldr1, oldg1, oldb1, centroide1[0], centroide1[1], centroide1[2]) < 16)
            {
                break;
            }
        }

        // funcao verifica
        (float[], float[]) tuple = verificada(centroide1, centroide2);
        // (float[], float[]) tuple = (centroide1, centroide2);

        return tuple;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float distance(float mr, float mg, float mb, float oldmr, float oldmg, float oldmb)
       => (mr - oldmr) * (mr - oldmr) + (mg - oldmg) * (mg - oldmg) + (mb - oldmb) * (mb - oldmb);

    private static (float[], float[]) verificada(float[] centroide1, float[] centroide2)
    {
        float diff1;
        float diff2;

        diff1 = distance(centroide1[0], centroide1[1], centroide1[1], 0, 0, 0);
        diff2 = distance(centroide2[0], centroide2[1], centroide2[1], 0, 0, 0);

        if (diff2 < diff1)
            return (centroide2, centroide1);
        
        return (centroide1, centroide2);
    }
}
