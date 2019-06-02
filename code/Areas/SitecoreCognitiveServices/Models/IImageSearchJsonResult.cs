namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models
{
    public interface IImageSearchJsonResult
    {
        string Url { get; set; }
        string Alt { get; set; }
        string Id { get; set; }
        string Title { get; set; }
    }
}