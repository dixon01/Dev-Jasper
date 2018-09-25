// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioFileComposer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AudioFileComposer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Composer
{
    using System.IO;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Motion.Infomedia.Entities.Screen;

    /// <summary>
    /// Composer that converts <see cref="AudioFileElement"/> to <see cref="AudioFileItem"/>.
    /// </summary>
    public partial class AudioFileComposer
    {
        /// <summary>
        /// Method to check if the enabled property on this element is true.
        /// This method also checks the parent's enabled property.
        /// If no dynamic "Enabled" property is defined, this method returns true.
        /// Subclasses can override this method to provide additional
        /// evaluation (e.g. check if files are available or other conditions
        /// are met).
        /// </summary>
        /// <returns>
        /// true if there is no property defined or if the property evaluates to true is valid.
        /// </returns>
        protected override bool IsEnabled()
        {
            return base.IsEnabled() && !string.IsNullOrEmpty(this.Item.Filename) && File.Exists(this.Item.Filename);
        }

        partial void Update()
        {
            this.Item.Filename = this.Context.Config.GetAbsolutePathRelatedToConfig(this.HandlerFilename.StringValue);
            this.Item.Enabled = this.IsEnabled();
        }
    }
}