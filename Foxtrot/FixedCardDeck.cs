using System.Drawing;

namespace Foxtrot;

public class FixedCardDeck : CardBlock
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
                if (this.Cards.Count == 0 && cursor.Y > upperBound)
                    return null;

                var newBlockTest = new CardBlock();

                float _x = cursor.X - this.Location.X;
                float _y = cursor.Y - this.Location.Y;
                newBlockTest.ptClick = new PointF(_x, _y);

                newBlockTest.Cards.Add(this.Cards[this.Cards.Count - 1]);

                this.Cards.Remove(this.Cards[this.Cards.Count - 1]);

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
        g.DrawRectangle(Pens.Red,
            this.Rect.X, Rect.Y,
            Rect.Width, Rect.Height
        );

        Card card = Cards[Cards.Count - 1];

        var rect = new Rectangle(
            (int)Location.X,
            (int)Location.Y,
            (int)card.Size.Width,
            (int)card.Size.Height
        );

        card.Draw(g, rect);


    }


}