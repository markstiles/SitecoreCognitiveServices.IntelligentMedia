using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Factories;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using Sitecore.DependencyInjection;
using System.Web.Mvc;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Controllers;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Setup;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.App_Start
{
    public class IocConfig : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            //system
            serviceCollection.AddTransient<IIntelligentMediaSettings, IntelligentMediaSettings>();

            //factory models
            serviceCollection.AddTransient<IImageSearch, ImageSearch>();
            serviceCollection.AddTransient<IImageSearchJsonResult, ImageSearchJsonResult>();
            serviceCollection.AddTransient<IImageAnalysis, ImageAnalysis>();
            serviceCollection.AddTransient<IImageDescription, ImageDescription>();
            serviceCollection.AddTransient<IAnalyzeAll, AnalyzeAll>();
            serviceCollection.AddTransient<ISetAltTagsAll, SetAltTagsAll>();
            serviceCollection.AddTransient<IJobResult, JobResult>();
            serviceCollection.AddTransient<ISetupInformation, SetupInformation>();

            //factories
            serviceCollection.AddTransient<IImageSearchFactory, ImageSearchFactory>();
            serviceCollection.AddTransient<IImageAnalysisFactory, ImageAnalysisFactory>();
            serviceCollection.AddTransient<IImageDescriptionFactory, ImageDescriptionFactory>();
            serviceCollection.AddTransient<ISetAltTagsAllFactory, SetAltTagsAllFactory>();
            serviceCollection.AddTransient<IJobResultFactory, JobResultFactory>();
            serviceCollection.AddTransient<ISetupInformationFactory, SetupInformationFactory>();

            //search
            serviceCollection.AddTransient<IImageSearchResult, ImageSearchResult>();
            serviceCollection.AddTransient<IIntelligentMediaSearchService, IntelligentMediaSearchService>();
            
            //services
            serviceCollection.AddTransient<IImageAnalysisService, ImageAnalysisService>();
            serviceCollection.AddTransient<IPersonGroupService, PersonGroupService>();
            
            //setup
            serviceCollection.AddTransient<ISetupService, SetupService>();

            //controllers
            serviceCollection.AddTransient(typeof(IntelligentMediaController));
        }
    }
}