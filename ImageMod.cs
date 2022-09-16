using Emgu.CV.Structure;
using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV.CvEnum;
using PublicUtility.Nms.Structs;

namespace PublicUtility.ScreenReader {
  internal static class ImageMod {
    internal static Image<Gray, byte> ToGrayImage(string filePath) {
      Image<Gray, byte> grayImage;
      var input = new Image<Bgr, byte>(filePath);

      grayImage = input.Convert<Gray, byte>();
      return grayImage;
    }

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
