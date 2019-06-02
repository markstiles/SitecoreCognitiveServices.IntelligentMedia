using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public class JobResultFactory : IJobResultFactory
    {
        protected readonly IServiceProvider Provider;

        public JobResultFactory(IServiceProvider provider)
        {
            Provider = provider;
        }
        
        public virtual IJobResult Create(long current, long total, bool completed)
        {
            var obj = Provider.GetService<IJobResult>();

            obj.Current = current;
            obj.Total = total;
            obj.Completed = completed;

            return obj;
        }
    }
}