// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021ConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021ConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Base class for DS021aConfig and DS021bConfig
    /// </summary>
    [Serializable]
    public abstract class DS021ConfigBase : TelegramConfig
    {
        /// <summary>
        /// Gets or sets the usage of this telegram for transfers.
        /// </summary>
        [XmlIgnore]
        public virtual GenericUsageDS021Base UsedForTransfers { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram for transfer symbols.
        /// </summary>
        [XmlIgnore]
        public virtual GenericUsageDS021Base UsedForTransferSymbols { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram (before the EndingStopValue)
        /// as a destination. If this value is null, you should configure DS003 or DS003a instead.
        /// </summary>
        [XmlIgnore]
        public abstract GenericUsage UsedForDestination { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram for destination transfers.
        /// </summary>
        [XmlIgnore]
        public virtual GenericUsage UsedForDestinationTransfers { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram for destination transfer symbols.
        /// </summary>
        [XmlIgnore]
        public virtual GenericUsage UsedForDestinationTransferSymbols { get; set; }

        /// <summary>
        /// Gets or sets the XML element called FlushNumberOfStations.
        /// It represents the number of stations to collect before sending
        /// all of them to the media player.
        /// Values admitted: a positive integer value {1; 65535}.
        /// Default value: 5
        /// </summary>
        [XmlIgnore]
        public abstract int FlushNumberOfStations { get; set; }

        /// <summary>
        /// Gets or sets the element called FlushTimeout.
        /// It represents the amount of time to wait before
        /// forcing a send of all the current stations collected.
        /// Default value: 30 seconds
        /// </summary>
        [XmlIgnore]
        public TimeSpan FlushTimeout { get; set; }

        /// <summary>
        /// Gets or sets the XML element called FlushTimeout.
        /// </summary>
        [XmlIgnore]
        public virtual string FlushTimeoutString
        {
            get
            {
                return XmlConvert.ToString(this.FlushTimeout);
            }

            set
            {
                this.FlushTimeout = XmlConvert.ToTimeSpan(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the last stop should be shown or not.
        /// You might want to hide the last stop if you prefer not to show the destination
        /// name twice when approaching the destination.
        /// Default value: false
        /// </summary>
        [XmlIgnore]
        public abstract bool HideLastStop { get; set; }

        /// <summary>
        /// Gets or sets the threshold below which the destination has to be hidden.
        /// Set this value to one more than the number of stops visible in your
        /// stop list to hide the destination information (see <see cref="UsedForDestination"/>, ...)
        /// if the last stop is visible in the stop list.
        /// Default value: 0 (this feature is disabled)
        /// </summary>
        [XmlIgnore]
        public abstract int HideDestinationBelow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show past stops.
        /// If this flag is true, stops that have a lower stop index than the current stop will be
        /// put into Ximple rows with negative indexes.
        /// </summary>
        [XmlIgnore]
        public abstract bool ShowPastStops { get; set; }
    }
}