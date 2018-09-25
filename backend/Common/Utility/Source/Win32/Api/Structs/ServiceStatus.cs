// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceStatus.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceStatus type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Structs
{
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Win32.Api.Enums;

    /// <summary>
    /// Contains status information for a service.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        /// <summary>
        /// The size of of this structure.
        /// </summary>
        public static readonly int SizeOf = Marshal.SizeOf(typeof(ServiceStatus));

        /// <summary>
        /// The type of service.
        /// </summary>
        public ServiceTypes ServiceType;

        /// <summary>
        /// The current state of the service.
        /// </summary>
        public ServiceState CurrentState;

        /// <summary>
        /// The control codes the service accepts and processes in its handler function
        /// </summary>
        public uint ControlsAccepted;

        /// <summary>
        /// The error code the service uses to report an error that occurs when it is starting or stopping.
        /// </summary>
        public uint Win32ExitCode;

        /// <summary>
        /// A service-specific error code that the service returns when
        /// an error occurs while the service is starting or stopping.
        /// This value is ignored unless the Win32ExitCode member is set to ERROR_SERVICE_SPECIFIC_ERROR.
        /// </summary>
        public uint ServiceSpecificExitCode;

        /// <summary>
        /// The check-point value the service increments periodically to report
        /// its progress during a lengthy start, stop, pause, or continue operation.
        /// </summary>
        public uint CheckPoint;

        /// <summary>
        /// The estimated time required for a pending start, stop, pause, or continue operation, in milliseconds.
        /// </summary>
        public uint WaitHint;
    }
}
