using System.Drawing;

namespace Foxtrot;

public class Sprite
{
    Bitmap img;
    public RectangleF Rect { get; set; }

    public Sprite(string path)
        => this.img = Bitmap.FromFile(path) as Bitmap;
    public Sprite(Bitmap bmp)
        => this.img = bmp;
    
    public void Draw(Graphics g, Rectangle drawRect)
    {
        g.DrawImage(
            this.img,
            drawRect,
            this.Rect,
            GraphicsUnit.Pixel
        );
    }
}