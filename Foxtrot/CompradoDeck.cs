using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Foxtrot;

public class CompradoDeck : CardBlock
{
    
    public override RectangleF Rect
    {
        get
        {
            float x, y, wid, hei;

            x = Location.X;
            y = Location.Y;
            wid = 79;
            hei = 110;

            return new RectangleF(x, y, wid, hei);
        }
    }
    public override void OnMove(Point cursor)
    {
      
    }
    public override CardBlock OnSelect(Point cursor)
    {
        if (this.Cards.Count > 1)
        {

            int upperBound = (int)this.Location.Y;
            int lowerBound = (int)this.Location.Y + 110;

            if (cursor.Y > this.Location.Y && cursor.Y < lowerBound)
            {
                var newBlockTest = new CardBlock();

                float _x = cursor.X - this.Location.X;
                float _y = cursor.Y - this.Location.Y;
                newBlockTest.ptClick = new PointF(_x, _y);

                newBlockTest.Cards.Add(this.Cards[^1]);

                this.Cards.Remove(this.Cards[^1]);

                ptClick = new PointF(
                    cursor.X - this.Rect.X,
                    cursor.Y - upperBound - this.Rect.Y
                );
                return newBlockTest;
            }
        }
        return null;
    }
    public override void Draw(Graphics g)
    {
        if (this.Cards.Count < 1)
            return;
        g.DrawRectangle(Pens.Red,
            this.Rect.X, Rect.Y,
            Rect.Width, Rect.Height
        );

        Card card = Cards[^1];

        var rect = new Rectangle(
            (int)Location.X,
            (int)Location.Y,
            (int)card.Size.Width,
            (int)card.Size.Height
        );

        card.Draw(g, rect);

    }
}
