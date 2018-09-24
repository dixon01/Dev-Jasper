// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectListPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The project list prompt.
    /// </summary>
    public class ProjectListPrompt : ProgressPromptBase
    {
        private readonly IMediaShell shell;

        private readonly ICommandRegistry commandRegistry;

        private MediaConfigurationDataViewModel highlightedProject;
        private string recentProjectBusyContent;
        private bool isRecentProjectBusy;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectListPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ProjectListPrompt(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.shell = shell;
            this.commandRegistry = commandRegistry;
        }

        /// <summary>
        /// Gets or sets the highlighted project.
        /// </summary>
        public MediaConfigurationDataViewModel HighlightedProject
        {
            get
            {
                return this.highlightedProject;
            }

            set
            {
                this.SetProperty(ref this.highlightedProject, value, () => this.HighlightedProject);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is recent project list busy.
        /// </summary>
        public bool IsRecentProjectListBusy
        {
            get
            {
                return this.isRecentProjectBusy;
            }

            set
            {
                this.SetProperty(ref this.isRecentProjectBusy, value, () => this.IsRecentProjectListBusy);
            }
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IMediaShell Shell
        {
            get
            {
                return this.shell;
            }
        }

        /// <summary>
        /// Gets the Open project command.
        /// </summary>
        public ICommand OpenProjectCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.Open);
            }
        }

        /// <summary>
        /// Gets or sets the recent project busy content.
        /// </summary>
        public string RecentProjectBusyContent
        {
            get
            {
                return this.recentProjectBusyContent;
            }

            set
            {
                this.SetProperty(ref this.recentProjectBusyContent, value, () => this.RecentProjectBusyContent);
            }
        }

        /// <summary>
        /// Gets the delete project command.
        /// </summary>
        public ICommand DeleteProjectCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeleteProject);
            }
        }
    }
}
