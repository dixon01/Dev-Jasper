// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
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
