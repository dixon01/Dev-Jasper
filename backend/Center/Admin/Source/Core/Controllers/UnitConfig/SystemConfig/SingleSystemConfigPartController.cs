// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleSystemConfigPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SingleSystemConfigPartController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig
{
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;

    /// <summary>
    /// The single system configuration part controller used if there is no I/O specific configuration needed.
    /// </summary>
    public class SingleSystemConfigPartController : SystemConfigPartControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingleSystemConfigPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        public SingleSystemConfigPartController(CategoryControllerBase parent)
            : base(UnitConfigKeys.SystemConfig.Single, parent)
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

            this.ViewModel.IsVisible = !this.ConfigModeController.ShouldUseMultiConfig()
                                       && (this.IpAddressEnabled || this.NetworkMaskEnabled || this.IpGatewayEnabled
                                           || this.DnsServersEnabled || this.TimeZoneEnabled);
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
            viewModel.DisplayName = AdminStrings.UnitConfig_SystemConfig_Single;
            viewModel.Description = AdminStrings.UnitConfig_SystemConfig_Single_Description;
            return viewModel;
        }
    }
}