// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutLine.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutLine type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Layouting
{
    using System.Collections.Generic;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// One line of text in the layout.
    /// </summary>
    /// <typeparam name="T">
    /// The type of items stored in this list.
    /// </typeparam>
    internal class LayoutLine<T> : List<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutLine{T}"/> class.
        /// </summary>
        /// <param name="horizontalAlignment">
        /// The optional horizontal alignment for this line.
        /// If this value is not set, the default value from the layout element should be used.
        /// </param>
        public LayoutLine(HorizontalAlignment? horizontalAlignment)
        {
            this.HorizontalAlignment = horizontalAlignment;
        }

        /// <summary>
        /// Gets the horizontal alignment for this line.
        /// If this value is not set, the default value from the layout element should be used.
        /// </summary>
        public HorizontalAlignment? HorizontalAlignment { get; private set; }
    }
}
