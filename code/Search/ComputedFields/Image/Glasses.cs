using System.Collections.Generic;
using System.Linq;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class Glasses : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            if (Faces == null || Faces.Length == 0)
                return null;
            
            List<int> returnValue = Faces?.Select(x => (int) x.FaceAttributes.Glasses).ToList();
            return returnValue;
        }
    }
}