// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessResourcesObserver.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessResourcesObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Class that monitors the RAM and CPU usage of a process and takes
    /// configurable actions when (configured) limits are reached.
    /// </summary>
    public partial class ProcessResourcesObserver
    {
        /// <summary>
        /// Gets the used RAM (working set) in bytes.
        /// </summary>
        public long RamBytes
        {
            get
            {
                var vmi = new NativeMethods.PROCVMINFO();
                int size = Marshal.SizeOf(typeof(NativeMethods.PROCVMINFO));
                var index = NativeMethods.GetProcessIndexFromID(this.process.Id);
                if (NativeMethods.CeGetProcVMInfo(index, size, ref vmi) == 0)
                {
                    return vmi.cbRwMemUsed;
                }

                return 0;
            }
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            private const int KMOD_CORE = 1;
            private const int IOCTL_KLIB_GETPROCMEMINFO = 8;

            public static int CeGetProcVMInfo(uint idxProc, int cbSize, ref PROCVMINFO pinfo)
            {
                if (KernelLibIoControl(
                    (IntPtr)KMOD_CORE,
                    IOCTL_KLIB_GETPROCMEMINFO,
                    IntPtr.Zero,
                    idxProc,
                    ref pinfo,
                    cbSize,
                    IntPtr.Zero))
                {
                    return 0;
                }

                return cbSize;
            }

            [DllImport("coredll.dll", SetLastError = true)]
            public static extern uint GetProcessIndexFromID(int hProc);

            [DllImport("coredll.dll", SetLastError = true)]
            private static extern bool KernelLibIoControl(
                IntPtr hLib,
                int dwIoControlCode,
                IntPtr lpOutBuf,
                uint nInBufSize,
                ref PROCVMINFO lpInBuf,
                int nOutBufSize,
                IntPtr lpBytesReturned);

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCVMINFO
            {
                public int hproc;
                public uint cbRwMemUsed;
            }

            // ReSharper restore InconsistentNaming
        }
    }
}