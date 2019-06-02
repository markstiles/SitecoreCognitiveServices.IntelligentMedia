using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Enums;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class Height: BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            MediaItem mediaItem = new MediaItem(cognitiveIndexable);

            if (string.IsNullOrEmpty(mediaItem.InnerItem["Height"]))
                return null;
            
            int height;
            if(int.TryParse(mediaItem.InnerItem["Height"], out height))
                return height;

            return null;
        }
    }
}