using PublicUtility.Nms;
using PublicUtility.Nms.Structs;
using System.Drawing;
using System.Drawing.Imaging;


namespace PublicUtility.ScreenReader {
  public static class ScreenManager {

    public static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.LocateAllOnScreen(imagePath, confidence, region);


      throw new PlatformNotSupportedException("This platform does not yet support this action.");
    }

    public static BoxOfScreen LocateOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) => LocateAllOnScreen(imagePath, confidence, region).FirstOrDefault();

    public static PointIntoScreen LocateOnScreen(PixelColor color) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.GetXYOnWindowsByColor(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));

      return new(0, 0);
    }

    public static ImageStream PrintScreen(BoxOfScreen box = default) {
      using var imgStream = new ImageStream();
      if(OperatingSystem.IsWindows())
        Windows.Screen.TakeScreenshot(box).Save(imgStream, ImageFormat.Png);

      if(imgStream.Length <= 0)
        throw new PlatformNotSupportedException("this platform does not yet support this action.");
      
      return imgStream;
    }
  
  }
}
