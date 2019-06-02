using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Jobs;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Search;
using SitecoreCognitiveServices.Foundation.MSSDK.Enums;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;
using SitecoreCognitiveServices.Foundation.SCSDK.Services.MSSDK.Vision;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services
{
    public class PersonGroupService : IPersonGroupService
    {
        #region Constructor

        protected readonly IIntelligentMediaSettings Settings;
        protected readonly ISitecoreDataWrapper DataWrapper;
        protected readonly IFaceService FaceService;
        protected readonly IIntelligentMediaSearchService SearchService;

        public PersonGroupService(
            IIntelligentMediaSettings settings,
            ISitecoreDataWrapper dataWrapper,
            IFaceService faceService,
            IIntelligentMediaSearchService searchService)
        {
            Settings = settings;
            DataWrapper = dataWrapper;
            FaceService = faceService;
            SearchService = searchService;
        }

        #endregion

        #region Person Group

        public string CreatePersonGroup(Item item, string groupName, string groupUserData)
        {
            var validGroupId = GetGroupOrListId(groupName);
            try
            {
                FaceService.CreatePersonGroup(validGroupId, groupName, groupUserData);
                DataWrapper.UpdateFields(item, new Dictionary<ID, string>
                {
                    { Settings.FaceEntityIdFieldId, validGroupId }
                });

                if (!item.HasChildren)
                    return validGroupId;
                
                Item[] people = item.GetChildren().ToArray();
                foreach (var p in people)
                {
                    var personId = CreatePerson(p, validGroupId, p[Settings.FaceEntityNameFieldId], p[Settings.FaceEntityUserDataFieldId]);
                    
                    if (!p.HasChildren || personId == Guid.Empty)
                        continue;

                    Item[] faces = p.GetChildren().ToArray();
                    foreach (var f in faces)
                    {
                        MediaItem m = ((ImageField) f.Fields[Settings.FaceImageFieldId]).MediaItem;

                        this.CreatePersonFace(f, validGroupId, personId, m.GetMediaStream());
                    }
                }

                return validGroupId;
            }
            catch { }

            return string.Empty;
        }

        public void UpdatePersonGroup(Item item, string groupId, string groupName, string groupUserData)
        {
            try
            {
                FaceService.UpdatePersonGroup(groupId, groupName, groupUserData);
                DataWrapper.UpdateItemName(item, groupName);
            }
            catch { }
        }

        public void DeletePersonGroup(string groupId)
        {
            try
            {
                FaceService.DeletePersonGroup(groupId);
            } catch { }
        }

        public void SetPersonGroupTrainingResult(ID groupItemId, string dbName, TrainingStatus status)
        {            
            var groupItem = DataWrapper.GetItemById(groupItemId, dbName);
            var fieldValues = new Dictionary<ID, string>
            {
                { Settings.PersonGroupStatusFieldId, status.Status.ToString() },
                { Settings.PersonGroupCreatedDateFieldId, DataWrapper.GetDateFieldValue(status.CreatedDateTime) },
                { Settings.PersonGroupMessageFieldId, status.Message }
            };
            if(status.LastActionDateTime.HasValue)
                fieldValues.Add(Settings.PersonGroupLastActionDateFieldId, DataWrapper.GetDateFieldValue(status.LastActionDateTime.Value));
                
            DataWrapper.UpdateFields(groupItem, fieldValues);
        }

        public List<Item> GetAllGroupItems(string db)
        {
            var folder = DataWrapper.GetItemById(Settings.PersonGroupsFolderId, db);
            if (folder == null)
                return new List<Item>();

            return folder.GetChildren()
                .Where(a => a.TemplateID.Equals(Settings.PersonGroupTemplateId))
                .ToList();
        }

        public List<string> GetAllGroupIds(string db)
        {
            List<string> groupIdSet = new List<string>();
            var groupItems = GetAllGroupItems(db);
            if (!groupItems.Any())
                return groupIdSet;

            groupIdSet.AddRange(groupItems.Select(a => a[Settings.FaceEntityIdFieldId]).ToList());

            return groupIdSet;
        }

        #endregion

        #region Person

        public Guid CreatePerson(Item item, string groupId, string personName, string personUserData)
        {
            try
            {
                var personResponse = FaceService.CreatePerson(groupId, personName, personUserData);
                if (personResponse == null)
                    return Guid.Empty;

                DataWrapper.UpdateFields(item, new Dictionary<ID, string>
                {
                    { Settings.FaceEntityIdFieldId, personResponse.PersonId.ToString("D") }
                });

                return personResponse.PersonId;
            }
            catch { }

            return Guid.Empty;
        }

        public void UpdatePerson(Item item, string groupId, Guid personId, string personName, string personUserData)
        {
            try
            {
                FaceService.UpdatePerson(groupId, personId, personName, personUserData);
                DataWrapper.UpdateItemName(item, personName);
            }
            catch { }
        }

        public void DeletePerson(string groupId, Guid personId)
        {
            try
            {
                FaceService.DeletePerson(groupId, personId);
            } catch { }
        }

        public List<Item> GetAllPersonItems(string db)
        {
            var folder = DataWrapper.GetItemById(Settings.PersonGroupsFolderId, db);
            if (folder == null)
                return new List<Item>();

            return folder.Axes.GetDescendants()
                .Where(a => a.TemplateID.Equals(Settings.PersonTemplateId))
                .ToList();
        }

        #endregion

        #region Person Face

        public Guid CreatePersonFace(Item item, string groupId, Guid personId, Stream imageStream)
        {
            try
            {
                var faceResponse = FaceService.AddPersonFace(groupId, personId, imageStream);
                if (faceResponse == null)
                    return Guid.Empty;

                DataWrapper.UpdateFields(item, new Dictionary<ID, string>
                {
                    { Settings.FacePersistedFaceIdFieldId, faceResponse.PersistedFaceId.ToString("D") }
                });

                return faceResponse.PersistedFaceId;
            }
            catch { }

            return Guid.Empty;
        }
        
        public void DeletePersonFace(string groupId, Guid personId, Guid faceId)
        {
            try
            {
                FaceService.DeletePersonFace(groupId, personId, faceId); 
                
            } catch { }
        }

        #endregion

        #region Face List

        public string CreateFaceList(Item item, string faceListName, string faceListUserData)
        {
            var validFaceListId = GetGroupOrListId(faceListName);
            try
            {
                FaceService.CreateFaceList(validFaceListId, faceListName, faceListUserData);
                DataWrapper.UpdateFields(item, new Dictionary<ID, string>
                {
                    { Settings.FaceEntityIdFieldId, validFaceListId }
                });

                return validFaceListId;
            }
            catch { }

            return string.Empty;
        }

        public void UpdateFaceList(Item item, string faceListId, string faceListName, string faceListUserData)
        {
            try
            {
                FaceService.UpdateFaceList(faceListId, faceListName, faceListUserData);
                DataWrapper.UpdateItemName(item, faceListName);
            }
            catch { }
        }

        public void DeleteFaceList(string faceListId)
        {
            try
            {
                FaceService.DeleteFaceList(faceListId);
            }
            catch { }
        }

        #endregion

        #region List Face

        public Guid CreateListFace(Item item, string faceListId, Stream imageStream)
        {
            try
            {
                var faceResponse = FaceService.AddFaceToFaceList(faceListId, imageStream);
                if (faceResponse == null)
                    return Guid.Empty;

                DataWrapper.UpdateFields(item, new Dictionary<ID, string>
                {
                    { Settings.FacePersistedFaceIdFieldId, faceResponse.PersistedFaceId.ToString("D") }
                });

                return faceResponse.PersistedFaceId;
            }
            catch { }

            return Guid.Empty;
        }
        
        public void DeleteListFace(string faceListId, Guid faceId)
        {
            try
            {
                FaceService.DeleteFaceFromFaceList(faceListId, faceId);

            }
            catch { }
        }

        #endregion

        #region Identify

        public virtual void DetectAndIdentifyPeople(MediaItem item, string db, List<string> groupIdSet)
        {
            var analysis = SearchService.GetImageAnalysisItem(item.ID.ToShortID().ToString(), item.InnerItem.Language.Name, db);
            if (analysis == null)
                return;

            if (FaceService.ValidateFaceImage(item).Any())
                return;

            var detection = FaceService.Detect(item.GetMediaStream(), true, true, new List<FaceAttributeType>
            {
                FaceAttributeType.Age,
                FaceAttributeType.FacialHair,
                FaceAttributeType.Gender,
                FaceAttributeType.Glasses,
                FaceAttributeType.HeadPose,
                FaceAttributeType.Smile,
                FaceAttributeType.Emotion
            });
            if (detection == null || !detection.Any())
                return;
            
            DataWrapper.UpdateFields(analysis, new Dictionary<ID, string> {
                { Settings.FacialAnalysisFieldId, JsonConvert.SerializeObject(detection) }
            });
            SearchService.UpdateItemInIndex(analysis, db);

            var faceIds = detection.Select(a => a.FaceId).ToArray();
            if (!faceIds.Any())
                return;

            IdentifyPeople(analysis, faceIds, db, groupIdSet);
        }

        public virtual void IdentifyPeople(Item analysisItem, Guid[] faces, string db, List<string> groupIdSet)
        {
            if (!groupIdSet.Any())
                return;
            
            var people = GetAllPersonItems(db).ToDictionary(a => new Guid(a.Fields[Settings.FaceEntityIdFieldId].Value));
            List<Candidate> candidates = new List<Candidate>();
            foreach (var groupId in groupIdSet)
            {
                var identifications = FaceService.Identify(groupId, faces);
                if (identifications == null)
                    continue; 

                var matches = identifications.SelectMany(a => a.Candidates).ToList();
                if (!matches.Any())
                    continue;

                candidates.AddRange(matches);
            }
            
            StringBuilder sb = new StringBuilder();
            foreach (var d in candidates)
            {
                if (!people.ContainsKey(d.PersonId))
                    continue;

                if (sb.Length > 0)
                    sb.Append("|");
                sb.Append(people[d.PersonId].ID);
            }

            if (sb.Length == 0)
                return;

            DataWrapper.UpdateFields(analysisItem, new Dictionary<ID, string>
            {
                { Settings.PeopleFieldId, sb.ToString() }
            });
        }

        public virtual int IdentifyGroup(string language, string db, string groupId)
        {
            List<string> groupIdSet = new List<string> { groupId };
            var list = SearchService.GetFaceIdResults(language, db);

            long line = 0;

            DataWrapper.SetJobPriority(ThreadPriority.Highest);
            DataWrapper.SetJobTotal(list.Count);
            
            foreach (var item in list)
            {
                line++;
                DataWrapper.SetJobStatus(line);

                DetectAndIdentifyPeople(item, db, groupIdSet);
                SearchService.UpdateItemInIndex(item, db);
            }

            DataWrapper.SetJobState(JobState.Finished);

            return list.Count;
        }

        public virtual int IdentifyAllGroups(string language, string db)
        {
            List<string> groupIdSet = GetAllGroupIds(db);
            var list = SearchService.GetFaceIdResults(language, db);
            
            long line = 0;

            DataWrapper.SetJobPriority(ThreadPriority.Highest);
            DataWrapper.SetJobTotal(list.Count);

            foreach (var item in list)
            {
                line++;
                DataWrapper.SetJobStatus(line);

                DetectAndIdentifyPeople(item, db, groupIdSet);
                SearchService.UpdateItemInIndex(item, db);
            }
            
            DataWrapper.SetJobState(JobState.Finished);

            return list.Count;
        }

        #endregion

        #region Helpers

        public string GetGroupOrListId(string name)
        {
            return new Regex("[^a-zA-Z0-9-_']").Replace(name, "").ToLower();
        }
        
        #endregion
    }
}