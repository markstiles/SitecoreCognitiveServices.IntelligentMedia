using System.Linq;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image.Emotions
{
    public class SurpriseMin : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            var values = Faces?.Select(x => x.FaceAttributes?.Emotion?.Surprise).OrderBy(a => a).ToList();

            return values != null && values.Any()
                ? values.FirstOrDefault()
                : 0f;
        }
    }
}