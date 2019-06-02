using System.Collections.Generic;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public interface IImageAnalysisFactory
    {
        IImageAnalysis Create();
        IImageAnalysis Create(IImageSearchResult result);
        IImageAnalysis Create(MediaItem mediaItem, Face[] facialAnalysis,
            OcrResults textAnalysis, AnalysisResult visionAnalysis);
        IImageAnalysis Create(MediaItem mediaItem, Face[] facialAnalysis, OcrResults textAnalysis,
            AnalysisResult visionAnalysis, List<string> validationErrors);
    }
}