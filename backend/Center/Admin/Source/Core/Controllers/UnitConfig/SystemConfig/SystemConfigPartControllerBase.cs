// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemConfigPartControllerBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemConfigPartControllerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.SystemConfig
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Common.Configuration.HardwareDescription;

    /// <summary>
    /// The base class for system config part controllers that have system settings
    /// (IP address, network mask, IP gateway and time zone).
    /// </summary>
    public abstract class SystemConfigPartControllerBase : MultiEditorPartControllerBase
    {
        private const string UseDhcpKey = "UseDHCP";
        private const string IpAddressKey = "IpAddress";
        private const string NetworkMaskKey = "NetworkMask";
        private const string IpGatewayKey = "IpGateway";
        private const string DnsServer1Key = "DnsServer1";
        private const string DnsServer2Key = "DnsServer2";
        private const string TimeZoneKey = "TimeZone";

        private CheckableEditorViewModel useDhcp;

        private TextEditorViewModel ipAddress;

        private TextEditorViewModel networkMask;

        private TextEditorViewModel ipGateway;

        private TextEditorViewModel dnsServer1;

        private TextEditorViewModel dnsServer2;

        private SelectionEditorViewModel timeZone;

        private bool ipAddressEnabled;

        private bool networkMaskEnabled;

        private bool ipGatewayEnabled;

        private bool dnsServersEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfigPartControllerBase"/> class.
        /// </summary>
        /// <param name="key">
        /// The unique key to identify this part.
        /// </param>
        /// <param name="parent">
        /// The parent controller.
        /// </param>
        protected SystemConfigPartControllerBase(string key, CategoryControllerBase parent)
            : base(key, parent)
        {
        }

        /// <summary>
        /// Gets or sets a value indicating whether to use DHCP.
        /// </summary>
        public bool UseDhcp
        {
            get
            {
                return this.useDhcp.IsChecked.HasValue && this.useDhcp.IsChecked.Value;
            }

            protected set
            {
                this.useDhcp.IsChecked = value;
            }
        }

        /// <summary>
        /// Gets or sets the IP address edited by the user.
        /// </summary>
        public string IpAddress
        {
            get
            {
                return this.ipAddress.Text;
            }

            protected set
            {
                this.ipAddress.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the network mask edited by the user.
        /// </summary>
        public string NetworkMask
        {
            get
            {
                return this.networkMask.Text;
            }

            protected set
            {
                this.networkMask.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the IP gateway edited by the user.
        /// </summary>
        public string IpGateway
        {
            get
            {
                return this.ipGateway.Text;
            }

            protected set
            {
                this.ipGateway.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets the DNS servers chosen by the user.
        /// </summary>
        public List<string> DnsServers
        {
            get
            {
                var servers = new List<string>(2);
                if (!string.IsNullOrEmpty(this.dnsServer1.Text))
                {
                    servers.Add(this.dnsServer1.Text);
                }

                if (!string.IsNullOrEmpty(this.dnsServer2.Text))
                {
                    servers.Add(this.dnsServer2.Text);
                }

                return servers;
            }

            protected set
            {
                this.dnsServer1.Text = value.Count > 0 ? value[0] : string.Empty;
                this.dnsServer2.Text = value.Count > 1 ? value[1] : string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the time zone edited by the user.
        /// </summary>
        public string TimeZone
        {
            get
            {
                return this.timeZone.SelectedOption == null ? string.Empty : (string)this.timeZone.SelectedOption.Value;
            }

            protected set
            {
                this.timeZone.SelectedOption = this.timeZone.Options.FirstOrDefault(tz => tz.Value.Equals(value));
            }
        }

        /// <summary>
        /// Gets the <see cref="ConfigModePartController"/> which is (indirectly) controlling
        /// some of the properties of this controller.
        /// </summary>
        protected ConfigModePartController ConfigModeController { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the IP address field is enabled.
        /// </summary>
        protected bool IpAddressEnabled
        {
            get
            {
                return this.ipAddressEnabled;
            }

            set
            {
                if (this.ipAddressEnabled == value)
                {
                    return;
                }

                this.ipAddressEnabled = value;
                this.UpdateIpEditorsEnabled();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether network mask field is enabled.
        /// </summary>
        protected bool NetworkMaskEnabled
        {
            get
            {
                return this.networkMaskEnabled;
            }

            set
            {
                if (this.networkMaskEnabled == value)
                {
                    return;
                }

                this.networkMaskEnabled = value;
                this.UpdateIpEditorsEnabled();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether IP gateway field is enabled.
        /// </summary>
        protected bool IpGatewayEnabled
        {
            get
            {
                return this.ipGatewayEnabled;
            }

            set
            {
                if (this.ipGatewayEnabled == value)
                {
                    return;
                }

                this.ipGatewayEnabled = value;
                this.UpdateIpEditorsEnabled();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DNS servers fields are enabled.
        /// </summary>
        protected bool DnsServersEnabled
        {
            get
            {
                return this.dnsServersEnabled;
            }

            set
            {
                if (this.dnsServersEnabled == value)
                {
                    return;
                }

                this.dnsServersEnabled = value;
                this.UpdateIpEditorsEnabled();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether time zone field is enabled.
        /// </summary>
        protected bool TimeZoneEnabled
        {
            get
            {
                return this.timeZone.IsEnabled;
            }

            set
            {
                this.timeZone.IsEnabled = value;
            }
        }

        /// <summary>
        /// Loads the unit config data into this part.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.useDhcp.IsChecked = partData.GetValue(false, UseDhcpKey);
            this.ipAddress.Text = partData.GetValue(string.Empty, IpAddressKey);
            this.networkMask.Text = partData.GetValue(string.Empty, NetworkMaskKey);
            this.ipGateway.Text = partData.GetValue(string.Empty, IpGatewayKey);
            this.dnsServer1.Text = partData.GetValue(string.Empty, DnsServer1Key);
            this.dnsServer2.Text = partData.GetValue(string.Empty, DnsServer2Key);
            this.timeZone.SelectValue(partData.GetValue(string.Empty, TimeZoneKey));

            this.UpdateFromConfigMode();
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.UseDhcp, UseDhcpKey);
            partData.SetValue(this.ipAddress.Text, IpAddressKey);
            partData.SetValue(this.networkMask.Text, NetworkMaskKey);
            partData.SetValue(this.ipGateway.Text, IpGatewayKey);
            partData.SetValue(this.dnsServer1.Text, DnsServer1Key);
            partData.SetValue(this.dnsServer2.Text, DnsServer2Key);
            partData.SetValue(this.timeZone.SelectedValue as string, TimeZoneKey);
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

            this.useDhcp = new CheckableEditorViewModel();
            this.useDhcp.Label = AdminStrings.UnitConfig_SystemConfig_UseDhcp;
            this.useDhcp.IsThreeState = false;
            this.useDhcp.PropertyChanged += (s, e) => this.UpdateIpEditorsEnabled();
            viewModel.Editors.Add(this.useDhcp);

            this.ipAddress = new TextEditorViewModel();
            this.ipAddress.Label = AdminStrings.UnitConfig_SystemConfig_IpAddress;
            viewModel.Editors.Add(this.ipAddress);

            this.networkMask = new TextEditorViewModel();
            this.networkMask.Label = AdminStrings.UnitConfig_SystemConfig_NetworkMask;
            viewModel.Editors.Add(this.networkMask);

            this.ipGateway = new TextEditorViewModel();
            this.ipGateway.Label = AdminStrings.UnitConfig_SystemConfig_IpGateway;
            viewModel.Editors.Add(this.ipGateway);

            this.dnsServer1 = new TextEditorViewModel();
            this.dnsServer1.Label = AdminStrings.UnitConfig_SystemConfig_DnsServer1;
            viewModel.Editors.Add(this.dnsServer1);

            this.dnsServer2 = new TextEditorViewModel();
            this.dnsServer2.Label = AdminStrings.UnitConfig_SystemConfig_DnsServer2;
            viewModel.Editors.Add(this.dnsServer2);

            this.timeZone = new SelectionEditorViewModel();
            this.timeZone.Label = AdminStrings.UnitConfig_SystemConfig_TimeZone;
            viewModel.Editors.Add(this.timeZone);
            foreach (
                var zone in
                    TimeZoneInfo.GetSystemTimeZones()
                        .OrderBy(z => z.DisplayName, StringComparer.CurrentCultureIgnoreCase))
            {
                this.timeZone.Options.Add(new SelectionOptionViewModel(zone.DisplayName, zone.Id));
            }

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
            this.ConfigModeController = this.GetPart<ConfigModePartController>();
            this.ConfigModeController.ViewModelUpdated += this.ConfigModeOnViewModelUpdated;

            var validator = new VersionedSettingValidator(
                this.useDhcp,
                false,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.SupportsDhcp,
                this.Parent.Parent);
            validator.Start();

            validator = new VersionedSettingValidator(
                this.dnsServer1,
                string.Empty,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.SupportsDnsServers,
                this.Parent.Parent);
            validator.Start();

            validator = new VersionedSettingValidator(
                this.dnsServer2,
                string.Empty,
                PackageIds.Motion.HardwareManager,
                SoftwareVersions.HardwareManager.SupportsDnsServers,
                this.Parent.Parent);
            validator.Start();
        }

        /// <summary>
        /// Updates this controller with the values from <see cref="ConfigModeController"/>.
        /// </summary>
        protected abstract void UpdateFromConfigMode();

        /// <summary>
        /// Updates the errors shown to the user.
        /// </summary>
        protected virtual void UpdateErrors()
        {
            this.UpdateIpAddressError(this.ipAddress);
            this.UpdateIpAddressError(this.networkMask);
            this.UpdateIpAddressError(this.ipGateway);
            this.UpdateIpAddressError(this.dnsServer1, false);
            this.UpdateIpAddressError(this.dnsServer2, false);

            if (!this.timeZone.IsEnabled || this.timeZone.SelectedOption != null)
            {
                this.timeZone.RemoveError("SelectedOption", AdminStrings.Errors_NoItemSelected);
                return;
            }

            this.timeZone.SetError(
                "SelectedOption",
                this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing,
                AdminStrings.Errors_NoItemSelected);
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

        private void UpdateIpEditorsEnabled()
        {
            this.useDhcp.IsEnabled = this.IpAddressEnabled;
            this.ipAddress.IsEnabled = this.IpAddressEnabled && !this.UseDhcp;
            this.networkMask.IsEnabled = this.NetworkMaskEnabled && !this.UseDhcp;
            this.ipGateway.IsEnabled = this.IpGatewayEnabled && !this.UseDhcp;
            this.dnsServer1.IsEnabled = this.DnsServersEnabled;
            this.dnsServer2.IsEnabled = this.DnsServersEnabled;
        }

        private void UpdateIpAddressError(TextEditorViewModel textEditor, bool mandatory = true)
        {
            IPAddress address;
            if (!textEditor.IsEnabled || IPAddress.TryParse(textEditor.Text, out address)
                || (!mandatory && string.IsNullOrEmpty(textEditor.Text)))
            {
                textEditor.RemoveError("Text", AdminStrings.Errors_ValidIpAddress);
                return;
            }

            textEditor.SetError(
                "Text",
                this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing,
                AdminStrings.Errors_ValidIpAddress);
        }

        private void ConfigModeOnViewModelUpdated(object sender, EventArgs e)
        {
            this.UpdateFromConfigMode();
        }
    }
}