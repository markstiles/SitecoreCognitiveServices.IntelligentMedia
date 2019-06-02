using System;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Shell.Applications.WebEdit.Commands;
using Sitecore.Shell.Framework.Commands;
using Sitecore.Web;
using Sitecore.Web.UI.Sheer;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Commands
{
    [Serializable]
    public class WebEditImageSearch : WebEditImageCommand
    {
        protected static IIntelligentMediaSettings _settings => DependencyResolver.Current.GetService<IIntelligentMediaSettings>();

        public override void Execute(CommandContext context)
        {
            Assert.ArgumentNotNull((object)context, "context");
            WebEditCommand.ExplodeParameters(context);
            string formValue = WebUtil.GetFormValue("scPlainValue");
            context.Parameters.Add("fieldValue", formValue);
            Context.ClientPage.Start((object)this, "Run", context.Parameters);
        }

        protected static void Run(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            
            if (args.IsPostBack)
            {
                if (!args.HasResult || args.Result.Equals(string.Empty))
                    return;

                string controlIdParam = args.Parameters["controlid"];

                var imgValue = $"<image mediaid=\"{args.Result}\" />";

                SheerResponse.SetAttribute("scHtmlValue", "value", RenderImage(args, imgValue));
                SheerResponse.SetAttribute("scPlainValue", "value", imgValue);
                SheerResponse.Eval($"scSetHtmlValue('{controlIdParam}')");
                
                return;
            }

            string langParam = args.Parameters["language"];
            if (string.IsNullOrEmpty(langParam))
                langParam = WebUtil.GetQueryString("la");

            var fieldSource = GetFieldSource(args);
            SheerResponse.ShowModalDialog($"/SitecoreCognitiveServices/IntelligentMedia/SearchForm?src=FieldEditor&lang={langParam}&db={Client.ContentDatabase.Name}&fieldSrc={fieldSource}", "1000px", "600px", string.Empty, true);
            args.WaitForPostBack();
        }

        protected static string GetFieldSource(ClientPipelineArgs args)
        {
            var fieldIdValue = args.Parameters["fieldid"];
            if (string.IsNullOrWhiteSpace(fieldIdValue))
                return string.Empty;

            var fieldId = new Sitecore.Data.ID(fieldIdValue);
            var fieldItem = Context.ContentDatabase.GetItem(fieldId);
            if(fieldItem == null)
                return string.Empty;
            
            var fieldSource = fieldItem.Fields["Source"];
            if(fieldSource == null)
                return string.Empty;

            return fieldSource.Value;
        }

        public override CommandState QueryState(CommandContext context)
        {
            return _settings.MissingKeys()
                ? CommandState.Disabled
                : CommandState.Enabled;
        }
    }
}