using System.Collections.Generic;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility
{
    public class ImageDescription : IImageDescription
    {
        public List<string> Descriptions { get; set; }
        public string AltDescription { get; set; }
    }
}