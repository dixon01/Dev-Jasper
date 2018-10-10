// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceManagementPrompt.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The resource management prompt
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Interaction
{
    using System.Windows.Input;

    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ViewModels;

    /// <summary>
    /// The resource management prompt
    /// </summary>
    public class ResourceManagementPrompt : PromptNotification
    {
        private readonly ICommandRegistry commandRegistry;

        private ExtendedObservableCollection<ResourceInfoDataViewModel> currentMediaList;

        private PoolConfigDataViewModel selectedPool;

        private ResourceType selectedMediaType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManagementPrompt"/> class.
        /// </summary>
        /// <param name="shell">the shell.</param>
        /// <param name="commandRegistry">the command Registry.</param>
        public ResourceManagementPrompt(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.Shell = shell;
            this.commandRegistry = commandRegistry;
            this.currentMediaList = new ExtendedObservableCollection<ResourceInfoDataViewModel>();
        }

        /// <summary>
        /// Gets or sets the media pools
        /// </summary>
        public IMediaShell Shell { get; set; }

        /// <summary>
        /// Gets or sets the current List of Media
        /// </summary>
        public ExtendedObservableCollection<ResourceInfoDataViewModel> CurrentMediaList
        {
            get
            {
                return this.currentMediaList;
            }

            set
            {
                this.SetProperty(ref this.currentMediaList, value, () => this.CurrentMediaList);
            }
        }

        /// <summary>
        /// Gets or sets the current selected pool
        /// </summary>
        public PoolConfigDataViewModel SelectedPool
        {
            get
            {
                return this.selectedPool;
            }

            set
            {
                this.SetProperty(ref this.selectedPool, value, () => this.SelectedPool);
            }
        }

        /// <summary>
        /// Gets or sets the selected type.
        /// </summary>
        public ResourceType SelectedType
        {
            get
            {
                return this.selectedMediaType;
            }

            set
            {
                this.SetProperty(ref this.selectedMediaType, value, () => this.SelectedType);
            }
        }

        /// <summary>
        /// Gets the Add new Pool command
        /// </summary>
        public ICommand AddNewPoolCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CreateMediaPool);
            }
        }

        /// <summary>
        /// Gets the add csv command.
        /// </summary>
        public ICommand AddCsvCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CsvMapping.CreateCsv);
            }
        }

        /// <summary>
        /// Gets the import csv command.
        /// </summary>
        public ICommand ImportCsvCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CsvMapping.ImportCsv);
            }
        }

        /// <summary>
        /// Gets the delete Pool command
        /// </summary>
        public ICommand DeletePoolCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeleteMediaPool);
            }
        }

        /// <summary>
        /// Gets the add new Picture command
        /// </summary>
        public ICommand AddNewPictureCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.AddResource);
            }
        }

        /// <summary>
        /// Gets the add new video command
        /// </summary>
        public ICommand AddNewVideoCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.AddResource);
            }
        }

        /// <summary>
        /// Gets the delete Media Command
        /// </summary>
        public ICommand DeleteMediaCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeleteResource);
            }
        }

        /// <summary>
        /// Gets the remove Media reference Command
        /// </summary>
        public ICommand RemoveMediaReferenceCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.DeleteResourceReference);
            }
        }

        /// <summary>
        /// Gets the add resource reference Command
        /// </summary>
        public ICommand AddResourceReferenceCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.AddResourceReference);
            }
        }

        /// <summary>
        /// Gets the update media pool command
        /// </summary>
        public ICommand UpdateResourceListElementCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.UpdateResourceListElement);
            }
        }

        /// <summary>
        /// Gets command to edit a csv file.
        /// </summary>
        public ICommand EditCsvCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CsvMapping.EditCsv);
            }
        }

        /// <summary>
        /// Gets the delete csv command.
        /// </summary>
        public ICommand DeleteCsvCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Project.CsvMapping.DeleteCsv);
            }
        }
    }
}