// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.BbCode;
    using Gorba.Motion.Infomedia.Entities.Messages;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Presenter for an <see cref="ImageElement"/>.
    /// It creates an <see cref="ImageItem"/>.
    /// </summary>
    public partial class ImageComposer
    {
        /// <summary>
        /// Method to check if the enabled property on this element is true.
        /// This method also checks the parent's enabled property.
        /// If no dynamic "Enabled" property is defined, this method returns true.
        /// </summary>
        /// <returns>
        /// true if there is no property defined or if the property evaluates to true is valid.
        /// </returns>
        public override bool IsVisible()
        {
            return base.IsVisible() && !string.IsNullOrEmpty(this.Item.Filename) && File.Exists(this.Item.Filename);
        }

        partial void Update()
        {
            var fileName = this.HandlerFilename.StringValue;

            var parser = new BbParser();
            var root = parser.Parse(fileName);
            var path = root.ToPlainString();
            if (!string.IsNullOrEmpty(path))
            {
                path = this.Context.Config.GetAbsolutePathRelatedToConfig(path);
            }

            this.Item.Blink = root.FindNodesOfType<Blink>().GetEnumerator().MoveNext();
            this.Item.SetFilename(path, this.HandlerFilename.Animation);
            this.SetItemVisibility();
            this.SendElementUpdateMessage(this.Element, this.Element.Filename, DrawableStatus.Initialized);
        }

        partial void Deinitialize()
        {
            this.SendElementUpdateMessage(this.Element, this.Element.Filename, DrawableStatus.Disposing);
        }
    }
}