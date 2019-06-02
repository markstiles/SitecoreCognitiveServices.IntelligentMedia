namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models
{
    public class JobResult : IJobResult
    {
        public long Current { get; set; }
        public long Total { get; set; }
        public bool Completed { get; set; }
    }
}