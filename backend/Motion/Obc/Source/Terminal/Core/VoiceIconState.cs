// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoiceIconState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the VoiceIconState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// State of the voice icon.
    /// </summary>
    public enum VoiceIconState
    {
        /// <summary>
        ///   Hides any voice icon
        /// </summary>
        None,

        /// <summary>
        ///   Shows a voice setup icon
        /// </summary>
        Requested,

        /// <summary>
        ///   Shows a voice connected icon
        /// </summary>
        Connected,

        /// <summary>
        ///   Shows a voice setup failed icon
        /// </summary>
        Failed,
    }
}