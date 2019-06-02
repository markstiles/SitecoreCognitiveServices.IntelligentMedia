using System;
using System.Linq;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class AddFace : BaseIntelligentMediaCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/AddFace?id={Id}&language={Language}&db={Db}")
            {
                Header = "Add Face",
                Height = "400",
                Width = "650",
                Message = "Add an image to a person for face training",
                Response = true
            };
            SheerResponse.ShowModalDialog(mdo);
            args.WaitForPostBack();
        }

        public override CommandState QueryState(CommandContext context)
        {
            if(Settings.MissingKeys())
                return CommandState.Disabled;

            Item ctxItem = DataWrapper?.ExtractItem(context);
            if (ctxItem == null || !MediaWrapper.IsMediaFile(ctxItem))
                return CommandState.Hidden;
            
            var hasPersons = PersonGroupService.GetAllPersonItems(ctxItem.Database.Name).Any();
            if(!hasPersons)
                return CommandState.Hidden;

            var faces = SearchService
                .GetImageAnalysis(ctxItem.ID.ToString(), ctxItem.Language.Name, ctxItem.Database.Name)
                ?.FacialAnalysis;
            var hasOneFace = faces != null && faces.Length == 1;
            if(!hasOneFace)
                return CommandState.Hidden;

            //show on images that have no face item pointing to them
            var links = Globals.LinkDatabase.GetItemReferrers(ctxItem, false);
            var hasFaceLink = links
                .Any(a => a.GetSourceItem().TemplateID.Guid == Settings.PersonFaceTemplateId.Guid);
            
            return hasFaceLink
                ? CommandState.Hidden
                : CommandState.Enabled;
        }
    }
}