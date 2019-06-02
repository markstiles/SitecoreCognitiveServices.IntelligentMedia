using Sitecore.Text;
using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class TrainAllGroups : BaseIntelligentMediaCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/TrainAllGroups?id={Id}&language={Language}&db={Db}")
            {
                Header = "Train All Groups",
                Height = "200",
                Width = "400",
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

            return (ctxItem != null && ctxItem.ID.Equals(Settings.PersonGroupsFolderId))
                ? CommandState.Enabled
                : CommandState.Hidden;
        }
    }
}