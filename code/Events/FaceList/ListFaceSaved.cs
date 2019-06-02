using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;  
using Sitecore.Events;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.FaceList
{
    public class ListFaceSaved
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IPersonGroupService PersonGroupService;
        protected readonly IIntelligentMediaSettings Settings;

        public ListFaceSaved(
            ISitecoreDataWrapper dataWrapper,
            IPersonGroupService personGroupService,
            IIntelligentMediaSettings settings)
        {
            DataWrapper = dataWrapper;
            PersonGroupService = personGroupService;
            Settings = settings;
        }

        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            var itemChanges = Event.ExtractParameter(args, 1) as ItemChanges;
            if (item == null 
                || item.Paths.FullPath.StartsWith("/sitecore/templates")
                || !item.TemplateID.Equals(Settings.ListFaceTemplateId) 
                || itemChanges == null)
                return;

            using (new Sitecore.Data.Events.EventDisabler())
            {
                var imageField = (ImageField)item.Fields[Settings.FaceImageFieldId];
                var faceListId = item.Parent.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(imageField?.Value) 
                    || string.IsNullOrWhiteSpace(faceListId))
                    return;
                
                var oldImageValue = itemChanges.FieldChanges[Settings.FaceImageFieldId]?.OriginalValue;
                var faceId = item.Fields[Settings.FacePersistedFaceIdFieldId]?.Value;

                //if they modified the image remove the face object
                if(!string.IsNullOrWhiteSpace(faceId) && oldImageValue != imageField.Value)
                    PersonGroupService.DeleteListFace(faceListId, new Guid(faceId)); 

                Sitecore.Data.Items.MediaItem m = imageField.MediaItem;
                PersonGroupService.CreateListFace(item, faceListId, m.GetMediaStream());
            }
        }
    }
}