using System.Collections.Generic;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis
{
    public interface IImageAnalysis
    {
        AnalysisResult VisionAnalysis { get; set; }
        OcrResults TextAnalysis { get; set; }
        Face[] FacialAnalysis { get; set; }
        string[] People { get; set; }
        string ImageUrl { get; set; }
        int ImageHeight { get; set; }
        int ImageWidth { get; set; }
        List<string> ValidationErrors { get; set; }
        bool HasAnyAnalysis();
        bool HasNoAnalysis();
    }
}