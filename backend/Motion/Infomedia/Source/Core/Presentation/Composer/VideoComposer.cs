// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoComposer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System;
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Medi.Core;
    using Gorba.Motion.Infomedia.Entities.Messages;

    /// <summary>
    /// Presenter for a <see cref="VideoElement"/>.
    /// </summary>
    public partial class VideoComposer
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
            return base.IsVisible() && !string.IsNullOrEmpty(this.Item.VideoUri)
                   && (this.Item.VideoUri.IndexOf("://", StringComparison.InvariantCulture) > 0
                       || File.Exists(this.Item.VideoUri));
        }

        partial void Update()
        {
            var path = string.Empty;
            if (this.HandlerVideoUri.StringValue.IndexOf("://", StringComparison.InvariantCulture) > 0)
            {
                path = this.HandlerVideoUri.StringValue;
            }
            else if (!string.IsNullOrEmpty(this.HandlerVideoUri.StringValue))
            {
                path = this.Context.Config.GetAbsolutePathRelatedToConfig(this.HandlerVideoUri.StringValue);
            }

            if (!string.IsNullOrEmpty(this.Item.FallbackImage))
            {
                var fallbackImagePath = this.Context.Config.GetAbsolutePathRelatedToConfig(this.Item.FallbackImage);
                this.Item.SetFallbackImage(fallbackImagePath, new PropertyChangeAnimation());
            }

            this.Item.SetVideoUri(path, this.HandlerVideoUri.Animation);
            this.SetItemVisibility();
            this.SendElementUpdateMessage(this.Element, this.Element.VideoUri, DrawableStatus.Initialized);
        }

        partial void Deinitialize()
        {
            this.SendElementUpdateMessage(this.Element, this.Element.VideoUri, DrawableStatus.Disposing);
        }
    }
}