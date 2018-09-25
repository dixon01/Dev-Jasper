// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DaylightSavingHandler.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DaylightSavingHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.HardwareManager.Core.Settings
{
    using System;

    using Gorba.Common.Configuration.HardwareManager;
    using Gorba.Common.Configuration.Persistence;
    using Gorba.Common.Utility.Core;

    using Microsoft.Practices.ServiceLocation;
    using Microsoft.Win32;

    using NLog;

    /// <summary>
    /// A special settings part handler that actually doesn't care
    /// about settings, but only looks at the current state of daylight saving.
    /// This is needed to ensure that we commit the system every time daylight saving changes.
    /// This is a different approach to solve the incompatibility between EWF and DST:
    /// <see cref="https://msdn.microsoft.com/en-us/library/jj980285(v=winembedded.81).aspx"/>
    /// </summary>
    public partial class DaylightSavingHandler : ISettingsPartHandler
    {
        private const string TimeZoneInfoKey = @"SYSTEM\CurrentControlSet\Control\TimeZoneInformation";

        private static readonly Logger Logger = LogHelper.GetLogger<DaylightSavingHandler>();

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
            bool isDaylightSavingTime;
            try
            {
                isDaylightSavingTime = IsDaylightSavingTime();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't compute current DST state");
                return false;
            }

            var shouldCommit = false;
            var persistenceService = ServiceLocator.Current.GetInstance<IPersistenceService>();
            var context = persistenceService.GetContext<DaylightSavingState>();
            if (context.Valid && context.Value != null)
            {
                shouldCommit = context.Value.WasDaylightSavingTime != isDaylightSavingTime;
                Logger.Info(
                    "Previous daylight saving state: {0}, new value: {1}, commit & reboot: {2}",
                    context.Value.WasDaylightSavingTime,
                    isDaylightSavingTime,
                    shouldCommit);
            }
            else
            {
                Logger.Info("No previous daylight saving state found, saving current value: {0}", isDaylightSavingTime);
            }

            context.Validity = TimeSpan.FromDays(3650);
            context.Value = new DaylightSavingState { WasDaylightSavingTime = isDaylightSavingTime };
            return shouldCommit;
        }

        private static bool IsDaylightSavingTime()
        {
            // based on information from:
            // http://www.digital-detective.net/manual-identification-of-suspect-computer-time-zone-2/
            using (var timeZoneInfo = Registry.LocalMachine.OpenSubKey(TimeZoneInfoKey))
            {
                if (timeZoneInfo == null)
                {
                    return false;
                }

                // This value is the current time difference from UTC in minutes,
                // regardless of whether daylight saving is in effect or not.
                var activeTimeBias = (int)timeZoneInfo.GetValue("ActiveTimeBias", 0);

                // This value is the normal Time difference from UTC in minutes. This value is the
                // number of minutes that would need to be added to a local time to return it to a UTC value.
                var bias = (int)timeZoneInfo.GetValue("Bias", 0);

                // This value is added to the value of the Bias member to form the bias used during standard time.
                // In most time zones the value of this member is zero.
                var standardBias = (int)timeZoneInfo.GetValue("StandardBias", 0);

                // This value specifies a bias value to be used during local time translations
                // that occur during daylight time.  This value is added to the value of the Bias member to
                // form the bias used during daylight time.  In most time zones the value of this member is –60.
                var daylightBias = (int)timeZoneInfo.GetValue("DaylightBias", 0);
                if (daylightBias == 0)
                {
                    // there is no difference between DST and standard time, so we are never in DST
                    return false;
                }

                return activeTimeBias != bias + standardBias;
            }
        }

        /// <summary>
        /// The current state of daylight saving to be stored in persistence.xml.
        /// </summary>
        [Serializable]
        public class DaylightSavingState
        {
            /// <summary>
            /// Gets or sets a value indicating whether at the last system boot it was daylight saving time.
            /// </summary>
            public bool WasDaylightSavingTime { get; set; }
        }
    }
}
