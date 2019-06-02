using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Data.Items;
using Sitecore.Shell.Framework.Commands;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    public class BaseIntelligentMediaCommand : BaseCommand
    {
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IIntelligentMediaSearchService SearchService;
        protected readonly IMediaWrapper MediaWrapper;
        protected readonly IPersonGroupService PersonGroupService;

        public BaseIntelligentMediaCommand()
        {
            Settings = DependencyResolver.Current.GetService<IIntelligentMediaSettings>();
            SearchService = DependencyResolver.Current.GetService<IIntelligentMediaSearchService>();
            MediaWrapper = DependencyResolver.Current.GetService<IMediaWrapper>();
            PersonGroupService = DependencyResolver.Current.GetService<IPersonGroupService>();
        }
    }
}