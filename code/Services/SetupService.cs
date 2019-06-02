using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Configuration;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK;
using SitecoreCognitiveServices.Foundation.SCSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services
{
    public class SetupService : ISetupService
    {
        #region Constructor

        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IMicrosoftCognitiveServicesApiKeys MSCSApiKeys;
        protected readonly IIntelligentMediaSettings SearchSettings;
        protected readonly IImageAnalysisService AnalysisService;
        protected readonly IIntelligentMediaSearchService SearchService;
        protected readonly IPublishWrapper PublishWrapper;
        protected readonly HttpContextBase Context;
        protected readonly ISCSDKSettings SCSDKSettings;
        protected readonly ILogWrapper Logger;

        protected readonly string ConfigFormat = "~/App_Config/Include/SitecoreCognitiveServices/SitecoreCognitiveServices.Feature.IntelligentMedia.{0}.config";

        public SetupService(
            ISitecoreDataWrapper dataWrapper,
            IMicrosoftCognitiveServicesApiKeys mscsApiKeys,
            IIntelligentMediaSettings searchSettings,
            IImageAnalysisService analysisService,
            IIntelligentMediaSearchService searchService,
            IPublishWrapper publishWrapper,
            HttpContextBase context,
            ISCSDKSettings scsdkSettings,
            ILogWrapper logger)
        {
            DataWrapper = dataWrapper;
            MSCSApiKeys = mscsApiKeys;
            SearchSettings = searchSettings;
            AnalysisService = analysisService;
            SearchService = searchService;
            Context = context;
            PublishWrapper = publishWrapper;
            SCSDKSettings = scsdkSettings;
            Logger = logger;
        }

        #endregion

        public IImageAnalysis SaveKeysAndAnalyze(string faceApi, string faceApiEndpoint, string computerVisionApi, string computerVisionApiEndpoint)
        {
            //save items to fields
            if (MSCSApiKeys.Face != faceApi)
                UpdateKey(SCSDKSettings.MSSDK_FaceFieldId, faceApi);
            if (MSCSApiKeys.FaceEndpoint != faceApiEndpoint)
                UpdateKey(SCSDKSettings.MSSDK_FaceEndpointFieldId, faceApiEndpoint);
            if (MSCSApiKeys.ComputerVision != computerVisionApi)
                UpdateKey(SCSDKSettings.MSSDK_ComputerVisionFieldId, computerVisionApi);
            if (MSCSApiKeys.ComputerVisionEndpoint != computerVisionApiEndpoint)
                UpdateKey(SCSDKSettings.MSSDK_ComputerVisionEndpointFieldId, computerVisionApiEndpoint);

            //get the sample image and analyze it to test responses
            try
            {
                var sampleItem = DataWrapper.GetItemById(SearchSettings.SampleImageId, SearchSettings.MasterDatabase);
                return AnalysisService.AnalyzeImage(sampleItem, null, null, null, false);
            }
            catch (Exception ex)
            {
                Logger.Error("There was an error setting up the Intelligent Media modules", this, ex);
                return null;
            }
        }

        public void UpdateKey(ID fieldId, string value)
        {
            var keyItem = DataWrapper?
                .GetDatabase(SCSDKSettings.MasterDatabase)
                .GetItem(SCSDKSettings.MSSDKId);
            DataWrapper.UpdateFields(keyItem, new Dictionary<ID, string>
            {
                { fieldId, value }
            });
        }

        /// <summary>
        /// change the field folder to a sitecore folder from a node
        /// </summary>
        public string SetFieldsFolderTemplate()
        {
            var coreDb = Factory.GetDatabase(SearchSettings.CoreDatabase);
            if (coreDb == null)
                return $"{SearchSettings.CoreDatabase} database";

            var template = coreDb.Templates["common/folder"];
            var fieldFolderItem = coreDb.GetItem(SearchSettings.IntelligentMediaFieldFolderId);
            if (fieldFolderItem == null)
                return "Field Folder in Core";

            fieldFolderItem.ChangeTemplate(template);

            return string.Empty;
        }

        public void PublishContent()
        {
            //publish templates folder for yourself and core, and publish scs root in modules
            List<ID> itemGuids = new List<ID>() {
                SearchSettings.SCSDKTemplatesFolderId,
                SearchSettings.IntelligentMediaTemplatesFolderId,
                SearchSettings.SCSModulesFolderId
            };

            Database fromDb = DataWrapper.GetDatabase(SearchSettings.MasterDatabase);
            Database toDb = DataWrapper.GetDatabase(SearchSettings.WebDatabase);
            foreach (var g in itemGuids)
            {
                var folder = fromDb.GetItem(g);

                PublishWrapper.PublishItem(folder, new[] { toDb }, new[] { folder.Language }, true, false, false);
            }
        }

        public void ConfigureIndexes(string indexOption)
        {
            //disable the unselected option config
            List<string> options = new List<string>{ "Lucene", "Solr", "Coveo" };
            options.Where(a => a != indexOption).ForEach(DisableConfig);
            
            //enable selected config
            var selectedPath = string.Format(ConfigFormat, indexOption);
            var selectedDisabledFile = Context.Server.MapPath($"{selectedPath}.disabled");
            var selectedEnabledFile = Context.Server.MapPath(selectedPath);
            if (System.IO.File.Exists(selectedDisabledFile))
            {
                System.IO.File.Copy(selectedDisabledFile, selectedEnabledFile, true);
                System.IO.File.Delete(selectedDisabledFile);
            }
        }

        public void DisableConfig(string unselectedOption)
        {
            //disable the unselected option config
            var unselectedPath = string.Format(ConfigFormat, unselectedOption);
            var unselectedDisabledFile = Context.Server.MapPath($"{unselectedPath}.disabled");
            var unselectedEnabledFile = Context.Server.MapPath(unselectedPath);
            if (System.IO.File.Exists(unselectedEnabledFile))
            {
                System.IO.File.Copy(unselectedEnabledFile, unselectedDisabledFile, true);
                System.IO.File.Delete(unselectedEnabledFile);
            }
        }

        public void ReindexAndResetDictionary() { 
            
            //get the congitive indexes build for the first time
            SearchService.RebuildCognitiveIndexes();

            Sitecore.Globalization.Translate.ResetCache(true);
        }
    }
}