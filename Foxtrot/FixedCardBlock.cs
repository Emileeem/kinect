using System;
using System.Drawing;

namespace Foxtrot;

public class FixedCardBlock : CardBlock
{
    public override RectangleF Rect
    {
        get
        {
            float x, y, wid, hei;

            x = Location.X;
            y = Location.Y;
            wid = 79;
            hei = (Cards.Count) * 20 + 110;
            return new RectangleF(x, y, wid, hei);
        }
    }
    public override void OnMove(Point cursor)
    {

    }
    public override CardBlock OnSelect(Point cursor)
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
                    float _x = cursor.X - this.Location.X;
                    float _y = cursor.Y - this.Location.Y - j * Gap;
                    newBlockTest.ptClick = new PointF(_x, _y);

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
        if (this.Cards.Count == 1 && cursor.Y > this.Location.Y)
        {

            var BlockTestParaUm = new CardBlock();
            float _x2 = cursor.X - this.Location.X;
            float _y2 = cursor.Y - this.Location.Y;
            BlockTestParaUm.ptClick = new PointF(_x2, _y2);

            BlockTestParaUm.Cards.Add(this.Cards[0]);
            this.Cards.Remove(this.Cards[this.Cards.Count - 1]);

            return BlockTestParaUm;
        }
        if (this.Cards.Count > 1 && cursor.Y > this.Location.Y && cursor.Y < this.Location.Y + 20)
        {
            var newBlockTest = new CardBlock();
            float _x = cursor.X - this.Location.X;
            float _y = cursor.Y - this.Location.Y;
            newBlockTest.ptClick = new PointF(_x, _y);

            for (int i = 0; i < this.Cards.Count; i++)
            {
                newBlockTest.Cards.Add(this.Cards[i]);
            }
            this.Cards.RemoveRange(0, this.Cards.Count);

            ptClick = new PointF(
                cursor.X - this.Rect.X,
                cursor.Y - this.Rect.Y
            );
            return newBlockTest;
        }
        return null;
    }
    public override void Draw(Graphics g)
    {
        if (this.Cards.Count == 0)
        {

            g.DrawRectangle(Pens.Red,
                this.Rect.X, Rect.Y,
                Rect.Width, Rect.Height
            );
        }
        else
        {
            g.DrawRectangle(Pens.Red,
                this.Rect.X, Rect.Y,
                Rect.Width, Rect.Height - 20
            );
        }



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