// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutNavigationPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The LayoutNavigationPrompt.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// the Layout Navigation Prompt
    /// </summary>
    public class LayoutNavigationPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutNavigationPrompt"/> class.
        /// </summary>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public LayoutNavigationPrompt(ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the list of Layouts
        /// </summary>
        public IEnumerable<LayoutConfigDataViewModel> Layouts { get; set; }

        /// <summary>
        /// Gets or sets the Project list
        /// </summary>
        public IEnumerable<InfomediaConfigDataViewModel> Projects { get; set; }

        /// <summary>
        /// Gets the command to choose a layout
        /// </summary>
        public ICommand ChooseLayoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.NavigateTo);
            }
        }

        /// <summary>
        /// Gets the command to delete a layout
        /// </summary>
        public ICommand DeleteLayoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.DeleteLayout);
            }
        }

        /// <summary>
        /// Gets the command to clone a layout.
        /// </summary>
        public ICommand CloneLayoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.CloneLayout);
            }
        }

        /// <summary>
        /// Gets the command to create a TFT layout
        /// </summary>
        public ICommand CreateTftLayoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.CreateTftLayout);
            }
        }

        /// <summary>
        /// Gets the command to create a LED layout
        /// </summary>
        public ICommand CreateLedLayoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.CreateLedLayout);
            }
        }

        /// <summary>
        /// Gets the command to create an audio layout
        /// </summary>
        public ICommand CreateAudioLayoutCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.CreateAudioLayout);
            }
        }

        /// <summary>
        /// Gets the layout rename command.
        /// </summary>
        public ICommand RenameEntityCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.Layout.Rename);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is valid.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is valid; otherwise, <c>false</c>.
        /// </returns>
        public override bool IsValid
        {
            get
            {
                return this.Layouts.All(layout => layout.IsValid());
            }
        }

        /// <summary>
        /// Gets or sets the MediaShell
        /// </summary>
        public IMediaShell Shell { get; set; }
    }
}