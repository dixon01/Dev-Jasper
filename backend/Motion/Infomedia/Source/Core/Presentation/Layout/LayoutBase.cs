// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Core.Presentation.Layout
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Base class for all layout logic implementations.
    /// </summary>
    public abstract class LayoutBase
    {
        /// <summary>
        /// Gets the name of this layout.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Loads all layout elements for a given resolution.
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
        public abstract IEnumerable<ElementBase> LoadLayoutElements(int width, int height);
    }
}