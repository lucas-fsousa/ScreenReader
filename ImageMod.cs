using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using PublicUtility.Nms.Structs;

namespace PublicUtility.ScreenReader {
  internal static class ImageMod {
    internal static Image<Gray, byte> ToGrayImage(string filePath) => new Image<Bgr, byte>(filePath).Convert<Gray, byte>();

    internal static Image<Gray, byte> ToGrayImage(this Image<Bgr, byte> image) => image.Convert<Gray, byte>();

    internal static IList<BoxOfScreen> CalcConfidence(Image<Gray, byte> source, Image<Gray, byte> template, double confidence) {
      var response = new List<BoxOfScreen>();
      Image<Gray, float> imgMatch = template.MatchTemplate(source, TemplateMatchingType.CcoeffNormed);

      float[,,] matches = imgMatch.Data;
      for(int y = 0; y < matches.GetLength(0); y++) {
        for(int x = 0; x < matches.GetLength(1); x++) {
          double matchScore = matches[y, x, 0];

          if(matchScore >= confidence) 
            response.Add(new(new(source.Width, source.Height), new(x, y)));
        }
      }

      return response;
    }
  }
}
