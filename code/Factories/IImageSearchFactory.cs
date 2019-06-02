using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public interface IImageSearchFactory
    {
        IImageSearch Create();
        IImageSearch Create(string db, string language, string fieldSource);
        IImageSearchJsonResult CreateMediaSearchJsonResult(IImageSearchResult searchResult);
    }
}