using static Bravo.Blur;

// Blur blur = new Blur();

Bitmap bitmap = new Bitmap("img/alameda.png");
// Bitmap bitmap = new Bitmap("img/perry.jpg");
// Bitmap bitmap = new Bitmap("img/muie.png");

DateTime dt = DateTime.Now;
var blured = UseBlur(bitmap, 2);
var time = DateTime.Now - dt;

MessageBox.Show(time.TotalMilliseconds.ToString());

blured.Save("blured.bmp");