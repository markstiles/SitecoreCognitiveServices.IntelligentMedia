namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models
{
    public interface IJobResult
    {
        long Current { get; set; }
        long Total { get; set; }
        bool Completed { get; set; }
    }
}