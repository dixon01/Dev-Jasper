// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFrameController.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFrameController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    /// <summary>
    /// The Frame Controller interface.
    /// The frame controller is responsible for checking that
    /// frames arrive in sequence and duplicate frames are recognized (and then ignored).
    /// </summary>
    internal interface IFrameController
    {
        /// <summary>
        /// Gets the last acknowledged frame id.
        /// </summary>
        uint LastAcknowledgedFrameId { get; }

        /// <summary>
        /// Verifies an incoming frame.
        /// </summary>
        /// <param name="info">
        /// The frame info.
        /// </param>
        /// <returns>
        /// The <see cref="FrameCheck"/> result.
        /// </returns>
        FrameCheck VerifyIncoming(FrameInfo info);

        /// <summary>
        /// Gets the information for the next frame.
        /// </summary>
        /// <returns>
        /// A new <see cref="FrameInfo"/>.
        /// </returns>
        FrameInfo GetNextFrameInfo();
    }
}