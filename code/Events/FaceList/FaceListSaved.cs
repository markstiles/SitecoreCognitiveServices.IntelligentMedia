using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;  
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Vision;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.FaceList
{
    public class FaceListSaved
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IPersonGroupService PersonGroupService;
        protected readonly IFaceService FaceService;
        protected readonly IIntelligentMediaSettings Settings;

        public FaceListSaved(
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
                || !item.TemplateID.Equals(Settings.FaceListTemplateId)
                || itemChanges == null)
                return;
            
            using (new Sitecore.Data.Events.EventDisabler())
            {
                var faceListName = item.Fields[Settings.FaceEntityNameFieldId].Value;
                if (string.IsNullOrWhiteSpace(faceListName))
                    return;

                var oldFaceListId = itemChanges.FieldChanges[Settings.FaceEntityIdFieldId]?.OriginalValue;
                var faceListId = item.Fields[Settings.FaceEntityIdFieldId].Value;
                var faceListUserData = item.Fields[Settings.FaceEntityUserDataFieldId]?.Value ?? string.Empty;

                //if they modified the group id you have to replace the group
                if (!string.IsNullOrWhiteSpace(oldFaceListId) && oldFaceListId != faceListId)
                    PersonGroupService.DeleteFaceList(oldFaceListId); 

                //if not found, create. otherwise update
                if (string.IsNullOrWhiteSpace(faceListId) || FaceService.GetPersonGroup(faceListId) == null)
                    PersonGroupService.CreateFaceList(item, faceListName, faceListUserData);
                else
                    PersonGroupService.UpdateFaceList(item, faceListId, faceListName, faceListUserData);
            }
        }
    }
}