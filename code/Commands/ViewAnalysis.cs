﻿using Sitecore.Text;
using System;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web.UI.Sheer;
using Sitecore.Data.Items;
using SitecoreCognitiveServices.Foundation.SCSDK.Commands;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class ViewAnalysis : BaseIntelligentMediaCommand
    {
        public virtual void Run(ClientPipelineArgs args)
        {
            if (args.IsPostBack)
                return;
            
            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/ImageAnalysis?id={Id}&language={Language}&db={Db}")
            {
                Header = "Cognitive Analysis",
                Height = "550",
                Width = "1100",
                Message = "View the cognitive analysis of the current item",
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
            if (ctxItem == null || !MediaWrapper.IsMediaFile(ctxItem))
                return CommandState.Hidden;
            
            return SearchService
                .GetImageAnalysis(ctxItem.ID.ToString(), ctxItem.Language.Name, ctxItem.Database.Name)
                .HasAnyAnalysis()
                ? CommandState.Enabled
                : CommandState.Hidden;
        }
    }
}