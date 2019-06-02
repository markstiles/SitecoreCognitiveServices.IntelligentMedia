using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;
using System.Collections.Generic;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search
{
    public interface IImageSearchResult
    {
        double AdultScore { get; set; }
        double AgeMin { get; set; }
        double AgeMax { get; set; }
        bool AllPeopleIdentified { get; set; }
        double AngerMin { get; set; }
        double AngerMax { get; set; }
        string[] Colors { get; set; }
        double ContemptMin { get; set; }
        double ContemptMax { get; set; }
        double DisgustMin { get; set; }
        double DisgustMax { get; set; }
        string FacialAnalysisValue { get; set; }
        Face[] FacialAnalysis { get; }
        double FearMin { get; set; }
        double FearMax { get; set; }
        int Gender { get; set; }
        List<int> Glasses { get; set; }
        double HappinessMin { get; set; }
        double HappinessMax { get; set; }
        bool IsImage { get; set; }
        ID ItemId { get; set; }
        string Language { get; set; }
        double NeutralMin { get; set; }
        double NeutralMax { get; set; }
        IEnumerable<ID> Paths { get; set; }
        string[] People { get; set; }
        double RacyScore { get; set; }
        double SadnessMin { get; set; }
        double SadnessMax { get; set; }
        double SurpriseMin { get; set; }
        double SurpriseMax { get; set; }
        int Height { get; set; }
        int Width { get; set; }
        string[] Tags { get; set; }
        ID TemplateId { get; set; }
        string TextAnalysisValue { get; set; }
        OcrResults TextAnalysis { get; }
        string UniqueId { get; set; }
        string VisionAnalysisValue { get; set; }
        AnalysisResult VisionAnalysis { get; }
        Item GetItem();
    }
}