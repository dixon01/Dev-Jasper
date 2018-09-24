// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConfigModePartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConfigModePartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig
{
    using System;
    using System.Linq;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The config mode part controller.
    /// </summary>
    public class ConfigModePartController : MultiEditorPartControllerBase
    {
        private const string MultiModeKey = "MultiMode";
        private const string NumConfigsKey = "NumConfigs";
        private const string GlobalIpAddressKey = "GlobalIpAddress";
        private const string GlobalNetworkMaskKey = "GlobalNetworkMask";
        private const string GlobalIpGatewayKey = "GlobalIpGateway";
        private const string GlobalDnsServersKey = "GlobalDnsServers";
        private const string GlobalTimeZoneKey = "GlobalTimeZone";

        private SelectionEditorViewModel useMultiConfig;

        private NumberEditorViewModel numberOfConfigs;

        private SelectionEditorViewModel ipAddressUsage;

        private SelectionEditorViewModel networkMaskUsage;

        private SelectionEditorViewModel ipGatewayUsage;

        private SelectionEditorViewModel dnsServersUsage;

        private SelectionEditorViewModel timeZoneUsage;

        private SelectionOptionViewModel useSpecificOption;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigModePartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public ConfigModePartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.SystemConfig.ConfigMode, parent)
        {
        }

        /// <summary>
        /// Gets a flag indicating if multiple per-I/O configurations should be supported.
        /// </summary>
        /// <returns>
        /// True for multiple configurations, false for single unit configuration wide settings.
        /// </returns>
        public bool ShouldUseMultiConfig()
        {
            return (bool)this.useMultiConfig.SelectedValue;
        }

        /// <summary>
        /// Gets the number of different I/O specific configurations that the user can edit.
        /// </summary>
        /// <returns>
        /// The number of I/O specific configurations visible to the user.
        /// </returns>
        public int GetIoConfigCount()
        {
            return (int)this.numberOfConfigs.Value;
        }

        /// <summary>
        /// Gets the way the IP address is configured.
        /// </summary>
        /// <returns>
        /// The <see cref="SettingItemMode"/> for the IP address.
        /// </returns>
        public SettingItemMode GetIpAddressUsage()
        {
            return GetUsage(this.ipAddressUsage);
        }

        /// <summary>
        /// Gets the way the IP network mask is configured.
        /// </summary>
        /// <returns>
        /// The <see cref="SettingItemMode"/> for the IP network mask.
        /// </returns>
        public SettingItemMode GetNetworkMaskUsage()
        {
            return GetUsage(this.networkMaskUsage);
        }

        /// <summary>
        /// Gets the way the IP gateway is configured.
        /// </summary>
        /// <returns>
        /// The <see cref="SettingItemMode"/> for the IP gateway.
        /// </returns>
        public SettingItemMode GetIpGatewayUsage()
        {
            return GetUsage(this.ipGatewayUsage);
        }

        /// <summary>
        /// Gets the way the DNS servers are configured.
        /// </summary>
        /// <returns>
        /// The <see cref="SettingItemMode"/> for the DNS servers.
        /// </returns>
        public SettingItemMode GetDnsServersUsage()
        {
            return GetUsage(this.dnsServersUsage);
        }

        /// <summary>
        /// Gets the way the time zone is configured.
        /// </summary>
        /// <returns>
        /// The <see cref="SettingItemMode"/> for the time zone.
        /// </returns>
        public SettingItemMode GetTimeZoneUsage()
        {
            return GetUsage(this.timeZoneUsage);
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.useMultiConfig.SelectValue(partData.GetValue(false, MultiModeKey));
            this.numberOfConfigs.Value = partData.GetValue(2, NumConfigsKey);
            this.ipAddressUsage.SelectValue(partData.GetEnumValue(SettingItemMode.NotConfigured, GlobalIpAddressKey));
            this.networkMaskUsage.SelectValue(
                partData.GetEnumValue(SettingItemMode.NotConfigured, GlobalNetworkMaskKey));
            this.ipGatewayUsage.SelectValue(partData.GetEnumValue(SettingItemMode.NotConfigured, GlobalIpGatewayKey));
            this.dnsServersUsage.SelectValue(partData.GetEnumValue(SettingItemMode.NotConfigured, GlobalDnsServersKey));
            this.timeZoneUsage.SelectValue(partData.GetEnumValue(SettingItemMode.NotConfigured, GlobalTimeZoneKey));

            this.UpdateEditors();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue((bool)this.useMultiConfig.SelectedValue, MultiModeKey);
            partData.SetValue(this.numberOfConfigs.Value, NumConfigsKey);
            partData.SetEnumValue(this.GetIpAddressUsage(), GlobalIpAddressKey);
            partData.SetEnumValue(this.GetNetworkMaskUsage(), GlobalNetworkMaskKey);
            partData.SetEnumValue(this.GetIpGatewayUsage(), GlobalIpGatewayKey);
            partData.SetEnumValue(this.GetDnsServersUsage(), GlobalDnsServersKey);
            partData.SetEnumValue(this.GetTimeZoneUsage(), GlobalTimeZoneKey);
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            this.useSpecificOption =
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_SystemConfig_ConfigMode_UseSpecific,
                    SettingItemMode.UseSpecific);

            var viewModel = new MultiEditorPartViewModel();

            viewModel.DisplayName = AdminStrings.UnitConfig_SystemConfig_ConfigMode;
            viewModel.Description = AdminStrings.UnitConfig_SystemConfig_ConfigMode_Description;
            viewModel.IsVisible = true;

            this.useMultiConfig = new SelectionEditorViewModel();
            this.useMultiConfig.Label = AdminStrings.UnitConfig_SystemConfig_ConfigMode_Mode;
            this.useMultiConfig.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_SystemConfig_ConfigMode_Single, false));
            this.useMultiConfig.Options.Add(
                new SelectionOptionViewModel(AdminStrings.UnitConfig_SystemConfig_ConfigMode_Multi, true));
            viewModel.Editors.Add(this.useMultiConfig);

            this.numberOfConfigs = new NumberEditorViewModel();
            this.numberOfConfigs.Label = AdminStrings.UnitConfig_SystemConfig_ConfigMode_NumConfigs;
            this.numberOfConfigs.MinValue = 2;
            this.numberOfConfigs.MaxValue = SystemConfigCategoryController.MaxMultiConfigCount;
            this.numberOfConfigs.IsInteger = true;
            viewModel.Editors.Add(this.numberOfConfigs);

            this.ipAddressUsage = CreateItemModeEditor();
            this.ipAddressUsage.Label = AdminStrings.UnitConfig_SystemConfig_IpAddress;
            viewModel.Editors.Add(this.ipAddressUsage);

            this.networkMaskUsage = CreateItemModeEditor();
            this.networkMaskUsage.Label = AdminStrings.UnitConfig_SystemConfig_NetworkMask;
            viewModel.Editors.Add(this.networkMaskUsage);

            this.ipGatewayUsage = CreateItemModeEditor();
            this.ipGatewayUsage.Label = AdminStrings.UnitConfig_SystemConfig_IpGateway;
            viewModel.Editors.Add(this.ipGatewayUsage);

            this.dnsServersUsage = CreateItemModeEditor();
            this.dnsServersUsage.Label = AdminStrings.UnitConfig_SystemConfig_DnsServers;
            viewModel.Editors.Add(this.dnsServersUsage);

            this.timeZoneUsage = CreateItemModeEditor();
            this.timeZoneUsage.Label = AdminStrings.UnitConfig_SystemConfig_TimeZone;
            viewModel.Editors.Add(this.timeZoneUsage);

            return viewModel;
        }

        /// <summary>
        /// Prepares this part controller with the given hardware descriptor.
        /// </summary>
        /// <param name="descriptor">
        /// The hardware descriptor.
        /// </param>
        protected override void Prepare(HardwareDescriptor descriptor)
        {
            base.Prepare(descriptor);

            var validator = new VersionedSettingValidator(
                this.dnsServersUsage,
                SettingItemMode.NotConfigured,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.SupportsDnsServers,
                this.Parent.Parent);
            validator.Start();
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
            this.UpdateEditors();
        }

        private static SelectionEditorViewModel CreateItemModeEditor()
        {
            var editor = new SelectionEditorViewModel();
            editor.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_SystemConfig_ConfigMode_NotConfigured,
                    SettingItemMode.NotConfigured));
            editor.Options.Add(
                new SelectionOptionViewModel(
                    AdminStrings.UnitConfig_SystemConfig_ConfigMode_UseGlobal,
                    SettingItemMode.UseGlobal));
            return editor;
        }

        private static SettingItemMode GetUsage(SelectionEditorViewModel selectionEditor)
        {
            return selectionEditor.SelectedValue == null
                       ? SettingItemMode.NotConfigured
                       : (SettingItemMode)selectionEditor.SelectedValue;
        }

        private void UpdateEditors()
        {
            if (this.useMultiConfig.SelectedValue == null)
            {
                return;
            }

            var useMulti = (bool)this.useMultiConfig.SelectedValue;
            this.numberOfConfigs.IsEnabled = useMulti;

            foreach (var editor in this.ViewModel.Editors.Skip(2).OfType<SelectionEditorViewModel>())
            {
                if (useMulti)
                {
                    if (!editor.Options.Contains(this.useSpecificOption))
                    {
                        editor.Options.Add(this.useSpecificOption);
                    }
                }
                else
                {
                    if (editor.SelectedOption == this.useSpecificOption)
                    {
                        editor.SelectedOption = null;
                    }

                    editor.Options.Remove(this.useSpecificOption);
                }

                editor.SetError(
                    "SelectedOption",
                    editor.SelectedOption == null ? ErrorState.Error : ErrorState.Ok,
                    AdminStrings.Errors_NoItemSelected);
            }
        }
    }
}