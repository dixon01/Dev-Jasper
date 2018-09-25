// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemSettingsViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The background system settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.Stages.Meta
{
    using System;
    using System.Collections;
    using System.Windows.Input;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.Wpf.Framework.Controllers;

    /// <summary>
    /// The background system settings view model.
    /// </summary>
    public class BackgroundSystemSettingsViewModel : EntityStageViewModelBase
    {
        private readonly string azureUpdateProviderEmptyRetryInterval = AdminStrings.Editor_EnterDuration;

        private ICommandRegistry commandRegistry;

        private bool isMaintenanceModeEnabled;

        private string maintenanceModeReason;

        private bool isMaintenanceModeVisible;

        private bool isAzureUpdateProviderEnabled;

        private TimeSpan azureUpdateProviderRetryInterval;

        private bool hasPendingChanges;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemSettingsViewModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public BackgroundSystemSettingsViewModel(
            string name,
            ICommandRegistry commandRegistry)
            : base(commandRegistry)
        {
            this.commandRegistry = commandRegistry;
            this.Name = name;
            this.PluralDisplayName = "Background System Settings";
            this.azureUpdateProviderRetryInterval = TimeSpan.FromMinutes(5);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the maintenance mode part is visible.
        /// </summary>
        public bool IsMaintenanceModeVisible
        {
            get
            {
                return this.isMaintenanceModeVisible;
            }

            set
            {
                this.SetProperty(ref this.isMaintenanceModeVisible, value, () => this.IsMaintenanceModeVisible);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether maintenance mode is enabled.
        /// </summary>
        public bool IsMaintenanceModeEnabled
        {
            get
            {
                return this.isMaintenanceModeEnabled;
            }

            set
            {
                this.SetProperty(ref this.isMaintenanceModeEnabled, value, () => this.IsMaintenanceModeEnabled);
            }
        }

        /// <summary>
        /// Gets or sets the maintenance mode reason.
        /// </summary>
        public string MaintenanceModeReason
        {
            get
            {
                return this.maintenanceModeReason;
            }

            set
            {
                this.SetProperty(ref this.maintenanceModeReason, value, () => this.MaintenanceModeReason);
            }
        }

        /// <summary>
        /// Gets a value indicating whether is dirty.
        /// </summary>
        public bool HasPendingChanges
        {
            get
            {
                return this.hasPendingChanges;
            }

            private set
            {
                this.SetProperty(
                    ref this.hasPendingChanges,
                    value,
                    () => this.HasPendingChanges);
            }
        }

        /// <summary>
        /// Gets the toggle maintenance mode command.
        /// </summary>
        public ICommand ToggleMaintenanceModeCommand
        {
            get
            {
                return
                    this.commandRegistry.GetCommand(
                        CommandCompositionKeys.Shell.BackgroundSystemSettings.ToggleMaintenanceMode);
            }
        }

        /// <summary>
        /// Gets the update azure provider command.
        /// </summary>
        public ICommand UpdateSettingsCommand
        {
            get
            {
                return
                    this.commandRegistry.GetCommand(
                        CommandCompositionKeys.Shell.BackgroundSystemSettings.UpdateSettings);
            }
        }

        /// <summary>
        /// Gets the empty content string shown when no value is entered in the editor.
        /// </summary>
        public string AzureUpdateProviderEmptyRetryInterval
        {
            get
            {
                return this.azureUpdateProviderEmptyRetryInterval;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the azure update provider is enabled.
        /// </summary>
        public bool IsAzureUpdateProviderEnabled
        {
            get
            {
                return this.isAzureUpdateProviderEnabled;
            }

            set
            {
                this.SetProperty(ref this.isAzureUpdateProviderEnabled, value, () => this.IsAzureUpdateProviderEnabled);
                this.HasPendingChanges = true;
            }
        }

        /// <summary>
        /// Gets or sets the retry interval for the Azure update provider.
        /// </summary>
        public TimeSpan AzureUpdateProviderRetryInterval
        {
            get
            {
                return this.azureUpdateProviderRetryInterval;
            }

            set
            {
                this.SetProperty(
                    ref this.azureUpdateProviderRetryInterval,
                    value,
                    () => this.AzureUpdateProviderRetryInterval);
                this.HasPendingChanges = true;
            }
        }

        /// <summary>
        /// Gets the instances.
        /// </summary>
        /// <exception cref="NotImplementedException">
        /// This property is not implemented because it is not used.
        /// </exception>
        public override IList Instances
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Gets or sets the selected instance.
        /// </summary>
        public override ReadOnlyDataViewModelBase SelectedInstance { get; set; }

        /// <summary>
        /// Gets a value indicating whether has details.
        /// This instance always returns <c>false</c>.
        /// </summary>
        public override bool HasDetails
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Clears the flag indicating whether the Azure Update Provider has changed.
        /// </summary>
        public void ClearAzureUpdateProviderChangedFlag()
        {
            this.hasPendingChanges = false;
        }
    }
}
