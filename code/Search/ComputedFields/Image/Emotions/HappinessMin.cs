using System.Linq;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image.Emotions
{
    public class HappinessMin : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            var values = Faces?.Select(x => x.FaceAttributes?.Emotion?.Happiness).OrderBy(a => a).ToList();

            return values != null && values.Any()
                ? values.FirstOrDefault()
                : 0f;
        }
    }
}