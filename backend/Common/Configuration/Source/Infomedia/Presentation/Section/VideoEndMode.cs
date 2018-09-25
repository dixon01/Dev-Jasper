// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoEndMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoEndMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation.Section
{
    /// <summary>
    /// Enumeration with the possible ways of dealing with a video that has
    /// a different duration than defined in the section.
    /// </summary>
    public enum VideoEndMode
    {
        /// <summary>
        /// Adapt the section duration to match the length of the video.
        /// </summary>
        Adapt,

        /// <summary>
        /// If the video is shorter than defined in the section,
        /// the last frame will be shown for the remaining time.
        /// If the video is longer than defined in the section,
        /// it will be cut (i.e. not played to the end).
        /// </summary>
        Freeze,

        /// <summary>
        /// If the video is shorter than defined in the section,
        /// the video will be repeated for the remaining time.
        /// If the video is longer than defined in the section,
        /// it will be cut (i.e. not played to the end).
        /// </summary>
        Repeat
    }
}