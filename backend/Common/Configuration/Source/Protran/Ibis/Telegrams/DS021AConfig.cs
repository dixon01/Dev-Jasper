// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DS021AConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DS021AConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a DS021a IBIS telegram.
    /// </summary>
    [Serializable]
    public sealed class DS021AConfig : DS021ConfigBase
    {
        private const string DefaultAbsoluteTimeFormat = "HH:mm";

        /// <summary>
        /// Initializes a new instance of the <see cref="DS021AConfig"/> class.
        /// </summary>
        public DS021AConfig()
        {
            this.UsedForDestination = null;
            this.FlushNumberOfStations = 5;
            this.FlushTimeout = TimeSpan.FromSeconds(30);
            this.FirstStopIndexValue = 1;
            this.EndingStopValue = 99;
            this.HideLastStop = false;
            this.HideDestinationBelow = 0;
            this.DeleteRouteIndexValue = -1;
            this.AbsoluteTimeFormat = DefaultAbsoluteTimeFormat;

            this.Connection = new ConnectionConfig();
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
        /// Gets or sets the usage of the relative time until a certain stop in minutes.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsageDS021Base UsedForRelativeTime { get; set; }

        /// <summary>
        /// Gets or sets the usage of the absolute time of arrival at a certain stop.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsageDS021Base UsedForAbsoluteTime { get; set; }

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
        /// Gets or sets the usage of the relative time until the destination in minutes.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForDestinationRelativeTime { get; set; }

        /// <summary>
        /// Gets or sets the usage of the absolute time of arrival at the destination.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForDestinationAbsoluteTime { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram for News Ticker text.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForText { get; set; }

        /// <summary>
        /// Gets or sets the connection configuration for handling 'A' stop indexes.
        /// </summary>
        public ConnectionConfig Connection { get; set; }

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
        /// Values admitted {0, 1}. Default value: 0.
        /// </summary>
        public int FirstStopIndexValue { get; set; }

        /// <summary>
        /// Gets or sets the XML element called EndingStopValue.
        /// It represents the value of the stop that has to be considered
        /// as the end of a sequence of stops (and not really a proper stop as the others).
        /// Values admitted {0, 65535}. Default value: 99
        /// </summary>
        public int EndingStopValue { get; set; }

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
        /// Gets or sets DeleteRouteIndexValue. Delete the entire route and clear
        /// the stop list when the index is 0.
        /// </summary>
        [DefaultValue(-1)]
        public int DeleteRouteIndexValue { get; set; }

        /// <summary>
        /// Gets or sets the string format used for formatting the absolute times
        /// in <see cref="UsedForAbsoluteTime"/> and <see cref="UsedForDestinationAbsoluteTime"/>.
        /// </summary>
        /// <seealso cref="DateTime.ToString(string)"/>
        [DefaultValue(DefaultAbsoluteTimeFormat)]
        public string AbsoluteTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show past stops.
        /// If this flag is true, stops that have a lower stop index than the current stop will be
        /// put into Ximple rows with negative indexes.
        /// </summary>
        [DefaultValue(false)]
        public override bool ShowPastStops { get; set; }
    }
}
