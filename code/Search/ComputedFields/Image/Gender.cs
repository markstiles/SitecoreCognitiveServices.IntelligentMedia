using System.Linq;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class Gender : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            if (Faces == null || Faces.Length == 0)
                return null;
            
            if (Faces.Any(x => x.FaceAttributes.Gender.Equals("male")) 
                && Faces.Any(x => x.FaceAttributes.Gender.Equals("female")))
                return 2;
            
            if (Faces.Any(x => x.FaceAttributes.Gender.Equals("male")))
                return 3;
            
            if (Faces.Any(x => x.FaceAttributes.Gender.Equals("female")))
                return 4;
            
            return 1;
        }
    }
}