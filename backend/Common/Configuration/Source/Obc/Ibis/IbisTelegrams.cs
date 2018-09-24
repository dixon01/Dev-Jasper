// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisTelegrams.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Common.Configuration.Obc.Ibis
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Obc.Common;
    using Gorba.Common.Configuration.Obc.Ibis.Telegrams;

    /// <summary>
    /// The IBIS telegrams configuration.
    /// </summary>
    [Serializable]
    public class IbisTelegrams
    {
        private ConfigItem<int> minRepeatInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisTelegrams"/> class.
        /// </summary>
        public IbisTelegrams()
        {
            const string RepeatDescription =
                "The minimum repeat interval time for IBIS messages which contains a RepeatInterval xml tag. "
                + "If the RepeatInterval is smaller than this value, the cycle duration will be set to this value. "
                + "Value is in seconds.";
            this.minRepeatInterval = new ConfigItem<int>(10, RepeatDescription);
            this.Telegrams = new List<TelegramConfigBase>();
        }

        /// <summary>
        /// Gets or sets the minimum allowed repeat interval.
        /// </summary>
        public ConfigItem<int> MinRepeatIntervalConfig
        {
            get
            {
                if (this.minRepeatInterval < 1)
                {
                    this.minRepeatInterval.Value = 1;
                }

                return this.minRepeatInterval;
            }

            set
            {
                this.minRepeatInterval = value;
            }
        }

        /// <summary>
        /// Gets or sets the configured telegrams.
        /// </summary>
        [XmlElement("DS001", typeof(DS001Config))]
        [XmlElement("DS002", typeof(DS002Config))]
        [XmlElement("DS003", typeof(DS003Config))]
        [XmlElement("DS003c", typeof(DS003CConfig))]
        [XmlElement("DS004", typeof(DS004Config))]
        [XmlElement("DS004a", typeof(DS004AConfig))]
        [XmlElement("DS004b", typeof(DS004BConfig))]
        [XmlElement("DS004c", typeof(DS004CConfig))]
        [XmlElement("DS005", typeof(DS005Config))]
        [XmlElement("DS006", typeof(DS006Config))]
        [XmlElement("DS009", typeof(DS009Config))]
        [XmlElement("DSHPW010b_1", typeof(DS010JConfig))] // confusing: what is configured as DS010b is actually DS010j
        [XmlElement("DS020", typeof(DS020Config))]
        [XmlElement("DSHPW021b_1", typeof(DS021CConfig))] // confusing: what is configured as DS021b is actually DS021c
        [XmlElement("DS036", typeof(DS036Config))]
        [XmlElement("DS037_1", typeof(DS037Config))] // confusing: what is configured as DS037 is actually DS038
        [XmlElement("DS038_1", typeof(DS038Config))] // confusing: what is configured as DS038 doesn't exist in IBIS
        [XmlElement("DS070", typeof(DS070Config))]
        [XmlElement("DS080", typeof(DS080Config))]
        [XmlElement("DS081", typeof(DS081Config))]
        [XmlElement("DS084", typeof(DS084Config))]
        public List<TelegramConfigBase> Telegrams { get; set; }
    }
}