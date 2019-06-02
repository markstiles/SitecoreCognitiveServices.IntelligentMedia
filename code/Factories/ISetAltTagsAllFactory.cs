using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public interface ISetAltTagsAllFactory
    {
        ISetAltTagsAll Create(int itemCount, int itemsModified, int threshold, bool overwrite);
    }
}