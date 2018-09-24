// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainMenuPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Client;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// the main menu prompt
    /// </summary>
    public class MainMenuPrompt : ProgressPromptBase, IDataErrorInfo
    {
        private readonly ResourceManagementPrompt resourceManagementPrompt;

        private readonly TextReplacementPrompt textReplacementPrompt;

        private readonly FormulaManagerPrompt formulaManagerPrompt;

        private readonly Lazy<List<ResolutionConfiguration>> availableResolutions;

        private string newProjectName;

        private string description;

        private string importFilePath;

        private bool isOpenAfterImport;

        private ICommand showMenuEntryCommand;

        private ICommand saveProjectCommand;
        private ICommand checkinProjectCommand;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainMenuPrompt"/> class.
        /// </summary>
        /// <param name="shell">
        /// the shell
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public MainMenuPrompt(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.CommandRegistry = commandRegistry;
            this.Shell = shell;
            this.resourceManagementPrompt = new ResourceManagementPrompt(shell, commandRegistry);
            this.textReplacementPrompt = new TextReplacementPrompt(shell, commandRegistry);
            this.formulaManagerPrompt = new FormulaManagerPrompt(shell, commandRegistry);
            this.isOpenAfterImport = true;
            this.availableResolutions = new Lazy<List<ResolutionConfiguration>>(this.GetAvailableResolutions);
            this.ExportScreen = new ExportScreenViewModel(this.Shell, commandRegistry);
            this.SaveAsScreen = new SaveAsScreenViewModel(this.Shell, commandRegistry);
            this.ProjectListScreen = new ProjectListPrompt(this.Shell, commandRegistry);
        }

        /// <summary>
        /// Gets or sets the Shell
        /// </summary>
        public IMediaShell Shell { get; set; }

        /// <summary>
        /// Gets or sets the command registry.
        /// </summary>
        /// <value>
        /// The command registry.
        /// </value>
        public ICommandRegistry CommandRegistry { get; set; }

        /// <summary>
        /// Gets the export screen view model.
        /// </summary>
        public ExportScreenViewModel ExportScreen { get; private set; }

        /// <summary>
        /// Gets the SaveAs screen view model.
        /// </summary>
        public SaveAsScreenViewModel SaveAsScreen { get; private set; }

        /// <summary>
        /// Gets the project list screen view model.
        /// </summary>
        public ProjectListPrompt ProjectListScreen { get; private set; }

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
        /// Gets the check in command.
        /// </summary>
        public ICommand CheckInCommand
        {
            get
            {
                if (this.checkinProjectCommand != null)
                {
                    return this.checkinProjectCommand;
                }

                this.checkinProjectCommand = this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.CheckIn);
                this.showMenuEntryCommand.CanExecuteChanged +=
                    (sender, args) => this.RaisePropertyChanged(() => this.CheckInCommand);

                return this.checkinProjectCommand;
            }
        }

        /// <summary>
        /// Gets the show about screen command.
        /// </summary>
        public ICommand ShowAboutScreenCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowAboutScreen);
            }
        }

        /// <summary>
        /// Gets the show options command.
        /// </summary>
        public ICommand ShowOptionsCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.ShowOptionsDialog);
            }
        }

        /// <summary>
        /// Gets or sets the import file path.
        /// </summary>
        public string ImportFilePath
        {
            get
            {
                return this.importFilePath;
            }

            set
            {
                this.SetProperty(ref this.importFilePath, value, () => this.ImportFilePath);
            }
        }

        /// <summary>
        /// Gets the navigate to main menu new command.
        /// </summary>
        public ICommand ShowMenuEntryCommand
        {
            get
            {
                if (this.showMenuEntryCommand == null)
                {
                    this.showMenuEntryCommand =
                        this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.UI.Menu.ShowMenuEntry);
                    this.showMenuEntryCommand.CanExecuteChanged +=
                        (sender, args) => this.RaisePropertyChanged(() => this.ShowMenuEntryCommand);
                }

                return this.showMenuEntryCommand;
            }
        }

        /// <summary>
        /// Gets the Export command.
        /// </summary>
        public ICommand ExportCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Presentation.ExportServer);
            }
        }

        /// <summary>
        /// Gets the import command.
        /// </summary>
        public ICommand ImportCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.Import);
            }
        }

        /// <summary>
        /// Gets the Open project command.
        /// </summary>
        public ICommand OpenProjectCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.Open);
            }
        }

        /// <summary>
        /// Gets the Save project command.
        /// </summary>
        public ICommand SaveProjectCommand
        {
            get
            {
                if (this.saveProjectCommand == null)
                {
                    this.saveProjectCommand = this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.Save);
                    this.saveProjectCommand.CanExecuteChanged +=
                        (sender, args) => this.RaisePropertyChanged(() => this.SaveProjectCommand);
                }

                return this.saveProjectCommand;
            }
        }

        /// <summary>
        /// Gets the SaveAs project command.
        /// </summary>
        public ICommand SaveAsProjectCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.SaveAs);
            }
        }

        /// <summary>
        /// Gets the Create new project command.
        /// </summary>
        public ICommand CreateNewProjectCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.New);
            }
        }

        /// <summary>
        /// Gets the Media Management Prompt
        /// </summary>
        public ResourceManagementPrompt ResourceManagementPrompt
        {
            get
            {
                return this.resourceManagementPrompt;
            }
        }

        /// <summary>
        /// Gets the Media Text Replacement Prompt
        /// </summary>
        public TextReplacementPrompt TextReplacementPrompt
        {
            get
            {
                return this.textReplacementPrompt;
            }
        }

        /// <summary>
        /// Gets the Media Text Replacement Prompt
        /// </summary>
        public FormulaManagerPrompt FormulaManagerPrompt
        {
            get
            {
                return this.formulaManagerPrompt;
            }
        }

        /// <summary>
        /// Gets the available resolutions
        /// </summary>
        public List<ResolutionConfiguration> AvailableResolutions
        {
            get
            {
                return this.availableResolutions.Value;
            }
        }

        /// <summary>
        /// Gets or sets the callback to navigate
        /// </summary>
        public Action<object> NavigateTo { get; set; }

        /// <summary>
        /// Gets or sets the project name.
        /// </summary>
        public string NewProjectName
        {
            get
            {
                return this.newProjectName;
            }

            set
            {
                this.SetProperty(ref this.newProjectName, value, () => this.NewProjectName);
            }
        }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value, () => this.Description);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is open after import.
        /// </summary>
        public bool IsOpenAfterImport
        {
            get
            {
                return this.isOpenAfterImport;
            }

            set
            {
                this.SetProperty(ref this.isOpenAfterImport, value, () => this.IsOpenAfterImport);
            }
        }

        /// <summary>
        /// Gets the error.
        /// </summary>
        public string Error { get; private set; }

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string this[string columnName]
        {
            get
            {
                if (columnName == "NewProjectName")
                {
                    if (string.IsNullOrEmpty(this.NewProjectName))
                    {
                        this.Error = MediaStrings.MainMenu_ProjectNameEmpty;
                        return this.Error;
                    }

                    if (
                        this.Shell.MediaApplicationState.ExistingProjects.Any(
                            p =>
                            p.Document.Name.Equals(this.NewProjectName, StringComparison.InvariantCultureIgnoreCase)))
                    {
                        this.Error = MediaStrings.MainMenu_DuplicateProjectName;
                        return MediaStrings.MainMenu_DuplicateProjectName;
                    }
                }

                this.Error = string.Empty;
                return string.Empty;
            }
        }

        /// <summary>
        /// The navigation request
        /// </summary>
        /// <param name="newSelection">
        /// the new selection parameter
        /// </param>
        public void RaiseNavigationRequest(object newSelection)
        {
            if (this.NavigateTo != null)
            {
                this.NavigateTo(newSelection);
            }
        }

        private List<ResolutionConfiguration> GetAvailableResolutions()
        {
            var configuration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            return configuration.PhysicalScreenSettings.PhysicalScreenTypes.First().AvailableResolutions;
        }
    }
}