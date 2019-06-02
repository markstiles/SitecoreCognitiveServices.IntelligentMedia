using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data.Items;
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.FaceList
{
    public class FaceListDeleted
    {
        protected readonly IIntelligentMediaSettings Settings;
        protected readonly IPersonGroupService PersonGroupService;

        public FaceListDeleted(
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
                || !item.TemplateID.Equals(Settings.FaceListTemplateId))
                return;

            var faceListId = item.Fields[Settings.FaceEntityIdFieldId]?.Value;
            if (string.IsNullOrEmpty(faceListId))
                return;

            PersonGroupService.DeleteFaceList(faceListId);
        }
    }
}