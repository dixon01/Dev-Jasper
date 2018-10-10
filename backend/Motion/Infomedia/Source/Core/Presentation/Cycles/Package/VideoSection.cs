// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoSection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The video section.
    /// </summary>
    public class VideoSection : VideoPlaybackSectionBase
    {
        private readonly VideoSectionConfig sectionConfig;

        private readonly LayoutConfig layoutConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="VideoSection"/> class.
        /// </summary>
        /// <param name="sectionConfig">
        /// The section config.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public VideoSection(VideoSectionConfig sectionConfig, IPresentationContext context)
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
        /// Gets the way of dealing with a video that has
        /// a different duration than defined in this section.
        /// </summary>
        protected override VideoEndMode VideoEndMode
        {
            get
            {
                return this.sectionConfig.VideoEndMode;
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
            var element = new VideoElement
                              {
                                  VideoUri = this.sectionConfig.VideoUri,
                                  Scaling = ElementScaling.Scale,
                                  Replay = this.sectionConfig.VideoEndMode == VideoEndMode.Repeat
                              };
            var layout = new FrameLayout(
                element, this.sectionConfig.Frame, this.layoutConfig, this.Context.Config.Config);

            return new Page(layout, this.sectionConfig.Duration);
        }

        /// <summary>
        /// Check if we are currently showing a video.
        /// </summary>
        /// <returns>
        /// True if we are showing a video, otherwise false.
        /// </returns>
        protected override bool IsShowingVideo()
        {
            return true;
        }
    }
}