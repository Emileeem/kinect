using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Foxtrot;

public class Card
{
    private Sprite sprite;
    private Bitmap cartaoClomado;

    public SizeF Size => sprite.Rect.Size;
    public bool Visible { get; set; } = true;
    public virtual void Draw(Graphics g, Rectangle rect)
    {
        if (Visible)
            sprite.Draw(g, rect);
        else
        {
            cartaoClomado = Bitmap.FromFile(@"img/cartao.png") as Bitmap;
            g.DrawImage(
            this.cartaoClomado,
            rect
            );
        };
        // g.FillRectangle(Brushes.RoyalBlue, rect); // Muda aqui garaio

    }
    public static List<Card> GetAll()
    {
        var cards = new List<Card>();
        var cardSprites = Bitmap.FromFile(@"img/cartas/caralho.png") as Bitmap;


        var y2 = 0;
        for (int j = 0; j < 4; j++)
        {

            for (int i = 0; i < 13; i++)
            {
                var sprite = new Sprite(cardSprites);
                sprite.Rect = new RectangleF(i * 88, y2, 79, 110);

                Card card = new Card();
                card.sprite = sprite;

                cards.Add(card);
            }
            y2 += 129;
        }

        return cards;
    }

    public static List<Card> GetDefaults()
    {
        var cards = new List<Card>();
        // var cardSprites = Bitmap.FromFile(@"img/cartas/caralho.png") as Bitmap;
        for (int i = 0; i < 4; i++)
        {
            var sprite = new Sprite(Bitmap.FromFile(@$"img/cartas/naipe/n{i}.png") as Bitmap);

            sprite.Rect = new RectangleF(0, 0, 79, 110);

            Card card = new Card();
            card.sprite = sprite;

            cards.Add(card);
        }

        return cards;
    }

    public static Card Coronga()
    {
        var card = new Card();
        var sprite = new Sprite(Bitmap.FromFile(@"img/coronga.png") as Bitmap);
        sprite.Rect = new RectangleF(0, 0, 79, 110);
        card.sprite = sprite;

        return card;
    }
}