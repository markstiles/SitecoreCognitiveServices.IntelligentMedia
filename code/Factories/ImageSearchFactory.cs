using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public class ImageSearchFactory : IImageSearchFactory
    {
        protected readonly IServiceProvider Provider;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IIntelligentMediaSearchService SearchService;

        public ImageSearchFactory(
            IServiceProvider provider,
            ISitecoreDataWrapper dataWrapper,
            IIntelligentMediaSearchService searchService)
        {
            Provider = provider;
            DataWrapper = dataWrapper;
            SearchService = searchService;
        }

        public virtual IImageSearch Create()
        {
            return Provider.GetService<IImageSearch>();
        }

        public virtual IImageSearch Create(string db, string language, string fieldSource)
        {
            var r = Create();
            r.Database = db;
            r.Language = language;
            var fieldSourceItem = DataWrapper.GetItemByPath(fieldSource, db);
            var fieldSourceId = fieldSourceItem != null ? fieldSourceItem.ID : ID.Null;
            r.FieldSourceId = fieldSourceId != ID.Null ? fieldSourceId.ToString() : "";
            r.Tags = SearchService.GetTags(language, db, fieldSourceId);
            var colors = SearchService.GetColors(language, db, fieldSourceId);
            r.Colors = new List<KeyValuePair<string, string>>();
            var converter = new ColorConverter();
            foreach (string c in colors)
            {
                var colorName = "";
                var col = converter.ConvertFromString(c);
                if (col != null)
                {
                    var colObj = (Color)col;
                    if (colObj.IsNamedColor)
                        colorName = colObj.Name;
                }
                r.Colors.Add(new KeyValuePair<string, string>(colorName, c));
            }

            return r;
        }
        
        public virtual IImageSearchJsonResult CreateMediaSearchJsonResult(IImageSearchResult searchResult)
        {
            var obj = Provider.GetService<IImageSearchJsonResult>();
            
            MediaItem m = DataWrapper.GetItemByUri(searchResult.UniqueId);

            try
            {
                obj.Url = $"/sitecore/shell/-/media/{m.ID.Guid:N}.ashx";
            }
            catch (Exception ex)
            {
                obj.Url = string.Empty;
            }
            try
            {
                obj.Alt = m.Alt;
            }
            catch (Exception ex)
            {
                obj.Alt = string.Empty;
            }
            try
            {
                obj.Id = m.ID.ToString();
            }
            catch (Exception ex)
            {
                obj.Id = string.Empty;
            }
            try
            {
                obj.Title = m.DisplayName;
            }
            catch (Exception ex)
            {
                obj.Title = string.Empty;
            }

            return obj;
        }
    }
}