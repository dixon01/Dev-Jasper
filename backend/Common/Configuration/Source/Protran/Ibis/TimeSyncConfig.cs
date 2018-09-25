// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSyncConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for IBIS time synchronization.
    /// </summary>
    [Serializable]
    public class TimeSyncConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeSyncConfig"/> class.
        /// </summary>
        public TimeSyncConfig()
        {
            this.InitialDelay = TimeSpan.FromSeconds(10);
            this.WaitTelegrams = 3;
            this.Tolerance = TimeSpan.Zero;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this time sync should be enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before the time sync is started.
        /// Default value: 10 seconds
        /// </summary>
        [XmlIgnore]
        public TimeSpan InitialDelay { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before the time sync is started in an XML serializable string.
        /// </summary>
        [XmlElement("InitialDelay", DataType = "duration")]
        public string InitialDelayString
        {
            get
            {
                return XmlConvert.ToString(this.InitialDelay);
            }

            set
            {
                this.InitialDelay = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the number of telegrams to check before
        /// the time is considered valid.
        /// </summary>
        public int WaitTelegrams { get; set; }

        /// <summary>
        /// Gets or sets the tolerance above which the
        /// difference has to be for the time to be synchronized.
        /// Default value: 0 seconds
        /// </summary>
        [XmlIgnore]
        public TimeSpan Tolerance { get; set; }

        /// <summary>
        /// Gets or sets the tolerance in an XML serializable format.
        /// </summary>
        [XmlElement("Tolerance", DataType = "duration")]
        public string ToleranceString
        {
            get
            {
                return XmlConvert.ToString(this.Tolerance);
            }

            set
            {
                this.Tolerance = XmlConvert.ToTimeSpan(value);
            }
        }
    }
}
