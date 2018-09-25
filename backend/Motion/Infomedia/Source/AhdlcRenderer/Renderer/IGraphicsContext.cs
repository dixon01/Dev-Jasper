// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IGraphicsContext.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IGraphicsContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.AhdlcRenderer.Renderer
{
    using Gorba.Common.Formats.AlphaNT.Bitmaps;

    /// <summary>
    /// Interface for classes that allow bitmaps to be rendered on.
    /// </summary>
    public interface IGraphicsContext
    {
        /// <summary>
        /// Draws an <see cref="IBitmap"/> onto this context.
        /// </summary>
        /// <param name="offsetX">
        /// The x position where to draw the bitmap.
        /// </param>
        /// <param name="offsetY">
        /// The y position where to draw the bitmap.
        /// </param>
        /// <param name="bitmap">
        /// The bitmap to draw.
        /// </param>
        void DrawBitmap(int offsetX, int offsetY, IBitmap bitmap);
    }
}