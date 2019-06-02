using System.Web.Script.Serialization;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class FacialAnalysis : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            if (Faces == null)
                return null;
            
            var json = new JavaScriptSerializer().Serialize(Faces);
            return json;
        }
    }
}