using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public class ImageAnalysisFactory : IImageAnalysisFactory
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IServiceProvider Provider;
        protected readonly IMediaWrapper MediaWrapper;


        public ImageAnalysisFactory(
            ISitecoreDataWrapper dataWrapper,
            IServiceProvider provider,
            IMediaWrapper mediaWrapper)
        {
            DataWrapper = dataWrapper;
            Provider = provider;
            MediaWrapper = mediaWrapper;
        }

        public virtual IImageAnalysis Create()
        {
            return Provider.GetService<IImageAnalysis>();
        }

        public virtual IImageAnalysis Create(IImageSearchResult result)
        {
            var analysis = Create();
            if (result == null)
                return analysis;
            
            analysis.FacialAnalysis = result.FacialAnalysis;
            analysis.TextAnalysis = result.TextAnalysis;
            analysis.VisionAnalysis = result.VisionAnalysis;
            analysis.People = result.People;

            Item i = DataWrapper.GetItemByUri(result?.UniqueId ?? string.Empty);
            if (i == null)
                return analysis;

            analysis.ImageHeight = MediaWrapper.GetImageHeight(i);
            analysis.ImageWidth = MediaWrapper.GetImageWidth(i);
            analysis.ImageUrl = $"/sitecore/shell/Applications/-/media/{i.ID.Guid:N}.ashx";

            return analysis;
        }

        public virtual IImageAnalysis Create(MediaItem mediaItem, Face[] facialAnalysis, OcrResults textAnalysis, AnalysisResult visionAnalysis)
        {
            var analysis = Create();
            
            analysis.FacialAnalysis = facialAnalysis ?? new Face[0];
            analysis.TextAnalysis = textAnalysis ?? new OcrResults();
            analysis.VisionAnalysis = visionAnalysis ?? new AnalysisResult();

            if (mediaItem == null)
                return analysis;

            analysis.ImageHeight = MediaWrapper.GetImageHeight(mediaItem);
            analysis.ImageWidth = MediaWrapper.GetImageWidth(mediaItem);
            analysis.ImageUrl = $"/sitecore/shell/Applications/-/media/{mediaItem.ID.Guid:N}.ashx";

            return analysis;
        }

        public virtual IImageAnalysis Create(MediaItem mediaItem, Face[] facialAnalysis, OcrResults textAnalysis, AnalysisResult visionAnalysis, List<string> validationErrors)
        {
            var analysis = Create(mediaItem, facialAnalysis, textAnalysis, visionAnalysis);
            analysis.ValidationErrors = validationErrors;

            return analysis;
        }
    }
}