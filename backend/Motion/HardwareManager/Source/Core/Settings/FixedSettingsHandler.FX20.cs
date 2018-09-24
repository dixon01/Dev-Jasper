// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FixedSettingsHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FixedSettingsHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using Gorba.Common.Configuration.HardwareManager;

    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// Settings handler that is responsible for fixed values that are missing in certain system images.
    /// </summary>
    public partial class FixedSettingsHandler : ISettingsPartHandler
    {
        private const string TightVncServerKey = @"SOFTWARE\TightVNC\Server\";
        private const string AllowLoopbackName = "AllowLoopback";

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
            return this.UpdateVncServerRegistry();
        }

        private bool UpdateVncServerRegistry()
        {
            using (var rootKey = Registry.LocalMachine.OpenSubKey(TightVncServerKey, true))
            {
                if (rootKey == null)
                {
                    Logger.Warn(@"Couldn't find HKLM\{0}", TightVncServerKey);
                    return false;
                }

                if (1.Equals(rootKey.GetValue(AllowLoopbackName)))
                {
                    Logger.Debug(@"HKLM\{0} is already set correctly", TightVncServerKey);
                    return false;
                }

                rootKey.SetValue(AllowLoopbackName, 1, RegistryValueKind.DWord);
                Logger.Info(@"Updated HKLM\{0}", TightVncServerKey);
                return true;
            }
        }
    }
}
