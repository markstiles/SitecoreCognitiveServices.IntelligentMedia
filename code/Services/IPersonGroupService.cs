using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.MSSDK.Vision.Models.Face;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Services
{
    public interface IPersonGroupService
    {
        string CreatePersonGroup(Item item, string groupName, string groupUserData);
        void UpdatePersonGroup(Item item, string groupId, string groupName, string groupUserData);
        void DeletePersonGroup(string groupId);
        void SetPersonGroupTrainingResult(ID groupItemId, string dbName, TrainingStatus status);
        List<Item> GetAllGroupItems(string db);
        List<string> GetAllGroupIds(string db);
        Guid CreatePerson(Item item, string groupId, string personName, string personUserData);
        void UpdatePerson(Item item, string groupId, Guid personId, string personName, string personUserData);
        void DeletePerson(string groupId, Guid personId);
        List<Item> GetAllPersonItems(string db);
        Guid CreatePersonFace(Item item, string groupId, Guid personId, Stream imageStream);
        void DeletePersonFace(string groupId, Guid personId, Guid faceId);
        string CreateFaceList(Item item, string faceListName, string faceListUserData);
        void UpdateFaceList(Item item, string faceListId, string faceListName, string faceListUserData);
        void DeleteFaceList(string faceListId);
        Guid CreateListFace(Item item, string faceListId, Stream imageStream);
        void DeleteListFace(string faceListId, Guid faceId);
        string GetGroupOrListId(string name);
        void DetectAndIdentifyPeople(MediaItem item, string db, List<string> groupIdSet);
        void IdentifyPeople(Item analysisItem, Guid[] faces, string db, List<string> groupIdSet);
        int IdentifyAllGroups(string language, string db);
    }
}