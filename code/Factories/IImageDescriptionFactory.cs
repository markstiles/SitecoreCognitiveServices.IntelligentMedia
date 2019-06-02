using System.Collections.Generic;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public interface IImageDescriptionFactory
    {
        IImageDescription Create();
        IImageDescription Create(List<string> descriptions, string altDescription);
    }
}