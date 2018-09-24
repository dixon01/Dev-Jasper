// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021CConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021CConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a DS021c IBIS telegram.
    /// </summary>
    [Serializable]
    public sealed class DS021CConfig : DS021ConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DS021CConfig"/> class.
        /// </summary>
        public DS021CConfig()
        {
            this.FlushNumberOfStations = 5;
            this.FlushTimeout = TimeSpan.FromSeconds(30);
            this.FirstStopIndexValue = 1;
            this.TakeDestinationFromLastStop = false;
        }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram for transfers.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsageDS021Base UsedForTransfers { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram for transfer symbols.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsageDS021Base UsedForTransferSymbols { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram (before the EndingStopValue)
        /// as a destination. If this value is null, you should configure DS003 or DS003a instead.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedForDestination { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram for destination transfers.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedForDestinationTransfers { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram for destination transfer symbols.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedForDestinationTransferSymbols { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram's ASCII line number (index 102).
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsageDS021Base AsciiLineNumberUsedFor { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Answer.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override Answer Answer { get; set; }

        /// <summary>
        /// Gets or sets the XML element called FlushNumberOfStations.
        /// It represents the number of stations to collect before sending
        /// all of them to the media player.
        /// Values admitted: a positive integer value {1; 65535}.
        /// Default value: 5
        /// </summary>
        public override int FlushNumberOfStations { get; set; }

        /// <summary>
        /// Gets or sets the XML element called FlushTimeout.
        /// </summary>
        [XmlElement("FlushTimeout", DataType = "duration")]
        public override string FlushTimeoutString
        {
            get
            {
                return base.FlushTimeoutString;
            }

            set
            {
                base.FlushTimeoutString = value;
            }
        }

        /// <summary>
        /// Gets or sets the XML element called FirstStopIndexValue.
        /// It represents the value of the index of the first stop.
        /// Values admitted {0, 1}. Default value: 1.
        /// </summary>
        public int FirstStopIndexValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last stop should be shown or not.
        /// You might want to hide the last stop if you prefer not to show the destination
        /// name twice when approaching the destination.
        /// Default value: false
        /// </summary>
        public override bool HideLastStop { get; set; }

        /// <summary>
        /// Gets or sets the threshold below which the destination has to be hidden.
        /// Set this value to one more than the number of stops visible in your
        /// stop list to hide the destination information (see <see cref="DS021ConfigBase.UsedForDestination"/>, ...)
        /// if the last stop is visible in the stop list.
        /// Default value: 0 (this feature is disabled)
        /// </summary>
        [DefaultValue(0)]
        public override int HideDestinationBelow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the destination should be taken from
        /// the last stop (true) or the index 101 record (false).
        /// </summary>
        [DefaultValue(false)]
        public bool TakeDestinationFromLastStop { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show past stops.
        /// If this flag is true, stops that have a lower stop index than the current stop will be
        /// put into Ximple rows with negative indexes.
        /// </summary>
        [DefaultValue(false)]
        public override bool ShowPastStops { get; set; }
    }
}
