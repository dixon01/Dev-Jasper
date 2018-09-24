// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowMode.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WindowMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.SharpDXRendererTest
{
    /// <summary>
    /// The render mode.
    /// </summary>
    public enum WindowMode
    {
        /// <summary>
        /// The presentation is shown in a window.
        /// </summary>
        Windowed,

        /// <summary>
        /// The presentation is shown in full screen taking ownership of the screen.
        /// </summary>
        FullScreenExclusive,

        /// <summary>
        /// The presentation is shown in a window extending to the entire screen.
        /// </summary>
        FullScreenWindowed
    }
}