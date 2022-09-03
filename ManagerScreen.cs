using System.Drawing;
using System.Drawing.Imaging;
using PublicUtility.ScreenReader.Structs;

namespace PublicUtility.ScreenReader {
  public static class ScreenManager {

    public static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.LocateAllOnScreenForWindows(imagePath, confidence, region);


      throw new PlatformNotSupportedException("This platform does not yet support this action.");
    }

    public static BoxOfScreen LocateOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) => LocateAllOnScreen(imagePath, confidence, region).FirstOrDefault();

    public static PointIntoScreen LocateOnScreen(PixelColor color) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.GetXYOnWindowsByColor(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));

      return new(0, 0);
    }

    public static Stream PrintScreen(BoxOfScreen box = default) {
      using var ms = new MemoryStream();
      if(OperatingSystem.IsWindows()) {
        var bmp = Windows.Screen.TakeScreenshot(box);
        bmp.Save(ms, ImageFormat.Png);
      }

      if(ms.Length <= 0)
        throw new PlatformNotSupportedException("this platform does not yet support this action.");
      
      return ms;
    }
  }
}
