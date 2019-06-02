using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.MediaItem
{
    public class MediaItemDeleted
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentMediaSearchService SearchService;
        protected readonly IMediaWrapper MediaWrapper;

        public MediaItemDeleted(
            ISitecoreDataWrapper dataWrapper,
            IIntelligentMediaSearchService searchService,
            IMediaWrapper mediaWrapper)
        {
            DataWrapper = dataWrapper;
            SearchService = searchService;
            MediaWrapper = mediaWrapper;
        }

        public void OnItemDeleted(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            Assert.IsNotNull(item, "No item in parameters");

            if (!MediaWrapper.IsMediaFile(item))
                return;

            var analysis = SearchService.GetImageAnalysisItem(item.ID.ToShortID().ToString(), item.Language.Name, item.Database.Name);

            analysis?.Delete();
        }
    }
}