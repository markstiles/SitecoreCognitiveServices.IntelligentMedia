using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.FaceList
{
    public class ListFaceDeleted
    {
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IPersonGroupService PersonGroupService;

        public ListFaceDeleted(
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
                || !item.TemplateID.Equals(Settings.ListFaceTemplateId))
                return;

            var faceListId = item.Parent.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
            var faceId = item.Fields[Settings.FacePersistedFaceIdFieldId]?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(faceListId) 
                || string.IsNullOrWhiteSpace(faceId))
                return;

            PersonGroupService.DeleteListFace(faceListId, new Guid(faceId));
        }
    }
}