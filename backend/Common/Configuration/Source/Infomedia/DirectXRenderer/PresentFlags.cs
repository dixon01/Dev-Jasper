// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PresentFlags.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PresentFlags type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.DirectXRenderer
{
    using System;

    /// <summary>
    /// The presentation flags supported by DirectX.
    /// </summary>
    [Flags]
    public enum PresentFlags
    {
        /// <summary>
        /// The video.
        /// </summary>
        Video = 16,

        /// <summary>
        /// The device clip.
        /// </summary>
        DeviceClip = 4,

        /// <summary>
        /// The discard depth stencil.
        /// </summary>
        DiscardDepthStencil = 2,

        /// <summary>
        /// The lockable back buffer.
        /// </summary>
        LockableBackBuffer = 1,

        /// <summary>
        /// The none.
        /// </summary>
        None = 0
    }
}