namespace Eco;
using System;
using System.Drawing;
using System.Drawing.Imaging;

public static class Esqueleto
{
    public static unsafe Point genEsqueleto(Bitmap imagem)
    {
        try
        {
            //Coordenadas extremidades
            int xEsquerda = imagem.Width;
            int yEsquerda = 0;

            int xDireita = 0;
            int yDireita = 0;

            int xAcima = 0;
            int yAcima = imagem.Height;

            int xAbaixo = 0;
            int yAbaixo = 0;

            int yAcimaMaoEsquerda = imagem.Height;
            int yAbaixoMaoEsquerda = 0;

            int yAcimaMaoDireita = imagem.Height;
            int yAbaixoMaoDireita = 0;

            int count = 0;

            var data = imagem.LockBits(
                new Rectangle(0, 0, imagem.Width, imagem.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb
            );

            byte* im = (byte*)data.Scan0.ToPointer();
            var stride = data.Stride;

            for (int j = 0; j < imagem.Height; j++)
            {
                int inicio = -1;

                for (int i = 0; i < imagem.Width; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        // imagem.SetPixel(i, j, Color.White);

                        //Primeiro ponto preto encontrado
                        if (inicio == -1)
                            inicio = i;

                        //Ponto mais a esquerda
                        if (i < xEsquerda)
                        {
                            xEsquerda = i;
                            yEsquerda = j;
                        }

                        //Ponto mais a direita
                        if (i > xDireita)
                        {
                            xDireita = i;
                            yDireita = j;
                        }

                        //Ponto mais abaixo
                        if (j > yAbaixo)
                        {
                            xAbaixo = i;
                            yAbaixo = j;
                        }
                    }
                }
            }

            bool black;

            for (int j = 0; j < imagem.Height; j++)
            {
                for (int i = imagem.Width / 2 - 200; i < imagem.Width / 2 + 200; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        black = true;
                        if (black && yAcima == imagem.Height)
                            //Ponto mais acima
                            if (j < yAcima)
                            {
                                xAcima = i;
                                yAcima = j;
                            }
                    }
                    else
                        black = false;
                    if (black && yAcima != imagem.Height)
                    {
                        
                    }
                }
            }

            for (int j = yAcima; j < yAbaixo; j++)
            {
                count++;
            }

            int yTorso = yAcima + count / 3;

            int CorpoInteiro = 3 * (yAcima + count) / 5;
            int CorpoCortado = 4 * (yAcima + count) / 5;

            int yPerna = yAbaixo <= imagem.Height && yAbaixo >= imagem.Height - 5 ? CorpoCortado : CorpoInteiro;

            for (int j = 0; j < imagem.Height; j++)
            {
                for (int i = xEsquerda; i < xEsquerda + 70; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        if (j < yAcimaMaoEsquerda)
                            yAcimaMaoEsquerda = j;

                        if (j > yAbaixoMaoEsquerda)
                            yAbaixoMaoEsquerda = j;
                    }
                }
            }

