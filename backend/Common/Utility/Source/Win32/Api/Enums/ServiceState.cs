// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    /// <summary>
    /// The current state of a service.
    /// </summary>
    public enum ServiceState : uint
    {
        /// <summary>
        /// The service is not running.
        /// </summary>
        Stopped = 0x00000001,

        /// <summary>
        /// The service is starting.
        /// </summary>
        StartPending = 0x00000002,

        /// <summary>
        /// The service is stopping.
        /// </summary>
        StopPending = 0x00000003,

        /// <summary>
        /// The service is running.
        /// </summary>
        Running = 0x00000004,

        /// <summary>
        /// The service continue is pending.
        /// </summary>
        ContinuePending = 0x00000005,

        /// <summary>
        /// The service pause is pending.
        /// </summary>
        PausePending = 0x00000006,

        /// <summary>
        /// The service is paused.
        /// </summary>
        Paused = 0x00000007,
    }
}