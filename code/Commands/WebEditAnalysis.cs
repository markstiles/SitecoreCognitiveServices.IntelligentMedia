using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;
using SitecoreCognitiveServices.Feature.IntelligentMedia.Services;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class WebEditAnalysis : WebEditImageCommand
    {
        protected static IIntelligentMediaSettings Settings => DependencyResolver.Current.GetService<IIntelligentMediaSettings>();
        protected static ISitecoreDataWrapper DataWrapper => DependencyResolver.Current.GetService<ISitecoreDataWrapper>();
        protected static IMediaWrapper MediaWrapper => DependencyResolver.Current.GetService<IMediaWrapper>();

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull(context, "context");
            ExplodeParameters(context);
            string formValue = WebUtil.GetFormValue("scPlainValue");
            context.Parameters.Add("fieldValue", formValue);
            Context.ClientPage.Start(this, "Run", context.Parameters);
        }
        
        protected static void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (args.IsPostBack)
                return;

            var itemId = ID.Parse(args.Parameters["itemid"]);
            var language = Language.Parse(args.Parameters["language"]);
            var version = Sitecore.Data.Version.Parse(args.Parameters["version"]);
            var fieldId = ID.Parse(args.Parameters["fieldId"]);

            Database database = Context.ContentDatabase ?? Context.Database;
            Assert.IsNotNull(database, "database");

            Item ctxItem = database.GetItem(itemId, language, version);
            ImageField field = ctxItem?.Fields[fieldId];
            if (field?.MediaItem == null)
                return;

            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/ImageAnalysis?id={field.MediaItem.ID}&language={language.Name}&db={database.Name}")
            {
                Header = "Cognitive Analysis",
                Height = DataWrapper.GetFieldDimension(field.MediaItem, "height", 500, 60),
                Width = DataWrapper.GetFieldDimension(field.MediaItem, "width", 810, 41),
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

            if (!context.Items.Any())
                return CommandState.Disabled;

            var ctxItem = context.Items[0];
            if (ctxItem == null)
                return CommandState.Hidden;

            if (context.Parameters.Count < 2)
                return CommandState.Hidden;

            var fieldName =  context.Parameters[1];
            ImageField field = ctxItem.Fields[fieldName];
            if (field?.MediaItem == null || !MediaWrapper.IsMediaFile(field.MediaItem))
                return CommandState.Hidden;

            return CommandState.Enabled;
        }
    }
}