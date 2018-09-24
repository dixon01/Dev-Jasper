namespace Luminator.Multicast.Core
{
    using System;
    using System.Linq;
    using System.Management;
    using System.Net;
    using System.Net.NetworkInformation;

    using Gorba.Common.Utility.Core;

    using Microsoft.Win32;

    using NLog;

    public class NetworkManagement
    {
        #region Static Fields

        private static readonly Logger Logger = LogHelper.GetLogger<NetworkManagement>();

        #endregion

        #region Public Methods and Operators

        public static IPAddress GetGatewayForAdaptor(string adaptorDescriptionOrName)
        {
            var niAdpaters = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                foreach (var networkInterface in niAdpaters)
                {
                    var nameMatched = networkInterface.Name.Equals(adaptorDescriptionOrName);
                    var descMatched = networkInterface.Description.Equals(adaptorDescriptionOrName);
                    if (nameMatched || descMatched)
                    {
                        var gatewayIpAddressInformation = networkInterface.GetIPProperties().GatewayAddresses.FirstOrDefault();
                        if (gatewayIpAddressInformation != null)
                        {
                            return gatewayIpAddressInformation.Address;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + (e.InnerException?.Message ?? ""));
            }
            return IPAddress.None;
        }

        public static bool IsNetworkedStatusDown()
        {
            var niAdpaters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var networkInterface in niAdpaters)
            {
                return networkInterface.OperationalStatus == OperationalStatus.Down;
            }

            return false;
        }

        public static bool IsNetworkedWithStaticIp()
        {
            var niAdpaters = NetworkUtils.GetAllActiveLocalNics();
            if (
                niAdpaters.Select(
                    networkInterface => networkInterface.GetIPProperties().GetIPv4Properties() != null && networkInterface.GetIPProperties().GetIPv4Properties().IsDhcpEnabled)
                    .FirstOrDefault())
            {
                return false;
            }
            return true;
        }

        public static bool IsNetworkedWithStaticIp(string nameOrDescription)
        {
            var niAdpaters = NetworkInterface.GetAllNetworkInterfaces();
            try
            {
                foreach (var networkInterface in niAdpaters)
                {
                    var pv4InterfaceProperties = networkInterface.GetIPProperties().GetIPv4Properties();
                    var name = networkInterface.Name.Equals(nameOrDescription);
                    var desc = networkInterface.Description.Equals(nameOrDescription);
                    if (pv4InterfaceProperties != null && (name || desc))
                    {
                        return !pv4InterfaceProperties.IsDhcpEnabled;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public static void PrintNetworkStatistics()
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var n in adapters)
            {
                Console.WriteLine(
                    "Name = {0} Desc =  {1}, Speed = {2}, Supports MultiCast = {3}, Status = {4}, Id = {5}",
                    n.Name,
                    n.Description,
                    n.Speed,
                    n.SupportsMulticast,
                    n.OperationalStatus,
                    n.Id);
            }
        }


        public static void SetDhcp(string nicName)
        {
            Logger.Info("SetDhcp for adaptor " + nicName);
            try
            {
                var mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                var moc = mc.GetInstances();

                foreach (var o in moc)
                {
                    var managementObject = (ManagementObject)o;

                    // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                    if ((bool)managementObject["IPEnabled"])
                    {
                        //// workaround of windows bug (windows refused to apply static ip in network properties dialog)
                        var settingId = o.GetPropertyValue("SettingID"); // adapter = the management object
                        using (var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces\" + settingId, true))
                        {
                            if (regKey != null)
                            {
                                regKey.SetValue("EnableDHCP", 1);
                                regKey.Close();
                            }
                        }
                        Logger.Info("Desc Matched?" + managementObject["Description"].Equals(nicName));
                        Logger.Info("Name Matched?" + managementObject["Caption"].Equals(nicName));
                        if (managementObject["Description"].Equals(nicName) || managementObject["Caption"].Equals(nicName))
                        {
                            Logger.Info("Nic Name matched => Setting Network Interface to DHCP");
                            var newDns = managementObject.GetMethodParameters("SetDNSServerSearchOrder");
                            newDns["DNSServerSearchOrder"] = null;
                            var resultOfEnableDhcp = managementObject.InvokeMethod("EnableDHCP", null, null);
                            var resultOfSetDnsServerSearchOrder = managementObject.InvokeMethod("SetDNSServerSearchOrder", newDns, null);

                            if (resultOfEnableDhcp != null)
                            {
                                var u = (uint)resultOfEnableDhcp["returnValue"];
                                Logger.Info("EnableDHCP Result - " + u);
                            }

                            if (resultOfSetDnsServerSearchOrder != null)
                            {
                                var u = (uint)resultOfSetDnsServerSearchOrder["returnValue"];
                                Logger.Info("SetDnsServerSearchOrder Result - " + u);
                            }
                        }
                    }
                    else
                    {
                        Logger.Info("IP Enabled was false - not doing SET DHCP: " + (bool)managementObject["IPEnabled"]);
                    }
                }
            }
            catch (Exception exception)
            {
                Logger.Info(exception.Message);
            }
            finally
            {
                Logger.Info("End of SetDhcp => " + nicName);
            }
        }

        /// <summary>
        ///     Set's a new Gateway address of the local machine
        /// </summary>
        /// <param name="gateway">The Gateway IP Address</param>
        /// <param name="nicNameOrDesc"></param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public static uint SetSystemGateway(string gateway, string nicNameOrDesc)
        {
            Logger.Info("SetSystemGateway to " + gateway + " for Nic Adaptor" + nicNameOrDesc);
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if (managementObject["Description"].Equals(nicNameOrDesc) || managementObject["Caption"].Equals(nicNameOrDesc))
                {
                    if ((bool)managementObject["IPEnabled"])
                    {
                        try
                        {
                            var newGateway = managementObject.GetMethodParameters("SetGateways");

                            newGateway["DefaultIPGateway"] = new[] { gateway };
                            newGateway["GatewayCostMetric"] = new[] { 1 };

                            var returnValueFromMo = managementObject.InvokeMethod("SetGateways", newGateway, null);

                            if (returnValueFromMo != null)
                            {
                                var u = (uint)returnValueFromMo["returnValue"];
                                Logger.Info("SetGateway Result - " + u);
                                return u;
                            }
                        }
                        catch (Exception exception)
                        {
                            Logger.Info(exception.Message);
                            throw;
                        }
                    }
                }
            }
            return 0;
        }

        /// <summary>
        ///     Set's a new Gateway address of the local machine
        /// </summary>
        /// <param name="gateway">The Gateway IP Address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public static void SetSystemGateway(string gateway)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if ((bool)managementObject["IPEnabled"])
                {
                    try
                    {
                        var newGateway = managementObject.GetMethodParameters("SetGateways");

                        newGateway["DefaultIPGateway"] = new[] { gateway };
                        newGateway["GatewayCostMetric"] = new[] { 1 };

                        var setGatewayResult = managementObject.InvokeMethod("SetGateways", newGateway, null);
                        if (setGatewayResult != null)
                        {
                            Logger.Info("Set System Gateway Result Was - " + (uint)setGatewayResult["returnValue"]);
                        }

                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception.Message);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///     Set's a new IP Address and it's Submask of the local machine
        /// </summary>
        /// <param name="ipAddress">The IP Address</param>
        /// <param name="subnetMask">The Submask IP Address</param>
        /// <param name="adapterDescription"></param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public static void SetSystemIp(string ipAddress, string subnetMask, string adapterDescription)
        {
            // var NetworkConnectionNames = NetworkInterface.GetAllNetworkInterfaces().Select(ni => ni.Name);
            //IEnumerable<NetworkInterface> NetworkConnections = NetworkInterface.GetAllNetworkInterfaces();
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;

                #region Windows Potential Issue - Might have been resolved

                //try
                //{
                //    // workaround of windows bug (windows refused to apply static ip in network properties dialog)
                //    var settingId = o.GetPropertyValue("SettingID"); // adapter = the management object
                //    using (var regKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\services\Tcpip\Parameters\Interfaces\" + settingId, true))
                //    {
                //        if (regKey != null)
                //        {
                //            regKey.SetValue("EnableDHCP", 0);
                //            regKey.Close();
                //        }
                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}

                #endregion

                if ((bool)managementObject["IPEnabled"])
                {
                    if (!string.IsNullOrEmpty(adapterDescription))
                    {
                        // Console.WriteLine(managementObject["Description"] + " =X? " + adapterDescription);
                        if (managementObject["Description"].Equals(adapterDescription))
                        {
                            try
                            {
                                var newIp = managementObject.GetMethodParameters("EnableStatic");

                                newIp["IPAddress"] = new[] { ipAddress };
                                newIp["SubnetMask"] = new[] { subnetMask };

                                var setIp = managementObject.InvokeMethod("EnableStatic", newIp, null);
                                if (setIp != null)
                                {
                                    Logger.Info("Set System Ip Result Was - " + (uint)setIp["returnValue"]);
                                }
                            }
                            catch (Exception exception)
                            {
                                Logger.Error(System.Reflection.MethodBase.GetCurrentMethod().Name + exception.Message + "\nInner Exception" + exception.InnerException?.Message);
                                throw;
                            }
                            finally
                            {
                                Logger.Info("SetSystemIp called with => " + ipAddress + " : " + subnetMask);
                            }
                        }
                        else
                        {
                            Logger.Info("SetSystemIp Adaptor description did not match ");
                        }
                    }
                    else
                    {
                        Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().Name + " Adaptor description was empty");
                    }
                }
            }
        }

        /// <summary>
        ///     Set's the DNS Server of the local machine
        /// </summary>
        /// <param name="nic">NIC address</param>
        /// <param name="dns">DNS server address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void SetSystemDns(string nic, string dns)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var mcInstance = managementClass.GetInstances();

            foreach (var o in mcInstance)
            {
                var managementObject = (ManagementObject)o;
                if ((bool)managementObject["IPEnabled"])
                {
                    // if you are using the System.Net.NetworkInformation.NetworkInterface you'll need to change this line to if (objMO["Caption"].ToString().Contains(NIC)) and pass in the Description property instead of the name 
                    if (managementObject["Caption"].Equals(nic))
                    {
                        try
                        {
                            var newDns = managementObject.GetMethodParameters("SetDNSServerSearchOrder");
                            newDns["DNSServerSearchOrder"] = dns.Split(',');
                            var setDns = managementObject.InvokeMethod("SetDNSServerSearchOrder", newDns, null);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            throw;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Set's WINS of the local machine
        /// </summary>
        /// <param name="nic">NIC Address</param>
        /// <param name="priWins">Primary WINS server address</param>
        /// <param name="secWins">Secondary WINS server address</param>
        /// <remarks>Requires a reference to the System.Management namespace</remarks>
        public void SetSystemWins(string nic, string priWins, string secWins)
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();

            foreach (var o in managementObjectCollection)
            {
                var managementObject = (ManagementObject)o;
                if ((bool)managementObject["IPEnabled"])
                {
                    if (managementObject["Caption"].Equals(nic))
                    {
                        try
                        {
                            var wins = managementObject.GetMethodParameters("SetWINSServer");
                            wins.SetPropertyValue("WINSPrimaryServer", priWins);
                            wins.SetPropertyValue("WINSSecondaryServer", secWins);

                            var managementBaseObject = managementObject.InvokeMethod("SetWINSServer", wins, null);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception.Message);
                            throw;
                        }
                    }
                }
            }
        }

        #endregion
    }
}