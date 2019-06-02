using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sitecore.Data;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.ComputerVision;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Analysis;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services {
    public interface IIntelligentMediaSearchService
    {
        void AddItemToIndex(string itemId, string dbName);
        void AddItemToIndex(Item item, string dbName);
        void UpdateItemInIndex(string itemId, string dbName);
        void UpdateItemInIndex(Item item, string dbName);
        int UpdateItemsInIndexRecursively(Item parentFolderItem, string db);
        int UpdateItemsInIndexRecursively(List<ImageSearchResult> itemList, string db);
        void RebuildCognitiveIndexes();
        /// <summary>
        /// Converts a cognitive search result into an image analysis
        /// </summary>
        IImageAnalysis GetImageAnalysis(string id, string language, string db);
        /// <summary>
        /// Gets the Description off the Computer Vision analysis
        /// </summary>
        /// <param name="m"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        IImageDescription GetImageDescription(MediaItem m, string language);
        /// <summary>
        /// Gets the highest confidence Caption off the Description in a Computer Vision analysis
        /// </summary>
        /// <param name="m"></param>
        /// <param name="language"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        Caption GetTopImageCaption(MediaItem m, string language, double threshold);
        /// <summary>
        /// Gets a single Cognitive Image Search result 
        /// </summary>
        IImageSearchResult GetSearchResult(string itemId, string language, string db);
        /// <summary>
        /// Gets any Image Search result that matches the provided criteria
        /// </summary>
        /// <param name="tagParameters">Text that appears in the image or is used to in the Description or Tags of the image</param>
        /// <param name="rangeParameters">A list of ranges (age, emotion)</param>
        /// <param name="gender">Type of gender</param>
        /// <param name="glasses">Type of glasses</param>
        /// <param name="size">Image dimensions</param>
        /// <param name="languageCode">Language Name</param>
        /// <param name="colors">Hex values to match</param>
        /// <param name="dbName">Database Name</param>
        /// <param name="fieldSourceId">Cognitive Image Field Source Item Id</param>
        /// <returns></returns>
        Tuple<int, List<IImageSearchResult>> GetFilteredSearchResults(Dictionary<string, string[]> tagParameters, Dictionary<string, string[]> rangeParameters, int gender, int glasses, string languageCode, List<string> colors, string dbName, ID fieldSourceId, int takeCount, int skipCount = 0);
        List<MediaItem> GetFaceIdResults(string languageCode, string dbName);
        /// <summary>
        /// Finds the analysis item in the content tree.
        /// </summary>
        /// <param name="itemName">Should be the short id of a media item</param>
        Item GetImageAnalysisItem(string itemName, string languageCode, string dbName);
        /// <summary>
        /// Gets a media search result from Sitecore's item index
        /// </summary>
        ImageSearchResult GetMediaItem(ID id, string languageCode, string dbName);
        /// <summary>
        /// Gets the items representing images in the media library
        /// </summary>
        List<ImageSearchResult> GetMediaItems(ID folderId, string languageCode, string dbName);
        List<KeyValuePair<string, int>> GetTags(string languageCode, string dbName, ID fieldSourceId);
        List<string> GetColors(string languageCode, string dbName, ID fieldSourceId);
        Expression<Func<ImageSearchResult, bool>> GetDefaultFilter(string[] parameterValues, string fieldName);
        Expression<Func<ImageSearchResult, bool>> GetRangeFilter(string[] parameterValues, string fieldName);
        string GetCognitiveIndexName(string dbName);
    }
}