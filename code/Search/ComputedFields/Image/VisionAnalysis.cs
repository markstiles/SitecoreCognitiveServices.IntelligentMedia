using System.Web.Script.Serialization;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class VisionAnalysis : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            if (Visions == null)
                return null;
            
            var json = new JavaScriptSerializer().Serialize(Visions);
            return json;
        }
    }
}