// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ViewMenuPrompt type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// the view menu prompt
    /// </summary>
    public class ViewMenuPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        private IMediaShell shell;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMenuPrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">the commandRegistry</param>
        /// <param name="shell">the shell</param>
        public ViewMenuPrompt(ICommandRegistry commandRegistry, IMediaShell shell)
        {
            this.commandRegistry = commandRegistry;
            this.shell = shell;
        }

        /// <summary>
        /// Gets the show simulation toggle command
        /// </summary>
        public ICommand ShowSimulationToggleCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.ToggleSimulation);
            }
        }

        /// <summary>
        /// Gets the show simulation toggle command
        /// </summary>
        public ICommand UseEdgeSnapToggleCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.ToggleEdgeSnap);
            }
        }

        /// <summary>
        /// Gets or sets the shell
        /// </summary>
        public IMediaShell Shell
        {
            get
            {
                return this.shell;
            }

            set
            {
                this.SetProperty(ref this.shell, value, () => this.Shell);
            }
        }
    }
}