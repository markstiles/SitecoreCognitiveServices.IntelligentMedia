
namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility
{
    public class SmartCrop : ISmartCrop
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public float AspectRatio => (float) Width / Height;
    }
}