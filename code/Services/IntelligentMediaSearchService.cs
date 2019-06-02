using System;
using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Factories;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Xml;
using Sitecore.Collections;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.ContentSearch.SearchTypes;
using Sitecore.ContentSearch.Security;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;
using Sitecore.Data;
using Sitecore.Data.Engines;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services {
    public class IntelligentMediaSearchService : IIntelligentMediaSearchService 
    {
        #region Constructor

        protected readonly IContentSearchWrapper ContentSearch;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IImageDescriptionFactory ImageDescriptionFactory;
        protected readonly IImageAnalysisFactory ImageAnalysisFactory;
        protected readonly IMediaWrapper MediaWrapper;

        public IntelligentMediaSearchService(
            IContentSearchWrapper contentSearch,
            ISitecoreDataWrapper dataWrapper,
            IIntelligentMediaSettings settings,
            IImageDescriptionFactory imageDescriptionFactory,
            IImageAnalysisFactory imageAnalysisFactory,
            IMediaWrapper mediaWrapper)
        {
            ContentSearch = contentSearch;
            DataWrapper = dataWrapper;
            Settings = settings;
            ImageDescriptionFactory = imageDescriptionFactory;
            ImageAnalysisFactory = imageAnalysisFactory;
            MediaWrapper = mediaWrapper;
        }

        #endregion
        
        #region Indexing

        public virtual void AddItemToIndex(string itemId, string dbName)
        {
            ContentSearch.AddItemToIndex(itemId, dbName, GetCognitiveIndexName(dbName));
        }

        public virtual void AddItemToIndex(Item item, string dbName)
        {
            ContentSearch.AddItemToIndex(item, GetCognitiveIndexName(dbName));
        }

        public virtual void UpdateItemInIndex(string itemId, string dbName)
        {
            ContentSearch.UpdateItemInIndex(itemId, dbName, GetCognitiveIndexName(dbName));
        }

        public virtual void UpdateItemInIndex(Item item, string dbName)
        {
            ContentSearch.UpdateItemInIndex(item, GetCognitiveIndexName(dbName));
        }

        public virtual int UpdateItemsInIndexRecursively(Item parentFolderItem, string db)
        {
            var list = GetMediaItems(
                parentFolderItem.ID,
                parentFolderItem.Language.Name,
                parentFolderItem.Database.Name);

            return UpdateItemsInIndexRecursively(list, db);
        }

        public virtual int UpdateItemsInIndexRecursively(List<ImageSearchResult> itemList, string db)
        { 
            itemList.ForEach(b => UpdateItemInIndex(b.GetItem(), db));

            return itemList.Count;
        }

        public virtual void RebuildCognitiveIndexes()
        {
            List<string> cogIndexes = new List<string>();
            var nodes = Sitecore.Configuration.Factory.GetConfigNodes("contentSearch/configuration/indexes/index");
            foreach (XmlNode n in nodes)
            {
                var id = n.Attributes?["id"];
                if (id == null || !id.Value.StartsWith("cognitive"))
                    continue;
                
                var dbNode = n.SelectSingleNode("locations/crawler/Database");
                var value = dbNode?.FirstChild?.InnerText;
                if (string.IsNullOrEmpty(value))
                    continue;

                cogIndexes.Add(value);    
            }
            
            foreach(var dbName in cogIndexes)
            { 
                var searchIndex = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));
                IndexCustodian.FullRebuild(searchIndex);
            }
        }

        #endregion

        #region Querying
        
        public virtual IImageAnalysis GetImageAnalysis(string id, string language, string db)
        {
            IImageSearchResult csr = GetSearchResult(id, language, db);

            return ImageAnalysisFactory.Create(csr);
        }

        public virtual IImageDescription GetImageDescription(MediaItem m, string language)
        {

            IImageSearchResult csr = GetSearchResult(m.ID.ToString(), language, m.Database.Name);
            if (csr == null)
                return null;

            List<string> descriptions = new List<string>();

            var captions = csr
                .VisionAnalysis
                ?.Description
                ?.Captions
                .Where(a => a.Confidence > Settings.TagConfidenceThreshold)
                .Select(a => a.Text)
                .ToList();

            var hasCaptions = captions != null && captions.Any();
            var hasPeople = csr.People != null && csr.People.Any();

            if (hasCaptions && hasPeople)
                descriptions.Add($"{string.Join(", ", csr.People)} - {captions.First()}");
            else if (hasCaptions)
                descriptions.AddRange(captions);
            else if (hasPeople)
                descriptions.Add(string.Join(", ", csr.People));
            
            return ImageDescriptionFactory.Create(descriptions, m.Alt);
        }

        public virtual Caption GetTopImageCaption(MediaItem m, string language, double threshold)
        {
            var csr = GetSearchResult(m.ID.ToString(), language, m.Database.Name);

            try
            {
                return csr
                    .VisionAnalysis
                    .Description
                    .Captions
                    .OrderByDescending(a => a.Confidence)
                    .First(c => c.Confidence >= threshold);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public virtual IImageSearchResult GetSearchResult(string itemId, string language, string dbName) {
            Assert.IsTrue(!string.IsNullOrEmpty(itemId), "The item id parameter is required");
            Assert.IsTrue(!string.IsNullOrEmpty(language), "The language parameter is required");
            Assert.IsTrue(!string.IsNullOrEmpty(dbName), "The database parameter is required");

            var index = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                return context.GetQueryable<ImageSearchResult>()
                    .Where(a =>
                        a.UniqueId.Contains(itemId.ToLower())
                        && a.Language == language)
                    .Take(1)
                    .FirstOrDefault();
            }
        }

        public virtual Tuple<int, List<IImageSearchResult>> GetFilteredSearchResults(
            Dictionary<string, string[]> tagParameters, 
            Dictionary<string, string[]> rangeParameters, 
            int gender, 
            int glasses,
            string languageCode, 
            List<string> colors, 
            string dbName,
            ID fieldSourceId,
            int takeCount,
            int skipCount = 0)
        {
            var index = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                IQueryable<ImageSearchResult> queryable = context.GetQueryable<ImageSearchResult>()
                    .Where(a => a.Language == languageCode && a.IsImage);

                if (fieldSourceId != ID.Null)
                    queryable = queryable.Where(a => a.Paths.Contains(fieldSourceId));

                if(colors != null) {
                    var colorPredicate = GetDefaultFilter(colors.ToArray(), "Colors");
                    if (colorPredicate != null)
                        queryable = queryable.Where(colorPredicate);
                }

                if (gender != 0)
                    queryable = queryable.Where(x => x.Gender == gender);
                
                if (glasses >= 0)
                    queryable = queryable.Where(x => x.Glasses.Contains(glasses));
                
                foreach (var parameter in tagParameters)
                {
                    var thisParamPredicate = GetDefaultFilter(parameter.Value, parameter.Key);
                    if (thisParamPredicate != null)
                        queryable = queryable.Where(thisParamPredicate);
                }

                var ageKey = "age";
                if (rangeParameters.ContainsKey(ageKey))
                {
                    var age = rangeParameters[ageKey];

                    var min = double.Parse(age[0]);
                    var max = double.Parse(age[1]);

                    if (min > 0d)
                        queryable = queryable.Where(r => r.AgeMin >= min);

                    if (max < 100d)
                        queryable = queryable.Where(r => r.AgeMax <= max);

                    rangeParameters.Remove(ageKey);
                }

                var heightKey = "height";
                if (rangeParameters.ContainsKey(heightKey))
                {
                    var height = rangeParameters[heightKey];

                    var min = double.Parse(height[0]);
                    var max = double.Parse(height[1]);

                    if (min > 0d)
                        queryable = queryable.Where(r => r.Height >= min);

                    if (max < 4000d)
                        queryable = queryable.Where(r => r.Height <= max);

                    rangeParameters.Remove(heightKey);
                }

                var widthKey = "width";
                if (rangeParameters.ContainsKey(widthKey))
                {
                    var width = rangeParameters[widthKey];

                    var min = double.Parse(width[0]);
                    var max = double.Parse(width[1]);

                    if (min > 0d)
                        queryable = queryable.Where(r => r.Width >= min);

                    if (max < 4000d)
                        queryable = queryable.Where(r => r.Width <= max);

                    rangeParameters.Remove(widthKey);
                }

                foreach (var parameter in rangeParameters)
                {
                    var thisParamPredicate = GetRangeFilter(parameter.Value, parameter.Key);
                    if (thisParamPredicate != null)
                        queryable = queryable.Where(thisParamPredicate);
                }

                var results = queryable.GetResults();
                var totalResults = results.TotalSearchResults;
                var trimResults = results.Hits.Skip(skipCount).Take(takeCount).Select(a => a.Document).ToList<IImageSearchResult>();

                return Tuple.Create(totalResults, trimResults);
            }
        }

        public virtual List<MediaItem> GetFaceIdResults(string languageCode, string dbName)
        {
            var templatePredicate = GetImageTemplatePredicate();

            var index = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                return context.GetQueryable<ImageSearchResult>()
                    .Where(templatePredicate)
                    .Where(a => a.Language == languageCode && !a.AllPeopleIdentified)
                    .Select(b => (MediaItem) b.GetItem())
                    .ToList();
            }
        }

        public virtual Item GetImageAnalysisItem(string itemName, string languageCode, string dbName)
        {
            var index = ContentSearch.GetIndex(ContentSearch.GetSitecoreIndexName(dbName));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                return context.GetQueryable<ImageSearchResult>()
                    .Where(a =>
                        a.Paths.Contains(Settings.ImageAnalysisFolderId)
                        && a.Name == itemName
                        && a.Language == languageCode)
                    .Take(1)
                    .FirstOrDefault()
                    ?.GetItem();
            }
        }

        public virtual ImageSearchResult GetMediaItem(ID id, string languageCode, string dbName)
        {
            var index = ContentSearch.GetIndex(ContentSearch.GetSitecoreIndexName(dbName));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                return context.GetQueryable<ImageSearchResult>()
                    .Where(a => a.ItemId == id && a.Language == languageCode)
                    .Take(1)
                    .FirstOrDefault();
            }
        }

        public virtual List<ImageSearchResult> GetMediaItems(ID folderId, string languageCode, string dbName)
        {
            var templatePredicate = GetImageTemplatePredicate();

            var index = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));
            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                return context.GetQueryable<ImageSearchResult>()
                    .Where(templatePredicate)
                    .Where(a => a.Language == languageCode && a.Paths.Contains(folderId))
                    .ToList();
            }
        }

        public virtual List<KeyValuePair<string, int>> GetTags(string languageCode, string dbName, ID fieldSourceId)
        {
            var index = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));

            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                var queryable = context.GetQueryable<ImageSearchResult>()
                    .Where(a => a.Language == languageCode);

                if (fieldSourceId != ID.Null)
                    queryable = queryable.Where(a => a.Paths.Contains(fieldSourceId));

                var results = queryable.ToArray();

                var tags = results
                    .Where(x => x.Tags != null)
                    .SelectMany(x => x.Tags)
                    .Select(a => a.Trim())
                    .ToArray();

                return tags.GroupBy(x => x)
                    .Select(x => new KeyValuePair<string, int>(x.Key, x.Count()))
                    .OrderByDescending(x => x.Value)
                    .ToList();
            }
        }

        public virtual List<string> GetColors(string languageCode, string dbName, ID fieldSourceId)
        {
            var index = ContentSearch.GetIndex(GetCognitiveIndexName(dbName));

            using (var context = index.CreateSearchContext(SearchSecurityOptions.DisableSecurityCheck))
            {
                var queryable = context.GetQueryable<ImageSearchResult>()
                    .Where(a => a.Language == languageCode);

                if (fieldSourceId != ID.Null)
                    queryable = queryable.Where(a => a.Paths.Contains(fieldSourceId));

                var results = queryable.ToArray();

                var colors = results
                    .Where(x => x.Colors != null)
                    .SelectMany(x => x.Colors)
                    .Distinct()
                    .OrderBy(a => a)
                    .ToList();

                return colors;
            }
        }
        
        #endregion

        #region Helpers
        
        public virtual Expression<Func<ImageSearchResult, bool>> GetImageTemplatePredicate()
        {
            List<ID> imageTemplateIds = new List<ID>
            {
                Settings.VersionedImageId,
                Settings.VersionedJpgId,
                Settings.UnversionedImageId,
                Settings.UnversionedJpgId
            };

            var templatePredicate = PredicateBuilder.False<ImageSearchResult>();
            foreach (var templateId in imageTemplateIds)
            {
                templatePredicate = templatePredicate.Or(o => o.TemplateId == templateId);
            }

            return templatePredicate;
        }
        
        public virtual string GetCognitiveIndexName(string dbName)
        {
            return string.Format(Settings.CognitiveIndexNameFormat, dbName ?? "master");
        }

        /// <summary>
        /// Default filters in query predicate
        /// </summary>
        /// <param name="parameterValues"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public virtual Expression<Func<ImageSearchResult, bool>> GetDefaultFilter(string[] parameterValues, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || parameterValues == null || !parameterValues.Any())
                return null;
            
            Expression<Func<ImageSearchResult, bool>> innerPredicate = PredicateBuilder.True<ImageSearchResult>();
            foreach (string val in parameterValues.Where(d => !string.IsNullOrEmpty(d)))
            {
                string parameterValue = val;
                innerPredicate = innerPredicate.Or(i => (string)i[(ObjectIndexerKey)fieldName] == parameterValue);
            }

            return innerPredicate;
        }

        public virtual Expression<Func<ImageSearchResult, bool>> GetRangeFilter(string[] parameterValues, string fieldName)
        {
            if (string.IsNullOrEmpty(fieldName) || !parameterValues.Any())
                return null;
    
            //scientific notation
            double min = double.Parse(parameterValues[0]) * .01;
            double max = double.Parse(parameterValues[1]) * .01;

            //no need for a query
            if (min < 0.01d && max > 0.99d)
                return null;
            
            Expression<Func<ImageSearchResult, bool>> innerPredicate = PredicateBuilder.True<ImageSearchResult>();

            //place limit only on the high end
            if (min > 0)
                innerPredicate = innerPredicate.And(i => (double)i[(ObjectIndexerKey)$"{fieldName}Min"] >= min);
            
            //place limit only on the low end
            if (max < 100)
                innerPredicate = innerPredicate.And(i => (double)i[(ObjectIndexerKey)$"{fieldName}Max"] <= max);
            
            return innerPredicate;
        }

        #endregion
    }
}