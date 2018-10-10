// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SntpConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SntpConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for SNTP configurations.
    /// </summary>
    [Serializable]
    public abstract class SntpConfigBase
    {
        private const SntpVersionNumber DefaultVersionNumber = SntpVersionNumber.Version3;

        /// <summary>
        /// Initializes a new instance of the <see cref="SntpConfigBase"/> class.
        /// </summary>
        protected SntpConfigBase()
        {
            this.Enabled = false;
            this.VersionNumber = DefaultVersionNumber;
            this.RetryInterval = TimeSpan.FromSeconds(10);
            this.RetryCount = 5;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the protocol is enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the SNTP protocol version number as an integer.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the given value is not a valid version number (3 or 4 only!)
        /// </exception>
        [XmlAttribute("Version")]
        public int Version
        {
            get
            {
                return (int)this.VersionNumber;
            }

            set
            {
                if (value < 3 || value > 4)
                {
                    throw new ArgumentOutOfRangeException("value", "Only versions 3 and 4 supported");
                }

                this.VersionNumber = (SntpVersionNumber)value;
            }
        }

        /// <summary>
        /// Gets or sets the SNTP protocol version number.
        /// </summary>
        [XmlIgnore]
        public SntpVersionNumber VersionNumber { get; set; }

        /// <summary>
        /// Gets or sets the retry interval that is used when the synchronization fails.
        /// After this interval, the client will try again to query the time.
        /// </summary>
        [XmlIgnore]
        public TimeSpan RetryInterval { get; set; }

        /// <summary>
        /// Gets or sets the retry interval as an XML serializable string.
        /// </summary>
        [XmlAttribute("RetryInterval", DataType = "duration")]
        [DefaultValue("PT10S")]
        public string RetryIntervalString
        {
            get
            {
                return XmlConvert.ToString(this.RetryInterval);
            }

            set
            {
                this.RetryInterval = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the number of retries to do before aborting the time synchronization.
        /// </summary>
        [XmlAttribute("RetryCount")]
        [DefaultValue(5)]
        public int RetryCount { get; set; }
    }
}
