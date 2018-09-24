// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Bus.Core.Route
{
    /// <summary>
    /// The service state.
    /// </summary>
    public enum ServiceState
    {
        /// <summary>
        /// No service is loaded.
        /// </summary>
        Free = 0,

        /// <summary>
        /// The duty request has to be sent to icenter.motion.
        /// </summary>
        ToSend = 1,

        /// <summary>
        /// The duty request is being sent to icenter.motion.
        /// </summary>
        Sending = 2,

        /// <summary>
        /// The duty request was sent to icenter.motion.
        /// </summary>
        Sent = 3,

        /// <summary>
        /// The duty was acknowledged locally or by icenter.motion.
        /// </summary>
        End = 4
    }
}