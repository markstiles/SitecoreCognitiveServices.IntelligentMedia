using System.Web.Mvc;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using SitecoreCognitiveServices.Foundation.SCSDK.Wrappers;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Fields
{
    public class IntelligentImage : Sitecore.Shell.Applications.ContentEditor.Image
    {
        protected readonly ISitecoreDataWrapper DataWrapper;
        
        public IntelligentImage()
        {
            Class = "scContentControlImage";
            Change = "#";
            Activation = true;
            DataWrapper = DependencyResolver.Current.GetService<ISitecoreDataWrapper>();
        }
        
        public override void HandleMessage(Message message)
        {
            Assert.ArgumentNotNull(message, "message");

            if (message["id"] != ID)
                return;

            if (message.Name.Equals("intelligent:browse"))
                BrowseIntelligent();
            else if (message.Name.Equals("intelligent:view"))
                ViewIntelligent();
            else
                base.HandleMessage(message);
        }

        #region Browse

        protected void BrowseIntelligent()
        {
            if (!Disabled)
                Sitecore.Context.ClientPage.Start(this, "BrowseIntelligentImage");
        }

        protected void BrowseIntelligentImage(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (!args.IsPostBack)
            {
                Language language = Language.Parse(ItemLanguage);
                SheerResponse.ShowModalDialog($"/SitecoreCognitiveServices/IntelligentMedia/SearchForm?src=FieldEditor&lang={language.Name}&db={Client.ContentDatabase.Name}&fieldSrc={Source}", "1000px", "600px", string.Empty, true);
                args.WaitForPostBack();

                return;
            }

            if (!args.HasResult || string.IsNullOrEmpty(args.Result) || args.Result == "undefined")
                return;

            MediaItem mediaItem = Client.ContentDatabase.Items[args.Result];
            if (mediaItem == null)
            {
                SheerResponse.Alert("Item not found.");
                return;
            }

            TemplateItem template = mediaItem.InnerItem.Template;
            if (template != null && !IsImageMedia(template))
            {
                SheerResponse.Alert("The selected item does not contain an image.");
                return;
            }

            XmlValue.SetAttribute("mediaid", mediaItem.ID.ToString());
            Value = mediaItem.MediaPath;
            Update();
            SetModified();
        }
        
        protected bool IsImageMedia(TemplateItem template)
        {
            Assert.ArgumentNotNull(template, "template");
            if (template.ID == TemplateIDs.VersionedImage || template.ID == TemplateIDs.UnversionedImage)
                return true;

            foreach (TemplateItem baseTemplate in template.BaseTemplates)
            {
                if (IsImageMedia(baseTemplate))
                    return true;
            }

            return false;
        }

        #endregion

        #region View

        protected void ViewIntelligent()
        {
            if (!Disabled)
                Sitecore.Context.ClientPage.Start(this, "ViewIntelligentImage");
        }

        protected void ViewIntelligentImage(ClientPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.IsPostBack)
                return;

            string attribute = XmlValue.GetAttribute("mediaid");
            if (string.IsNullOrEmpty(attribute))
            {
                SheerResponse.Alert("Select an image from the Media Library first.");
                return;
            }

            Language language = Language.Parse(ItemLanguage);
            Item i = Client.ContentDatabase.GetItem(attribute, language);

            ModalDialogOptions mdo = new ModalDialogOptions($"/SitecoreCognitiveServices/IntelligentMedia/ImageAnalysis?id={attribute}&language={language.Name}&db={Client.ContentDatabase.Name}")
            {
                Header = "Cognitive Analysis",
                Height = DataWrapper.GetFieldDimension(i, "height", 500, 60),
                Width = DataWrapper.GetFieldDimension(i, "width", 810, 41),
                Message = "View the cognitive analysis of the current item",
                Response = true
            };
            SheerResponse.ShowModalDialog(mdo);
            args.WaitForPostBack();
        }
        
        #endregion
    }
}