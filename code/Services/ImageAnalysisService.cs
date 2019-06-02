using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Jobs;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Factories;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK.Enums;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Vision;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services
{
    public class ImageAnalysisService : IImageAnalysisService
    {
        #region Constructor

        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IIntelligentMediaSearchService SearchService;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IFaceService FaceService;
        protected readonly IComputerVisionService VisionService;
        protected readonly IImageAnalysisFactory ImageAnalysisFactory;
        protected readonly IPersonGroupService PersonService;

        public ImageAnalysisService(
            IIntelligentMediaSettings settings,
            IIntelligentMediaSearchService searchService,
            ISitecoreDataWrapper dataWrapper,
            IFaceService faceService,
            IComputerVisionService visionService,
            IImageAnalysisFactory imageAnalysisFactory,
            IPersonGroupService personService)
        {
            Settings = settings;
            SearchService = searchService;
            DataWrapper = dataWrapper;
            FaceService = faceService;
            VisionService = visionService;
            ImageAnalysisFactory = imageAnalysisFactory;
            PersonService = personService;
        }

        #endregion
        
        public IImageAnalysis AnalyzeImage(MediaItem imageItem, Face[] facialAnalysis, OcrResults textAnalysis, AnalysisResult visionAnalysis, bool updateInIndex = true, bool overwrite = false)
        {
            var validationErrors = VisionService.ValidateVisionImage(imageItem);
            validationErrors.AddRange(FaceService.ValidateFaceImage(imageItem));

            var imageAnalysis = ImageAnalysisFactory.Create(imageItem, facialAnalysis, textAnalysis, visionAnalysis, validationErrors);
            var isNewAnalysis = false;
            if (overwrite || facialAnalysis == null || facialAnalysis.Length == 0) { 
                imageAnalysis.FacialAnalysis = GetFacialAnalysis(imageItem);
                isNewAnalysis = true;
            }
            if (overwrite || textAnalysis?.Regions == null)
                imageAnalysis.TextAnalysis = GetTextualAnalysis(imageItem);
            if(overwrite || visionAnalysis?.Description == null)
                imageAnalysis.VisionAnalysis = GetVisualAnalysis(imageItem);

            Dictionary<ID, string> fields = new Dictionary<ID, string>
            {
                { Settings.VisualAnalysisFieldId, JsonConvert.SerializeObject(imageAnalysis.VisionAnalysis) },
                { Settings.TextualAnalysisFieldId, JsonConvert.SerializeObject(imageAnalysis.TextAnalysis) },
                { Settings.FacialAnalysisFieldId, JsonConvert.SerializeObject(imageAnalysis.FacialAnalysis) },
                { Settings.AnalyzedImageFieldId, imageItem.ID.ToString() }
            };

            var analysisItem = SearchService.GetImageAnalysisItem(imageItem.ID.ToShortID().ToString(), imageItem.InnerItem.Language.Name, imageItem.Database.Name);
            if (analysisItem != null)
                DataWrapper.UpdateFields(analysisItem, fields);
            else
                analysisItem = CreateAnalysisItem(imageItem, fields);
            
            if (analysisItem == null)
                return imageAnalysis;
            
            var faceIds = imageAnalysis.FacialAnalysis?.Select(a => a.FaceId).ToArray() ?? new Guid[0];
            var peopleDiffer = imageAnalysis.People == null || imageAnalysis.People.Length != faceIds.Length;
            if (faceIds.Any() && (overwrite || peopleDiffer))
            {
                if(isNewAnalysis)
                    PersonService.IdentifyPeople(analysisItem, faceIds, analysisItem.Database.Name, PersonService.GetAllGroupIds(analysisItem.Database.Name));
                else
                    PersonService.DetectAndIdentifyPeople(imageItem, analysisItem.Database.Name, PersonService.GetAllGroupIds(analysisItem.Database.Name));
            }
            if (updateInIndex)
                SearchService.UpdateItemInIndex(imageItem, imageItem.Database.Name);

            return imageAnalysis;
        }

        public virtual int AnalyzeImagesRecursively(Item parentFolderItem, string db, bool overwrite)
        {
            var list = SearchService.GetMediaItems(
                parentFolderItem.ID,
                parentFolderItem.Language.Name,
                parentFolderItem.Database.Name);

            return AnalyzeImagesRecursively(list, db, overwrite);
        }

        public virtual int AnalyzeImagesRecursively(List<ImageSearchResult> itemList, string db, bool overwrite)
        {
            long line = 0;

            DataWrapper.SetJobPriority(ThreadPriority.Highest);
            DataWrapper.SetJobTotal(itemList.Count);
            
            foreach (ImageSearchResult i in itemList)
            {
                line++;
                var analysis = ImageAnalysisFactory.Create(i);
                if (overwrite || analysis == null || analysis.HasNoAnalysis())
                {
                    AnalyzeImage(i.GetItem(), i.FacialAnalysis, i.TextAnalysis, i.VisionAnalysis);
                }

                DataWrapper.SetJobStatus(line);
            }

            DataWrapper.SetJobState(JobState.Finished);

            return itemList.Count;
        }

        #region API Calls
        
        public virtual Face[] GetFacialAnalysis(MediaItem m)
        {
            if(FaceService.ValidateFaceImage(m).Any())
                return new Face[0];

            return FaceService.Detect(m.GetMediaStream(), true, true, new List<FaceAttributeType>()
            {
                FaceAttributeType.Age,
                FaceAttributeType.FacialHair,
                FaceAttributeType.Gender,
                FaceAttributeType.Glasses,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Smile,
                FaceAttributeType.Emotion
            });
        }

        public virtual AnalysisResult GetVisualAnalysis(MediaItem m)
        {
            if (VisionService.ValidateVisionImage(m).Any())
                return new AnalysisResult();

            return VisionService.AnalyzeImage(m.GetMediaStream(), new List<VisualFeature>() {
                VisualFeature.Adult,
                VisualFeature.Categories,
                VisualFeature.Color,
                VisualFeature.Description,
                VisualFeature.Faces,
                VisualFeature.ImageType,
                VisualFeature.Tags
            });
        }

        public virtual OcrResults GetTextualAnalysis(MediaItem m)
        {
            if (VisionService.ValidateVisionImage(m).Any())
                return new OcrResults();

            return VisionService.RecognizeText(m.GetMediaStream(), "en", true);
        }

        #endregion

        #region Helpers
        
        public virtual Item CreateAnalysisItem(Item imageItem, Dictionary<ID, string> fields)
        {
            return DataWrapper.CreateItem(Settings.ImageAnalysisFolderId, Settings.ImageAnalysisTemplateId, imageItem.Database.Name, imageItem.ID.ToShortID().ToString(), fields);
        }

        #endregion
    }
}