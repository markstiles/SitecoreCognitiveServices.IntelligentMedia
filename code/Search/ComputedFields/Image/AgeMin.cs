using System.Linq;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class AgeMin : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            return Faces?.Select(x => x.FaceAttributes.Age).OrderBy(a => a).FirstOrDefault() ?? 0d;
        }
    }
}