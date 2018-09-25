// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundSystemSettingsStageController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The background system settings stage controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Meta
{
    using System;
    using System.Linq;
    using System.Windows;

    using Gorba.Center.Admin.Core.DataViewModels;
    using Gorba.Center.Admin.Core.DataViewModels.Meta;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.Stages.Meta;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Center.Common.Wpf.Client.Interaction;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;

    using Microsoft.Practices.ServiceLocation;

    /// <summary>
    /// The background system settings stage controller.
    /// </summary>
    public class BackgroundSystemSettingsStageController : EntityStageControllerBase
    {
        private readonly SystemConfigDataController systemConfigDataController;
        private SystemConfigReadOnlyDataViewModel systemConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundSystemSettingsStageController"/> class.
        /// </summary>
        /// <param name="dataController">
        /// The data controller.
        /// </param>
        public BackgroundSystemSettingsStageController(SystemConfigDataController dataController)
            : base(dataController)
        {
            this.Name = "BackgroundSystemSettings";
            this.PartitionName = "Meta";
            this.StageViewModel =
                this.Stage = new BackgroundSystemSettingsViewModel(this.Name, dataController.Factory.CommandRegistry);
            var commandRegistry = dataController.Factory.CommandRegistry;
            this.systemConfigDataController = dataController;
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.BackgroundSystemSettings.ToggleMaintenanceMode,
                new RelayCommand<BackgroundSystemSettingsViewModel>(this.ToggleMaintenanceModeAsync));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Shell.BackgroundSystemSettings.UpdateSettings,
                new RelayCommand<BackgroundSystemSettingsViewModel>(
                    this.UpdateSettings,
                    this.CanUpdateAzureProvider));
        }

        /// <summary>
        /// Gets the stage managed by this controller.
        /// </summary>
        public BackgroundSystemSettingsViewModel Stage { get; private set; }

        /// <summary>
        /// The supports entity.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool SupportsEntity(ReadOnlyDataViewModelBase dataViewModel)
        {
            return false;
        }

        /// <summary>
        /// The supports entity.
        /// </summary>
        /// <param name="dataViewModel">
        /// The data view model.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool SupportsEntity(DataViewModelBase dataViewModel)
        {
            return false;
        }

        /// <summary>
        /// Loads the entity instances from the given connection controller.
        /// </summary>
        public override async void LoadData()
        {
            try
            {
                var state = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
                var currentUser = state.CurrentUser;
                this.Stage.IsMaintenanceModeVisible = currentUser.Username.Equals(
                    CommonNames.AdminUsername,
                    StringComparison.CurrentCultureIgnoreCase);

                await this.systemConfigDataController.AwaitAllDataAsync();
                this.systemConfig = this.systemConfigDataController.All.First();
                var settings = (BackgroundSystemSettings)this.systemConfig.ReadableModel.Settings.Deserialize();
                this.Stage.IsMaintenanceModeEnabled = settings.MaintenanceMode.IsEnabled;
                this.Stage.MaintenanceModeReason = settings.MaintenanceMode.Reason;
                this.Stage.IsAzureUpdateProviderEnabled = settings.AzureUpdateProvider != null;
                if (settings.AzureUpdateProvider != null)
                {
                    this.Stage.AzureUpdateProviderRetryInterval = settings.AzureUpdateProvider.RetryInterval;
                }

                this.Stage.ClearAzureUpdateProviderChangedFlag();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't load data");
                var prompt = new ConnectionExceptionPrompt(
                    ex,
                    AdminStrings.BgsSettingsError_LoadData_Message,
                    AdminStrings.BgsSettingsError_LoadData_Title);
                this.StartNew(() => InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt));
            }
        }

        /// <summary>
        /// Updates the permissions of the <see cref="EntityStageControllerBase.StageViewModel"/>
        /// with the permission controller.
        /// </summary>
        protected override void UpdatePermissions()
        {
            this.UpdatePermissions(DataScope.Meta);
        }

        private async void ToggleMaintenanceModeAsync(
          BackgroundSystemSettingsViewModel backgroundSystemSettingsViewModel)
        {
            try
            {
                if (backgroundSystemSettingsViewModel.IsMaintenanceModeEnabled)
                {
                    var result = MessageBox.Show(
                        AdminStrings.BgsSettings_MaintenanceModeConfirmationMessage,
                        AdminStrings.BgsSettings_MaintenanceModeConfirmationTitle,
                        MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No)
                    {
                        backgroundSystemSettingsViewModel.IsMaintenanceModeEnabled = false;
                        return;
                    }
                }

                var config =
                (SystemConfigDataViewModel)(await this.systemConfigDataController.EditEntityAsync(this.systemConfig));
                var settings = (BackgroundSystemSettings)config.Settings.XmlData.Deserialize();
                settings.MaintenanceMode = new MaintenanceModeSettings
                {
                    IsEnabled = backgroundSystemSettingsViewModel.IsMaintenanceModeEnabled,
                    Reason = backgroundSystemSettingsViewModel.MaintenanceModeReason
                };
                config.Settings.XmlData = new XmlData(settings);
                await this.systemConfigDataController.SaveEntityAsync(config);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't update maintenance mode");
                var prompt = new ConnectionExceptionPrompt(
                    ex,
                    AdminStrings.BgsSettingsError_UpdateMaintenanceMode_Message,
                    AdminStrings.BgsSettingsError_UpdateMaintenanceMode_Title);
                this.StartNew(() => InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt));
            }
        }

        private async void UpdateSettings(BackgroundSystemSettingsViewModel backgroundSystemSettingsViewModel)
        {
            try
            {
                var config =
                    (SystemConfigDataViewModel)
                    (await this.systemConfigDataController.EditEntityAsync(this.systemConfig));
                var settings = (BackgroundSystemSettings)config.Settings.XmlData.Deserialize();
                if (backgroundSystemSettingsViewModel.IsAzureUpdateProviderEnabled)
                {
                    settings.AzureUpdateProvider = new AzureUpdateProviderConfig
                                                       {
                                                           Name = "AzureUpdateProvider",
                                                           RetryInterval =
                                                               backgroundSystemSettingsViewModel
                                                               .AzureUpdateProviderRetryInterval
                                                       };
                }
                else
                {
                    settings.AzureUpdateProvider = null;
                }

                config.Settings.XmlData = new XmlData(settings);
                await this.systemConfigDataController.SaveEntityAsync(config);
                backgroundSystemSettingsViewModel.ClearAzureUpdateProviderChangedFlag();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Couldn't update Azure Update Provider");
                var prompt = new ConnectionExceptionPrompt(
                    ex,
                    AdminStrings.BgsSettingsError_UpdateAzureProvider_Message,
                    AdminStrings.BgsSettingsError_UpdateAzureProvider_Title);
                this.StartNew(() => InteractionManager<ConnectionExceptionPrompt>.Current.Raise(prompt));
            }
        }

        private bool CanUpdateAzureProvider(BackgroundSystemSettingsViewModel backgroundSystemSettingsViewModel)
        {
            return backgroundSystemSettingsViewModel != null
                   && backgroundSystemSettingsViewModel.HasPendingChanges;
        }
    }
}
