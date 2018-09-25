// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScheduleConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Bus
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The schedule configuration.
    /// </summary>
    [Serializable]
    public class ScheduleConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduleConfig"/> class.
        /// </summary>
        public ScheduleConfig()
        {
            this.DayStart = TimeSpan.FromHours(2);
        }

        /// <summary>
        /// Gets or sets the time when the day starts.
        /// </summary>
        [XmlIgnore]
        public TimeSpan DayStart { get; set; }

        /// <summary>
        /// Gets or sets the time when the day starts for XML serialization.
        /// </summary>
        [XmlElement("DayStart")]
        public string LaunchDelayString
        {
            get
            {
                return this.DayStart.ToString();
            }

            set
            {
                this.DayStart = TimeSpan.Parse(value);
            }
        }
    }
}