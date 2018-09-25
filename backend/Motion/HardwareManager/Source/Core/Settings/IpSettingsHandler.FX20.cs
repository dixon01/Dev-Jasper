// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IpSettingsHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IpSettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Net.Sockets;

    using Gorba.Common.Configuration.HardwareManager;

    using NLog;

    /// <summary>
    /// Handler that sets the IP address and network mask according to the configuration.
    /// </summary>
    public partial class IpSettingsHandler : ISettingsPartHandler
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Apply the given settings.
        /// </summary>
        /// <param name="setting">
        /// The setting object.
        /// </param>
        /// <returns>
        /// True if the system drive should be committed and the system rebooted.
        /// </returns>
        public bool ApplySetting(HardwareManagerSetting setting)
        {
            return this.ApplySetting(
                setting.UseDhcp,
                setting.IpAddress,
                setting.SubnetMask,
                setting.Gateway,
                setting.DnsServers.ConvertAll(d => d.Value));
        }

        private bool ApplySetting(
            bool useDhcp, IPAddress address, IPAddress subnetMask, IPAddress gateway, IList<IPAddress> dnsServers)
        {
            if (useDhcp)
            {
                Logger.Debug("Setting IP configuration to DHCP");
            }
            else
            {
                Logger.Debug(
                    "Setting IP address to {0}, subnet mask to {1} and gateway to {2}",
                    address,
                    subnetMask,
                    gateway);
            }

            var dnsSearchOrder = new string[dnsServers.Count];
            for (int i = 0; i < dnsSearchOrder.Length; i++)
            {
                dnsSearchOrder[i] = dnsServers[i].ToString();
            }

            Logger.Debug("Using DNS servers: {0}", string.Join(", ", dnsSearchOrder));

            if (!useDhcp && (address == null || subnetMask == null))
            {
                Logger.Info("DHCP, IP address or network mask not configured, not changing anything");
                return false;
            }

            if (this.HasCorrectConfiguration(useDhcp, address, subnetMask, gateway, dnsServers))
            {
                return false;
            }

            if (!useDhcp)
            {
                Logger.Info("DHCP not being used, setting static IP configuration to address: {0}, subnetMask: {1}, gateway: {2}.", address, subnetMask, gateway);
                var  result =  this.SetStaticConfiguration(address, subnetMask, gateway, dnsSearchOrder);
                if (result)
                {
                    Logger.Info("Static IP configuration was successfully set.");
                }

                return false;  // Returning 'false' prevents the system from entering a continuous reboot cycle due to the 'C' drive being read-only.
            }

            if (address != null)
            {
                Logger.Warn("IP settings are set to use DHCP, but IP adress is set to {0}", address);
            }

            if (subnetMask != null)
            {
                Logger.Warn("IP settings are set to use DHCP, but subnet mask is set to {0}", subnetMask);
            }

            if (gateway != null)
            {
                Logger.Warn("IP settings are set to use DHCP, but gateway is set to {0}", gateway);
            }

            return this.SetDhcp(dnsSearchOrder);
        }

        private bool HasCorrectConfiguration(
            bool useDhcp, IPAddress address, IPAddress subnetMask, IPAddress gateway, IList<IPAddress> dnsServers)
        {
            var setGateway = gateway != null;
            if (!setGateway)
            {
                Logger.Debug("Not setting gateway since it is not configured");
            }

            try
            {
                var netIfs = NetworkInterface.GetAllNetworkInterfaces();
                foreach (var netIf in netIfs)
                {
                    if (netIf.NetworkInterfaceType == NetworkInterfaceType.Loopback)
                    {
                        continue;
                    }

                    var props = netIf.GetIPProperties();
                    if (props == null)
                    {
                        continue;
                    }

                    if (!this.HasCorrectDnsConfig(props, dnsServers))
                    {
                        if (!useDhcp || dnsServers.Count != 0)
                        {
                            // this is only wrong if we are not using DHCP or DNS servers were configured
                            // otherwise we just get the list of DNS servers that were received from DHCP
                            continue;
                        }
                    }

                    if (useDhcp)
                    {
                        if (this.HasValidDhcpConfig(netIf))
                        {
                            return true;
                        }

                        continue;
                    }

                    if (!this.HasValidAddress(netIf, address, subnetMask))
                    {
                        continue;
                    }

                    if (!setGateway)
                    {
                        Logger.Info(
                            "Network interface '{0}' has the right IP config: {1}/{2}, not changing anything",
                            netIf.Name,
                            address,
                            subnetMask);
                        return true;
                    }

                    foreach (var gatewayAddress in props.GatewayAddresses)
                    {
                        if (gatewayAddress.Address != null && gatewayAddress.Address.Equals(gateway))
                        {
                            Logger.Info(
                                "Network interface '{0}' has the right IP config: {1}/{2} ({3}), not changing anything",
                                netIf.Name,
                                address,
                                subnetMask,
                                gateway);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't verify existing network interfaces");
            }

            return false;
        }

        private bool HasCorrectDnsConfig(IPInterfaceProperties props, ICollection<IPAddress> dnsServers)
        {
            if (props.DnsAddresses.Count != dnsServers.Count)
            {
                return false;
            }

            var missing = new List<IPAddress>(dnsServers);
            foreach (var address in props.DnsAddresses)
            {
                if (!missing.Remove(address))
                {
                    return false;
                }
            }

            return missing.Count == 0;
        }

        private bool HasValidDhcpConfig(NetworkInterface netIf)
        {
            var props = netIf.GetIPProperties();
            var ipv4 = props.GetIPv4Properties();
            if (ipv4 == null)
            {
                return false;
            }

            if (ipv4.IsDhcpEnabled)
            {
                Logger.Info(
                    "Network interface '{0}' has the right IP config: DHCP, not changing anything",
                    netIf.Name);
                return true;
            }

            return false;
        }

        private bool HasValidAddress(NetworkInterface netIf, IPAddress address, IPAddress subnetMask)
        {
            var addresses = new List<UnicastIPAddressInformation>(netIf.GetIPProperties().UnicastAddresses);
            foreach (var addressInfo in addresses.FindAll(a => a.Address.AddressFamily == AddressFamily.InterNetwork))
            {
                if (addressInfo.Address != null && addressInfo.Address.Equals(address)
                    && addressInfo.IPv4Mask != null && addressInfo.IPv4Mask.Equals(subnetMask))
                {
                    return true;
                }
            }

            return false;
        }

        private bool SetDhcp(string[] dnsSearchOrder)
        {
            var success = false;
            try
            {
                var management = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var objectCollection = management.GetInstances();
                foreach (ManagementObject managementObject in objectCollection)
                {
                    if (!((bool)managementObject["IPEnabled"]))
                    {
                        continue;
                    }

                    try
                    {
                        var parameters = managementObject.GetMethodParameters("EnableDHCP");
                        this.InvokeMethod(managementObject, "EnableDHCP", parameters);
                        Logger.Info("Set '{0}' to DHCP", managementObject["Caption"]);

                        this.SetDnsServerSearchOrder(managementObject, dnsSearchOrder);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Could not set " + managementObject["Caption"] + " to DHCP");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not set to DHCP");
            }

            return success;
        }

        private bool SetStaticConfiguration(
            IPAddress address, IPAddress subnetMask, IPAddress gateway, string[] dnsSearchOrder)
        {
            var success = false;
            try
            {
                var management = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var objectCollection = management.GetInstances();
                foreach (ManagementObject managementObject in objectCollection)
                {
                    if (!((bool)managementObject["IPEnabled"]))
                    {
                        continue;
                    }

                    try
                    {
                        var parameters = managementObject.GetMethodParameters("EnableStatic");

                        parameters["IPAddress"] = new[] { address.ToString() };
                        parameters["SubnetMask"] = new[] { subnetMask.ToString() };

                        this.InvokeMethod(managementObject, "EnableStatic", parameters);
                        Logger.Info("IP address of '{0}' set to: {1}", managementObject["Caption"], address);
                        Logger.Info("Subnet mask of '{0}' set to: {1}", managementObject["Caption"], subnetMask);

                        this.SetDnsServerSearchOrder(managementObject, dnsSearchOrder);
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Could not set IP address of " + managementObject["Caption"]);
                    }

                    if (gateway == null)
                    {
                        continue;
                    }

                    try
                    {
                        var parameters = managementObject.GetMethodParameters("SetGateways");

                        parameters["DefaultIPGateway"] = new[] { gateway.ToString() }; // string array
                        parameters["GatewayCostMetric"] = new[] { (ushort)1 }; // uint16 array

                        this.InvokeMethod(managementObject, "SetGateways", parameters);
                        success = true;
                        Logger.Info("Gateway address of '{0}' set to: {1}", managementObject["Caption"], gateway);
                    }
                    catch (Exception ex)
                    {
                        Logger.Warn(ex, "Could not set gateway address of " + managementObject["Caption"]);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Could not set IP address");
            }

            return success;
        }

        private void SetDnsServerSearchOrder(ManagementObject managementObject, string[] dnsSearchOrder)
        {
            var parameters = managementObject.GetMethodParameters("SetDNSServerSearchOrder");
            parameters["DNSServerSearchOrder"] = dnsSearchOrder;
            this.InvokeMethod(managementObject, "SetDNSServerSearchOrder", parameters);
            Logger.Info(
                "Set '{0}' to use DNS servers: {1}",
                managementObject["Caption"],
                string.Join(", ", dnsSearchOrder));
        }

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Long switch statement that can't be shortened.")]
        private void InvokeMethod(ManagementObject managementObject, string methodName, ManagementBaseObject parameters)
        {
            var result = managementObject.InvokeMethod(methodName, parameters, null);
            if (result == null)
            {
                throw new ManagementException("Couldn't read result value");
            }

            // return values according to http://msdn.microsoft.com/en-us/library/aa390383(v=vs.85).aspx
            var returnValue = Convert.ToUInt32(result["ReturnValue"]);
            switch (returnValue)
            {
                case 0: // Successful completion, no reboot required
                case 1: // Successful completion, reboot required
                    return;
                case 64:
                    throw new ManagementException("Method not supported on this platform");
                case 65:
                    throw new ManagementException("Unknown failure");
                case 66:
                    throw new ManagementException("Invalid subnet mask");
                case 67:
                    throw new ManagementException("An error occurred while processing an instance that was returned");
                case 68:
                    throw new ManagementException("Invalid input parameter");
                case 69:
                    throw new ManagementException("More than five gateways specified");
                case 70:
                    throw new ManagementException("Invalid IP address");
                case 71:
                    throw new ManagementException("Invalid gateway IP address");
                case 72:
                    throw new ManagementException(
                        "An error occurred while accessing the registry for the requested information");
                case 73:
                    throw new ManagementException("Invalid domain name");
                case 74:
                    throw new ManagementException("Invalid host name");
                case 75:
                    throw new ManagementException("No primary or secondary WINS server defined");
                case 76:
                    throw new ManagementException("Invalid file");
                case 77:
                    throw new ManagementException("Invalid system path");
                case 78:
                    throw new ManagementException("File copy failed");
                case 79:
                    throw new ManagementException("Invalid security parameter");
                case 80:
                    throw new ManagementException("Unable to configure TCP/IP service");
                case 81:
                    throw new ManagementException("Unable to configure DHCP service");
                case 82:
                    throw new ManagementException("Unable to renew DHCP lease");
                case 83:
                    throw new ManagementException("Unable to release DHCP lease");
                case 84:
                    throw new ManagementException("IP not enabled on adapter");
                case 85:
                    throw new ManagementException("IPX not enabled on adapter");
                case 86:
                    throw new ManagementException("Frame or network number bounds error");
                case 87:
                    throw new ManagementException("Invalid frame type");
                case 88:
                    throw new ManagementException("Invalid network number");
                case 89:
                    throw new ManagementException("Duplicate network number");
                case 90:
                    throw new ManagementException("Parameter out of bounds");
                case 91:
                    throw new ManagementException("Access denied");
                case 92:
                    throw new ManagementException("Out of memory");
                case 93:
                    throw new ManagementException("Already exists");
                case 94:
                    throw new ManagementException("Path, file, or object not found");
                case 95:
                    throw new ManagementException("Unable to notify service");
                case 96:
                    throw new ManagementException("Unable to notify DNS service");
                case 97:
                    throw new ManagementException("Interface not configurable");
                case 98:
                    throw new ManagementException("Not all DHCP leases could be released or renewed");
                case 100:
                    throw new ManagementException("DHCP not enabled on adapter");
                default:
                    throw new ManagementException("Unknown error " + returnValue);
            }
        }
    }
}