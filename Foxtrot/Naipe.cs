// using System.Drawing;
// using System.Collections.Generic;

// namespace Foxtrot
// {
//     public class Naipe : Card
//     {
//         private Sprite sprite;
//         public SizeF Size => sprite.Rect.Size;

//         public Naipe(Bitmap bitmap)
//         {
//             sprite = new Sprite(bitmap);
//         }

//         public virtual void Draw(Graphics g, Rectangle rect)
//         {
//             sprite.Draw(g, rect);
//         }

//         public static List<Naipe> GetAll()
//         {
//             var naipes = new List<Naipe>();

//             for (var i = 0; i < 3; i++)
//             {
//                 string imagePath = $"img/cartas/naipe/n{i}.png";
//                 Bitmap bitmap = new Bitmap(imagePath);
//                 naipes.Add(new Naipe(bitmap));
//             }

//             return naipes;
//         }
//     }
// }
