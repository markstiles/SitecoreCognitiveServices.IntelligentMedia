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
    public class PersonGroupSaved
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IPersonGroupService PersonGroupService;
        protected readonly IFaceService FaceService;
        protected readonly IIntelligentMediaSettings Settings;

        public PersonGroupSaved(
            ISitecoreDataWrapper dataWrapper,
            IPersonGroupService personGroupService,
            IFaceService faceService, 
            IIntelligentMediaSettings settings)
        {
            DataWrapper = dataWrapper;
            PersonGroupService = personGroupService;
            FaceService = faceService;
            Settings = settings;
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            var itemChanges = Event.ExtractParameter(args, 1) as ItemChanges;
            if (item == null
                || item.Paths.FullPath.StartsWith("/sitecore/templates")
                || !item.TemplateID.Equals(Settings.PersonGroupTemplateId)
                || itemChanges == null)
                return;
            
            using (new Sitecore.Data.Events.EventDisabler())
            {
                var groupName = item.Fields[Settings.FaceEntityNameFieldId].Value;
                if (string.IsNullOrWhiteSpace(groupName))
                    return;

                var oldGroupId = itemChanges.FieldChanges[Settings.FaceEntityIdFieldId]?.OriginalValue;
                var groupId = item.Fields[Settings.FaceEntityIdFieldId].Value;
                var groupUserData = item.Fields[Settings.FaceEntityUserDataFieldId]?.Value ?? string.Empty;

                //if they modified the group id you have to replace the group
                if (!string.IsNullOrWhiteSpace(oldGroupId) && oldGroupId != groupId)
                    PersonGroupService.DeletePersonGroup(oldGroupId); 

                //if not found, create. otherwise update
                if (string.IsNullOrWhiteSpace(groupId) || FaceService.GetPersonGroup(groupId) == null)
                    PersonGroupService.CreatePersonGroup(item, groupName, groupUserData);
                else
                    PersonGroupService.UpdatePersonGroup(item, groupId, groupName, groupUserData);
            }
        }
    }
}