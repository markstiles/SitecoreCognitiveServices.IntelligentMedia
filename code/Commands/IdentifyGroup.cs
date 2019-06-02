using Sitecore.Text;
using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class IdentifyGroup : BaseIntelligentMediaCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            var groupId = ContextItem.Fields[Settings.FaceEntityIdFieldId]?.Value;

            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/IdentifyGroup?id={Id}&language={Language}&db={Db}&groupId={groupId}")
            {
                Header = "Identify Group",
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
            
            return (ctxItem != null && ctxItem.TemplateID.Equals(Settings.PersonGroupTemplateId))
                ? CommandState.Enabled
                : CommandState.Hidden;
        }
    }
}