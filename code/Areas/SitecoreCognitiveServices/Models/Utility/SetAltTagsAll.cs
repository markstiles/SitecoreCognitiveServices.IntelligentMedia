
namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility
{
    public class SetAltTagsAll : ISetAltTagsAll
    {
        public int ItemCount { get; set; }
        public int ItemsModified { get; set; }
        public int Threshold { get; set; }
        public bool Overwrite { get; set; }
    }
}