// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProcessHelper.WIN32.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProcessHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.OSWrappers.Process
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Helper class to get the parent process of the current process.
    /// </summary>
    public static class ProcessHelper
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(ProcessHelper));

        /// <summary>
        /// Returns the parent process of the current process
        /// </summary>
        /// <returns>The parent process of current process (or null if not found).</returns>
        public static Process GetParentProcess()
        {
            int parentPid = 0;
            int processPid = Process.GetCurrentProcess().Id;

            try
            {
                // Take snapshot of processes
                var ptrSnapshot = NativeMethods.CreateToolhelp32Snapshot(NativeMethods.TH32CS_SNAPPROCESS, 0);

                if (ptrSnapshot == IntPtr.Zero)
                {
                    return null;
                }

                try
                {
                    var procInfo = new NativeMethods.PROCESSENTRY32();

                    procInfo.dwSize = (uint)Marshal.SizeOf(typeof(NativeMethods.PROCESSENTRY32));

                    // Read first
                    if (NativeMethods.Process32First(ptrSnapshot, ref procInfo) == false)
                    {
                        return null;
                    }

                    // Loop through the snapshot
                    do
                    {
                        // If it's me, then ask for my parent.
                        if (processPid == procInfo.th32ProcessID)
                        {
                            parentPid = (int)procInfo.th32ParentProcessID;
                        }
                    }
                    while (parentPid == 0 && NativeMethods.Process32Next(ptrSnapshot, ref procInfo)); // Read next
                }
                finally
                {
                    NativeMethods.CloseToolhelp32Snapshot(ptrSnapshot);
                }

                if (parentPid <= 0)
                {
                    return null;
                }

                return Process.GetProcessById(parentPid);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static class NativeMethods
        {
            // ReSharper disable InconsistentNaming
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // ReSharper disable MemberCanBePrivate.Local
            public const uint TH32CS_SNAPPROCESS = 2;

            /// <summary>
            /// Takes a snapshot of the specified processes, as well as the heaps,
            /// modules, and threads used by these processes.
            /// </summary>
            /// <param name="dwFlags">
            /// The portions of the system to be included in the snapshot.
            /// </param>
            /// <param name="th32ProcessID">
            /// The process identifier of the process to be included in the snapshot.
            /// </param>
            /// <returns>
            /// If the function succeeds, it returns an open handle to the specified snapshot.
            /// If the function fails, it returns INVALID_HANDLE_VALUE.
            /// </returns>
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

            /// <summary>
            /// Fake method for FX 2.0: there is no method called <c>CloseToolhelp32Snapshot</c>,
            /// but instead the handle has to be closed with <c>CloseHandle</c>.
            /// </summary>
            /// <param name="hSnapshot">
            /// A handle created with <see cref="CloseToolhelp32Snapshot"/>.
            /// </param>
            /// <returns>
            /// If the function succeeds, the return value is nonzero.
            /// If the function fails, the return value is zero.
            /// </returns>
            [DllImport("kernel32.dll", EntryPoint = "CloseHandle", SetLastError = true)]
            public static extern int CloseToolhelp32Snapshot(IntPtr hSnapshot);

            /// <summary>
            /// Retrieves information about the first process encountered in a system snapshot.
            /// </summary>
            /// <param name="hSnapshot">A handle to the snapshot.</param>
            /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
            /// <returns>
            /// Returns TRUE if the first entry of the process list has been copied to the buffer.
            /// Returns FALSE otherwise.
            /// </returns>
            [DllImport("kernel32.dll")]
            public static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

            /// <summary>
            /// Retrieves information about the next process recorded in a system snapshot.
            /// </summary>
            /// <param name="hSnapshot">A handle to the snapshot.</param>
            /// <param name="lppe">A pointer to a PROCESSENTRY32 structure.</param>
            /// <returns>
            /// Returns TRUE if the next entry of the process list has been copied to the buffer.
            /// Returns FALSE otherwise.</returns>
            [DllImport("kernel32.dll")]
            public static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

            /// <summary>
            /// Describes an entry from a list of the processes residing
            /// in the system address space when a snapshot was taken.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESSENTRY32
            {
                public uint dwSize;
                public uint cntUsage;
                public uint th32ProcessID;
                public IntPtr th32DefaultHeapID;
                public uint th32ModuleID;
                public uint cntThreads;
                public uint th32ParentProcessID;
                public int pcPriClassBase;
                public uint dwFlags;
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string szExeFile;
            }

            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore FieldCanBeMadeReadOnly.Local
            // ReSharper restore InconsistentNaming
        }
    }
}