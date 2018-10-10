// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebmediaSection.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WebmediaSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Common.Configuration.Infomedia.Webmedia;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The web section.
    /// </summary>
    public class WebmediaSection : VideoPlaybackSectionBase
    {
        private readonly WebmediaSectionConfig sectionConfig;

        private readonly LayoutConfig layoutConfig;

        private readonly IPool<WebmediaElementBase> webmediaPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebmediaSection"/> class.
        /// </summary>
        /// <param name="sectionConfig">
        /// The web config.
        /// </param>
        /// <param name="poolManager">
        /// The pool manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public WebmediaSection(
            WebmediaSectionConfig sectionConfig, PoolManager poolManager, IPresentationContext context)
            : base(sectionConfig, context)
        {
            this.sectionConfig = sectionConfig;
            this.Logger.Info("Loading webmedia section {0}", sectionConfig.Filename);
            this.webmediaPool =
                poolManager.GetWebmediaPool(context.Config.GetAbsolutePathRelatedToConfig(sectionConfig.Filename));

            this.layoutConfig = context.Config.Config.Layouts.Find(l => l.Name == sectionConfig.Layout);
            if (this.layoutConfig == null)
            {
                this.Logger.Warn("Couldn't find layout {0} for webmedia section", sectionConfig.Layout);
            }
        }

        /// <summary>
        /// Gets the currently shown webmedia element (or null if non is shown).
        /// This is either an <see cref="ImageElement"/>, a <see cref="VideoElement"/> or a <see cref="GroupElement"/>.
        /// </summary>
        public GraphicalElementBase CurrentElement { get; private set; }

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
            if (!this.webmediaPool.MoveNext(true))
            {
                return null;
            }

            var webElement = this.webmediaPool.CurrentItem;
            var layout = this.CreateLayout(webElement);
            if (layout == null)
            {
                return null;
            }

            return new Page(layout, webElement.Duration);
        }

        /// <summary>
        /// Check if we are currently showing a video.
        /// </summary>
        /// <returns>
        /// True if we are showing a video, otherwise false.
        /// </returns>
        protected override bool IsShowingVideo()
        {
            if (this.CurrentElement is VideoElement)
            {
                return true;
            }

            var group = this.CurrentElement as GroupElement;
            if (group == null)
            {
                return false;
            }

            foreach (var element in group.Elements)
            {
                if (element is VideoElement)
                {
                    return true;
                }
            }

            return false;
        }

        private FrameLayout CreateLayout(WebmediaElementBase webElement)
        {
            this.CurrentElement = this.ConvertWebElement(webElement);
            if (this.CurrentElement == null)
            {
                return null;
            }

            return new FrameLayout(
                this.CurrentElement, webElement.Frame, this.layoutConfig, this.Context.Config.Config);
        }

        private GraphicalElementBase ConvertWebElement(WebmediaElementBase element)
        {
            var image = element as ImageWebmediaElement;
            if (image != null)
            {
                return new ImageElement { Filename = image.Filename, Scaling = image.Scaling };
            }

            var video = element as VideoWebmediaElement;
            if (video != null)
            {
                return new VideoElement { VideoUri = video.VideoUri, Scaling = video.Scaling };
            }

            var layout = element as LayoutWebmediaElement;
            if (layout != null)
            {
                return new GroupElement { Elements = layout.Elements.ConvertAll(e => (GraphicalElementBase)e.Clone()) };
            }

            this.Logger.Warn("Couldn't create layout element for {0}", element);
            return null;
        }
    }
}
