// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Obc.Ibis.Telegrams
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The telegram config.
    /// </summary>
    [Serializable]
    public abstract class TelegramConfigBase
    {
        /// <summary>
        /// Text used in description of cyclic telegrams.
        /// </summary>
        [XmlIgnore]
        protected const string TxtCycleAndChange = "(Send condition: Cyclic and Event)";

        /// <summary>
        /// Text used in description of event telegrams.
        /// </summary>
        [XmlIgnore]
        protected const string TxtEvent = "(Send condition: Event)";

        /// <summary>
        /// Gets or sets the description used in the XML file.
        /// </summary>
        [XmlAttribute("Description")]
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this telegram is enabled.
        /// </summary>
        [XmlElement]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the repeat interval in seconds.
        /// </summary>
        /// <remarks>
        /// Value = 0: Do not repeat this message. Just send the message by the corresponding event
        /// Value > 0: Repeat all seconds. valid range is between 5 and 120 seconds.
        /// </remarks>
        [XmlElement]
        public int RepeatInterval { get; set; }
    }
}