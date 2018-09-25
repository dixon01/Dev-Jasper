// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameLayout.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameLayout type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Layout
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Layout that replaces the <see cref="FrameElement"/> placeholder with
    /// the given element. This layout is used for Webmedia, Pool and Image/Video sections.
    /// </summary>
    public class FrameLayout : StandardLayout
    {
        private readonly GraphicalElementBase replaceElement;

        private readonly int frameId;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameLayout"/> class.
        /// </summary>
        /// <param name="replaceElement">
        /// The element that will replace the <see cref="FrameElement"/> in the layout.
        /// </param>
        /// <param name="frameId">
        /// The frame id that should be replace. Only exactly this frame id will be
        /// replaced, all other <see cref="FrameElement"/>s are skipped.
        /// </param>
        /// <param name="layoutConfig">
        /// The layout config.
        /// </param>
        /// <param name="config">
        /// The Infomedia config.
        /// </param>
        public FrameLayout(
            GraphicalElementBase replaceElement, int frameId, LayoutConfig layoutConfig, InfomediaConfig config)
            : base(layoutConfig, config)
        {
            this.replaceElement = replaceElement;
            this.frameId = frameId;
        }

        /// <summary>
        /// Gets the name of this layout.
        /// </summary>
        public override string Name
        {
            get
            {
                // we need a unique name for every layout depending on the replaced content
                return string.Format("{0}#{1}<{2}>", base.Name, this.frameId, this.replaceElement);
            }
        }

        /// <summary>
        /// Loads all layout elements for a given resolution by querying
        /// the <see cref="LayoutConfig.Resolutions"/> and then handling also
        /// base layouts (see <see cref="LayoutConfig.BaseLayoutName"/>).
        /// All <see cref="FrameElement"/>s are either replaced (if they contain
        /// the right <see cref="FrameElement.FrameId"/>) or skipped.
        /// </summary>
        /// <param name="width">
        /// The width of the target area.
        /// </param>
        /// <param name="height">
        /// The height of the target area.
        /// </param>
        /// <returns>
        /// An enumeration over all elements contained in this layout for the given resolution.
        /// </returns>
        public override IEnumerable<ElementBase> LoadLayoutElements(int width, int height)
        {
            if (this.frameId == 0)
            {
                // we are in fullscreen mode, we just need a single element with the given size
                var replacement = (GraphicalElementBase)this.replaceElement.Clone();
                replacement.X = 0;
                replacement.Y = 0;
                replacement.Width = width;
                replacement.Height = height;
                replacement.Visible = true;
                replacement.VisibleProperty = null;

                var drawable = replacement as DrawableElementBase;
                if (drawable != null)
                {
                    drawable.ZIndex = 0;
                }

                yield return replacement;
                yield break;
            }

            foreach (var element in this.LoadDefaultLayoutElements(width, height))
            {
                var frameElement = element as FrameElement;
                if (frameElement != null)
                {
                    if (frameElement.FrameId != this.frameId)
                    {
                        continue;
                    }

                    // copy all properties
                    var replacement = (GraphicalElementBase)this.replaceElement.Clone();
                    replacement.X = frameElement.X;
                    replacement.Y = frameElement.Y;
                    replacement.Width = frameElement.Width;
                    replacement.Height = frameElement.Height;
                    replacement.Visible = frameElement.Visible;
                    replacement.VisibleProperty = frameElement.VisibleProperty;

                    var drawable = replacement as DrawableElementBase;
                    if (drawable != null)
                    {
                        drawable.ZIndex = frameElement.ZIndex;
                    }

                    yield return replacement;
                    continue;
                }

                yield return element;
            }
        }

        private IEnumerable<ElementBase> LoadDefaultLayoutElements(int width, int height)
        {
            // added this method to generate verifiable code in CF 3.5
            return base.LoadLayoutElements(width, height);
        }
    }
}