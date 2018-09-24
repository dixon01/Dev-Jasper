// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerShellOptions.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerShellOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.ShellApp
{
    using CommandLineParser.Arguments;

    using Gorba.Motion.SystemManager.Core;

    /// <summary>
    /// Command line options for System Manager as a shell.
    /// </summary>
    public class SystemManagerShellOptions : SystemManagerOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to install this application as the Windows shell.
        /// </summary>
        [SwitchArgument(
            'v',
            "VerifyShell",
            false,
            Description = "Verifies if this application is the Windows shell, if not the user is asked to isntall it")]
        public bool VerifyShell { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to install this application as the Windows shell.
        /// </summary>
        [SwitchArgument('s', "InstallShell", false, Description = "Install this application as the Windows shell")]
        public bool InstallShell { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to uninstall this application as the Windows shell.
        /// </summary>
        [SwitchArgument(
            'u',
            "UninstallShell",
            false,
            Description =
                "Uninstall this application as the Windows shell and replace it with the default (Windows Explorer)")]
        public bool UninstallShell { get; set; }
    }
}