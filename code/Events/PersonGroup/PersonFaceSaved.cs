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

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Events.PersonGroup
{
    public class PersonFaceSaved
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IPersonGroupService PersonGroupService;
        protected readonly IIntelligentMediaSettings Settings;

        public PersonFaceSaved(
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
                || !item.TemplateID.Equals(Settings.PersonFaceTemplateId) 
                || itemChanges == null)
                return;

            using (new Sitecore.Data.Events.EventDisabler())
            {
                var imageField = (ImageField)item.Fields[Settings.FaceImageFieldId];
                var groupId = item.Parent.Parent.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
                var personId = item.Parent.Fields[Settings.FaceEntityIdFieldId]?.Value ?? string.Empty;
                if (string.IsNullOrWhiteSpace(imageField?.Value) 
                    || string.IsNullOrWhiteSpace(groupId)
                    || string.IsNullOrWhiteSpace(personId))
                    return;
                
                var oldImageValue = itemChanges.FieldChanges[Settings.FaceImageFieldId]?.OriginalValue;
                var faceId = item.Fields[Settings.FacePersistedFaceIdFieldId]?.Value;

                //if they modified the image remove the face object
                if(!string.IsNullOrWhiteSpace(faceId) && oldImageValue != imageField.Value)
                    PersonGroupService.DeletePersonFace(groupId, new Guid(personId), new Guid(faceId)); 

                Sitecore.Data.Items.MediaItem m = imageField.MediaItem;
                PersonGroupService.CreatePersonFace(item, groupId, new Guid(personId), m.GetMediaStream());
            }
        }
    }
}