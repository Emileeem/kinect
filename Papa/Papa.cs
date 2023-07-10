using System;

namespace Papa
{
    class Vetor
    {
        public float[] CalcularVetor((float x, float y, float z) p, (float x, float y, float z) q)
        {
            // Calcula as diferenças nas coordenadas entre p e q para obter as componentes do vetor
            float vetorX = q.x - p.x;
            float vetorY = q.y - p.y;
            float vetorZ = q.z - p.z;

            // Retorna um array contendo as componentes do vetor
            return new float[] { vetorX, vetorY, vetorZ };
        }
    }
}



//  (float a, float b, float c, float d) findPlane(
//         (float x1, float y1, float z1) p, // Definir as coordenadas dos pontos p e q
//         (float x2, float y2, float z2) q
//     )
//     {
//         float a = 0;
//         float b = 0;
//         float c = 0;
//         float d = 0;
//     }
//   return (a, b, c, d);