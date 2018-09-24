// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemManagerOptions.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemManagerOptions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.SystemManager.Core
{
    using CommandLineParser.Arguments;

    using Gorba.Common.SystemManagement.Core;

    /// <summary>
    /// Command line options to start <see cref="SystemManagementControllerBase"/>.
    /// </summary>
    public class SystemManagerOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether to show usage
        /// (either in the console or in a popup message box).
        /// </summary>
        [SwitchArgument('?', "help", false, Description = "Show this usage information")]
        public bool ShowUsage { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to force the start of the application.
        /// </summary>
        [SwitchArgument(
            'f', "force", false, Description = "Force the start of the application",
            FullDescription =
            "If this option is set, System Manager starts even if there is already an instance running")]
        public bool ForceStart { get; set; }

        /// <summary>
        /// Gets or sets the process ID for which system Manager should wait before continuing its start-up.
        /// </summary>
        [ValueArgument(
            typeof(int), 'w', "waitforexit", Description = "Wait for the given process ID to exit",
            FullDescription = "System Manager waits for the given process ID to exit and then starts its internal")]
        public int WaitForExit { get; set; }
    }
}
