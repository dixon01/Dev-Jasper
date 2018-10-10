// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RectangleComponent.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RectangleComponent type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Engines
{
    using Gorba.Common.Formats.AlphaNT.Common;

    /// <summary>
    /// The rectangle component.
    /// </summary>
    public class RectangleComponent : ComponentBase
    {
        /// <summary>
        /// Gets or sets the color or the rectangle.
        /// </summary>
        public IColor Color { get; set; }
    }
}