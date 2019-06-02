namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Setup
{
    public interface ISetupInformation
    {        
        string FaceApiKey { get; set; }
        string FaceApiEndpoint { get; set; }
        string EmotionApiKey { get; set; }
        string EmotionApiEndpoint { get; set; }
        string ComputerVisionApiKey { get; set; }
        string ComputerVisionApiEndpoint { get; set; }
    }
}