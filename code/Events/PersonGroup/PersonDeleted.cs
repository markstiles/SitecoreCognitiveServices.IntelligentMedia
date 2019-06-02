using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.PersonGroup
{
    public class PersonDeleted
    {
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IPersonGroupService PersonGroupService;

        public PersonDeleted(
            IIntelligentMediaSettings settings,
            IPersonGroupService personGroupService)
        {
            Settings = settings;
            PersonGroupService = personGroupService;
        }

        public void OnItemDeleted(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            if (item == null
                || item.Paths.FullPath.StartsWith("/sitecore/templates")
                || !item.TemplateID.Equals(Settings.PersonTemplateId))
                return;

            var groupId = item.Parent?.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
            var personId = item.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(groupId) || string.IsNullOrWhiteSpace(personId))
                return;

            PersonGroupService.DeletePerson(groupId, new Guid(personId)); 
        }
    }
}