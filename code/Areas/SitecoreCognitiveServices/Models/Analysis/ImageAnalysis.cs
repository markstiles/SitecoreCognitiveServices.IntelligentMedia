using System.Collections.Generic;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis
{
    public class ImageAnalysis : IImageAnalysis
    {
        public ImageAnalysis()
        {
            VisionAnalysis = new AnalysisResult();
            TextAnalysis = new OcrResults();
            FacialAnalysis = new Face[0];
        }

        public AnalysisResult VisionAnalysis { get; set; }
        public OcrResults TextAnalysis { get; set; }
        public Face[] FacialAnalysis { get; set; }
        public string[] People { get; set; }
        public string ImageUrl { get; set; }
        public int ImageHeight { get; set; }
        public int ImageWidth { get; set; }
        public List<string> ValidationErrors { get; set; }
        public bool HasAnyAnalysis()
        {
            var hasAnyAnalyis = FacialAnalysis?.Length > 0
                                || TextAnalysis?.Regions != null
                                || VisionAnalysis?.Description != null;

            return hasAnyAnalyis;
        }

        public bool HasNoAnalysis()
        {
            return !HasAnyAnalysis();
        }
    }
}