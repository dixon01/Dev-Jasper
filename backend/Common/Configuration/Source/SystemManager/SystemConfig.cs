// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SystemConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.SystemManager
{
    using System;
    using System.ComponentModel;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.SystemManager.Limits;

    /// <summary>
    /// The system configuration.
    /// </summary>
    [Serializable]
    public class SystemConfig
    {
        private const string RebootAtFormat = "HH:mm:ss";

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemConfig"/> class.
        /// </summary>
        public SystemConfig()
        {
            this.EnableReboot = true;
            this.KickWatchdog = true;

            this.IgnitionHoldTime = TimeSpan.FromSeconds(10);
            this.ShutDownSplashScreenVisibleTime = TimeSpan.FromSeconds(20);
        }

        /// <summary>
        /// Gets or sets a value indicating whether system reboots are enabled or not.
        /// This option can be set to false on developer systems to prevent actual reboots
        /// from happening.
        /// </summary>
        [DefaultValue(true)]
        public bool EnableReboot { get; set; }

        /// <summary>
        /// Gets or sets the time of day at which the system should automatically reboot.
        /// Default value is null.
        /// </summary>
        [XmlIgnore]
        public TimeSpan? RebootAt { get; set; }

        /// <summary>
        /// Gets or sets the reboot time as an XML compatible string.
        /// </summary>
        [XmlElement("RebootAt")]
        public string RebootAtString
        {
            get
            {
                return this.RebootAt == null ? null : (DateTime.Today + this.RebootAt.Value).ToString(RebootAtFormat);
            }

            set
            {
                if (value == null)
                {
                    this.RebootAt = null;
                }
                else
                {
                    this.RebootAt = DateTime.ParseExact(value, RebootAtFormat, CultureInfo.InvariantCulture).TimeOfDay;
                }
            }
        }

        /// <summary>
        /// Gets or sets the time after which the system should reboot (maximum uptime).
        /// </summary>
        [XmlIgnore]
        public TimeSpan? RebootAfter { get; set; }

        /// <summary>
        /// Gets or sets the reboot after time as an XML compatible string.
        /// </summary>
        [XmlElement("RebootAfter", DataType = "duration")]
        public string RebootAfterString
        {
            get
            {
                return this.RebootAfter == null ? null : XmlConvert.ToString(this.RebootAfter.Value);
            }

            set
            {
                this.RebootAfter = value == null ? null : (TimeSpan?)XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the visible time of the splash screen upon shutdown of system.
        /// </summary>
        [XmlIgnore]
        public TimeSpan ShutDownSplashScreenVisibleTime { get; set; }

        /// <summary>
        /// Gets or sets the visible time as an XML compatible string.
        /// </summary>
        [XmlElement("ShutDownSplashScreenVisibleTime", DataType = "duration")]
        public string ShutDownSplashScreenVisibleTimeString
        {
            get
            {
                return XmlConvert.ToString(this.ShutDownSplashScreenVisibleTime);
            }

            set
            {
                this.ShutDownSplashScreenVisibleTime = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to regularly kick the hardware watchdog.
        /// Default value is true.
        /// </summary>
        [DefaultValue(true)]
        public bool KickWatchdog { get; set; }

        /// <summary>
        /// Gets or sets the time to wait for the ignition signal to be at 0 before shutting down the system.
        /// </summary>
        [XmlIgnore]
        public TimeSpan IgnitionHoldTime { get; set; }

        /// <summary>
        /// Gets or sets the ignition hold time as an XML serializable string.
        /// </summary>
        [XmlElement("IgnitionHoldTime", DataType = "duration")]
        public string IgnitionHoldTimeString
        {
            get
            {
                return XmlConvert.ToString(this.IgnitionHoldTime);
            }

            set
            {
                this.IgnitionHoldTime = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the CPU limit configuration.
        /// </summary>
        public SystemRamLimitConfig RamLimit { get; set; }

        /// <summary>
        /// Gets or sets the RAM limit configuration.
        /// </summary>
        public CpuLimitConfig CpuLimit { get; set; }

        /// <summary>
        /// Gets or sets the disk limit configuration.
        /// </summary>
        public DiskLimitConfigList DiskLimits { get; set; }

        /// <summary>
        /// Gets or sets the popup blocking configuration.
        /// </summary>
        public PreventPopupsConfig PreventPopups { get; set; }
    }
}