using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Diagnostics.PerformanceCounters;
using Sitecore.Events;
using Sitecore.Jobs;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.MediaItem
{
    public class MediaItemSaved
    {
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IImageAnalysisService AnalysisService;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentMediaSearchService SearchService;
        protected readonly IContextItemsWrapper ContextItems;
        protected readonly IMediaWrapper MediaWrapper;

        public MediaItemSaved(
            IIntelligentMediaSettings settings,
            IImageAnalysisService analysisService,
            ISitecoreDataWrapper dataWrapper,
            IIntelligentMediaSearchService searchService,
            IContextItemsWrapper contextItems,
            IMediaWrapper mediaWrapper)
        {
            Settings = settings;
            AnalysisService = analysisService;
            DataWrapper = dataWrapper;
            SearchService = searchService;
            ContextItems = contextItems;
            MediaWrapper = mediaWrapper;
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            if (!Settings.AnalyzeNewImageField)
                return;

            var item = Event.ExtractParameter(args, 0) as Item;
            if (item == null || !MediaWrapper.IsMediaFile(item))
                return;

            string analysisKey = $"ImageAnalysis-{item.ID}";
            if (ContextItems.KeyExists(analysisKey))
                return;
                
            var stream = ((Sitecore.Data.Items.MediaItem) item).GetMediaStream();
            if (stream == null)
                return;

            ContextItems.Add(analysisKey, analysisKey);

            string handleName = $"ImageCreatedAnalysis{new Random(DateTime.Now.Millisecond).Next(0, 100)}";

            var jobOptions = new JobOptions(
                handleName,
                "Cognitive Image Analysis",
                Sitecore.Context.Site.Name,
                this,
                "AnalyzeUploadedImages",
                new object[] { item });

            JobManager.Start(jobOptions);
        }
        
        public void AnalyzeUploadedImages(Item image)
        {
            AnalysisService.AnalyzeImage(image, null, null, null);
        }
    }
}