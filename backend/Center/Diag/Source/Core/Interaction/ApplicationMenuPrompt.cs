// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ApplicationMenuPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Diag.Core.ViewModels;

    /// <summary>
    /// The ApplicationMenuPrompt.
    /// </summary>
    public class ApplicationMenuPrompt : PromptNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationMenuPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="commandRegistry">The command registry.</param>
        public ApplicationMenuPrompt(IDiagShell shell, ICommandRegistry commandRegistry)
        {
            this.CommandRegistry = commandRegistry;
            this.Shell = shell;
        }

        /// <summary>
        /// Gets or sets the Shell
        /// </summary>
        public IDiagShell Shell { get; set; }

        /// <summary>
        /// Gets or sets the command registry.
        /// </summary>
        /// <value>
        /// The command registry.
        /// </value>
        public ICommandRegistry CommandRegistry { get; set; }

        /// <summary>
        /// Gets the Launch application command
        /// </summary>
        public ICommand LaunchCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.Launch);
            }
        }

        /// <summary>
        /// Gets the re-launch application command
        /// </summary>
        public ICommand RelaunchCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.Relaunch);
            }
        }

        /// <summary>
        /// Gets the End application command
        /// </summary>
        public ICommand EndCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.UnitApplication.End);
            }
        }
    }
}