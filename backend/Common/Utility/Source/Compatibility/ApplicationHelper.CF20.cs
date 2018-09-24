// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationHelper.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Runtime.InteropServices;
    using System.Text;

    using OpenNETCF.Diagnostics;

    /// <summary>
    /// Helper class for application-wide properties.
    /// </summary>
    public static partial class ApplicationHelper
    {
        /// <summary>
        /// Gets the name of the local machine.
        /// </summary>
        public static string MachineName
        {
            get
            {
                return Dns.GetHostName();
            }
        }

        /// <summary>
        /// Gets the current directory.
        /// </summary>
        public static string CurrentDirectory
        {
            get
            {
                return Path.GetDirectoryName(GetEntryAssemblyLocation()) + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets the full path of the entry assembly (EXE).
        /// </summary>
        /// <returns>
        /// The full path of the entry assembly.
        /// </returns>
        public static string GetEntryAssemblyLocation()
        {
            var moduleHandle = NativeMethods.GetModuleHandle(null);

            if (moduleHandle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            var moduleName = new StringBuilder(255);
            if (NativeMethods.GetModuleFileName(moduleHandle, moduleName, moduleName.Capacity) == 0)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }

            return moduleName.ToString();
        }

        /// <summary>
        /// Gets the file version of the application.
        /// </summary>
        /// <param name="filename">
        /// The filename of the DLL or EXE for which you want the version.
        /// </param>
        /// <returns>
        /// The file version of the binary.
        /// </returns>
        public static string GetFileVersion(string filename)
        {
            if (filename == null)
            {
                return null;
            }

            // http://blog.opennetcf.com/2010/09/28/getting-native-file-info-in-the-compact-framework/
            IntPtr handle;

            var size = NativeMethods.GetFileVersionInfoSize(filename, out handle);
            var buffer = Marshal.AllocHGlobal(size);

            try
            {
                if (!NativeMethods.GetFileVersionInfo(filename, handle, size, buffer))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                IntPtr versionPtr;
                int versionLength;
                NativeMethods.VerQueryValue(buffer, "\\", out versionPtr, out versionLength);

                var versionInfo =
                    (NativeMethods.VS_FIXEDFILEINFO)
                    Marshal.PtrToStructure(versionPtr, typeof(NativeMethods.VS_FIXEDFILEINFO));

                var version = new Version(
                    (int)versionInfo.dwFileVersionMS >> 16,
                    (int)versionInfo.dwFileVersionMS & 0xFFFF,
                    (int)versionInfo.dwFileVersionLS >> 16,
                    (int)versionInfo.dwFileVersionLS & 0xFFFF);
                return version.ToString();
            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }
        }

        /// <summary>
        /// Exits this application with the given exit code.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code.
        /// </param>
        public static void Exit(int exitCode)
        {
            NativeMethods.TerminateProcess(ProcessHelper.GetCurrentProcessHandle(), exitCode);
        }

        private static class NativeMethods
        {
            [DllImport("CoreDll.dll", SetLastError = true)]
            public static extern int TerminateProcess(IntPtr hProcess, int uExitCode);

            [DllImport("CoreDll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern IntPtr GetModuleHandle(string module);

            [DllImport("CoreDll.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern int GetModuleFileName(IntPtr modulePtr, StringBuilder fileName, int size);

            [DllImport("CoreDll.dll", SetLastError = true)]
            public static extern int GetFileVersionInfoSize(string lptstrFilename, out IntPtr lpdwHandle);

            [DllImport("CoreDll.dll", SetLastError = true)]
            public static extern bool GetFileVersionInfo(
                string lptstrFilename, IntPtr dwHandle, int dwLen, IntPtr lpData);

            [DllImport("CoreDll.dll", SetLastError = true)]
            public static extern bool VerQueryValue(
                IntPtr pBlock, string lpSubBlock, out IntPtr lplpBuffer, out int puLen);

            // ReSharper disable InconsistentNaming
            // ReSharper disable FieldCanBeMadeReadOnly.Local
            // ReSharper disable MemberCanBePrivate.Local
            [StructLayout(LayoutKind.Sequential)]
            public struct VS_FIXEDFILEINFO
            {
                public uint dwSignature;
                public uint dwStrucVersion;
                public uint dwFileVersionMS;
                public uint dwFileVersionLS;
                public uint dwProductVersionMS;
                public uint dwProductVersionLS;
                public uint dwFileFlagsMask;
                public uint dwFileFlags;
                public uint dwFileOS;
                public uint dwFileType;
                public uint dwFileSubtype;
                public uint dwFileDateMS;
                public uint dwFileDateLS;
            }

            // ReSharper restore MemberCanBePrivate.Local
            // ReSharper restore FieldCanBeMadeReadOnly.Local
            // ReSharper restore InconsistentNaming
        }
    }
}
