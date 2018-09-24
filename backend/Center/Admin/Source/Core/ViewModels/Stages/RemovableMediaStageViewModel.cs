// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemovableMediaStageViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemovableMediaStageViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Stages
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.ViewModels.RemovableMedia;
    using Gorba.Center.Common.Wpf.Core.Collections;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The view model for the stage representing a single removable media (USB stick).
    /// </summary>
    public class RemovableMediaStageViewModel : StageViewModelBase
    {
        private readonly ICommandRegistry commandRegistry;

        private bool isBusy;

        private int busyProgress;

        private string busyMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemovableMediaStageViewModel"/> class.
        /// </summary>
        /// <param name="driveName">
        /// The name of the drive.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public RemovableMediaStageViewModel(string driveName, ICommandRegistry commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Name = driveName;

            this.FeedbackUnits = new ObservableCollection<string>();
            this.FeedbackUnits.CollectionChanged += (s, e) => this.RaisePropertyChanged(() => this.HasFeedback);

            this.UpdateGroups = new ObservableItemCollection<UpdateGroupSelectionViewModel>();
            this.UpdateGroups.CollectionChanged += this.UpdateGroupsOnChanged;
            this.UpdateGroups.ItemPropertyChanged += this.UpdateGroupsOnChanged;
        }

        /// <summary>
        /// Gets the list of units for which there is feedback on the USB stick.
        /// </summary>
        public ObservableCollection<string> FeedbackUnits { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the media has feedback that can be imported.
        /// </summary>
        public bool HasFeedback
        {
            get
            {
                return this.FeedbackUnits.Any();
            }
        }

        /// <summary>
        /// Gets the list of update groups to select for exporting.
        /// </summary>
        public ObservableItemCollection<UpdateGroupSelectionViewModel> UpdateGroups { get; private set; }

        /// <summary>
        /// Gets a value indicating whether there are units selected for export.
        /// </summary>
        public bool HasSelectedExportUnits
        {
            get
            {
                return this.UpdateGroups.SelectMany(g => g.Units).Any(u => u.IsChecked);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this stage is busy (importing or exporting).
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                this.SetProperty(ref this.isBusy, value, () => this.IsBusy);
            }
        }

        /// <summary>
        /// Gets or sets the busy progress (value from 0 to 100).
        /// </summary>
        public int BusyProgress
        {
            get
            {
                return this.busyProgress;
            }

            set
            {
                this.SetProperty(ref this.busyProgress, value, () => this.BusyProgress);
            }
        }

        /// <summary>
        /// Gets or sets the message shown on the busy indicator.
        /// </summary>
        public string BusyMessage
        {
            get
            {
                return this.busyMessage;
            }

            set
            {
                this.SetProperty(ref this.busyMessage, value, () => this.BusyMessage);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the availability of units for export
        /// </summary>
        public bool NoUnitsAvailableForExport
        {
            get
            {
                return this.UpdateGroups.All(updateGroup => updateGroup.Units.Count == 0);
            }
        }

        /// <summary>
        /// Gets the import feedback command.
        /// </summary>
        public ICommand ImportFeedbackCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.RemovableMedia.ImportFeedback);
            }
        }

        /// <summary>
        /// Gets the export updates command.
        /// </summary>
        public ICommand ExportUpdatesCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.RemovableMedia.ExportUpdates);
            }
        }

        /// <summary>
        /// Gets the cancel operation command.
        /// </summary>
        public ICommand CancelOperationCommand
        {
            get
            {
                return this.commandRegistry.GetCommand(CommandCompositionKeys.Shell.RemovableMedia.CancelOperation);
            }
        }

        private void UpdateGroupsOnChanged(object source, EventArgs e)
        {
            this.RaisePropertyChanged(() => this.HasSelectedExportUnits);
            this.RaisePropertyChanged(() => this.NoUnitsAvailableForExport);
        }
    }
}
