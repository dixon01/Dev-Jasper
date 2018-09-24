// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EditIpSettingsPromptNotification.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EditIpSettingsPromptNotification type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Interaction
{
    using System;
    using System.Collections;
    using System.ComponentModel;

    using Gorba.Center.Common.Wpf.Client.ViewModels;
    using Gorba.Center.Common.Wpf.Framework.Notifications;
    using Gorba.Center.Diag.Core.Resources;
    using Gorba.Center.Diag.Core.ViewModels.Unit;

    /// <summary>
    /// The edit ip settings prompt notification.
    /// </summary>
    public class EditIpSettingsPromptNotification : PromptNotification, INotifyDataErrorInfo
    {
        private readonly ErrorViewModelBase errors = new ErrorViewModelBase();

        private bool useDhcp;

        private string ipAddress;

        private string networkMask;

        private string gatewayAddress;

        private bool isDhcpEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditIpSettingsPromptNotification"/> class.
        /// </summary>
        public EditIpSettingsPromptNotification()
        {
            this.errors.PropertyChanged += (s, e) => this.RaisePropertyChanged(e.PropertyName);
        }

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged
        {
            add
            {
                this.errors.ErrorsChanged += value;
            }

            remove
            {
                this.errors.ErrorsChanged -= value;
            }
        }

        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        public MotionUnitViewModel Unit { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether DHCP should be used on the Unit.
        /// </summary>
        public bool UseDhcp
        {
            get
            {
                return this.useDhcp;
            }

            set
            {
                if (!this.SetProperty(ref this.useDhcp, value, () => this.UseDhcp))
                {
                    return;
                }

                if (value)
                {
                    this.errors.ClearErrors();
                }
                else
                {
                    this.UpdateIpAddressError(this.IpAddress, "IpAddress");
                    this.UpdateIpAddressError(this.NetworkMask, "NetworkMask");
                    this.UpdateIpAddressError(this.GatewayAddress, "GatewayAddress", true);
                }

                // raise the PropertyChanged events, otherwise the error indicators don't get updated
                this.RaisePropertyChanged(() => this.IpAddress);
                this.RaisePropertyChanged(() => this.NetworkMask);
                this.RaisePropertyChanged(() => this.GatewayAddress);
            }
        }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        public string IpAddress
        {
            get
            {
                return this.ipAddress;
            }

            set
            {
                if (this.SetProperty(ref this.ipAddress, value, () => this.IpAddress))
                {
                    this.UpdateIpAddressError(value, "IpAddress");
                }
            }
        }

        /// <summary>
        /// Gets or sets the network mask.
        /// </summary>
        public string NetworkMask
        {
            get
            {
                return this.networkMask;
            }

            set
            {
                if (this.SetProperty(ref this.networkMask, value, () => this.NetworkMask))
                {
                    this.UpdateIpAddressError(value, "NetworkMask");
                }
            }
        }

        /// <summary>
        /// Gets or sets the gateway address.
        /// </summary>
        public string GatewayAddress
        {
            get
            {
                return this.gatewayAddress;
            }

            set
            {
                if (this.SetProperty(ref this.gatewayAddress, value, () => this.GatewayAddress))
                {
                    this.UpdateIpAddressError(value, "GatewayAddress", true);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the DHCP checkbox is enabled.
        /// </summary>
        public bool IsDhcpEnabled
        {
            get
            {
                return this.isDhcpEnabled;
            }

            set
            {
                if (this.SetProperty(ref this.isDhcpEnabled, value, () => this.IsDhcpEnabled) && !value)
                {
                    this.UseDhcp = false;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the entity has validation errors.
        /// </summary>
        /// <returns>
        /// true if the entity currently has validation errors; otherwise, false.
        /// </returns>
        public bool HasErrors
        {
            get
            {
                return this.errors.HasErrors;
            }
        }

        /// <summary>
        /// Gets the validation errors for a specified property or for the entire entity.
        /// </summary>
        /// <returns>
        /// The validation errors for the property or entity.
        /// </returns>
        /// <param name="propertyName">
        /// The name of the property to retrieve validation errors for; or null or <see cref="F:System.String.Empty"/>,
        /// to retrieve entity-level errors.
        /// </param>
        public IEnumerable GetErrors(string propertyName)
        {
            return this.errors.GetErrors(propertyName);
        }

        private void UpdateIpAddressError(string value, string propertyName, bool allowEmpty = false)
        {
            this.errors.ClearErrors(propertyName);
            if (string.IsNullOrEmpty(value))
            {
                if (allowEmpty)
                {
                    return;
                }

                this.errors.AddError(propertyName, DiagStrings.IPValidation_NoIP);
                return;
            }

            var parts = value.Split('.');
            if (parts.Length != 4)
            {
                this.errors.AddError(propertyName, DiagStrings.IPValidation_NotCorrectLength);
                return;
            }

            foreach (var part in parts)
            {
                int intPart;
                if (!int.TryParse(part, out intPart))
                {
                    this.errors.AddError(propertyName, DiagStrings.IPValidation_OnlyNumbers);
                    return;
                }

                if (intPart < 0 || intPart > 255)
                {
                    this.errors.AddError(propertyName, DiagStrings.IPValidation_InvalidValues);
                    return;
                }
            }
        }
    }
}
