using Emgu.CV;
using Emgu.CV.Structure;
using PublicUtility.Nms.Structs;
using SixLabors.ImageSharp.Formats.Png;
using System.Drawing;
using System.Runtime.InteropServices;
using IS = SixLabors.ImageSharp;

namespace PublicUtility.ScreenReader.Windows {
#pragma warning disable CA1416 // Validar a compatibilidade da plataforma

  internal static class Screen {

    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern int GetSystemMetrics(int nIndex);

    private static Bitmap GetBitmap(this IS.Image imageSharp) {
      using var stream = new MemoryStream();
      imageSharp.Save(stream, new PngEncoder());
      return Image.FromStream(stream) as Bitmap;
    }

    internal static ScreenSize GetScreenSizeOnWindows() => new(GetSystemMetrics(0), GetSystemMetrics(1));

    internal static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      Bitmap screenshot;

      if(region.Filled) {
        screenshot = TakeScreenshot(region).GetBitmap();

        if(region.Size.Width > screenshot.Width || region.Size.Height > screenshot.Height)
          throw new Exception($"{nameof(imagePath)} out of bounds");

      } else {
        screenshot = TakeScreenshot().GetBitmap();
      }

      var source = new Image<Gray, byte>(imagePath);
      var template = screenshot.ToImage<Gray, byte>();
      var response = ImageMod.CalcConfidence(source, template, confidence);

      return response;
    }

    internal static IS.Image TakeScreenshot(BoxOfScreen box = default) {
      string path = string.Format(@$"C:\Windows\Temp\{DateTime.Now.Ticks:x2}.png");

      if(!box.Filled)
        box = new(GetScreenSizeOnWindows(), new(1, 1));

      Bitmap bmp = new(box.Size.Width, box.Size.Height);
      Graphics graphics = Graphics.FromImage(bmp);
      graphics.CopyFromScreen(box.Point.X, box.Point.Y, 0, 0, bmp.Size);
      bmp.Save(path);

      var newImg = IS.Image.Load(path);

      if(newImg != null)
        File.Delete(path);

      return newImg;
    }

    internal static PointIntoScreen GetXYOnWindowsByColor(Color color) {
      var screen = TakeScreenshot().GetBitmap();

      for(int x = 0; x < screen?.Width; x++) {
        for(int y = 0; y < screen?.Height; y++) {

          Color pixel = screen.GetPixel(x, y);
          if(pixel.R == color.R && pixel.G == color.G && pixel.B == color.B)
            return new PointIntoScreen(x, y);
        }
      }

      return new PointIntoScreen(0, 0);
    }
  }
}