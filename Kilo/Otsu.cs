namespace Kilo;

public static class Otsu
{
    /// <summary>
    /// Encontra um ponto ideal de separação entre duas classes via método de Otsu
    /// </summary>
    /// <param name="hist">Histograma</param>
    /// <param name="N">Soma total dos elementos do histograma</param>
    /// <returns>Retorna um Threshold</returns>
    public static int Method(int[] hist, int N)
    {
        var minSigma = float.MaxValue;
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