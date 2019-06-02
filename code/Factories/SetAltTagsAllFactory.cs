using System;
using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Utility;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public class SetAltTagsAllFactory : ISetAltTagsAllFactory
    {
        protected readonly IServiceProvider Provider;

        public SetAltTagsAllFactory(IServiceProvider provider)
        {
            Provider = provider;
        }

        public virtual ISetAltTagsAll Create(int itemCount, int itemsModified, int threshold, bool overwrite)
        {
            var obj = Provider.GetService<ISetAltTagsAll>();
            
            obj.ItemCount = itemCount;
            obj.ItemsModified = itemsModified;
            obj.Threshold = threshold;
            obj.Overwrite = overwrite;

            return obj;
        }
    }
}