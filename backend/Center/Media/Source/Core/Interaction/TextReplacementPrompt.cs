// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextReplacementPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The TextReplacementPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.Controllers;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// the TextReplacementPrompt Prompt
    /// </summary>
    public class TextReplacementPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextReplacementPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell.</param>
        /// <param name="commandRegistry">the command Registry.</param>
        public TextReplacementPrompt(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.Shell = shell;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the media pools
        /// </summary>
        public IMediaShell Shell { get; set; }

        /// <summary>
        /// Gets the current List of Media
        /// </summary>
        public ExtendedObservableCollection<TextualReplacementDataViewModel> Replacements
        {
            get
            {
                if (this.Shell.MediaApplicationState.CurrentProject == null)
                {
                    return null;
                }

                return this.Shell.MediaApplicationState.CurrentProject.Replacements;
            }
        }

        /// <summary>
        /// Gets the Add new Replacement command
        /// </summary>
        public ICommand AddNewReplacementCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CreateReplacement);
            }
        }

        /// <summary>
        /// Gets the delete Replacement command
        /// </summary>
        public ICommand DeleteReplacementCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeleteReplacement);
            }
        }

        /// <summary>
        /// Gets the update Replacement command
        /// </summary>
        public ICommand UpdateReplacementCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.UpdateReplacement);
            }
        }

        /// <summary>
        /// Gets the show select Media dialog command
        /// </summary>
        public ICommand ShowSelectMediaDialogCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowMediaSelector);
            }
        }

        /// <summary>
        /// Gets the SelectMedia Interaction Request.
        /// </summary>
        public IInteractionRequest SelectMediaInteractionRequest
        {
            get
            {
                return InteractionManager<MenuSelectMediaPrompt>.Current.GetOrCreateInteractionRequest();
            }
        }
    }
}