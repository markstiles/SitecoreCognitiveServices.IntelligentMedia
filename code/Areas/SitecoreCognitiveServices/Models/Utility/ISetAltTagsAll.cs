
namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility
{
    public interface ISetAltTagsAll
    {
        int ItemCount { get; set; }
        int ItemsModified { get; set; }
        int Threshold { get; set; }
        bool Overwrite { get; set; }
    }
}