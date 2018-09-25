// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoElementDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    /// <summary>
    /// The video element data model.
    /// </summary>
    public partial class VideoElementDataModel
    {
        /// <summary>
        /// Gets or sets the hash of the video file.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the hash of the preview image.
        /// </summary>
        public string PreviewImageHash { get; set; }
    }
}
