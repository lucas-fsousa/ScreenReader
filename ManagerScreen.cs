using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System.Drawing;
using System.Runtime.InteropServices;

namespace PublicUtility.ScreenReader {
  public static class ScreenManager {
    public static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.LocateAllOnScreenForWindows(imagePath, confidence, region);


      throw new InvalidOperationException("This platform does not yet support this action.");
    }
    
    public static BoxOfScreen LocateOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) => LocateAllOnScreen(imagePath, confidence, region).FirstOrDefault();
    
    public static PointIntoScreen LocateOnScreen(PixelColor color) {
      if(OperatingSystem.IsWindows())
        return Windows.Screen.GetXYOnWindowsByColor(Color.FromArgb(color.Alpha, color.Red, color.Green, color.Blue));
      
      return new(0, 0);
    }

    public static Stream PrintScreen() {
      throw new NotImplementedException();
    }
  }
}
