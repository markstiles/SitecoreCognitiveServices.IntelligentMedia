using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Links;

namespace SitecoreCognitiveServices.Feature.IntelligentMedia.Fields
{
    public class IntelligentImageField : XmlField
    {
        /// <summary>The _media version.</summary>
        private Sitecore.Data.Version mediaVersion = Sitecore.Data.Version.Latest;
        /// <summary>The _media database.</summary>
        private Database mediaDatabase;
        /// <summary>The _media item.</summary>
        private Item mediaItem;
        /// <summary>The _media language.</summary>
        private Language mediaLanguage;

        /// <summary>Gets or sets the Alt text.</summary>
        /// <value>The alt text.</value>
        public string Alt
        {
            get
            {
                string str = this.GetAttribute("alt");
                if (str.Length == 0)
                {
                    Item mediaItem = this.MediaItem;
                    if (mediaItem != null)
                    {
                        str = ((Sitecore.Data.Items.MediaItem)mediaItem).Alt;
                        if (str.Length == 0)
                            str = mediaItem["Alt"];
                    }
                }
                return str;
            }
            set
            {
                this.SetAttribute("alt", value);
            }
        }

        /// <summary>Gets or sets the Border value.</summary>
        /// <value>The Border value.</value>
        public string Border
        {
            get
            {
                return this.GetAttribute("border");
            }
            set
            {
                this.SetAttribute("border", value);
            }
        }

        /// <summary>Gets or sets the HTML class.</summary>
        /// <value>The class.</value>
        public string Class
        {
            get
            {
                return this.GetAttribute("class");
            }
            set
            {
                this.SetAttribute("class", value);
            }
        }

        /// <summary>Gets or sets the height.</summary>
        /// <value>The height.</value>
        public string Height
        {
            get
            {
                string attribute = this.GetAttribute("height");
                if (attribute.Length > 0)
                    return attribute;
                Item mediaItem = this.MediaItem;
                if (mediaItem != null)
                    return mediaItem["height"];
                return string.Empty;
            }
            set
            {
                this.SetAttribute("height", value);
            }
        }

