
namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models
{
    public class ImageSearchJsonResult : IImageSearchJsonResult
    {
        public string Url { get; set; }
        public string Alt { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
    }
}