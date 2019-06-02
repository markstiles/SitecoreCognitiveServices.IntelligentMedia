using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Fields;
using SitecoreCognitiveServices.Foundation.MSSDK;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia
{
    public class IntelligentMediaSettings : IIntelligentMediaSettings
    {
        protected readonly IMicrosoftCognitiveServicesApiKeys MSApiKeys;
        protected readonly ISitecoreDataWrapper DataWrapper;

        public IntelligentMediaSettings(
            ISitecoreDataWrapper dataWrapper,
            IMicrosoftCognitiveServicesApiKeys msApiKeys)
        {
            MSApiKeys = msApiKeys;
            DataWrapper = dataWrapper;
        }

        #region Globally Shared Settings

        public virtual string CoreDatabase => Settings.GetSetting("CognitiveService.CoreDatabase");
        public virtual string MasterDatabase => Settings.GetSetting("CognitiveService.MasterDatabase");
        public virtual string WebDatabase => Settings.GetSetting("CognitiveService.WebDatabase");
        public virtual ID SCSDKTemplatesFolderId => new ID(Settings.GetSetting("CognitiveService.SCSDKTemplatesFolder"));
        public virtual ID SCSModulesFolderId => new ID(Settings.GetSetting("CognitiveService.SCSModulesFolder"));

        #endregion

        public virtual ID VersionedImageId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.VersionedImageId"));
        public virtual ID VersionedJpgId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.VersionedJpgId"));
        public virtual ID UnversionedImageId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.UnversionedImageId"));
        public virtual ID UnversionedJpgId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.UnversionedJpgId"));
        public virtual string SitecoreIndexNameFormat => Settings.GetSetting("CognitiveService.IntelligentMedia.SitecoreIndexNameFormat");
        public virtual string CognitiveIndexNameFormat => Settings.GetSetting("CognitiveService.IntelligentMedia.CognitiveIndexNameFormat");
        public virtual ID VisualAnalysisFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.VisualAnalysisFieldId"));
        public virtual ID TextualAnalysisFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.TextualAnalysisFieldId"));
        public virtual ID FacialAnalysisFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FacialAnalysisFieldId"));
        public virtual ID AnalyzedImageFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.AnalyzedImageFieldId"));
        public virtual bool AnalyzeNewImageField
        {
            get
            {
                var searchRoot = DataWrapper.GetItemById(IntelligentMediaFolderId, MasterDatabase);
                CheckboxField f = searchRoot?.Fields[AnalyzeNewImageFieldId];
                if (f == null)
                    return false;

                return f.Checked;
            }
        }
        public virtual ID UseImageNameAsTagFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.UseImageNameAsTagFieldId"));
        public virtual bool UseImageNameAsTag
        {
            get
            {
                var searchRoot = DataWrapper.GetItemById(IntelligentMediaFolderId, MasterDatabase);
                CheckboxField f = searchRoot?.Fields[UseImageNameAsTagFieldId];
                if (f == null)
                    return false;

                return f.Checked;
            }
        }
        public virtual ID TagConfidenceThresholdFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.TagConfidenceThresholdFieldId"));
        public virtual float TagConfidenceThreshold
        {
            get
            {
                float defaultValue = 0.6f;
                var searchRoot = DataWrapper.GetItemById(IntelligentMediaFolderId, MasterDatabase);

                Field f = searchRoot?.Fields[TagConfidenceThresholdFieldId];
                if (f == null)
                    return defaultValue;

                float returnObj;
                return (float.TryParse(f.Value, out returnObj))
                    ? returnObj
                    : defaultValue;
            }
        }
        public virtual ID PeopleFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PeopleFieldId"));
        public virtual ID AnalyzeNewImageFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.AnalyzeNewImageFieldId"));
        public virtual ID ImageAnalysisFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.ImageAnalysisFolder"));
        public virtual ID ImageAnalysisTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.ImageAnalysisTemplate"));
        public virtual ID SampleImageId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.SampleImage"));
        public virtual ID IntelligentMediaFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.IntelligentMediaFolder"));
        public virtual ID BlogFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.BlogField"));
        public virtual ID IntelligentMediaFieldFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.IntelligentMediaFieldFolder"));
        public virtual ID IntelligentMediaTemplatesFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.IntelligentMediaTemplatesFolder"));
        public virtual ID FaceEntityIdFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FaceEntityIdFieldId"));
        public virtual ID FaceEntityNameFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FaceEntityNameFieldId"));
        public virtual ID FaceEntityUserDataFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FaceEntityUserDataFieldId"));
        public virtual ID PersonGroupsFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonGroupsFolderId"));
        public virtual ID PersonGroupTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonGroupTemplateId"));
        public virtual ID PersonGroupStatusFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonGroupStatusFieldId"));
        public virtual ID PersonGroupCreatedDateFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonGroupCreatedDateFieldId"));
        public virtual ID PersonGroupLastActionDateFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonGroupLastActionDateFieldId"));
        public virtual ID PersonGroupMessageFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonGroupMessageFieldId"));
        public virtual ID FaceListsFolderId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FaceListsFolderId"));
        public virtual ID FaceListTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FaceListTemplateId"));
        public virtual ID PersonTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonTemplateId"));
        public virtual ID PersonFaceTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.PersonFaceTemplateId"));
        public virtual ID ListFaceTemplateId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.ListFaceTemplateId"));
        public virtual ID FacePersistedFaceIdFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FacePersistedFaceIdFieldId"));
        public virtual ID FaceImageFieldId => new ID(Settings.GetSetting("CognitiveService.IntelligentMedia.FaceImageFieldId"));

        public virtual string DictionaryDomain => Settings.GetSetting("CognitiveService.IntelligentMedia.DictionaryDomain");
        public bool MissingKeys()
        {
            if (MSApiKeys == null)
                return true;

            return HasNoValue(MSApiKeys.Face)
                   || HasNoValue(MSApiKeys.FaceEndpoint)
                   || HasNoValue(MSApiKeys.ComputerVision)
                   || HasNoValue(MSApiKeys.ComputerVisionEndpoint);
        }
        public bool HasNoValue(string str)
        {
            return string.IsNullOrWhiteSpace(str.Trim());
        }
    }
}