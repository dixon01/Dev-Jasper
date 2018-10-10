// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MotionUnitViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MotionUnitViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    using System;
    using System.Net;

    using Gorba.Common.Protocols.Udcp;

    /// <summary>
    /// The unit view model for imotion units (TFTs, ...).
    /// </summary>
    public class MotionUnitViewModel : UnitViewModelBase
    {
        private bool hasUdcpInformation;

        private bool dhcpEnabled;

        private IPAddress ipAddress;

        private UdcpAddress udcpAddress;

        private IPAddress gatewayAddress;

        private IPAddress networkMask;

        private string softwareVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="MotionUnitViewModel"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        public MotionUnitViewModel(IDiagShell shell)
            : base(shell)
        {
        }

        /// <summary>
        /// Gets the name to display when showing this unit.
        /// Depending on the type of unit, this can differ from <see cref="UnitViewModelBase.Name"/>.
        /// </summary>
        public override string DisplayName
        {
            get
            {
                if (string.IsNullOrEmpty(base.DisplayName) && this.IpAddress != null)
                {
                    return this.IpAddress.ToString();
                }

                return base.DisplayName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this unit has UDCP information.
        /// </summary>
        public bool HasUdcpInformation
        {
            get
            {
                return this.hasUdcpInformation;
            }

            private set
            {
                this.SetProperty(ref this.hasUdcpInformation, value, () => this.HasUdcpInformation);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether DHCP is enabled.
        /// </summary>
        public bool DhcpEnabled
        {
            get
            {
                return this.dhcpEnabled;
            }

            set
            {
                this.SetProperty(ref this.dhcpEnabled, value, () => this.DhcpEnabled);
            }
        }

        /// <summary>
        /// Gets or sets the IP address of the unit.
        /// </summary>
        public IPAddress IpAddress
        {
            get
            {
                return this.ipAddress;
            }

            set
            {
                if (this.SetProperty(ref this.ipAddress, value, () => this.IpAddress))
                {
                    this.RaisePropertyChanged(() => this.DisplayName);
                    this.RaisePropertyChanged(() => this.IpAddressString);
                }
            }
        }

        /// <summary>
        /// Gets the IP address of the unit as string.
        /// </summary>
        public string IpAddressString
        {
            get
            {
                return Convert.ToString(this.ipAddress);
            }
        }

        /// <summary>
        /// Gets or sets the IP network mask of the unit.
        /// </summary>
        public IPAddress NetworkMask
        {
            get
            {
                return this.networkMask;
            }

            set
            {
                if (this.SetProperty(ref this.networkMask, value, () => this.NetworkMask))
                {
                    this.RaisePropertyChanged(() => this.NetworkMaskString);
                }
            }
        }

        /// <summary>
        /// Gets the IP network mask as string.
        /// </summary>
        public string NetworkMaskString
        {
            get
            {
                return Convert.ToString(this.networkMask);
            }
        }

        /// <summary>
        /// Gets or sets the IP gateway address of the unit.
        /// </summary>
        public IPAddress GatewayAddress
        {
            get
            {
                return this.gatewayAddress;
            }

            set
            {
                if (this.SetProperty(ref this.gatewayAddress, value, () => this.GatewayAddress))
                {
                    this.RaisePropertyChanged(() => this.GatewayAddressString);
                }
            }
        }

        /// <summary>
        /// Gets the IP gateway address of the unit as string.
        /// </summary>
        public string GatewayAddressString
        {
            get
            {
                return Convert.ToString(this.gatewayAddress);
            }
        }

        /// <summary>
        /// Gets or sets the UDCP address of the unit.
        /// </summary>
        public UdcpAddress UdcpAddress
        {
            get
            {
                return this.udcpAddress;
            }

            set
            {
                if (this.SetProperty(ref this.udcpAddress, value, () => this.UdcpAddress))
                {
                    this.HasUdcpInformation = this.udcpAddress != null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the byte[] data of the UDCP address of the unit.
        /// </summary>
        public byte[] UdcpAddressData
        {
            get
            {
                return this.UdcpAddress == null ? null : this.UdcpAddress.GetAddressBytes();
            }

            set
            {
                this.UdcpAddress = value == null ? null : new UdcpAddress(value);
            }
        }

        /// <summary>
        /// Gets or sets the software version.
        /// </summary>
        public string SoftwareVersion
        {
            get
            {
                return this.softwareVersion;
            }

            set
            {
                this.SetProperty(ref this.softwareVersion, value, () => this.SoftwareVersion);
            }
        }
    }
}