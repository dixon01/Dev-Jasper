// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImageTexture.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IImageTexture type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.DirectXRenderer.Engine
{
    using System.Drawing;

    using Microsoft.DirectX.Direct3D;

    /// <summary>
    /// Image texture interface that allows to draw the image to a given sprite.
    /// </summary>
    public interface IImageTexture
    {
        /// <summary>
        /// Gets the size of the original image.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Draws this image to the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="srcRectangle">
        /// The source rectangle.
        /// </param>
        /// <param name="destinationSize">
        /// The destination size.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        void DrawTo(Sprite sprite, Rectangle srcRectangle, SizeF destinationSize, PointF position, Color color);

        /// <summary>
        /// Draws this image with the given rotation to the given sprite.
        /// </summary>
        /// <param name="sprite">
        /// The sprite.
        /// </param>
        /// <param name="srcRectangle">
        /// The source rectangle.
        /// </param>
        /// <param name="destinationSize">
        /// The destination size.
        /// </param>
        /// <param name="rotationCenter">
        /// The rotation center.
        /// </param>
        /// <param name="rotationAngle">
        /// The rotation angle.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <param name="color">
        /// The color.
        /// </param>
        void DrawTo(
            Sprite sprite,
            Rectangle srcRectangle,
            SizeF destinationSize,
            PointF rotationCenter,
            float rotationAngle,
            PointF position,
            Color color);
    }
}