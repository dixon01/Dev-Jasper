// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VideoMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VideoMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest.Config
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
        DirectXWindow
    }
}
