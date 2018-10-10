// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Winmm.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Win32.Api.DLLs
{
    using System.Runtime.InteropServices;

    /// <summary>
    ///  /// Wrapper for the <c>winmm.dll</c>.
    /// </summary>
    public static partial class Winmm
    {
        /// <summary>
        /// The timeGetTime function retrieves the system time, in milliseconds.
        /// The system time is the time elapsed since Windows was started.
        /// More accurate than Environment.TickCount for smoother motion in DirectX.
        /// </summary>
        /// <returns>
        /// Returns the system time in milliseconds <see cref="int"/>.
        /// </returns>
        [System.Security.SuppressUnmanagedCodeSecurity]
        [DllImport("winmm.dll", EntryPoint = "timeGetTime",  CharSet = CharSet.Ansi)]
        public static extern int TimerGetTime();
    }
}
