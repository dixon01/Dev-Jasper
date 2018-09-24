// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationReason.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationReason type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.ServiceModel
{
    /// <summary>
    /// Reason for an application launch or exit.
    /// </summary>
    public enum ApplicationReason
    {
        /// <summary>
        /// The reason is unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// The the launch or exit was requested by the user or an application.
        /// </summary>
        Requested,

        /// <summary>
        /// The system booted (or System Manager started).
        /// </summary>
        SystemBoot,

        /// <summary>
        /// The system was shutting down.
        /// </summary>
        SystemShutdown,

        /// <summary>
        /// The system crashed (i.e. no proper shutdown).
        /// </summary>
        SystemCrash,

        /// <summary>
        /// The application re-launched itself.
        /// </summary>
        ApplicationRelaunch,

        /// <summary>
        /// The application exited.
        /// </summary>
        ApplicationExit
    }
}