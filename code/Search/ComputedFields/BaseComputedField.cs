using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Newtonsoft.Json;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.ContentSearch.Diagnostics;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Search.ComputedFields
{
    public abstract class BaseComputedField : IComputedIndexField
    {
        public virtual string FieldName { get; set; }
        public virtual string ReturnType { get; set; }
        public OcrResults Text { get; set; }
        public AnalysisResult Visions { get; set; }
        public Face[] Faces { get; set; }
        public List<string> People { get; set; }
        public float ConfidenceThreshold { get; set; }
        public bool UseImageNameAsTag { get; set; }
        public List<TemplateItem> BaseTemplates { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            Assert.ArgumentNotNull(indexable, "indexable");

            Item indexItem = indexable as SitecoreIndexableItem;
            if (indexItem == null)
                return null;

            if (indexItem?.Database?.Name == "core")
                return null;
            
            var settings = DependencyResolver.Current.GetService<IIntelligentMediaSettings>();
            if (settings == null)
                return null;
            
            var dataWrapper = DependencyResolver.Current.GetService<ISitecoreDataWrapper>();
            if (dataWrapper == null)
                return null;
            
            var searchService = DependencyResolver.Current.GetService<IIntelligentMediaSearchService>();
            var analysisItem = searchService?.GetImageAnalysisItem(indexItem.ID.ToShortID().ToString(), indexItem.Language.Name, indexItem.Database.Name);
            if (analysisItem == null)
                return null;

            BaseTemplates = dataWrapper.GetBaseTemplates(indexItem).ToList();
            ConfidenceThreshold = settings.TagConfidenceThreshold;
            UseImageNameAsTag = settings.UseImageNameAsTag;
            Visions = JsonConvert.DeserializeObject<AnalysisResult>(analysisItem.Fields[settings.VisualAnalysisFieldId]?.Value ?? string.Empty);
            Text = JsonConvert.DeserializeObject<OcrResults>(analysisItem.Fields[settings.TextualAnalysisFieldId]?.Value ?? string.Empty);
            Faces = JsonConvert.DeserializeObject<Face[]>(analysisItem.Fields[settings.FacialAnalysisFieldId]?.Value ?? string.Empty);
            People = new List<string>();

            var peopleField = (DelimitedField)analysisItem.Fields[settings.PeopleFieldId];
            if (string.IsNullOrWhiteSpace(peopleField?.Value)) return GetFieldValue(indexItem);

            foreach (var idValue in peopleField.Items)
            {
                var personItem = dataWrapper.GetItemById(new ID(idValue), indexItem.Database.Name);
                People.Add(personItem[settings.FaceEntityNameFieldId]);
            }

            return GetFieldValue(indexItem);
        }

        protected virtual object GetFieldValue(Item indexItem)
        {
            return null;
        }
    }
}