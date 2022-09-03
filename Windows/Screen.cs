using Emgu.CV;
using Emgu.CV.CvEnum;
using System.Drawing;
using Emgu.CV.Structure;
using System.Runtime.InteropServices;
using PublicUtility.ScreenReader.Structs;

namespace PublicUtility.ScreenReader.Windows {

  internal static class Screen {

    [DllImport("User32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
    private static extern int GetSystemMetrics(int nIndex);

    internal static ScreenSize GetScreenSizeOnWindows() => new(GetSystemMetrics(0), GetSystemMetrics(1));

    internal static IList<BoxOfScreen> LocateAllOnScreenForWindows(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      var response = new List<BoxOfScreen>();

      if(OperatingSystem.IsWindows()) {
        Bitmap screenshot;
        var source = new Image<Gray, byte>(imagePath);

        if(region.Filled) {
          screenshot = TakeScreenshot(region);

          if(region.Size.Width > screenshot.Width || region.Size.Height > screenshot.Height)
            throw new Exception($"{nameof(imagePath)} out of bounds");

        } else {
          screenshot = TakeScreenshot();
        }

        var template = screenshot.ToImage<Gray, byte>();
        Image<Gray, float> imgMatch = template.MatchTemplate(source, TemplateMatchingType.CcoeffNormed);

        float[,,] matches = imgMatch.Data;
        for(int y = 0; y < matches.GetLength(0); y++) {
          for(int x = 0; x < matches.GetLength(1); x++) {
            double matchScore = matches[y, x, 0];

            if(matchScore >= confidence) {
              response.Add(new(new(source.Width, source.Height), new(x, y)));
            }

          }
        }

      }

      return response;
    }

    internal static Bitmap TakeScreenshot(BoxOfScreen box = default) {
      if(OperatingSystem.IsWindows()) {
        if(!box.Filled)
          box = new(GetScreenSizeOnWindows(), new(1,1));

        Bitmap bmp = new(box.Size.Width, box.Size.Height);
        Graphics graphics = Graphics.FromImage(bmp);
        graphics.CopyFromScreen(box.Point.X, box.Point.Y, 0, 0, bmp.Size);
        return bmp;
      }

      return default;
    }

    internal static Image<Gray, byte> ToGrayImageOnWindows(string filePath) {
      Image<Gray, byte> _GrayImage;
      var _input = new Image<Bgr, byte>(filePath);

      _GrayImage = _input.Convert<Gray, byte>();
      return _GrayImage;
    }

    internal static Image<Gray, byte> ToGrayImageOnWindows(this Image<Bgr, byte> image) => image.Convert<Gray, byte>();

    internal static PointIntoScreen GetXYOnWindowsByColor(Color color) {
      if(OperatingSystem.IsWindows()) {
        var screen = TakeScreenshot();

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