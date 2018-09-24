// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MasterLayout.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MasterLayout type.
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
    /// Layout used to represent a <see cref="MasterLayoutConfig"/>.
    /// </summary>
    public class MasterLayout : LayoutBase
    {
        private static readonly Logger Logger = LogHelper.GetLogger<MasterLayout>();

        private readonly MasterLayoutConfig layoutConfig;

        private readonly PhysicalScreenRefConfig screenRef;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterLayout"/> class.
        /// </summary>
        /// <param name="layoutConfig">
        /// The layout config.
        /// </param>
        /// <param name="screenConfig">
        /// The screen config for which the <see cref="PhysicalScreenRefConfig"/> has to
        /// be found in the <see cref="layoutConfig"/>.
        /// </param>
        public MasterLayout(MasterLayoutConfig layoutConfig, PhysicalScreenConfig screenConfig)
        {
            this.layoutConfig = layoutConfig;
            this.screenRef = layoutConfig.PhysicalScreens.Find(r => r.Reference == screenConfig.Name);

            if (this.screenRef == null)
            {
                Logger.Warn("Couldn't find screen {0} for layout {1}", screenConfig.Name, layoutConfig.Name);
            }
        }

        /// <summary>
        /// Gets the name of this layout.
        /// </summary>
        public override string Name
        {
            get { return this.layoutConfig.Name; }
        }

        /// <summary>
        /// Loads all layout elements, ignoring the resolution (it is defined in the screen already).
        /// This method returns all virtual displays of the given <see cref="PhysicalScreenRefConfig"/>.
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
            if (this.screenRef == null)
            {
                return new ElementBase[0];
            }

            return this.screenRef.VirtualDisplays.ConvertAll(r => (ElementBase)r);
        }
    }
}