// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    /// <summary>
    /// The mode to be used when rendering videos.
    /// </summary>
    public enum VideoMode
    {
        /// <summary>
        /// Render videos using DirectShow.
        /// </summary>
        DirectShow,

        /// <summary>
        /// Render videos using DirectX and a separate window.
        /// </summary>
        DirectXWindow,

        /// <summary>
        /// Render videos using VLC media player in a separate window.
        /// </summary>
        VlcWindow
    }
}
