using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services
{
    public interface ISetupService
    {
        IImageAnalysis SaveKeysAndAnalyze(string faceApi, string faceApiEndpoint, string computerVisionApi, string computerVisionApiEndpoint);
        void UpdateKey(ID fieldId, string value);
        string SetFieldsFolderTemplate();
        void PublishContent();
        void ConfigureIndexes(string indexOption);
        void ReindexAndResetDictionary();
    }
}