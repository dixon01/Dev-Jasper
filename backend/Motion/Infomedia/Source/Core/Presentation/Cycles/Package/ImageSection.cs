// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageSection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ImageSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The image section.
    /// </summary>
    public class ImageSection : SinglePageSectionBase
    {
        private readonly ImageSectionConfig sectionConfig;

        private readonly LayoutConfig layoutConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageSection"/> class.
        /// </summary>
        /// <param name="sectionConfig">
        /// The section config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public ImageSection(ImageSectionConfig sectionConfig, IPresentationContext context)
            : base(sectionConfig, context)
        {
            this.sectionConfig = sectionConfig;

            this.layoutConfig = context.Config.Config.Layouts.Find(l => l.Name == sectionConfig.Layout);
            if (this.layoutConfig == null)
            {
                this.Logger.Warn("Couldn't find layout {0} for pool section", sectionConfig.Layout);
            }
        }

        /// <summary>
        /// Create the next single page to be shown by this section.
        /// </summary>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        protected override Page CreateNextPage()
        {
            var element = new ImageElement { Filename = this.sectionConfig.Filename, Scaling = ElementScaling.Scale };
            var layout = new FrameLayout(
                element, this.sectionConfig.Frame, this.layoutConfig, this.Context.Config.Config);

            return new Page(layout, this.sectionConfig.Duration);
        }
    }
}