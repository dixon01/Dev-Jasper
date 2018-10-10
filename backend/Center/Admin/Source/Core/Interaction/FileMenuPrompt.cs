// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileMenuPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.ViewModels;
    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;

    /// <summary>
    /// The FileMenuPrompt.
    /// </summary>
    public class FileMenuPrompt : PromptNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMenuPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="commandRegistry">The command registry.</param>
        public FileMenuPrompt(IAdminShell shell, ICommandRegistry commandRegistry)
        {
            this.CommandRegistry = commandRegistry;
            this.Shell = shell;
        }

        /// <summary>
        /// Gets or sets the Shell
        /// </summary>
        public IAdminShell Shell { get; set; }

        /// <summary>
        /// Gets or sets the command registry.
        /// </summary>
        /// <value>
        /// The command registry.
        /// </value>
        public ICommandRegistry CommandRegistry { get; set; }

        /// <summary>
        /// Gets the Exit Command.
        /// </summary>
        public ICommand ExitCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(ClientCommandCompositionKeys.Application.Exit);
            }
        }

        /// <summary>
        /// Gets the About Command.
        /// </summary>
        public ICommand AboutCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowAboutScreen);
            }
        }

        /// <summary>
        /// Gets the Options command.
        /// </summary>
        public ICommand OptionsCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowOptionsDialog);
            }
        }
    }
}
