// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The ViewMenuPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Diag.Core.ViewModels;

    /// <summary>
    /// The ViewMenuPrompt.
    /// </summary>
    public class ViewMenuPrompt : PromptNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewMenuPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell</param>
        /// <param name="commandRegistry">The command registry.</param>
        public ViewMenuPrompt(IDiagShell shell, ICommandRegistry commandRegistry)
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
        /// Gets the Reset command
        /// </summary>
        public ICommand ResetCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Reset);
            }
        }

        /// <summary>
        /// Gets the Refresh command
        /// </summary>
        public ICommand RefreshCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Unit.RefreshAllUnits);
            }
        }
    }
}