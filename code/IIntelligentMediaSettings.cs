using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia
{
    public interface IIntelligentMediaSettings {
        string WebDatabase { get; }
        string MasterDatabase { get; }
        string CoreDatabase { get; }
        ID VersionedImageId { get; }
        ID VersionedJpgId { get; }
        ID UnversionedImageId { get; }
        ID UnversionedJpgId { get; }
        ID SCSDKTemplatesFolderId { get; }
        ID SCSModulesFolderId { get; }
        string SitecoreIndexNameFormat { get; }
        string CognitiveIndexNameFormat { get; }
        ID VisualAnalysisFieldId { get; }
        ID TextualAnalysisFieldId { get; }
        ID FacialAnalysisFieldId { get; }
        ID AnalyzedImageFieldId { get; }
        ID PeopleFieldId { get; }
        ID AnalyzeNewImageFieldId { get; }
        bool AnalyzeNewImageField { get; }
        ID UseImageNameAsTagFieldId { get; }
        bool UseImageNameAsTag { get; }
        ID TagConfidenceThresholdFieldId { get; }
        float TagConfidenceThreshold { get; }
        ID ImageAnalysisFolderId { get; }
        ID ImageAnalysisTemplateId { get; }
        ID SampleImageId { get; }
        ID IntelligentMediaFolderId { get; }
        ID BlogFieldId { get; }
        ID IntelligentMediaFieldFolderId { get; }
        ID IntelligentMediaTemplatesFolderId { get; }
        ID FaceEntityIdFieldId { get; }
        ID FaceEntityNameFieldId { get; }
        ID FaceEntityUserDataFieldId { get; }
        ID PersonGroupsFolderId { get; }
        ID PersonGroupTemplateId { get; }
        ID PersonGroupStatusFieldId { get; }
        ID PersonGroupCreatedDateFieldId { get; }
        ID PersonGroupLastActionDateFieldId { get; }
        ID PersonGroupMessageFieldId { get; }
        ID FaceListsFolderId { get; }
        ID FaceListTemplateId { get; }
        ID PersonTemplateId { get; }
        ID PersonFaceTemplateId { get; }
        ID ListFaceTemplateId { get; }
        ID FacePersistedFaceIdFieldId { get; }
        ID FaceImageFieldId { get; }
        string DictionaryDomain { get; }
        bool MissingKeys();
        bool HasNoValue(string str);
    }
}