            for (int j = 0; j < imagem.Height; j++)
            {
                for (int i = xDireita - 40; i < xDireita; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        if (j < yAcimaMaoDireita)
                            yAcimaMaoDireita = j;

                        if (j > yAbaixoMaoDireita)
                            yAbaixoMaoDireita = j;
                    }
                }
            }

            int xEsquerdaCabeca = imagem.Width;
            int xDireitaCabeca = 0;

            for (int j = yAcima; j < yAcima + 120; j++)
            {
                for (int i = xAcima - 150; i < xAcima + 150; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        //Ponto mais a esquerda
                        if (i < xEsquerdaCabeca)
                            xEsquerdaCabeca = i;

                        //Ponto mais a direita
                        if (i > xDireitaCabeca)
                            xDireitaCabeca = i;
                    }
                }
            }

            int xMeioRetanguloCabeca = xDireitaCabeca - (xDireitaCabeca - xEsquerdaCabeca) / 2;

            int xCabeca = xMeioRetanguloCabeca;
            int yCabeca = yAcima;

            int xEsquerdaPerna = imagem.Width;
            int xDireitaPerna = 0;

            for (int j = yPerna; j < yAbaixo; j++)
            {
                // int inicio = -1;

                for (int i = 0; i < imagem.Width; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        // if (inicio == -1)
                        //     inicio = i;

                        //Ponto mais a esquerda
                        if (i < xEsquerdaPerna)
                            xEsquerdaPerna = i;

                        //Ponto mais a direita
                        if (i > xDireitaPerna)
                            xDireitaPerna = i;
                    }
                    // else
                    // {
                    //     if (inicio != -1)
                    //         break;
                    // }

                }
            }
            int xMeioRetanguloPerna = xDireitaPerna - (xDireitaPerna - xEsquerdaPerna) / 2;

            int xMeioCintura = xMeioRetanguloPerna;
            int yMeioCintura = yPerna;

            int yAbaixoPernaEsquerda = 0;
            int yAbaixoPernaDireita = 0;

            int xAbaixoPernaEsquerda = 0;
            int xAbaixoPernaDireita = 0;

            for (int j = 0; j < imagem.Height; j++)
            {
                for (int i = xEsquerdaPerna; i < xMeioRetanguloPerna; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        if (j > yAbaixoPernaEsquerda)
                            yAbaixoPernaEsquerda = j;
                        xAbaixoPernaEsquerda = i;
                    }
                }
            }

            int xMeioPernaEsquerda = 0;
            count = 0;

            int inicioPernaEsquerda = -1;
            int fimPernaEsquerda = -1;

            for (int i = 0; i < xMeioRetanguloPerna; i++)
            {

                int index = (3 * i + yAbaixoPernaEsquerda * stride) < 0 ? 0 : (3 * i + yAbaixoPernaEsquerda * stride);
                byte b = im[index + 0];
                byte g = im[index + 1];
                byte r = im[index + 2];

                if (b == 0 && r == 0 && g == 0)
                {
                    if (inicioPernaEsquerda == -1)
                        inicioPernaEsquerda = i;
                    count++;
                    fimPernaEsquerda = i;
                }

                xMeioPernaEsquerda = fimPernaEsquerda - (fimPernaEsquerda - inicioPernaEsquerda) / 2;
            }


            for (int j = 0; j < imagem.Height; j++)
            {
                for (int i = xMeioRetanguloPerna; i < xDireitaPerna; i++)
                {
                    int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);
                    byte b = im[index + 0];
                    byte g = im[index + 1];
                    byte r = im[index + 2];

                    if (b == 0 && r == 0 && g == 0)
                    {
                        if (j > yAbaixoPernaDireita)
                            yAbaixoPernaDireita = j;
                        xAbaixoPernaDireita = i;
                    }
                }
            }

            int xMeioPernaDireita = 0;
            count = 0;

            int inicioPernaDireita = -1;
            int fimPernaDireita = -1;

            for (int i = xMeioRetanguloPerna; i < imagem.Width; i++)
            {

                int index = (3 * i + yAbaixoPernaDireita * stride) < 0 ? 0 : (3 * i + yAbaixoPernaDireita * stride);
                byte b = im[index + 0];
                byte g = im[index + 1];
                byte r = im[index + 2];

                if (b == 0 && r == 0 && g == 0)
                {
                    if (inicioPernaDireita == -1)
                        inicioPernaDireita = i;
                    count++;
                    fimPernaDireita = i;
                }

                xMeioPernaDireita = fimPernaDireita - (fimPernaDireita - inicioPernaDireita) / 2;
            }

            int xPeEsquerdo = xAbaixoPernaEsquerda;
            int yPeEsquerdo = yAbaixoPernaEsquerda;

            int xPeDireito = xAbaixoPernaDireita;
            int yPeDireito = yAbaixoPernaDireita;

            int xJoelhoEsquerdo = xMeioPernaEsquerda;
            int yJoelhoEsquerdo = yAbaixoPernaEsquerda - (yAbaixoPernaEsquerda - yPerna) / 2;

            int xJoelhoDireito = xMeioPernaDireita;
            int yJoelhoDireito = yAbaixoPernaDireita - (yAbaixoPernaDireita - yPerna) / 2;

            float fatorInterpolacao = (float)(yTorso - yCabeca) / (yMeioCintura - yCabeca);
            int xInterpolado = xCabeca + (int)((xMeioCintura - xCabeca) * fatorInterpolacao);

            int xTorso = xInterpolado;
            // int yTorso = yTorso;

            int xMeioBracoEsquerdo = (xEsquerda + xTorso) / 2;
            int yMeioBracoEsquerdo = (yEsquerda + yTorso) / 2;

            int xMeioBracoDireito = (xDireita + xTorso) / 2;
            int yMeioBracoDireito = (yDireita + yTorso) / 2;

            int xEsqInfCabeca = xEsquerdaCabeca;
            int yEsqInfCabeca = yAcima + 120;

            int xDirInfCabeca = xDireitaCabeca;
            int yDirInfCabeca = yAcima + 120;

            int xOmbroEsquerdo = (xMeioBracoEsquerdo + xEsqInfCabeca + xTorso) / 3;
            int yOmbroEsquerdo = (yMeioBracoEsquerdo + yEsqInfCabeca + yTorso) / 3;

            int xOmbroDireito = (xMeioBracoDireito + xDirInfCabeca + xTorso) / 3;
            int yOmbroDireito = (yMeioBracoDireito + yDirInfCabeca + yTorso) / 3;

            int xMeioMaoOmbroEsquerdo = (xEsquerda + xOmbroEsquerdo) / 2;
            int yMeioMaoOmbroEsquerdo = yEsquerda;

            int yAbaixoCotoveloEsquerdo = 0;
            int yAcimaCotoveloEsquerdo = imagem.Height;

            // int altura = yEsquerda > imagem.Height / 2 ? imagem.Height : imagem.Height / 2;
            bool preto; 

            for (int j = 0; j < imagem.Height; j++)
            {
                int index = (3 * xMeioMaoOmbroEsquerdo + j * stride) < 0 ? 0 : (3 * xMeioMaoOmbroEsquerdo + j * stride);
                byte b = im[index + 0];
                byte g = im[index + 1];
                byte r = im[index + 2];

                // im[index + 2] = 255;
                // im[index + 1] = 0;
                // im[index + 0] = 0;

                if (b == 0 && r == 0 && g == 0)
                {
                    preto = true;
                    if (j > yAbaixoCotoveloEsquerdo)
                        yAbaixoCotoveloEsquerdo = j;

                    if (j < yAcimaCotoveloEsquerdo)
                        yAcimaCotoveloEsquerdo = j;
                }
                else    
                    preto = false;
                if (!preto && yAcimaCotoveloEsquerdo != imagem.Height)
                    break;
            }
            // imagem.UnlockBits(data);
            // return imagem;
            

            int xCotoveloEsquerdo = xMeioMaoOmbroEsquerdo;
            int yCotoveloEsquerdo = yAbaixoCotoveloEsquerdo - (yAbaixoCotoveloEsquerdo - yAcimaCotoveloEsquerdo) / 2;

            int xMeioMaoOmbroDireito = (xDireita + xOmbroDireito) / 2;
            int yMeioMaoOmbroDireito = yDireita;

            int yAbaixoCotoveloDireito = 0;
            int yAcimaCotoveloDireito = imagem.Height;

            for (int j = 0; j < imagem.Height; j++)
            {
                int index = (3 * xMeioMaoOmbroDireito + j * stride) < 0 ? 0 : (3 * xMeioMaoOmbroDireito + j * stride);
                byte b = im[index + 0];
                byte g = im[index + 1];
                byte r = im[index + 2];

                if (b == 0 && r == 0 && g == 0)
                {
                    if (j > yAbaixoCotoveloDireito)
                        yAbaixoCotoveloDireito = j;

                    if (j < yAcimaCotoveloDireito)
                        yAcimaCotoveloDireito = j;
                }
            }

            int xCotoveloDireito = xMeioMaoOmbroDireito;
            int yCotoveloDireito = yAbaixoCotoveloDireito - (yAbaixoCotoveloDireito - yAcimaCotoveloDireito) / 2;

            int xMeioTronco = xAcima;
            // int yMeioTronco = yMeioCintura - (yMeioCintura - yAcima) / 3;

            int DistanciaBracoEsquerdoTorso = (int)Math.Sqrt(Math.Pow(xTorso - xEsquerda, 2) + Math.Pow(yTorso - yEsquerda, 2));
            int DistanciaTorsoBracoDireito = (int)Math.Sqrt(Math.Pow(xDireita - xTorso, 2) + Math.Pow(yDireita - yTorso, 2));
            int DistanciaCabecaBarriga = 2 * ((int)Math.Sqrt(Math.Pow(xAcima - xMeioCintura, 2) + Math.Pow(yAcima - yMeioCintura, 2))) / 3;

            bool maoOuBracoEsquerdo;
            bool maoOuBracoDireito;

            int xMaoEsquerda = 0;
            int yMaoEsquerda = imagem.Height;

            if (DistanciaBracoEsquerdoTorso > DistanciaCabecaBarriga)
                maoOuBracoEsquerdo = true;
            else
            {
                maoOuBracoEsquerdo = false;

                for (int j = 0; j < imagem.Height; j++)
                {
                    for (int i = 0; i < xEsquerdaCabeca; i++)
                    {
                        int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);

                        byte b = im[index + 0];
                        byte g = im[index + 1];
                        byte r = im[index + 2];
                        if (b == 0 && r == 0 && g == 0)
                        {
                            if (j < yMaoEsquerda)
                            {
                                yMaoEsquerda = j;
                                xMaoEsquerda = i;
                            }
                        }   
                    }
                }
            }

            int xMaoDireita = 0;
            int yMaoDireita = imagem.Height;


            if (DistanciaTorsoBracoDireito > DistanciaCabecaBarriga)
                maoOuBracoDireito = true;
            else
            {
                maoOuBracoDireito = false;

                for (int j = 0; j < imagem.Height; j++)
                {
                    for (int i = xDireitaCabeca; i < imagem.Width; i++)
                    {
                        int index = (3 * i + j * stride) < 0 ? 0 : (3 * i + j * stride);

                        byte b = im[index + 0];
                        byte g = im[index + 1];
                        byte r = im[index + 2];
                        if (b == 0 && r == 0 && g == 0)
                        {
                            if (j < yMaoDireita)
                            {
                                yMaoDireita = j;
                                xMaoDireita = i;
                            }
                        }   
                    }
                }
            }

            int xEsquerdaCotovelo = imagem.Width;

            for (int i = 0; i < xTorso; i++)
            {
                int index = (3 * i + yCotoveloEsquerdo * stride) < 0 ? 0 : (3 * i + yCotoveloEsquerdo * stride);
                byte b = im[index + 0];
                byte g = im[index + 1];
                byte r = im[index + 2];

                if (b == 0 && r == 0 && g == 0)
                {
                    if (i < xEsquerdaCotovelo)
                        xEsquerdaCotovelo = i;
                }   
            }

            int xDireitaCotovelo = 0;

            for (int i = xTorso; i < imagem.Width; i++)
            {
                int index = (3 * i + yCotoveloDireito * stride) < 0 ? 0 : (3 * i + yCotoveloDireito * stride);
                byte b = im[index + 0];
                byte g = im[index + 1];
                byte r = im[index + 2];

                if (b == 0 && r == 0 && g == 0)
                {
                    if (i > xDireitaCotovelo)
                    {
                        xDireitaCotovelo = i;
                    }
                }   
            }

            
            // for(int i = xAcima; i < imagem.Width; i++)
            // {

            //     int index = 3 * i + yCotoveloDireito  * strid//     
            // byte b = im[index + 0];
            //     byte g = im[index + 1];
            //     byte r = im[index + 2];

                
            //     if (b == 0 && r == 0 && g == 0)
            //     {
            //         xAcima = i;
            //     }   
            // }
            
            int x34TorsoEsquerdo = (2 * xTorso + xCotoveloEsquerdo) / 3;
            int y34TorsoEsquerdo = (2 * yTorso + yCotoveloEsquerdo) / 3;

            int x34TorsoDireito = (2 * xTorso + xCotoveloDireito) / 3;
            int y34TorsoDireito = (2 * yTorso + yCotoveloDireito) / 3;

            imagem.UnlockBits(data);

            using Graphics graphics = Graphics.FromImage(imagem);

            using Pen pen = new(Color.Red, 2);

            Point pointCabeca = new(xCabeca, yCabeca);

            Point pointEsquerda = new(xEsquerda, yEsquerda);
            Point pointDireita = new(xDireita, yDireita);

            Point pointCotoveloEsquerdo = new(xCotoveloEsquerdo, yCotoveloEsquerdo);
            Point pointCotoveloDireito = new(xCotoveloDireito, yCotoveloDireito);

            Point pointOmbroEsquerdo = new(xOmbroEsquerdo, yTorso);
            Point pointOmbroDireito = new(xOmbroDireito, yTorso);

            Point pointTorso = new(xTorso, yTorso);
            Point pointTorsoEsquerdo = new(x34TorsoEsquerdo, yTorso);
            Point pointTorsoDireito = new(x34TorsoDireito, yTorso);

            Point pointCintura = new(xMeioCintura, yMeioCintura);

            Point pointJoelhoEsquerdo = new(xJoelhoEsquerdo, yJoelhoEsquerdo);
            Point pointJoelhoDireito = new(xJoelhoDireito, yJoelhoDireito);

            Point pointPeEsquerdo = new(xMeioPernaEsquerda, yPeEsquerdo);
            Point pointPeDireito = new(xMeioPernaDireita, yPeDireito);

            Point pointMaoEsquerda = new(xMaoEsquerda, yMaoEsquerda);
            Point pointMaoDireita = new(xMaoDireita, yMaoDireita);

            Point NovoCotoveloEsquerdo = new(xCotoveloEsquerdo - (xCotoveloEsquerdo - xEsquerdaCotovelo) / 2, yCotoveloEsquerdo);
            Point NovoCotoveloDireito = new(xDireitaCotovelo - (xDireitaCotovelo - xCotoveloDireito) / 2, yCotoveloDireito);

            Point[] VerticalEsquerdo =
            {
                pointCabeca,
                pointCintura,
                pointPeEsquerdo
            };

            Point[] VerticalEsquerdoCompleto =
            {
                pointCabeca,
                pointCintura,
                pointJoelhoEsquerdo,
                pointPeEsquerdo
            };

            Point[] VerticalDireito =
            {
                pointCabeca,
                pointCintura,
                pointPeDireito
            };

            Point[] VerticalDireitoCompleto =
            {
                pointCabeca,
                pointCintura,
                pointJoelhoDireito,
                pointPeDireito
            };

            Point[] Horizontal =
            {
                maoOuBracoEsquerdo ? pointEsquerda : pointMaoEsquerda,
                maoOuBracoEsquerdo ? pointCotoveloEsquerdo : NovoCotoveloEsquerdo,
                maoOuBracoEsquerdo ? pointOmbroEsquerdo : pointTorsoEsquerdo,
                pointTorso,
                maoOuBracoDireito ? pointOmbroDireito : pointTorsoDireito,
                maoOuBracoDireito ? pointCotoveloDireito : NovoCotoveloDireito,
                maoOuBracoDireito ? pointDireita : pointMaoDireita,
            };

            graphics.DrawLines(pen, Horizontal);
            graphics.DrawLines(pen, yPerna == CorpoCortado ? VerticalEsquerdo : VerticalEsquerdoCompleto);
            graphics.DrawLines(pen, yPerna == CorpoCortado ? VerticalDireito : VerticalDireitoCompleto);

            return new Point(xEsquerda, yEsquerda);
        }
        catch
        {
            return Point.Empty;
        }
    }
}
