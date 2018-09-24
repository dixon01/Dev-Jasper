// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AdvApi32.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AdvApi32 type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.DLLs
{
    using System;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Win32.Api.Structs;

    /// <summary>
    /// Wrapper for the <c>advapi32.dll</c>.
    /// </summary>
    public static partial class AdvApi32
    {
        /// <summary>
        /// Updates the service control manager's status information for the calling service.
        /// </summary>
        /// <param name="serviceStatus">
        /// A handle to the status information structure for the current service.
        /// This handle is returned by the RegisterServiceCtrlHandlerEx function.
        /// </param>
        /// <param name="refServiceStatus">
        /// A pointer to the ServiceStatus structure the contains
        /// the latest status information for the calling service.
        /// </param>
        /// <returns>
        /// Returns true if the function succeeds otherwise false.
        /// </returns>
        [DllImport("ADVAPI32.DLL", EntryPoint = "SetServiceStatus", SetLastError = true)]
        public static extern bool SetServiceStatus(IntPtr serviceStatus, ref ServiceStatus refServiceStatus);
    }
}
