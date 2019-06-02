using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Jobs;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Setup;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Factories;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK.Enums;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Vision;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Controllers
{
    public class IntelligentMediaController : Controller
    {
        #region Constructor

        protected readonly IIntelligentMediaSearchService SearchService;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IWebUtilWrapper WebUtil;
        protected readonly IImageSearchFactory MediaSearchFactory;
        protected readonly ISetAltTagsAllFactory SetAltTagsAllFactory;
        protected readonly IImageAnalysisService AnalysisService;
        protected readonly IJobResultFactory JobResultFactory;
        protected readonly ISetupInformationFactory SetupFactory;
        protected readonly ISetupService SetupService;
        protected readonly IIntelligentMediaSettings SearchSettings;
        protected readonly IFaceService FaceService;
        protected readonly IPersonGroupService PersonGroupService;
        protected readonly IComputerVisionService ComputerVisionService;
        protected readonly IMediaWrapper MediaWrapper;

        public IntelligentMediaController(
            IIntelligentMediaSearchService searchService,
            ISitecoreDataWrapper dataWrapper,
            IWebUtilWrapper webUtil,
            IImageSearchFactory msFactory,
            ISetAltTagsAllFactory satarFactory,
            IImageAnalysisService analysisService,
            IJobResultFactory jobResultFactory,
            ISetupInformationFactory setupFactory,
            ISetupService setupService,
            IIntelligentMediaSettings searchSettings,
            IFaceService faceService,
            IPersonGroupService personGroupService,
            IComputerVisionService computerVisionService,
            IMediaWrapper mediaWrapper)
        {
            Assert.IsNotNull(searchService, typeof(IIntelligentMediaSearchService));
            Assert.IsNotNull(dataWrapper, typeof(ISitecoreDataWrapper));
            Assert.IsNotNull(webUtil, typeof(IWebUtilWrapper));
            Assert.IsNotNull(msFactory, typeof(IImageSearchFactory));
            Assert.IsNotNull(satarFactory, typeof(ISetAltTagsAllFactory));
            Assert.IsNotNull(analysisService, typeof(IImageAnalysisService));
            Assert.IsNotNull(jobResultFactory, typeof(IJobResultFactory));
            Assert.IsNotNull(setupFactory, typeof(ISetupInformationFactory));
            Assert.IsNotNull(setupService, typeof(ISetupService));
            Assert.IsNotNull(searchSettings, typeof(IIntelligentMediaSettings));
            Assert.IsNotNull(faceService, typeof(IFaceService));
            Assert.IsNotNull(personGroupService, typeof(IPersonGroupService));
            Assert.IsNotNull(computerVisionService, typeof(IComputerVisionService));
            Assert.IsNotNull(mediaWrapper, typeof(IMediaWrapper));
            
            SearchService = searchService;
            DataWrapper = dataWrapper;
            WebUtil = webUtil;
            MediaSearchFactory = msFactory;
            SetAltTagsAllFactory = satarFactory;
            AnalysisService = analysisService;
            JobResultFactory = jobResultFactory;
            SetupFactory = setupFactory;
            SetupService = setupService;
            SearchSettings = searchSettings;
            FaceService = faceService;
            PersonGroupService = personGroupService;
            ComputerVisionService = computerVisionService;
            MediaWrapper = mediaWrapper;
        }

        #endregion

        #region Image Search

        public ActionResult SearchForm()
        {
            if (!IsSitecoreUser())
                return LoginPage();

            var lang = WebUtil.GetQueryString("lang");
            var db = WebUtil.GetQueryString("db", "master");
            var fieldSrc = WebUtil.GetQueryString("fieldSrc");

            var ms = MediaSearchFactory.Create(db, lang, fieldSrc);

            return View("SearchForm", ms);
        }

        public ActionResult RTESearchQuery(
            Dictionary<string, string[]> tagParameters,
            Dictionary<string, string[]> rangeParameters,
            int gender,
            int glasses,
            string language,
            List<string> colors,
            string db,
            string fieldSourceId,
            int page,
            int pageLength)
        {
            if (!IsSitecoreUser())
                return new EmptyResult();

            var sourceId = string.IsNullOrWhiteSpace(fieldSourceId) ? ID.Null : new ID(fieldSourceId);
            var skipCount = (page - 1) * pageLength;

            Tuple<int, List<IImageSearchResult>> csr = SearchService.GetFilteredSearchResults(
                tagParameters,
                rangeParameters,
                gender,
                glasses,
                language,
                colors,
                db,
                sourceId,
                pageLength,
                skipCount);
            
            return Json(new
            {
                Results = csr.Item2.Select(r => MediaSearchFactory.CreateMediaSearchJsonResult(r)),
                ResultCount = csr.Item1
            });
        }

        #endregion Image Search

        #region Set Alt Tags

        public ActionResult ViewImageDescription(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            Item item = DataWrapper.GetItemByIdValue(id, db);
            if (item == null)
                return View("ImageDescription");

            MediaItem m = item;

            var desc = SearchService.GetImageDescription(m, language);

            return View("ImageDescription", desc);
        }

        [HttpPost]
        public ActionResult UpdateImageDescription(string descriptionOption, string id, string db,
            string language)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            if (string.IsNullOrEmpty(descriptionOption))
                return View("ImageDescription");

            Item item = DataWrapper.GetItemByIdValue(id, db);
            if (item == null)
                return View("ImageDescription");

            MediaItem m = item;
            MediaWrapper.SetImageAlt(m, descriptionOption);

            return View("ImageDescription", SearchService.GetImageDescription(m, language));
        }

        public ActionResult ViewImageDescriptionThreshold(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            var result = SetAltTagsAllFactory.Create(0, 0, 50, false);

            return View("ImageDescriptionThreshold", result);
        }

        [HttpPost]
        public ActionResult UpdateImageDescriptionAll(string id, string language, string db, int threshold,
            bool overwrite)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            IEnumerable<MediaItem> fullList = MediaWrapper
                .GetMediaFileDescendents(id, db)
                .ToList();

            IEnumerable<MediaItem> list = fullList
                .Where(a => string.IsNullOrEmpty(a.Alt) || overwrite)
                .ToList();

            if (!list.Any())
                return Json(SetAltTagsAllFactory.Create(fullList.Count(), 0, 50, false));

            int updatedCount = 0;
            var thresholdDouble = (double) threshold / 100;
            foreach (var m in list)
            {
                Caption cap = SearchService.GetTopImageCaption(m, language, thresholdDouble);
                if (cap == null || cap.Confidence < thresholdDouble)
                    continue;

                IImageSearchResult csr = SearchService.GetSearchResult(m.ID.ToString(), language, db);
                var newDescription = csr?.People != null && csr.People.Any()
                    ? $"{string.Join(", ", csr.People)} - {cap.Text}"
                    : cap.Text;

                MediaWrapper.SetImageAlt(m, newDescription);
                updatedCount++;
            }

            var result = SetAltTagsAllFactory.Create(fullList.Count(), updatedCount, threshold,
                overwrite);

            return Json(result);
        }

        #endregion Set Alt Tags

        #region Analysis

        public ActionResult ImageAnalysis(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            IImageAnalysis cia = SearchService.GetImageAnalysis(id, language, db);

            return View("ImageAnalysis", cia);
        }

        public ActionResult Analyze(string id, string language, string db, bool overwrite = false)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            var result = SearchService.GetSearchResult(id, language, db);
            var item = DataWrapper.GetItemByIdValue(id, db);

            var analysis = AnalysisService.AnalyzeImage(item, result.FacialAnalysis, result.TextAnalysis, result.VisionAnalysis, true, overwrite);

            return View("ImageAnalysis", analysis);
        }

        public ActionResult ViewAnalyzeAll(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();
            
            return View("AnalyzeAll");
        }

        public ActionResult AnalyzeAll(string id, string language, string db, bool overwrite)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            Item item = DataWrapper.GetItemByIdValue(id, db);
            if (item == null)
                return ViewAnalyzeAll(id, language, db);

            string handleName = $"BatchImageAnalysis{new Random(DateTime.Now.Millisecond).Next(0, 100)}";

            var jobOptions = new JobOptions(
                handleName,
                "Cognitive Image Analysis",
                Sitecore.Context.Site.Name,
                AnalysisService,
                "AnalyzeImagesRecursively",
                new object[] {item, db, overwrite});

            JobManager.Start(jobOptions);
            
            return Json(handleName);
        }
        
        #endregion Analysis

        #region People

        public ActionResult TrainGroup(string groupId)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            FaceService.TrainPersonGroup(groupId);

            return View("TrainGroup", model: groupId);
        }

        public ActionResult TrainAllGroups(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            Dictionary<Guid, string> groupIdSet = PersonGroupService.GetAllGroupItems(db).ToDictionary(a => a.ID.Guid, b => b[SearchSettings.FaceEntityIdFieldId]);
            if (!groupIdSet.Any())
                return View("TrainAllGroups");

            groupIdSet.ForEach(gid => FaceService.TrainPersonGroup(gid.Value));

            return View("TrainAllGroups", model: JsonConvert.SerializeObject(groupIdSet));
        }

        public ActionResult TrainGroupStatus(string id, string db, string groupId)
        {
            var result = FaceService.GetPersonGroupTrainingStatus(groupId);
            if (result == null)
                return Json(new { status = "failed", message = "bad group id" });

            if (result.Status == Status.Succeeded || result.Status == Status.Failed)
                PersonGroupService.SetPersonGroupTrainingResult(new ID(id), db, result);

            return Json(new
            {
                status = result.Status.ToString(),
                message = result.Message
            });
        }

        public ActionResult IdentifyPeople(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            MediaItem item = DataWrapper.GetItemByIdValue(id, db);
            List<string> groupIds = PersonGroupService.GetAllGroupIds(db);
            PersonGroupService.DetectAndIdentifyPeople(item, db, groupIds);
            SearchService.UpdateItemInIndex(item, db);

            IImageAnalysis cia = SearchService.GetImageAnalysis(id, language, db);
            
            return View("ImageAnalysis", cia);
        }

        public ActionResult IdentifyGroup(string id, string language, string db, string groupId)
        {
            if (!IsSitecoreUser())
                return LoginPage();
            
            string handleName = $"BatchPersonIdentification{new Random(DateTime.Now.Millisecond).Next(0, 100)}";
            
            var jobOptions = new JobOptions(
                handleName,
                "Identify Group",
                Sitecore.Context.Site.Name,
                PersonGroupService,
                "IdentifyGroup",
                new object[] { language, db, groupId });

            JobManager.Start(jobOptions);

            return View("IdentifyGroup", model: handleName);
        }

        public ActionResult IdentifyAllGroups(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();
            
            string handleName = $"BatchPersonGroupIdentification{new Random(DateTime.Now.Millisecond).Next(0, 100)}";
            
            var jobOptions = new JobOptions(
                handleName,
                "Identify All Groups",
                Sitecore.Context.Site.Name,
                PersonGroupService,
                "IdentifyAllGroups",
                new object[] { language, db });

            JobManager.Start(jobOptions);

            return View("IdentifyAllGroups", model: handleName);
        }

        public ActionResult AddFace(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            var obj = "";

            var viewModel = new Dictionary<string, Dictionary<string, Guid>>();
            var people = PersonGroupService.GetAllPersonItems(db);

            foreach (var p in people)
            {
                var groupName = p.Parent[SearchSettings.FaceEntityNameFieldId];
                if(!viewModel.ContainsKey(groupName))
                    viewModel.Add(groupName, new Dictionary<string, Guid>());

                var personName = p[SearchSettings.FaceEntityNameFieldId];
                if(!viewModel[groupName].ContainsKey(personName))
                    viewModel[groupName].Add(personName, p.ID.Guid);
            }

            return View("AddFace", model: viewModel);
        }

        public ActionResult AddFaceSubmit(string id, string db, Guid personId)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            var personItem = DataWrapper.GetItemById(new ID(personId), db);
            if (personItem == null)
                return Json(new {Failed = true});

            DataWrapper.CreateItem(personItem.ID, SearchSettings.PersonFaceTemplateId, db, $"{personItem.GetChildren().Count + 1}", new Dictionary<ID, string>()
            {
                { SearchSettings.FaceImageFieldId, $"<image mediaid=\"{id}\" />" }
            });
            
            return Json(new { Failed = false });
        }

        #endregion

        #region Smart Crop

        public ActionResult SmartCrop(string id, string language, string db)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            Item m = DataWrapper.GetItemByIdValue(id, db);

            var height = MediaWrapper.GetImageHeight(m);
            var width = MediaWrapper.GetImageWidth(m);

            var model = new SmartCrop()
            {
                Height = height,
                Width = width
            };

            return View("SmartCrop", model);
        }

        public ActionResult SmartCropSubmit(string id, string db, int height, int width)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            MediaItem m = DataWrapper.GetItemByIdValue(id, db);

            var validationErrors = ComputerVisionService.ValidateVisionImage(m);
            if (validationErrors.Any())
                return Json(new
                {
                    Failed = true,
                    FileName = "",
                    Messages = validationErrors,
                });

            var result = ComputerVisionService.GetThumbnail(m.GetMediaStream(), width, height);
            string fileName = $@"temp/SmartCrop-{new Random(DateTime.Now.Millisecond).Next(0, 1000000)}.{m.Extension}";
            string filePath = MediaWrapper.GetFilePath(Request, fileName);
            var succeeded = MediaWrapper.WriteBytesToFile(result, filePath);
            
            return Json(new
            {
                Failed = !succeeded,
                FileName = fileName,
                Messages = new string[0]
            });
        }
        
        public ActionResult SmartCropSave(string id, string db, string fileName, int width, int height)
        {
            if (!IsSitecoreUser())
                return LoginPage();

            var item = DataWrapper.GetItemByIdValue(id, db);

            var filePath = MediaWrapper.GetFilePath(Request, fileName);

            var mediaItem = MediaWrapper.UploadImageFile(filePath, $"{item.Paths.FullPath}/{width} x {height}", db);
            
            var failed = mediaItem == null;

            return Json(new {
                Failed = failed
            });
        }

        #endregion

        #region Setup

        public ActionResult Setup()
        {
            if (!IsSitecoreUser())
                return LoginPage();
            
            var db = Sitecore.Configuration.Factory.GetDatabase(SearchSettings.MasterDatabase);
            using (new DatabaseSwitcher(db))
            {
                ISetupInformation info = SetupFactory.Create();

                return View("Setup", info);
            }
        }

        public ActionResult SetupSubmit(string indexOption, string faceApi, string faceApiEndpoint, string computerVisionApi, string computerVisionApiEndpoint)
        {
            if (!IsSitecoreUser())
                return LoginPage();
            
            IImageAnalysis analysis = SetupService.SaveKeysAndAnalyze(faceApi, faceApiEndpoint, computerVisionApi, computerVisionApiEndpoint);
            var items = new List<string>();
            if (analysis == null || analysis.FacialAnalysis?.Length < 1)
                items.Add("Face API");
            if (analysis?.TextAnalysis?.Regions == null || analysis?.VisionAnalysis?.Description == null)
                items.Add("Computer Vision API");

            string err = SetupService.SetFieldsFolderTemplate();
            if(!string.IsNullOrEmpty(err))
                items.Add(err);

            SetupService.PublishContent();

            if (!indexOption.Equals("Skip"))
                SetupService.ConfigureIndexes(indexOption);
            
            return Json(new
            {
                Failed = (analysis == null || items.Count > 0),
                Items = string.Join(",", items)
            });
        }

        public ActionResult ReindexSubmit()
        {
            SetupService.ReindexAndResetDictionary();
            SetupService.ReindexAndResetDictionary();

            return Json(new
            {
                Failed = false
            });
        }

        #endregion

        #region Shared

        public ActionResult GetJobStatus(string handleName)
        {
            Job j = JobManager.GetJob(handleName);

            var result = JobResultFactory.Create(j?.Status.Processed ?? 0, j?.Status.Total ?? 0, j?.IsDone ?? true);

            return Json(result);
        }

        public bool IsSitecoreUser()
        {
            return DataWrapper.ContextUser.IsAuthenticated 
                && DataWrapper.ContextUser.Domain.Name.ToLower().Equals("sitecore");
        }

        public ActionResult LoginPage()
        {
            return new RedirectResult("/sitecore/login");
        }

        #endregion
    }
}
 