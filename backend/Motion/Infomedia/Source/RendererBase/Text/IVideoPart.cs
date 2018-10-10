// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IVideoPart.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IVideoPart type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.RendererBase.Text
{
    /// <summary>
    /// Interface for a part of a formatted text that shows a video.
    /// </summary>
    public interface IVideoPart : IPart
    {
        /// <summary>
        /// Gets the video URI.
        /// </summary>
        string VideoUri { get; }
    }
}