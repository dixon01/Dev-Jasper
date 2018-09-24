// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RenderHelper.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RenderHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase
{
    using System.Drawing;

    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// Helper class for renderer implementations.
    /// </summary>
    public static class RenderHelper
    {
        /// <summary>
        /// Applies the given scaling to the bounds by looking a the original size.
        /// </summary>
        /// <param name="bounds">
        /// The bounds.
        /// </param>
        /// <param name="origSize">
        /// The original size.
        /// </param>
        /// <param name="scaling">
        /// The scaling.
        /// </param>
        /// <returns>
        /// The new <see cref="Rectangle"/>.
        /// </returns>
        public static Rectangle ApplyScaling(Rectangle bounds, Size origSize, ElementScaling scaling)
        {
            if (scaling == ElementScaling.Fixed)
            {
                return new Rectangle(
                    bounds.X + ((bounds.Width - origSize.Width) / 2),
                    bounds.Y + ((bounds.Height - origSize.Height) / 2),
                    origSize.Width,
                    origSize.Height);
            }

            if (scaling != ElementScaling.Scale)
            {
                return bounds;
            }

            var aspectRatio = (double)origSize.Width / origSize.Height;

            if (bounds.Width / aspectRatio < bounds.Height)
            {
                // adjust height
                var height = (int)(bounds.Width / aspectRatio);
                return new Rectangle(bounds.X, bounds.Y + ((bounds.Height - height) / 2), bounds.Width, height);
            }
            
            var width = (int)(bounds.Height * aspectRatio);
            return new Rectangle(bounds.X + ((bounds.Width - width) / 2), bounds.Y, width, bounds.Height);
        }
    }
}
