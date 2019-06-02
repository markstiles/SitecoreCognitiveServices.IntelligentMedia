
using System.Collections.Generic;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models
{
    public interface IImageSearch
    {
        string Database { get; set; }
        string Language { get; set; }
        string FieldSourceId { get; set; }

        List<KeyValuePair<string, int>> Tags { get; set; }
        List<KeyValuePair<string, string>> Colors { get; set; }
    }
}