// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GlobalSystemConfigPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GlobalSystemConfigPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig
{
    using System.Collections.Generic;

    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The global system config part controller.
    /// </summary>
    public class GlobalSystemConfigPartController : SystemConfigPartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GlobalSystemConfigPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public GlobalSystemConfigPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.SystemConfig.Global, parent)
        {
        }

        /// <summary>
        /// Updates this controller with the values from
        /// <see cref="SystemConfigPartControllerBase.ConfigModeController"/>.
        /// </summary>
        protected override void UpdateFromConfigMode()
        {
            this.IpAddressEnabled = this.ConfigModeController.GetIpAddressUsage() == SettingItemMode.UseGlobal;
            this.NetworkMaskEnabled = this.ConfigModeController.GetNetworkMaskUsage() == SettingItemMode.UseGlobal;
            this.IpGatewayEnabled = this.ConfigModeController.GetIpGatewayUsage() == SettingItemMode.UseGlobal;
            this.DnsServersEnabled = this.ConfigModeController.GetDnsServersUsage() == SettingItemMode.UseGlobal;
            this.TimeZoneEnabled = this.ConfigModeController.GetTimeZoneUsage() == SettingItemMode.UseGlobal;

            this.ViewModel.IsVisible = this.ConfigModeController.ShouldUseMultiConfig()
                                       && (this.IpAddressEnabled || this.NetworkMaskEnabled || this.IpGatewayEnabled
                                           || this.DnsServersEnabled || this.TimeZoneEnabled);

            if (!this.IpAddressEnabled)
            {
                this.IpAddress = string.Empty;
            }

            if (!this.NetworkMaskEnabled)
            {
                this.NetworkMask = string.Empty;
            }

            if (!this.IpGatewayEnabled)
            {
                this.IpGateway = string.Empty;
            }

            if (!this.DnsServersEnabled)
            {
                this.DnsServers = new List<string>();
            }

            if (!this.TimeZoneEnabled)
            {
                this.TimeZone = string.Empty;
            }
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override MultiEditorPartViewModel CreateViewModel()
        {
            var viewModel = base.CreateViewModel();
            viewModel.DisplayName = AdminStrings.UnitConfig_SystemConfig_Global;
            viewModel.Description = AdminStrings.UnitConfig_SystemConfig_Global_Description;
            return viewModel;
        }
    }
}