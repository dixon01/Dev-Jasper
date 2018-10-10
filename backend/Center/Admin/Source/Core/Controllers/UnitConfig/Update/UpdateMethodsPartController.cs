// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateMethodsPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateMethodsPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Update
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.Controllers.UnitConfig.Software;
    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Meta;
    using Gorba.Center.Common.ServiceModel.Settings;
    using Gorba.Common.Configuration.HardwareDescription;
    using Gorba.Common.Configuration.Update.Providers;

    /// <summary>
    /// The update methods part controller where the user can choose the possible update methods.
    /// </summary>
    public class UpdateMethodsPartController : MultiEditorPartControllerBase
    {
        private const string AzureKey = "Azure";
        private const string UsbKey = "USB";
        private const string FtpKey = "FTP";
        private const string MediMasterKey = "MediMaster";

        private const string MediSlaveKey = "MediSlave";
        private const string RequireWifiKey = "RequireWifi";

        private CheckableEditorViewModel azureCheckBox;

        private CheckableEditorViewModel usbCheckBox;

        private CheckableEditorViewModel ftpCheckBox;

        private CheckableEditorViewModel mediMasterCheckBox;

        private CheckableEditorViewModel mediSlaveCheckBox;

        private CheckableEditorViewModel requireWifiCheckBox;

        private IncomingPartController incoming;

        private OutgoingPartController outgoing;

        private BackgroundSystemSettings backgroundSystemSettings;

        private BackgroundSystemConnectionPartController backgroundSystemConnection;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateMethodsPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public UpdateMethodsPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.Update.Methods, parent)
        {
        }

        /// <summary>
        /// Gets a value indicating whether Azure has been checked.
        /// </summary>
        public bool HasAzureChecked
        {
            get
            {
                return this.azureCheckBox.IsEnabled && this.azureCheckBox.IsChecked.HasValue
                       && this.azureCheckBox.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether USB has been checked.
        /// </summary>
        public bool HasUsbChecked
        {
            get
            {
                return this.usbCheckBox.IsChecked.HasValue && this.usbCheckBox.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether FTP has been checked.
        /// </summary>
        public bool HasFtpChecked
        {
            get
            {
                return this.ftpCheckBox.IsEnabled && this.ftpCheckBox.IsChecked.HasValue
                       && this.ftpCheckBox.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Medi Master has been checked.
        /// </summary>
        public bool HasMediMasterChecked
        {
            get
            {
                return this.mediMasterCheckBox.IsEnabled && this.mediMasterCheckBox.IsChecked.HasValue
                       && this.mediMasterCheckBox.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Medi Slave has been checked.
        /// </summary>
        public bool HasMediSlaveChecked
        {
            get
            {
                return this.mediSlaveCheckBox.IsEnabled && this.mediSlaveCheckBox.IsChecked.HasValue
                       && this.mediSlaveCheckBox.IsChecked.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether Medi Slave has been checked.
        /// </summary>
        public bool HasRequireWifiChecked
        {
            get
            {
                if (requireWifiCheckBox.IsChecked.HasValue)
                {
                    return requireWifiCheckBox.IsChecked.Value;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Asynchronously prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to wait on.
        /// </returns>
        public async override Task PrepareAsync(HardwareDescriptor descriptor)
        {
            await base.PrepareAsync(descriptor);

            this.incoming = this.GetPart<IncomingPartController>();
            this.incoming.ViewModelUpdated += (s, e) => this.UpdateMedi();
            this.outgoing = this.GetPart<OutgoingPartController>();
            this.outgoing.ViewModelUpdated += (s, e) => this.UpdateMedi();
            this.UpdateMedi();

            var connectionController = this.Parent.Parent.DataController.ConnectionController;
            using (var systemConfig = connectionController.CreateChannelScope<ISystemConfigDataService>())
            {
                var query = SystemConfigQuery.Create().IncludeSettings();
                var config = (await systemConfig.Channel.QueryAsync(query)).FirstOrDefault();
                if (config == null)
                {
                    return;
                }

                this.backgroundSystemSettings = (BackgroundSystemSettings)config.Settings.Deserialize();
            }

            this.backgroundSystemConnection = this.GetPart<BackgroundSystemConnectionPartController>();
            this.backgroundSystemConnection.ViewModelUpdated += (s, e) => this.UpdateErrors();
            this.UpdateErrors();

            var validator = new VersionedSettingValidator(
                this.azureCheckBox,
                false,
                PackageIds.Motion.Update,
                SoftwareVersions.Update.SupportsAzureUpdateClient,
                this.Parent.Parent);
            validator.Start();
        }

        /// <summary>
        /// Gets the list of FTP update providers configured in the background system.
        /// </summary>
        /// <returns>
        /// The FTP update providers configured in the background system.
        /// </returns>
        public IEnumerable<FtpUpdateProviderConfig> GetFtpUpdateProviders()
        {
            if (this.backgroundSystemSettings == null || this.backgroundSystemSettings.FtpUpdateProviders == null)
            {
                return Enumerable.Empty<FtpUpdateProviderConfig>();
            }

            return this.backgroundSystemSettings.FtpUpdateProviders;
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.azureCheckBox.IsEnabled = this.backgroundSystemSettings.AzureUpdateProvider != null;
            this.ftpCheckBox.IsEnabled = this.GetFtpUpdateProviders().Any();

            this.azureCheckBox.IsChecked = partData.GetValue(false, AzureKey);
            this.usbCheckBox.IsChecked = partData.GetValue(true, UsbKey);
            this.ftpCheckBox.IsChecked = partData.GetValue(this.ftpCheckBox.IsEnabled, FtpKey);
            this.mediMasterCheckBox.IsChecked = partData.GetValue(false, MediMasterKey);
            this.mediSlaveCheckBox.IsChecked = partData.GetValue(false, MediSlaveKey);

            this.UpdateErrors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.HasAzureChecked, AzureKey);
            partData.SetValue(this.HasUsbChecked, UsbKey);
            partData.SetValue(this.HasFtpChecked, FtpKey);
            partData.SetValue(this.HasMediMasterChecked, MediMasterKey);
            partData.SetValue(this.HasMediSlaveChecked, MediSlaveKey);
            partData.SetValue(this.HasRequireWifiChecked, RequireWifiKey);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = new MultiEditorPartViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_Update_Methods;
            viewModel.Description = AdminStrings.UnitConfig_Update_Methods_Description;
            viewModel.IsVisible = true;

            this.azureCheckBox = new CheckableEditorViewModel();
            this.azureCheckBox.Label = AdminStrings.UnitConfig_Update_Methods_Azure;
            viewModel.Editors.Add(this.azureCheckBox);

            this.usbCheckBox = new CheckableEditorViewModel();
            this.usbCheckBox.Label = AdminStrings.UnitConfig_Update_Methods_USB;
            viewModel.Editors.Add(this.usbCheckBox);

            this.ftpCheckBox = new CheckableEditorViewModel();
            this.ftpCheckBox.Label = AdminStrings.UnitConfig_Update_Methods_FTP;
            viewModel.Editors.Add(this.ftpCheckBox);

            this.mediMasterCheckBox = new CheckableEditorViewModel();
            this.mediMasterCheckBox.Label = AdminStrings.UnitConfig_Update_Methods_MediMaster;
            viewModel.Editors.Add(this.mediMasterCheckBox);

            this.mediSlaveCheckBox = new CheckableEditorViewModel();
            this.mediSlaveCheckBox.Label = AdminStrings.UnitConfig_Update_Methods_MediSlave;
            viewModel.Editors.Add(this.mediSlaveCheckBox);

            this.requireWifiCheckBox = new CheckableEditorViewModel();
            this.requireWifiCheckBox.Label = AdminStrings.UnitConfig_Update_Methods_RequireWifi;
            viewModel.Editors.Add(this.requireWifiCheckBox);

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        private void UpdateMedi()
        {
            this.mediMasterCheckBox.IsEnabled = this.outgoing.HasSelected(OutgoingData.Medi);
            this.mediSlaveCheckBox.IsEnabled = this.incoming.HasSelected(IncomingData.Medi);
        }

        private void UpdateErrors()
        {
            var error = !this.HasAzureChecked && !this.HasUsbChecked && !this.HasFtpChecked && !this.HasMediSlaveChecked
                            ? ErrorState.Warning
                            : ErrorState.Ok;
            this.azureCheckBox.SetError("IsChecked", error, AdminStrings.UnitConfig_Update_Methods_ChooseOne);
            this.usbCheckBox.SetError("IsChecked", error, AdminStrings.UnitConfig_Update_Methods_ChooseOne);
            this.ftpCheckBox.SetError("IsChecked", error, AdminStrings.UnitConfig_Update_Methods_ChooseOne);

            error = this.HasMediMasterChecked && this.HasMediSlaveChecked ? ErrorState.Error : ErrorState.Ok;
            this.mediMasterCheckBox.SetError("IsChecked", error, AdminStrings.UnitConfig_Update_Methods_BothMedi);
            this.mediSlaveCheckBox.SetError("IsChecked", error, AdminStrings.UnitConfig_Update_Methods_BothMedi);

            error = this.HasAzureChecked && this.HasMediSlaveChecked ? ErrorState.Warning : ErrorState.Ok;
            this.azureCheckBox.SetError(
                "IsChecked", error, AdminStrings.UnitConfig_Update_Methods_AzureAndMediSlave);

            error = this.HasAzureChecked && !this.backgroundSystemConnection.ShouldConnect
                        ? ErrorState.Error
                        : ErrorState.Ok;
            this.azureCheckBox.SetError("IsChecked", error, AdminStrings.UnitConfig_Update_Methods_AzureNoBGS);
        }
    }
}