// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StandardLayout.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StandardLayout type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Layout
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// A normal layout made from a <see cref="LayoutConfig"/>.
    /// </summary>
    public class StandardLayout : LayoutBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<StandardLayout>();

        private readonly LayoutConfig layoutConfig;
        private readonly InfomediaConfig config;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardLayout"/> class.
        /// </summary>
        /// <param name="layoutConfig">
        /// The layout config.
        /// </param>
        /// <param name="config">
        /// The Infomedia config.
        /// </param>
        public StandardLayout(LayoutConfig layoutConfig, InfomediaConfig config)
        {
            this.layoutConfig = layoutConfig;
            this.config = config;
        }

        /// <summary>
        /// Gets the name of this layout.
        /// </summary>
        public override string Name
        {
            get
            {
                return this.layoutConfig.Name;
            }
        }

        /// <summary>
        /// Loads all layout elements for a given resolution by querying
        /// the <see cref="LayoutConfig.Resolutions"/> and then handling also
        /// base layouts (see <see cref="LayoutConfig.BaseLayoutName"/>).
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
            var nestedLayouts = new List<LayoutConfig>();
            return this.GetAllElements(this.layoutConfig, width, height, nestedLayouts);
        }

        private IEnumerable<ElementBase> GetAllElements(
            LayoutConfig layout, int width, int height, List<LayoutConfig> nestedLayouts)
        {
            var resolution = layout.Resolutions.Find(r => r.Width == width && r.Height == height);
            if (resolution == null)
            {
                Logger.Warn(
                    "Couldn't find resolution {0}x{1} for layout {2}",
                    width,
                    height,
                    layout.Name);
                yield break;
            }

            foreach (var element in resolution.Elements)
            {
                yield return element;
            }

            nestedLayouts.Add(layout);

            var baseLayout = this.config.Layouts.Find(l => l.Name == layout.BaseLayoutName);
            if (baseLayout == null)
            {
                yield break;
            }

            if (nestedLayouts.Contains(baseLayout))
            {
                Logger.Warn(
                    "Found recursive base layouts: {0} references {1} which is already used in {2}",
                    layout.Name,
                    baseLayout.Name,
                    this.layoutConfig.Name);
                yield break;
            }

            foreach (var element in this.GetAllElements(baseLayout, width, height, nestedLayouts))
            {
                yield return element;
            }
        }
    }
}