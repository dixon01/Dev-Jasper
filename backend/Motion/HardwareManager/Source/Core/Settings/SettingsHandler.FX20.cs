// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;
    using System.Diagnostics;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.SystemManagement.Client;

    /// <summary>
    /// Class responsible for choosing the right <see cref="HardwareManagerSetting"/>
    /// and applying them to the system.
    /// </summary>
    public partial class SettingsHandler
    {
        private const string EwfManagerPath = "ewfmgr.exe";
        private const string CommitArguments = "c: -commit";

        partial void Initialize()
        {
            this.settingParts.Add(new IpSettingsHandler());
            this.settingParts.Add(new HostnameSettingsHandler());
            this.settingParts.Add(new TimeZoneSettingsHandler());
            this.settingParts.Add(new DisplaySettingsHandler());
            this.settingParts.Add(new DaylightSavingHandler());
            this.settingParts.Add(new FixedSettingsHandler());
        }

        partial void CommitChanges()
        {
            Logger.Info("Committing changes and restarting now");

            try
            {
                var process = Process.Start(EwfManagerPath, CommitArguments);
                if (process == null)
                {
                    Logger.Warn("Couldn't start {0}", EwfManagerPath);
                    return;
                }

                if (!process.WaitForExit(20 * 1000))
                {
                    Logger.Warn("{0} didn't exit as expected, restarting anyway");
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't commit settings changes, restarting anyway");
            }

            SystemManagerClient.Instance.Reboot("Updated system settings");
        }
    }
}
