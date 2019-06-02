
using System.Collections.Generic;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models
{
    public class ImageSearch : IImageSearch
    {
        public string Database { get; set; }
        public string Language { get; set; }
        public string FieldSourceId { get; set; }
        public List<KeyValuePair<string, int>> Tags { get; set; }
        public List<KeyValuePair<string, string>> Colors { get; set; }
    }
}