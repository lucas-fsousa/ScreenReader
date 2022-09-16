using Emgu.CV;
using Emgu.CV.Structure;
using PublicUtility.Nms.Structs;
using System.Diagnostics;

namespace PublicUtility.ScreenReader.Linux {
  internal static class Screen {
    private static string Terminal(string program, string command) {
      try {
        var proc = new Process {
          StartInfo = {
          FileName = program,
          Arguments = command,
          UseShellExecute = false,
          RedirectStandardOutput = true,
          RedirectStandardInput = false,
          RedirectStandardError = false
          }
        };
        proc.Start();

        return proc.StandardOutput.ReadToEnd();
      } catch(Exception ex) { throw new Exception(ex.Message, ex); }

    }

    internal static ScreenSize GetScreenSizeOnLinux() {
      var terminalOutput = Terminal("xrandr", "--screen 0").ToLower().Split(',')[2];
      var resolution = terminalOutput[..(terminalOutput.IndexOf("default") - 1)].Replace("maximum ", "").Replace(" ", "").Split('x');
      var w = Convert.ToInt32(resolution[0]);
      var h = Convert.ToInt32(resolution[1]);
      return new ScreenSize(w, h);
    }

    internal static string TakeScreenshot(BoxOfScreen box) {
      string path = string.Concat("/tmp/", DateTime.Now.Ticks.ToString("x2"));
      try {
        if(box.Filled)
          Terminal("scrot", $"-a {box.Point.X},{box.Point.Y},{box.Size.Width},{box.Size.Height} -F {path}");
        else
          Terminal($"scrot", path);

        return path;
      } catch(Exception ex) {
        throw new Exception("Make sure you have \"scrot\" installed. If not, try: \"sudo apt install scrot\" for ubuntu or debian. More info in: [ https://github.com/resurrecting-open-source-projects/scrot ]", ex);
      }
    }

    internal static IList<BoxOfScreen> LocateAllOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) {
      var source = ImageMod.ToGrayImage(imagePath);
      var template = ImageMod.ToGrayImage(TakeScreenshot(region));
      var response = ImageMod.CalcConfidence(source, template, confidence);
      return response;
    }

    internal static BoxOfScreen LocateOnScreen(string imagePath, double confidence = 0.90, BoxOfScreen region = default) => LocateAllOnScreen(imagePath, confidence, region).FirstOrDefault();
  }
}