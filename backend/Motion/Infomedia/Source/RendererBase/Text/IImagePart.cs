// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IImagePart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IImagePart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    /// <summary>
    /// Interface for a part of a formatted text that shows an image.
    /// </summary>
    public interface IImagePart : IPart
    {
        /// <summary>
        /// Gets the image file name.
        /// </summary>
        string FileName { get; }
    }
}