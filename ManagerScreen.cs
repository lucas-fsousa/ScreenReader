using PublicUtility.Nms;
using PublicUtility.Nms.Structs;
using System.Drawing;


namespace PublicUtility.ScreenReader {
  public static class ScreenManager {

    public static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.LocateAllOnScreen(imagePath, confidence, region);

      if(OperatingSystem.IsLinux())
        return Linux.Screen.LocateAllOnScreen(imagePath, confidence, region);

      throw new PlatformNotSupportedException("This platform does not yet support this action.");
    }

    public static BoxOfScreen LocateOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) => LocateAllOnScreen(imagePath, confidence, region).FirstOrDefault();

    public static PointIntoScreen LocateOnScreen(PixelColor color) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.GetXYOnWindowsByColor(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));

      if(OperatingSystem.IsLinux())
        throw new PlatformNotSupportedException();


      return new(0, 0);
    }

    public static ImageStream PrintScreen(BoxOfScreen box = default) {
      ImageStream imgStream;
      if(OperatingSystem.IsWindows()) { 
        imgStream = Windows.Screen.TakeScreenshot(box);
      
      } else if(OperatingSystem.IsLinux()) {
        var path = Linux.Screen.TakeScreenshot(box);
        using var file = new FileStream(path, FileMode.OpenOrCreate);
        using var stream = new ImageStream();
        var buffer = new byte[file.Length];
        file.Read(buffer);
        stream.Write(buffer);
        return stream;

      } else { 
        throw new PlatformNotSupportedException("This system does not yet support this action.");
      }

      return imgStream;
    }
  
  }
}
