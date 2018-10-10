// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExportScreenViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The export screen view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Windows.Input;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Update;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Media.Core.Resources;

    /// <summary>
    /// The export screen view model.
    /// </summary>
    public class ExportScreenViewModel : ViewModelBase, IDataErrorInfo
    {
        private DateTime? startDate;
        private DateTime? endDate;
        private bool isStartDateChecked;
        private bool isEndDateChecked;

        private string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExportScreenViewModel"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ExportScreenViewModel(IMediaShell shell, ICommandRegistry commandRegistry)
        {
            this.Shell = shell;
            this.CommandRegistry = commandRegistry;

            this.UpdateGroups = new ObservableCollection<UpdateGroupItemViewModel>();
        }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public IMediaShell Shell { get; private set; }

        /// <summary>
        /// Gets the command registry.
        /// </summary>
        public ICommandRegistry CommandRegistry { get; private set; }

        /// <summary>
        /// Gets the SelectExportFile command.
        /// </summary>
        public ICommand SelectExportFileCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Presentation.SelectExportFile);
            }
        }

        /// <summary>
        /// Gets the command to export a project to a local destination.
        /// </summary>
        public ICommand ExportLocalCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Presentation.ExportLocal);
            }
        }

        /// <summary>
        /// Gets the command to export a project to server.
        /// </summary>
        public ICommand ExportServerCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Shell.Presentation.ExportServer);
            }
        }

        /// <summary>
        /// Gets the command to transfer a project to a local file.
        /// </summary>
        public ICommand TransferProjectCommand
        {
            get
            {
                return this.CommandRegistry.GetCommand(CommandCompositionKeys.Project.Transfer);
            }
        }

        /// <summary>
        /// Gets the list of update groups that can be selected for the server export.
        /// </summary>
        public ObservableCollection<UpdateGroupItemViewModel> UpdateGroups
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the start date.
        /// </summary>
        public DateTime? StartDate
        {
            get
            {
                return this.startDate;
            }

            set
            {
                this.SetProperty(ref this.startDate, value, () => this.StartDate);
            }
        }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public DateTime? EndDate
        {
            get
            {
                return this.endDate;
            }

            set
            {
                this.SetProperty(ref this.endDate, value, () => this.EndDate);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is start date checked.
        /// </summary>
        public bool IsStartDateChecked
        {
            get
            {
                return this.isStartDateChecked;
            }

            set
            {
                this.SetProperty(ref this.isStartDateChecked, value, () => this.IsStartDateChecked);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is end date checked.
        /// </summary>
        public bool IsEndDateChecked
        {
            get
            {
                return this.isEndDateChecked;
            }

            set
            {
                this.SetProperty(ref this.isEndDateChecked, value, () => this.IsEndDateChecked);
            }
        }

        /// <summary>
        /// Gets or sets the export description.
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
        /// Gets the error.
        /// </summary>
        public string Error
        {
            get
            {
                return this.ValidateStartDate() ?? this.ValidateEndDate();
            }
        }

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
                if (columnName == "StartDate")
                {
                    return this.ValidateStartDate();
                }

                if (columnName == "EndDate")
                {
                    return this.ValidateEndDate();
                }

                return null;
            }
        }

        private string ValidateStartDate()
        {
            if (!this.StartDate.HasValue && this.IsStartDateChecked)
            {
                return MediaStrings.ExportScreen_StartDateEmpty;
            }

            return null;
        }

        private string ValidateEndDate()
        {
            if (this.endDate.HasValue)
            {
                if (this.isEndDateChecked)
                {
                    if (this.isStartDateChecked && this.startDate.HasValue && this.startDate.Value > this.endDate.Value)
                    {
                        return MediaStrings.ExportScreen_EndDateEarlierStart;
                    }
                }
            }
            else
            {
                if (this.isEndDateChecked)
                {
                    return MediaStrings.ExportScreen_EndDateEmpty;
                }
            }

            return null;
        }
    }
}
