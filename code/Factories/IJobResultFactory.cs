using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public interface IJobResultFactory
    {
        IJobResult Create(long current, long total, bool completed);
    }
}