using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using NSubstitute;
using NUnit.Framework;
using Sitecore.Security.Accounts;
using Sitecore.Security.Domains;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Factories;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Tests.Controllers
{
    [TestFixture]
    public class CognitiveImageSearchControllerTests
    {
        protected IIntelligentMediaSearchService SearchService;
        protected ISitecoreDataWrapper DataWrapper;
        protected IWebUtilWrapper WebUtil;
        protected IImageSearchFactory MediaSearchFactory;
        protected ISetAltTagsAllFactory SetAltTagsAllFactory;
        protected IImageAnalysisFactory ImageAnalysisFactory;
        protected IImageAnalysisService ImageAnalysisService;
        protected IJobResultFactory JobResultFactory;
        protected ISetupInformationFactory SetupInformationFactory;
        protected IIntelligentMediaSettings Settings;
        protected ISetupService SetupService;

        [SetUp]
        public void Setup()
        {
            SearchService = Substitute.For<IIntelligentMediaSearchService>();
            DataWrapper = Substitute.For<ISitecoreDataWrapper>();
            WebUtil = Substitute.For<IWebUtilWrapper>();
            MediaSearchFactory = Substitute.For<IImageSearchFactory>();
            SetAltTagsAllFactory = Substitute.For<ISetAltTagsAllFactory>();
            ImageAnalysisFactory = Substitute.For<IImageAnalysisFactory>();
            ImageAnalysisService = Substitute.For<IImageAnalysisService>();
            JobResultFactory = Substitute.For<IJobResultFactory>();
            SetupInformationFactory = Substitute.For<ISetupInformationFactory>();
            Settings = Substitute.For<IIntelligentMediaSettings>();
            SetupService = Substitute.For<ISetupService>();

            User u = User.FromName("sitecore\\name", true);
            DataWrapper.ContextUser.Returns(u);
        }

        //[Test]
        //public void Constructor_NullParameters_Throws()
        //{
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(null, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, null, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, null, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, null, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, null, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, null, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, null, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, null, SetupInformationFactory, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, null, SetupService, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, null, ImageSearchSettings));
        //    Assert.Throws<InvalidOperationException>(() => new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, null));
        //}

        //[Test]
        //public void ID_Empty_Returns_NullModel()
        //{
        //    //arrange
        //    CognitiveImageSearchController controller = new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings);
            
        //    //act
        //    var result = controller.Analyze(string.Empty, "en", "master") as ViewResult;

        //    //assert
        //    Assert.IsNotNull(result);
        //    Assert.IsNull(result.Model);
        //}

        //[Test]
        //public void ValidID_Returns_NoChoices()
        //{
        //    //arrange
        //    CognitiveImageSearchController controller = new CognitiveImageSearchController(SearchService, DataWrapper, WebUtil, MediaSearchFactory, SetAltTagsAllFactory, AnalyzeAllFactory, ImageAnalysisService, JobResultFactory, SetupInformationFactory, SetupService, ImageSearchSettings);

        //    //act
        //    var result = controller.Analyze(string.Empty, "en", "master") as ViewResult;

        //    //assert
        //    Assert.IsNotNull(result);
        //    Assert.IsNull(result.Model);
        //}
    }
}
