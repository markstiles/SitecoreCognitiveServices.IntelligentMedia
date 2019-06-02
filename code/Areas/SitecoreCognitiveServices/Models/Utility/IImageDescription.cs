
using System.Collections.Generic;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility
{
    public interface IImageDescription
    {
        List<string> Descriptions { get; set; }
        string AltDescription { get; set; }
    }
}