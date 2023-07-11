using System;

namespace Papa
{
    public static class Plano
    {
        /// <summary>
        /// Função para calcular o vetor entre dois pontos p e q
        /// </summary>
        /// <param name="p">Ponto P</param>
        /// <param name="q">Ponto Q</param>
        /// <returns>Retorna um array contendo os componentes do vetor</returns>
        public static (float a, float b, float c) CalcularVetor((float x, float y, float z) p, (float x, float y, float z) q)
        {
            // Calcula as diferenças nas coordenadas entre p e q para obter as componentes do vetor
            float vetorX = q.x - p.x;
            float vetorY = q.y - p.y;
            float vetorZ = q.z - p.z;

            return (a: vetorX, b: vetorY, c: vetorZ);
        }

        /// <summary>
        /// Função para calcular o Ponto médio entre dois pontos, ou seja, a metade do vetor.
        /// </summary>
        /// <param name="p">Ponto P</param>
        /// <param name="q">Ponto Q</param>
        /// <returns>Retorna ponto médio em abc</returns>
        public static (float a, float b, float c) CalcularPontoMedio((float x, float y, float z) p, (float x, float y, float z) q)
        {
            var vetor = CalcularVetor(p, q); //usa a função CalcularVetor para obter o vetor entre dois pontos (p,q), que retorna as direções a, b e c do vetor
            float pontoMedioA = p.x + vetor.a / 2; //divide esse vetor por 2 para encontrar o ponto médio entre os dois pontos.
            float pontoMedioB = p.y + vetor.b / 2;
            float pontoMedioC = p.z + vetor.c / 2;

            return (a: pontoMedioA, b: pontoMedioB, c: pontoMedioC);
        }

        /// <summary>
        /// Essa função calcula os coeficientes, valor que deve ser multiplicado por outro, de um plano com base em dois pontos e retorna esses coeficientes como valores flutuantes
        /// </summary>
        /// <param name="p">Ponto P</param>
        /// <param name="q">Ponto Q</param>
        /// <returns>Retorna as coordenadas do plano, sendo abcd</returns>
        public static (float a, float b, float c, float d) CalcularPlano((float x, float y, float z) p, (float x, float y, float z) q)
        {
            var vetor = CalcularVetor(p, q);
            var PontoMedio = CalcularPontoMedio(p, q);  // ponto C (x, y, z) que é a media entre A e B
            float a = vetor.a; //puxa do CalcularVetor
            float b = vetor.b; //puxa do CalcularVetor
            float c = vetor.c; //puxa do CalcularVetor
            float d = -(a * PontoMedio.a + b * PontoMedio.b + c * PontoMedio.c); // d = -(a * x + b * y + c * z) 

            return (a, b, c, d);
        }
    }
}