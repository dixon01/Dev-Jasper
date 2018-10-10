// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IComposerContext.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IComposerContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Simulation.Composers
{
    using Gorba.Center.Media.Core.Models;

    /// <summary>
    /// Context for composers that provide simple access to regularly used objects.
    /// </summary>
    public interface IComposerContext
    {
        /// <summary>
        /// Gets the application state.
        /// </summary>
        IMediaApplicationState ApplicationState { get; }

        /// <summary>
        /// Gets the image file name for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the image resource.
        /// </param>
        /// <returns>
        /// The image file name.
        /// </returns>
        string GetImageFileName(string hash);

        /// <summary>
        /// The get resource filename by filename.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetResourceUriByFilename(string filename);

        /// <summary>
        /// Gets the video file URI for the given hash.
        /// </summary>
        /// <param name="hash">
        /// The hash of the video resource.
        /// </param>
        /// <returns>
        /// The video file URI.
        /// </returns>
        string GetVideoUri(string hash);
    }
}