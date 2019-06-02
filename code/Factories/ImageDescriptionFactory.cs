using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public class ImageDescriptionFactory : IImageDescriptionFactory
    {
        protected readonly IServiceProvider Provider;

        public ImageDescriptionFactory(IServiceProvider provider)
        {
            Provider = provider;
        }

        public virtual IImageDescription Create()
        {
            var obj = Provider.GetService<IImageDescription>();

            obj.Descriptions = new List<string>();
            obj.AltDescription = string.Empty;

            return obj;
        }

        public virtual IImageDescription Create(List<string> descriptions, string altDescription)
        {
            var obj = Create();

            obj.Descriptions = descriptions;
            obj.AltDescription = altDescription;

            return obj;
        }
    }
}