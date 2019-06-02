using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.Data.Items;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class IsImage : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            return BaseTemplates.Any(a 
                => a.ID.Guid.Equals(TemplateIDs.UnversionedFile.Guid)
                || a.ID.Guid.Equals(TemplateIDs.VersionedFile.Guid));
        }
    }
}