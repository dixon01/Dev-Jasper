// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The UnitMenuPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Diag.Core.ViewModels;

    /// <summary>
    /// The UnitMenuPrompt.
    /// </summary>
    public class UnitMenuPrompt : PromptNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitMenuPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="commandRegistry">The command registry.</param>
        public UnitMenuPrompt(IDiagShell shell, ICommandRegistry commandRegistry)
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
        /// Gets the announce command
        /// </summary>
        public ICommand RequestAddCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.RequestAdd);
            }
        }

        /// <summary>
        /// Gets the announce command
        /// </summary>
        public ICommand AnnounceCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.Announce);
            }
        }

        /// <summary>
        /// Gets the Reboot command
        /// </summary>
        public ICommand RebootCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.Reboot);
            }
        }

        /// <summary>
        /// Gets the Remove command
        /// </summary>
        public ICommand RemoveCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.Remove);
            }
        }

        /// <summary>
        /// Gets the Connect command
        /// </summary>
        public ICommand ConnectCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.Connect);
            }
        }

        /// <summary>
        /// Gets the Disconnect command
        /// </summary>
        public ICommand DisconnectCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.Disconnect);
            }
        }

        /// <summary>
        /// Gets the edit ip settings command
        /// </summary>
        public ICommand EditIpSettingsCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.EditIpSettings);
            }
        }
    }
}