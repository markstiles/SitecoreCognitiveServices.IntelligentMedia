using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Enums;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class Width: BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            MediaItem mediaItem = new MediaItem(cognitiveIndexable);

            if (string.IsNullOrEmpty(mediaItem.InnerItem["Width"]))
                return null;
            
            int width;
            if (int.TryParse(mediaItem.InnerItem["Width"], out width))
                return width;

            return null;
        }
    }
}