// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Gioom.Core;
    using Gorba.Common.Gioom.Core.Values;
    using Gorba.Common.Protocols.Udcp;
    using Gorba.Common.Protocols.Udcp.Datagram;
    using Gorba.Common.Protocols.Udcp.Fields;
    using Gorba.Common.SystemManagement.Client;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Motion.HardwareManager.Core.Settings;

    using NLog;

    // [WES] we use an #if here because like this we can reuse the entire code without having to write it twice
#if WindowsCE
    using OpenNETCF.Net.NetworkInformation;
#else
    using GatewayIPAddressInformation = System.Net.NetworkInformation.GatewayIPAddressInformation;
    using INetworkInterface = System.Net.NetworkInformation.NetworkInterface;
    using NetworkInterface = System.Net.NetworkInformation.NetworkInterface;
    using NetworkInterfaceType = System.Net.NetworkInformation.NetworkInterfaceType;
    using OperationalStatus = System.Net.NetworkInformation.OperationalStatus;
    using UnicastIPAddressInformation = System.Net.NetworkInformation.UnicastIPAddressInformation;

#endif

    /// <summary>
    /// Handler for the Unit Discovery and Configuration Protocol (UDCP).
    /// </summary>
    public class UdcpHandler
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UdcpHandler>();

        private readonly UdcpServer server;

        private readonly ITimer configurationResponseTimer;

        private readonly ITimer announceTimer;

        private readonly SimplePort announcePort;

        private UdcpResponse configurationResponse;
        private int configurationResponseCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpHandler"/> class.
        /// </summary>
        public UdcpHandler()
        {
            this.server = new UdcpServer();

            this.configurationResponseTimer = TimerFactory.Current.CreateTimer("ConfigurationResponse");
            this.configurationResponseTimer.AutoReset = false;
            this.configurationResponseTimer.Interval = TimeSpan.FromSeconds(1);
            this.configurationResponseTimer.Elapsed += this.ConfigurationResponseTimerOnElapsed;

            this.announceTimer = TimerFactory.Current.CreateTimer("Announce");
            this.announceTimer.AutoReset = false;
            this.announceTimer.Interval = TimeSpan.FromSeconds(10);
            this.announceTimer.Elapsed += (s, e) => this.announcePort.Value = FlagValues.False;

            this.announcePort = new SimplePort("UDCPAnnounce", true, false, new FlagValues(), FlagValues.False);
        }

        /// <summary>
        /// Starts this handler.
        /// </summary>
        public void Start()
        {
            Logger.Trace("Starting");
            this.server.RequestReceived += this.ServerOnRequestReceived;
            this.server.Start();

            GioomClient.Instance.RegisterPort(this.announcePort);
            Logger.Info("Started");
        }

        /// <summary>
        /// Stops this handler.
        /// </summary>
        public void Stop()
        {
            Logger.Trace("Stopping");
            this.server.RequestReceived -= this.ServerOnRequestReceived;
            this.announcePort.Value = FlagValues.False;
            GioomClient.Instance.DeregisterPort(this.announcePort);
            this.server.Stop();
            Logger.Info("Stopped");
        }

        private void SendResponse(UdcpResponse response)
        {
            Logger.Debug("Sending {0} response", response.Header.Type);
            this.server.SendDatagram(response);
        }

        private void SendInformation()
        {
            var response = new UdcpResponse(DatagramType.GetInformation, this.server.LocalAddress);
            response.Fields.Add(new UnitNameField(ApplicationHelper.MachineName));
            response.Fields.Add(new SoftwareVersionField(ApplicationHelper.GetApplicationFileVersion()));

            try
            {
                foreach (var netIf in this.GetNetworkInterfaces())
                {
                    var ipProps = netIf.GetIPProperties();
                    if (ipProps == null)
                    {
                        continue;
                    }

                    var ipv4Props = ipProps.GetIPv4Properties();
                    if (ipv4Props != null)
                    {
                        response.Fields.Add(new DhcpEnabledField(ipv4Props.IsDhcpEnabled));
                    }

                    var addresses = new List<UnicastIPAddressInformation>(ipProps.UnicastAddresses);
                    foreach (var addr in addresses.FindAll(a => a.Address.AddressFamily == AddressFamily.InterNetwork))
                    {
                        response.Fields.Add(new IpAddressField(addr.Address));
                        response.Fields.Add(new NetworkMaskField(addr.IPv4Mask));
                    }

                    foreach (GatewayIPAddressInformation addr in ipProps.GatewayAddresses)
                    {
                        if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            response.Fields.Add(new GatewayField(addr.Address));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't get IP address information");
            }

            this.SendResponse(response);
        }

        private void SetConfiguration(UdcpRequest request)
        {
            var response = new UdcpResponse(DatagramType.SetConfiguration, this.server.LocalAddress);
            var setting = new HardwareManagerSetting();

            var dhcp = request.GetField<DhcpEnabledField>();
            if (dhcp != null && dhcp.Value)
            {
                setting.UseDhcp = true;
            }
            else
            {
                var address = request.GetField<IpAddressField>();
                if (address == null)
                {
                    Logger.Warn("Got SetConfiguration without IP address field");
                    response.Fields.Add(new ErrorCodeField(ErrorCode.BadField));
                    response.Fields.Add(new ErrorFieldField(FieldType.IpAddress));
                    response.Fields.Add(new ErrorMessageField("IP address field missing"));
                    this.SendResponse(response);
                    return;
                }

                var mask = request.GetField<NetworkMaskField>();
                if (mask == null)
                {
                    Logger.Warn("Got SetConfiguration without network mask field");
                    response.Fields.Add(new ErrorCodeField(ErrorCode.BadField));
                    response.Fields.Add(new ErrorFieldField(FieldType.NetworkMask));
                    response.Fields.Add(new ErrorMessageField("Network mask field missing"));
                    this.SendResponse(response);
                    return;
                }

                setting.IpAddress = new XmlIpAddress(address.Value);
                setting.SubnetMask = new XmlIpAddress(mask.Value);
                var gateway = request.GetField<GatewayField>();
                if (gateway != null)
                {
                    setting.Gateway = new XmlIpAddress(gateway.Value);
                }
            }

            // TODO: implement setting IP configuration in .NET CF
#if WindowsCE
            response.Fields.Add(new ErrorCodeField(ErrorCode.CouldNotProcess));
            response.Fields.Add(new ErrorMessageField("SetConfiguration not yet supported in .NET CF"));
#else
            var settingsHandler = new IpSettingsHandler();
            if (!settingsHandler.ApplySetting(setting))
            {
                Logger.Warn("Couldn't change IP settings, responding with error");
                response.Fields.Add(new ErrorCodeField(ErrorCode.CouldNotProcess));
                response.Fields.Add(new ErrorMessageField("Given settings couldn't be applied"));
            }
#endif
            this.configurationResponse = response;
            this.configurationResponseCounter = 0;
            this.configurationResponseTimer.Enabled = true;
        }

        private void Announce()
        {
            this.announceTimer.Enabled = false;
            this.announcePort.Value = FlagValues.True;
            this.announceTimer.Enabled = true;

            var response = new UdcpResponse(DatagramType.Announce, this.server.LocalAddress);
            this.SendResponse(response);
        }

        private IEnumerable<INetworkInterface> GetNetworkInterfaces()
        {
            var all = NetworkInterface.GetAllNetworkInterfaces();
            var interfaces = new List<INetworkInterface>(all.Length);
            foreach (var netIf in all)
            {
                if (netIf.NetworkInterfaceType != NetworkInterfaceType.Loopback
                    && netIf.NetworkInterfaceType != NetworkInterfaceType.Tunnel
                    && netIf.NetworkInterfaceType != NetworkInterfaceType.Unknown
                    && netIf.OperationalStatus == OperationalStatus.Up)
                {
                    interfaces.Add(netIf);
                }
            }

            return interfaces;
        }

        private void ServerOnRequestReceived(object sender, UdcpDatagramEventArgs<UdcpRequest> e)
        {
            Logger.Debug("Received {0} request", e.Datagram.Header.Type);
            switch (e.Datagram.Header.Type)
            {
                case DatagramType.GetInformation:
                    this.SendInformation();
                    return;
                case DatagramType.SetConfiguration:
                    this.SetConfiguration(e.Datagram);
                    return;
                case DatagramType.Announce:
                    this.Announce();
                    return;
                case DatagramType.Reboot:
                    this.SendResponse(new UdcpResponse(DatagramType.Reboot, this.server.LocalAddress));
                    SystemManagerClient.Instance.Reboot("Requested by UDCP");
                    return;
            }
        }

        private void ConfigurationResponseTimerOnElapsed(object sender, EventArgs eventArgs)
        {
            var response = this.configurationResponse;
            if (response == null)
            {
                return;
            }

            try
            {
                this.SendResponse(response);
                this.SendInformation();
            }
            catch (Exception ex)
            {
                if (++this.configurationResponseCounter > 10)
                {
                    Logger.Warn(ex, "Couldn't send configuration response after 10 tries, aborting");
                    return;
                }

                Logger.Debug(ex, "Couldn't send configuration response, retrying");
                this.configurationResponseTimer.Enabled = true;
            }
        }
    }
}
