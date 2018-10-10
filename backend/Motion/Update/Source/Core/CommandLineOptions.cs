// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandLineOptions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommandLineOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core
{
    using CommandLineParser.Arguments;

    /// <summary>
    /// The command line options of the update application.
    /// </summary>
    public class CommandLineOptions
    {
        /// <summary>
        /// The "install" command.
        /// </summary>
        public const string InstallCommand = "install";

        /// <summary>
        /// The "target" argument.
        /// </summary>
        public const string TargetArgument = "target";

        /// <summary>
        /// The <c>waitforexit</c> argument.
        /// </summary>
        public const string WaitForExitArgument = "waitforexit";

        /// <summary>
        /// Gets or sets the full path to the update command file to be installed.
        /// </summary>
        [ValueArgument(
            typeof(string), 'i', InstallCommand, Description = "Install Update.exe from an update command",
            FullDescription = "Installs the Update.exe and all depenedencies from the given update command file")]
        public string InstallFile { get; set; }

        /// <summary>
        /// Gets or sets the target path where Update.exe should be installed to.
        /// </summary>
        [ValueArgument(
            typeof(string), 't', TargetArgument, Description = "Install Update.exe to this directory",
            FullDescription = "The full folder path where the Update.exe will be installed")]
        public string TargetPath { get; set; }

        /// <summary>
        /// Gets or sets the process ID for which to wait before the installation continues.
        /// </summary>
        [ValueArgument(
            typeof(int), 'w', WaitForExitArgument, Description = "Wait for the given process ID to exit",
            FullDescription = "Wait for the given process ID to exit before installation continues")]
        public int WaitForExit { get; set; }
    }
}
