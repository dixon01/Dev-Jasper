// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SimulationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information about the IBIS simulation.
    /// </summary>
    [Serializable]
    public class SimulationConfig
    {
        /// <summary>
        /// Gets or sets the XML field called SimulationFile
        /// Free text (case insensitive) Default: ibis.trace
        /// </summary>
        public string SimulationFile { get; set; }

        /// <summary>
        /// Gets or sets the field called InitialDelay
        /// Values admitted: any positive timespan. Default: 0 seconds
        /// </summary>
        [XmlIgnore]
        public TimeSpan InitialDelay { get; set; }

        /// <summary>
        /// Gets or sets the XML field called InitialDelay
        /// Values admitted: any positive timespan. Default: 0 seconds
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
        /// Gets or sets the field called IntervalBetweenTelegrams
        /// Values admitted: any positive timespan.
        /// empty means that the simulation will follow specific time stamps. Default: empty
        /// </summary>
        [XmlIgnore]
        public TimeSpan? IntervalBetweenTelegrams { get; set; }

        /// <summary>
        /// Gets or sets the XML field called IntervalBetweenTelegrams
        /// Values admitted: any positive timespan.
        /// empty means that the simulation will follow specific time stamps. Default: empty
        /// </summary>
        [XmlElement("IntervalBetweenTelegrams", DataType = "duration")]
        public string IntervalBetweenTelegramsString
        {
            get
            {
                return this.IntervalBetweenTelegrams == null
                           ? null
                           : XmlConvert.ToString(this.IntervalBetweenTelegrams.Value);
            }

            set
            {
                this.IntervalBetweenTelegrams = string.IsNullOrEmpty(value)
                                                    ? (TimeSpan?)null
                                                    : XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets the XML field called TimesToRepeat
        /// Values admitted: {0, max integer} 0 means infinite times. Default: 1
        /// </summary>
        public int TimesToRepeat { get; set; }
    }
}