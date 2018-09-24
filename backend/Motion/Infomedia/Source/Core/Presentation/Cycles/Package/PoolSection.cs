// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PoolSection.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PoolSection type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Cycles.Package
{
    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Configuration.Infomedia.Presentation.Section;
    using Gorba.Motion.Infomedia.Core.Presentation.Layout;

    /// <summary>
    /// The pool section.
    /// </summary>
    public class PoolSection : VideoPlaybackSectionBase
    {
        private readonly PoolSectionConfig sectionConfig;

        private readonly LayoutConfig layoutConfig;

        private readonly IPool<DrawableElementBase> mediaPool;

        /// <summary>
        /// Initializes a new instance of the <see cref="PoolSection"/> class.
        /// </summary>
        /// <param name="sectionConfig">
        /// The pool config.
        /// </param>
        /// <param name="poolManager">
        /// The pool manager.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        public PoolSection(PoolSectionConfig sectionConfig, PoolManager poolManager, IPresentationContext context)
            : base(sectionConfig, context)
        {
            this.Logger.Info("Loading pool section {0}", sectionConfig.Pool);
            this.sectionConfig = sectionConfig;

            this.layoutConfig = context.Config.Config.Layouts.Find(l => l.Name == sectionConfig.Layout);
            if (this.layoutConfig == null)
            {
                this.Logger.Warn("Couldn't find layout {0} for pool section", sectionConfig.Layout);
            }

            this.mediaPool = poolManager.GetMediaPool(sectionConfig.Pool);
        }

        /// <summary>
        /// Gets the currently shown pool element (or null if non is shown).
        /// This is either an <see cref="ImageElement"/> or a <see cref="VideoElement"/>.
        /// </summary>
        public DrawableElementBase CurrentElement
        {
            get
            {
                return this.mediaPool.CurrentItem;
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
            if (!this.mediaPool.MoveNext(true))
            {
                return null;
            }

            var video = this.CurrentElement as VideoElement;
            if (video != null)
            {
                video.Replay = this.sectionConfig.VideoEndMode == VideoEndMode.Repeat;
            }

            var layout = new FrameLayout(
                this.CurrentElement, this.sectionConfig.Frame, this.layoutConfig, this.Context.Config.Config);

            this.Logger.Info("Changing to pool element {0}", this.CurrentElement);

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
            return this.CurrentElement is VideoElement;
        }
    }
}