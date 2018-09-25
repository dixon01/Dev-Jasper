// --------------------------------------------------------------------------------------------------------------------
// <copyright file="OperatingSystemVersion.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the OperatingSystemVersion type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    /// <summary>
    /// The supported operating system versions.
    /// </summary>
    public enum OperatingSystemVersion
    {
        /// <summary>
        /// Windows XP Embedded.
        /// </summary>
        WindowsXPe,

        /// <summary>
        /// Windows Embedded 8 standard.
        /// </summary>
        WindowsEmbedded8Standard,

        /// <summary>
        /// The linux fake until real name is determined.
        /// </summary>
        LinuxFake,

        /// <summary>
        /// Micro controller.
        /// </summary>
        MicroController
    }
}