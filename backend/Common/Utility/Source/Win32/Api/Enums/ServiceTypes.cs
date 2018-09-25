// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceTypes.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceTypes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.Enums
{
    using System;

    /// <summary>
    /// The type of service.
    /// </summary>
    [Flags]
    public enum ServiceTypes : uint
    {
        /// <summary>
        /// The service is a device driver.
        /// </summary>
        KernelDriver = 0x00000001,

        /// <summary>
        /// The service is a file system driver.
        /// </summary>
        FileSystemDriver = 0x00000002,

        /// <summary>
        /// The service runs in its own process.
        /// </summary>
        Win32OwnProcess = 0x00000010,

        /// <summary>
        /// The service shares a process with other services.
        /// </summary>
        Win32ShareProcess = 0x00000020,

        /// <summary>
        /// The service can interact with the desktop.
        /// </summary>
        InteractiveProcess = 0x00000100
    }
}