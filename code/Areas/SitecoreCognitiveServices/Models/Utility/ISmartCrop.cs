
namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility
{
    public interface ISmartCrop
    {
        int Height { get; set; }
        int Width { get; set; }
        float AspectRatio { get; }
    }
}