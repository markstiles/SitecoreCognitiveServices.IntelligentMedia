using System;
using Microsoft.Extensions.DependencyInjection;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Areas.SitecoreCognitiveServices.Models.Setup;
using SitecoreCognitiveServices.Foundation.MSSDK;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Factories
{
    public class SetupInformationFactory : ISetupInformationFactory
    {
        protected readonly IServiceProvider Provider;
        protected readonly IMicrosoftCognitiveServicesApiKeys ApiKeys;

        public SetupInformationFactory(IServiceProvider provider, IMicrosoftCognitiveServicesApiKeys apiKeys)
        {
            Provider = provider;
            ApiKeys = apiKeys;
        }

        public virtual ISetupInformation Create()
        {
            var obj = Provider.GetService<ISetupInformation>();

            obj.FaceApiKey = ApiKeys.Face;
            obj.FaceApiEndpoint = ApiKeys.FaceEndpoint;
            obj.ComputerVisionApiKey = ApiKeys.ComputerVision;
            obj.ComputerVisionApiEndpoint = ApiKeys.ComputerVisionEndpoint;

            return obj;
        }
    }
}