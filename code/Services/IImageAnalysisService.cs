using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services
{
    public interface IImageAnalysisService
    {
        IImageAnalysis AnalyzeImage(MediaItem imageItem, Face[] facialAnalysis, OcrResults textAnalysis, AnalysisResult visionAnalysis, bool updateInIndex = true, bool overwrite = false);
        int AnalyzeImagesRecursively(Item parentFolderItem, string db, bool overwrite);
        int AnalyzeImagesRecursively(List<ImageSearchResult> itemList, string db, bool overwrite);
        Face[] GetFacialAnalysis(MediaItem m);
        AnalysisResult GetVisualAnalysis(MediaItem m);
        OcrResults GetTextualAnalysis(MediaItem m);
        Item CreateAnalysisItem(Item imageItem, Dictionary<ID, string> fields);
    }
}