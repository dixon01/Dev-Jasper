// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostnameSettingsHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HostnameSettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;
    using System.Management;
    using System.Net.NetworkInformation;
    using System.Threading;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.SystemManagement.Host.Path;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Handler that sets the host name according to the serial number or MAC address (for legacy hardware).
    /// </summary>
    public partial class HostnameSettingsHandler : ISettingsPartHandler
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
            if (setting.HostnameSource == HostnameSource.None)
            {
                Logger.Info("Setting of the host name is disabled");
                return false;
            }
         
            Logger.Debug("Verify host name with {0}", setting.HostnameSource);
            var expectedHostname = string.Format("TFT-{0}", this.GetExpectedHostId(setting.HostnameSource));
            if (Environment.MachineName.Equals(expectedHostname, StringComparison.InvariantCultureIgnoreCase))
            {
                Logger.Info(
                    "Expected hostname ({0}) matches current system name ({1})",
                    expectedHostname,
                    Environment.MachineName);
                return false;
            }

            // LTG
            // double check if we have renamed the machine already, skip if so.
            bool setHostName = true;     
            
            // Are we using mac address to name machine?
            if (setting.HostnameSource == HostnameSource.MacAddress)
            {
                // if the current machine name starts with TFT then ignore changing
                if (Environment.MachineName.StartsWith("TFT", StringComparison.InvariantCultureIgnoreCase) == true)
                {
                    setHostName = false; // exit do not rename machine
                }
            }

            if (setHostName)
            {
                Logger.Info("Setting hostname to '{0}'", expectedHostname);
                if (this.SetHostname(expectedHostname))
                {
                    Logger.Info("Hostname set successfully: will be '{0}', currently is '{1}'", expectedHostname, Environment.MachineName);
                    return true;
                }
            }
            else
            {
                Logger.Warn("Skipped setting Host Name to {0}, operation to ignored and already completed!", expectedHostname);
                return false;
            }

            Logger.Warn("Couldn't set hostname: should be '{0}', is '{1}'", expectedHostname, Environment.MachineName);
            return false;
        }

        private bool SetHostname(string hostname)
        {
            // Invoke WMI to populate the machine name
            var query = string.Format("Win32_ComputerSystem.Name='{0}'", Environment.MachineName);
            using (var wmiObject = new ManagementObject(new ManagementPath(query)))
            {
                var inputArgs = wmiObject.GetMethodParameters("Rename");
                inputArgs["Name"] = hostname;

                // Set the name
                var outParams = wmiObject.InvokeMethod("Rename", inputArgs, null);
                
                if (outParams == null)
                {
                    return false;
                }

                var ret = (uint)outParams.Properties["ReturnValue"].Value;
                Logger.Debug("Win32_ComputerSystem.Rename() returned {0}", ret);
                return ret == 0;
            }
        }

        private string GetExpectedHostId(HostnameSource source)
        {
            switch (source)
            {
                case HostnameSource.MacAddress:
                    return this.GetMacAddress();
                case HostnameSource.SerialNumber:
                    return this.GetSerialNumber();
                default:
                    throw new ArgumentException("source", "Unknown source: " + source);
            }
        }

        private string GetSerialNumber()
        {
            IHardwareHandler hardwareHandler = null;
            try
            {
                hardwareHandler = ServiceLocator.Current.GetInstance<IHardwareHandler>();
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Couldn't get hardware handler");
            }

            if (hardwareHandler == null)
            {
                // we fall back to the MAC address if there is no hardware handler (i.e. no MGI hardware)
                return this.GetMacAddress();
            }

            for (int i = 0; i < 60; i++)
            {
                if (!string.IsNullOrEmpty(hardwareHandler.SerialNumber))
                {
                    return hardwareHandler.SerialNumber;
                }

                Logger.Trace("Serial number is not yet available, waiting 1 second");
                Thread.Sleep(1000);
            }

            throw new NotSupportedException("Couldn't determine hostname from serial number");
        }

        private string GetMacAddress()
        {
            var netIfs = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var netIf in netIfs)
            {
                if (netIf.NetworkInterfaceType == NetworkInterfaceType.Loopback
                    || netIf.IsReceiveOnly)
                {
                    continue;
                }

                var addressBytes = netIf.GetPhysicalAddress().GetAddressBytes();
                if (addressBytes.Length != 6)
                {
                    continue;
                }

                Logger.Info("Using Network Information Name={0}", netIf.Name);
                return string.Format("{0:X2}-{1:X2}-{2:X2}", addressBytes[3], addressBytes[4], addressBytes[5]);
            }

            throw new NotSupportedException("Couldn't determine hostname from MAC address");
        }
    }
}