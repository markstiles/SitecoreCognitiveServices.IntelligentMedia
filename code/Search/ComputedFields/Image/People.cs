using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields.Image
{
    public class People : BaseComputedField
    {
        protected override object GetFieldValue(Item cognitiveIndexable)
        {
            return People.AsEnumerable();
        }
    }
}