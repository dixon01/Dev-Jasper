// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationHelper.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Compatibility
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

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
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Gets the current directory.
        /// </summary>
        public static string CurrentDirectory
        {
            get
            {
                return Environment.CurrentDirectory;
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
            var asm = Assembly.GetEntryAssembly();
            return asm == null ? null : asm.Location;
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

            var version = FileVersionInfo.GetVersionInfo(filename);
            return version.FileVersion;
        }

        /// <summary>
        /// Exits this application with the given exit code.
        /// </summary>
        /// <param name="exitCode">
        /// The exit code.
        /// </param>
        public static void Exit(int exitCode)
        {
            Environment.Exit(exitCode);
        }
    }
}
