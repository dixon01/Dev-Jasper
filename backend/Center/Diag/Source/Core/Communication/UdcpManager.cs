// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.Communication
{
    using System;
    using System.Net;

    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Manager responsible for handing UDCP communication.
    /// </summary>
    internal class UdcpManager
    {
        private static readonly TimeSpan DefaultRefreshInterval = TimeSpan.FromSeconds(5);

        private readonly UdcpServer server;

        private readonly ITimer refreshTimer = TimerFactory.Current.CreateTimer("GioomRefresh");

        private bool running;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpManager"/> class.
        /// </summary>
        public UdcpManager()
        {
            this.server = new UdcpServer();
            this.server.ResponseReceived += this.ServerOnResponseReceived;

            this.refreshTimer.AutoReset = true;
            this.refreshTimer.Interval = DefaultRefreshInterval;
            this.refreshTimer.Elapsed += this.RefreshTimerOnElapsed;
        }

        /// <summary>
        /// Event that is risen whenever we receive a GetInformation response.
        /// </summary>
        public event EventHandler<UdcpDatagramEventArgs<UdcpResponse>> GetInformationResponseReceived;

        /// <summary>
        /// Gets or sets the refresh interval.
        /// Default value is 20 seconds.
        /// </summary>
        public TimeSpan RefreshInterval
        {
            get
            {
                return this.refreshTimer.Interval;
            }

            set
            {
                this.refreshTimer.Interval = value;
            }
        }

        /// <summary>
        /// Starts this manager.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;
            this.server.Start();

            this.SendGetInformationRequest();
        }

        /// <summary>
        /// Stops this manager.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;
            this.refreshTimer.Enabled = false;
            this.server.Stop();
        }

        /// <summary>
        /// Sends a broadcast request to get information from all available units.
        /// </summary>
        public void SendGetInformationRequest()
        {
            this.CheckRunning();

            this.refreshTimer.Enabled = false;
            this.DoSendGetInformationRequest();
            this.refreshTimer.Enabled = true;
        }

        /// <summary>
        /// Sends a request to change the IP settings of a unit.
        /// </summary>
        /// <param name="udcpAddress">
        /// The UDCP address of the unit for which the IP settings have to be changed.
        /// </param>
        /// <param name="useDhcp">
        /// A flag indicating whether to configure the network interface with DHCP (true) or manually (false).
        /// </param>
        /// <param name="ipAddress">
        /// The IP address (mandatory if not <paramref name="useDhcp"/>).
        /// </param>
        /// <param name="networkMask">
        /// The IP network mask (mandatory if not <paramref name="useDhcp"/>).
        /// </param>
        /// <param name="gateway">
        /// The gateway IP address (optional).
        /// </param>
        public void SendSetConfigurationRequest(
            UdcpAddress udcpAddress, bool useDhcp, IPAddress ipAddress, IPAddress networkMask, IPAddress gateway)
        {
            this.CheckRunning();

            var request = new UdcpRequest(DatagramType.SetConfiguration, udcpAddress);
            if (useDhcp)
            {
                request.Fields.Add(new DhcpEnabledField(true));
            }
            else
            {
                request.Fields.Add(new IpAddressField(ipAddress));
                request.Fields.Add(new NetworkMaskField(networkMask));
                if (gateway != null)
                {
                    request.Fields.Add(new GatewayField(gateway));
                }
            }

            this.server.SendDatagram(request);
        }

        /// <summary>
        /// Sends an announcement request to the given UDCP address.
        /// </summary>
        /// <param name="udcpAddress">
        /// The UDCP address of the unit to show an announcement.
        /// </param>
        public void SendAnnounceRequest(UdcpAddress udcpAddress)
        {
            this.CheckRunning();

            this.server.SendDatagram(new UdcpRequest(DatagramType.Announce, udcpAddress));
        }

        /// <summary>
        /// Sends a reboot request to the given UDCP address.
        /// </summary>
        /// <param name="udcpAddress">
        /// The UDCP address of the unit to reboot.
        /// </param>
        public void SendRebootRequest(UdcpAddress udcpAddress)
        {
            this.CheckRunning();

            this.server.SendDatagram(new UdcpRequest(DatagramType.Reboot, udcpAddress));
        }

        /// <summary>
        /// Raises the <see cref="GetInformationResponseReceived"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseGetInformationResponseReceived(UdcpDatagramEventArgs<UdcpResponse> e)
        {
            var handler = this.GetInformationResponseReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void DoSendGetInformationRequest()
        {
            this.server.SendDatagram(new UdcpRequest(DatagramType.GetInformation, UdcpAddress.BroadcastAddress));
        }

        private void CheckRunning()
        {
            if (!this.running)
            {
                throw new NotSupportedException("Can't send request when the manager was not started");
            }
        }

        private void ServerOnResponseReceived(object sender, UdcpDatagramEventArgs<UdcpResponse> e)
        {
            if (e.Datagram.Header.Type != DatagramType.GetInformation)
            {
                return;
            }

            // restart the timer whenever we get a response
            this.refreshTimer.Enabled = false;
            this.refreshTimer.Enabled = this.running;
            this.RaiseGetInformationResponseReceived(e);
        }

        private void RefreshTimerOnElapsed(object sender, EventArgs e)
        {
            this.DoSendGetInformationRequest();
        }
    }
}
