using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Kilo;
public static class Threshold
{   
    /// <summary>
    /// Calcula o Threshold para melhor ponto de separação para uma imagem
    /// </summary>
    /// <param name="imgvector">Vetor da imagem</param>
    /// <returns>Tupla com dois centróides 3D</returns>
    public static (float[], float[]) Calculate(float[] imgvector)
    {
        return Kmeans.Kmeans3D(imgvector);
    }
}
