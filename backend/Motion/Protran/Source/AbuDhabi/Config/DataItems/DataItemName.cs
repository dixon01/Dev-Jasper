// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataItemName.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DataItemName type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config.DataItems
{
    /// <summary>
    /// Common ISI data item names.
    /// </summary>
    public static class DataItemName
    {
        /// <summary>
        /// The GorbaSystemFallbackActive name.
        /// </summary>
        public static readonly string GorbaSystemFallbackActive = "GorbaSystemFallbackActive";

        /// <summary>
        /// The VehicleNo name.
        /// </summary>
        public static readonly string VehicleNo = "VehicleNo";

        /// <summary>
        /// The Time_ISO8601 name.
        /// </summary>
        public static readonly string TimeIso8601 = "Time_ISO8601";

        /// <summary>
        /// The AppName name.
        /// </summary>
        public static readonly string AppName = "AppName";

        /// <summary>
        /// The CurrentParameterVersion name.
        /// </summary>
        public static readonly string CurrentParameterVersion = "CurrentParameterVersion";

        /// <summary>
        /// The CurrentSoftwareVersion name.
        /// </summary>
        public static readonly string CurrentSoftwareVersion = "CurrentSoftwareVersion";

        /// <summary>
        /// The DeviceState name.
        /// </summary>
        public static readonly string DeviceState = "DeviceState";

        /// <summary>
        /// The IsiClientRunsFtpTransfers name.
        /// </summary>
        public static readonly string IsiClientRunsFtpTransfers = "IsiClientRunsFtpTransfers";

        /// <summary>
        /// The SerialNumber name.
        /// </summary>
        public static readonly string SerialNumber = "SerialNumber";

        /// <summary>
        /// The ISI tag's name for the data item referring to the line's name (alpha-numeric).
        /// </summary>
        public static readonly string LineNameForDisplay = "LineNameForDisplay";

        /// <summary>
        /// The ISI tag's name for the data item referring to the current direction number (1 or 2).
        /// </summary>
        public static readonly string CurrentDirectionNo = "CurrentDirectionNo";

        /// <summary>
        /// The ISI tag's name for the data item referring to the destination's name.
        /// </summary>
        public static readonly string Destination = "Destination";

        /// <summary>
        /// The ISI tag's name for the data item referring to the destination's name
        /// in arabic language.
        /// </summary>
        public static readonly string DestinationArabic = "DestinationArabic";

        /// <summary>
        /// The ISI tag's name for the data item referring to the destination number.
        /// </summary>
        public static readonly string DestinationNo = "DestinationNo";

        /// <summary>
        /// The ISI tag's name for the data item referring to the ticker text.
        /// </summary>
        public static readonly string TickerText = "TickerText";

        /// <summary>
        /// The ISI tag's name for the data item referring to the ticker text
        /// in arabic language.
        /// </summary>
        public static readonly string TickerTextArabic = "TickerTextArabic";

        /// <summary>
        /// The current stop connection info.
        /// </summary>
        public static readonly string CurrentStopConnectionInfo = "CurrentStopConnectionInfo";

        /// <summary>
        /// The current stop connection info arabic.
        /// </summary>
        public static readonly string CurrentStopConnectionInfoArabic = "CurrentStopConnectionInfoArabic";

        /// <summary>
        /// The stop departure countdown state.
        /// </summary>
        public static readonly string StopDepartureCountdownState = "StopDepartureCountdownState";
    }
}
