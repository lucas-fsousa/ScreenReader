using Emgu.CV;
using Emgu.CV.Structure;
using PublicUtility.Nms;
using PublicUtility.Nms.Structs;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace PublicUtility.ScreenReader.Windows {

  internal static class Screen {

    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern int GetSystemMetrics(int nIndex);

    internal static ScreenSize GetScreenSizeOnWindows() => new(GetSystemMetrics(0), GetSystemMetrics(1));
    
    internal static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      IList<BoxOfScreen> response = default;

      if(OperatingSystem.IsWindows()) {
        Bitmap screenshot;

        if(region.Filled) {
          screenshot = new Bitmap(Image.FromStream(TakeScreenshot(region)));

          if(region.Size.Width > screenshot.Width || region.Size.Height > screenshot.Height)
            throw new Exception($"{nameof(imagePath)} out of bounds");

        } else {
          screenshot = new Bitmap(Image.FromStream(TakeScreenshot()));
        }

        var source = new Image<Gray, byte>(imagePath);
        var template = screenshot.ToImage<Gray, byte>();
        response = ImageMod.CalcConfidence(source, template, confidence);
      }

      return response;
    }

    internal static ImageStream TakeScreenshot(BoxOfScreen box = default) {
      if(OperatingSystem.IsWindows()) {
        if(!box.Filled)
          box = new(GetScreenSizeOnWindows(), new(1,1));

        Bitmap bmp = new(box.Size.Width, box.Size.Height);
        Graphics graphics = Graphics.FromImage(bmp);
        graphics.CopyFromScreen(box.Point.X, box.Point.Y, 0, 0, bmp.Size);
        var imgStream = new ImageStream();
        bmp.Save(imgStream, ImageFormat.Png);

        return imgStream;
      }

      return default;
    }

    internal static PointIntoScreen GetXYOnWindowsByColor(Color color) {
      if(OperatingSystem.IsWindows()) {
        var screen = Image.FromStream(TakeScreenshot()) as Bitmap;

        for(int x = 0; x < screen?.Width; x++) {
          for(int y = 0; y < screen?.Height; y++) {

            Color pixel = screen.GetPixel(x, y);
            if(pixel.R == color.R && pixel.G == color.G && pixel.B == color.B) {
              return new PointIntoScreen(x, y);
            }

          }

        }
      }

      return new PointIntoScreen(0, 0);
    }
  }
}