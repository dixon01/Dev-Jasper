// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameCheck.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameCheck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    /// <summary>
    /// The result of <see cref="IFrameController.VerifyIncoming"/>.
    /// </summary>
    public enum FrameCheck
    {
        /// <summary>
        /// The the frame has the correct sequence number.
        /// </summary>
        Ok,

        /// <summary>
        /// There are frames missing between the last frame received and the current frame.
        /// </summary>
        MissingFrame,

        /// <summary>
        /// The current frame has already arrived once before.
        /// </summary>
        DuplicateFrame
    }
}