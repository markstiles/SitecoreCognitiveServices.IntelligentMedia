namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Setup
{
    public class SetupInformation : ISetupInformation
    {
        public string FaceApiKey { get; set; }
        public string FaceApiEndpoint { get; set; }
        public string EmotionApiKey { get; set; }
        public string EmotionApiEndpoint { get; set; }
        public string ComputerVisionApiKey { get; set; }
        public string ComputerVisionApiEndpoint { get; set; }
    }
}