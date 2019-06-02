using System.Linq;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class AgeMax : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            var ages = Faces?.Select(x => x.FaceAttributes.Age).OrderByDescending(a => a).ToList();
            
            return ages != null && ages.Any()
                ? ages.FirstOrDefault()
                : 100d;
        }
    }
}