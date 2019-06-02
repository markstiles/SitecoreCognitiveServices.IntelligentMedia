using Sitecore.Collections;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.RenderField;
using Sitecore.Xml.Xsl;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Pipelines.RenderField
{
    public class GetImageFieldValue : Sitecore.Pipelines.RenderField.GetImageFieldValue
    {
        protected override bool IsImage(RenderFieldArgs args)
        {
            var type = args.FieldTypeKey;
            return type.Equals("image") || type.Equals("intelligent image");
        }
    }
}