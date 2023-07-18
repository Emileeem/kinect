using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Security.Cryptography;

namespace Foxtrot;

public class CardBlock
{
    public static int Gap { get; set; } = 20;
    public static int CardHeight { get; set; } = 110;

    internal PointF? ptClick = null;

    public List<Card> Cards { get; set; } = new List<Card>();
    public  virtual PointF Location { get; set; } = new PointF(140, 20);

    public virtual RectangleF Rect
    {
        get
        {
            float x = 0, y = 0, wid = 0, hei = 0;

            x = Location.X;
            y = Location.Y;
            wid = 79;
            hei = (Cards.Count - 1) * 20 + 110;

            return new RectangleF(x, y, wid, hei);
        }
    }

    public virtual CardBlock OnSelect(Point cursor)
    {
        if (this.Cards.Count > 1 && cursor.Y > this.Location.Y + Gap)
        {
            for (int j = 1; j < this.Cards.Count; j++)
            {
                int upperBound = Gap + Gap * (j - 1);
                int lowerBound = Gap + Gap * j;

                if (j == this.Cards.Count - 1)
                    lowerBound += CardHeight - Gap;

                if (cursor.Y > this.Location.Y + upperBound && cursor.Y < this.Location.Y + lowerBound)
                {
                    if (this.Cards.Count < 2 && cursor.Y > this.Location.Y + Gap)
                        continue;

                    var newBlockTest = new CardBlock();
                    newBlockTest.ptClick = this.ptClick;

                    for (int k = j; k < this.Cards.Count; k++)
                    {
                        newBlockTest.Cards.Add(this.Cards[k]);
                    }
                    this.Cards.RemoveRange(j, this.Cards.Count - j);

                    ptClick = new PointF(
                        cursor.X - this.Rect.X,
                        cursor.Y - upperBound - this.Rect.Y
                    );
                    return newBlockTest;
                }

            }
        }
        else
        {
            float _x , _y;
            _x = cursor.X - this.Location.X;
            _y = cursor.Y - this.Location.Y;
            ptClick = new PointF(_x, _y);
            return this;
        }

        return null;
    }

    public virtual void OnMove(Point cursor)
    {
        if (ptClick is null)
            return;

        Location = new PointF(
            cursor.X - ptClick.Value.X,
            cursor.Y - ptClick.Value.Y
        );
    }

    public virtual void Draw(Graphics g)
    {
        int dy = 0;
        foreach (var card in Cards)
        {
            var rect = new Rectangle(
                (int)Location.X,
                (int)Location.Y + dy,
                (int)card.Size.Width,
                (int)card.Size.Height
            );
            
            card.Draw(g, rect);

            dy += 20;
        }
    }
}