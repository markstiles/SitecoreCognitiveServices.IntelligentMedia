using Sitecore.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class IdentifyPeople : BaseIntelligentMediaCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/IdentifyPeople?id={Id}&language={Language}&db={Db}")
            {
                Header = "Identify People",
                Height = DataWrapper.GetFieldDimension(ContextItem, "height", 500, 56),
                Width = DataWrapper.GetFieldDimension(ContextItem, "width", 810, 20),
                Message = "",
                Response = true
            };
            SheerResponse.ShowModalDialog(mdo);
            args.WaitForPostBack();
        }

        public override CommandState QueryState(CommandContext context)
        {
            if (Settings.MissingKeys())
                return CommandState.Disabled;

            Item ctxItem = DataWrapper?.ExtractItem(context);
            if(ctxItem == null || !MediaWrapper.IsMediaFile(ctxItem))
                return CommandState.Hidden;

            var persons = PersonGroupService.GetAllPersonItems(ctxItem.Database.Name);
            if(persons == null || !persons.Any())
                return CommandState.Hidden;

            var searchResult = SearchService.GetSearchResult(ctxItem.ID.ToString(), ctxItem.Language.Name, ctxItem.Database.Name);
            if(searchResult == null)
                return CommandState.Hidden;
            
            var faceIds = searchResult.FacialAnalysis?.Select(a => a.FaceId).ToArray() ?? new Guid[0];
            var peopleDiffer = searchResult.People == null || searchResult.People.Length != faceIds.Length;
 
            return peopleDiffer
                ? CommandState.Enabled
                : CommandState.Hidden;
        }
    }
}