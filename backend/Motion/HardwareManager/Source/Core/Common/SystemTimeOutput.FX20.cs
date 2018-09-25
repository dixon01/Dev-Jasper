// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemTimeOutput.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemTimeOutput type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// GIOoM port for setting the system time.
    /// </summary>
    public partial class SystemTimeOutput
    {
        private static partial class NativeMethods
        {
            [DllImport("kernel32.dll", EntryPoint = "SetSystemTime", SetLastError = true)]
            public static extern bool SetSystemTime(ref SystemTime st);
        }
    }
}
