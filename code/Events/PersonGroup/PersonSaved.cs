using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items; 
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Vision;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.PersonGroup
{
    public class PersonSaved
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IFaceService FaceService;
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IPersonGroupService PersonGroupService;

        public PersonSaved(
            ISitecoreDataWrapper dataWrapper,
            IFaceService faceService, 
            IIntelligentMediaSettings settings,
            IPersonGroupService personGroupService)
        {
            DataWrapper = dataWrapper;
            FaceService = faceService;
            Settings = settings;
            PersonGroupService = personGroupService;
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            var itemChanges = Event.ExtractParameter(args, 1) as ItemChanges;
            if (item == null
                || item.Paths.FullPath.StartsWith("/sitecore/templates")
                || !item.TemplateID.Equals(Settings.PersonTemplateId)
                || itemChanges == null)
                return;
            
            using (new Sitecore.Data.Events.EventDisabler())
            {
                var personName = item.Fields[Settings.FaceEntityNameFieldId]?.Value ?? string.Empty;
                var groupId = item.Parent.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(personName)
                    || string.IsNullOrWhiteSpace(groupId))
                    return;
                
                var oldPersonId = itemChanges.FieldChanges[Settings.FaceEntityIdFieldId]?.OriginalValue;
                var personId = item.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
                var personUserData = item.Fields[Settings.FaceEntityUserDataFieldId]?.Value ?? string.Empty;

                //if they modified the person id you have to replace the person
                if (!string.IsNullOrWhiteSpace(oldPersonId) && oldPersonId != personId)
                   PersonGroupService.DeletePerson(groupId, new Guid(oldPersonId)); 

                //if not found, create. otherwise update
                if (string.IsNullOrWhiteSpace(personId) || FaceService.GetPerson(groupId, new Guid(personId)) == null)
                    PersonGroupService.CreatePerson(item, groupId, personName, personUserData);
                else
                    PersonGroupService.UpdatePerson(item, groupId, new Guid(personId), personName, personUserData);
            }
        }
    }
}