        /// <summary>Gets or sets the HSpace value.</summary>
        /// <value>The HSpace value.</value>
        public string HSpace
        {
            get
            {
                return this.GetAttribute("hspace");
            }
            set
            {
                this.SetAttribute("hspace", value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the image is internal.
        /// </summary>
        /// <value><c>true</c> if the image is internal; otherwise, <c>false</c>.</value>
        public bool IsInternal
        {
            get
            {
                if (string.Compare(this.LinkType, "internal", StringComparison.InvariantCultureIgnoreCase) != 0)
                    return !this.MediaID.IsNull;
                return true;
            }
        }

        /// <summary>Gets or sets the link type.</summary>
        /// <value>The type of the link.</value>
        public string LinkType
        {
            get
            {
                return this.GetAttribute("linktype");
            }
            set
            {
                this.SetAttribute("linktype", value);
                this.mediaItem = (Item)null;
            }
        }

        /// <summary>Gets or sets the database containing the the media.</summary>
        /// <value>The media database.</value>
        public Database MediaDatabase
        {
            get
            {
                if (this.mediaDatabase == null)
                    return this.InnerField.Item.Database;
                return this.mediaDatabase;
            }
            set
            {
                this.mediaDatabase = value;
                this.mediaItem = (Item)null;
            }
        }

        /// <summary>Gets or sets the ID of the media item.</summary>
        /// <value>The media ID.</value>
        public ID MediaID
        {
            get
            {
                string attribute = this.GetAttribute("mediaid");
                if (ID.IsID(attribute))
                    return ID.Parse(attribute);
                return ID.Null;
            }
            set
            {
                this.SetAttribute("mediaid", value.ToString());
                this.mediaItem = (Item)null;
            }
        }

        /// <summary>Gets the media item.</summary>
        /// <value>The media item.</value>
        public Item MediaItem
        {
            get
            {
                if (this.mediaItem != null)
                    return this.mediaItem;
                ID mediaId = this.MediaID;
                if (mediaId.IsNull)
                    return (Item)null;
                this.mediaItem = this.MediaDatabase.GetItem(mediaId, this.MediaLanguage, this.MediaVersion);
                return this.mediaItem;
            }
        }

        /// <summary>Gets or sets the language of the media item to use.</summary>
        /// <value>The media language.</value>
        public Language MediaLanguage
        {
            get
            {
                if (this.mediaLanguage == (Language)null)
                    return this.InnerField.Item.Language;
                return this.mediaLanguage;
            }
            set
            {
                this.mediaLanguage = value;
                this.mediaItem = (Item)null;
            }
        }

        /// <summary>Gets or sets the version of the media item to use.</summary>
        /// <value>The media version.</value>
        public Sitecore.Data.Version MediaVersion
        {
            get
            {
                return this.mediaVersion;
            }
            set
            {
                this.mediaVersion = value;
                this.mediaItem = (Item)null;
            }
        }

        /// <summary>Gets or sets the VSpace value.</summary>
        /// <value>The VSpace value.</value>
        public string VSpace
        {
            get
            {
                return this.GetAttribute("vspace");
            }
            set
            {
                this.SetAttribute("vspace", value);
            }
        }

        /// <summary>Gets or sets the width.</summary>
        /// <value>The width.</value>
        public string Width
        {
            get
            {
                string attribute = this.GetAttribute("width");
                if (attribute.Length > 0)
                    return attribute;
                Item mediaItem = this.MediaItem;
                if (mediaItem != null)
                    return mediaItem["Width"];
                return string.Empty;
            }
            set
            {
                this.SetAttribute("width", value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Data.Fields.ImageField" /> class.
        /// Creates a new <see cref="T:Sitecore.Data.Fields.ImageField" /> instance.
        /// </summary>
        /// <param name="innerField">Inner field.</param>
        /// <contract>
        ///   <requires name="innerField" condition="none" />
        /// </contract>
        public IntelligentImageField(Field innerField)
            : base(innerField, "image")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Sitecore.Data.Fields.ImageField" /> class.
        /// </summary>
        /// <param name="innerField">The inner field.</param>
        /// <param name="runtimeValue">The runtime value.</param>
        /// <contract>
        ///   <requires name="innerField" condition="none" />
        ///   <requires name="runtimeValue" condition="none" />
        /// </contract>
        public IntelligentImageField(Field innerField, string runtimeValue)
            : base(innerField, "image", runtimeValue)
        {
        }

        /// <summary>
        /// Converts a <see cref="T:Sitecore.Data.Fields.Field" /> to a <see cref="T:Sitecore.Data.Fields.ImageField" />.
        /// </summary>
        /// <param name="field">Field to convert.</param>
        public static implicit operator IntelligentImageField(Field field)
        {
            if (field != null)
                return new IntelligentImageField(field);
            return (IntelligentImageField)null;
        }

        /// <summary>Clears this instance.</summary>
        public void Clear()
        {
            this.Value = string.Empty;
        }

        /// <summary>Relinks the specified item.</summary>
        /// <param name="itemLink">The item link.</param>
        /// <param name="newLink">The new link.</param>
        /// <contract>
        ///   <requires name="itemLink" condition="not null" />
        ///   <requires name="newLink" condition="not null" />
        /// </contract>
        public override void Relink(ItemLink itemLink, Item newLink)
        {
            Assert.ArgumentNotNull((object)itemLink, "itemLink");
            Assert.ArgumentNotNull((object)newLink, "newLink");
            if (!(this.MediaID == itemLink.TargetItemID))
                return;
            this.MediaID = newLink.ID;
        }

        /// <summary>Removes the link.</summary>
        /// <param name="itemLink">The item link.</param>
        /// <contract>
        ///   <requires name="itemLink" condition="not null" />
        /// </contract>
        public override void RemoveLink(ItemLink itemLink)
        {
            Assert.ArgumentNotNull((object)itemLink, "itemLink");
            if (!(this.MediaID == itemLink.TargetItemID))
                return;
            this.Clear();
        }

        /// <summary>Validates the links.</summary>
        /// <param name="result">The result.</param>
        /// <contract>
        ///   <requires name="result" condition="not null" />
        /// </contract>
        public override void ValidateLinks(LinksValidationResult result)
        {
            Assert.ArgumentNotNull((object)result, "result");
            if (this.MediaItem != null)
            {
                result.AddValidLink(this.MediaItem, this.MediaItem.Paths.FullPath);
            }
            else
            {
                if (this.MediaID.IsNull)
                    return;
                result.AddBrokenLink(this.MediaID.ToString());
            }
        }
    }
}