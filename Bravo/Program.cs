using Bravo;

Blur blur = new Blur();

Bitmap bitmap = new Bitmap("img/alameda.png");

DateTime dt = DateTime.Now;
var blured = blur.UseBlur(bitmap, 11);
var time = DateTime.Now - dt;
MessageBox.Show(time.TotalMilliseconds.ToString());


blured.Save("blured.png");