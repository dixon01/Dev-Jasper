// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO005Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a GO005 IBIS telegram.
    /// </summary>
    [Serializable]
    public sealed class GO005Config : DS021ConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO005Config"/> class.
        /// </summary>
        public GO005Config()
        {
            this.FlushNumberOfStations = 5;
            this.FlushTimeout = TimeSpan.FromSeconds(30);
            this.BufferNextRoute = true;
            this.HideNextStopForIndex = 999;
            this.DeleteRoute = true;
        }

        /// <summary>
        /// Gets or sets the usage of this telegram.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last DS021a telegram (before the EndingStopValue)
        /// as a destination. If this value is null, you should configure DS003 or DS003a instead.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override GenericUsage UsedForDestination { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram's ASCII line number (index 102).
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage AsciiLineNumberUsedFor { get; set; }

        /// <summary>
        /// Gets or sets the XML element called Answer.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public override Answer Answer { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to buffer newly arrived route or not
        /// </summary>
        public bool BufferNextRoute { get; set; }

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
        /// Gets or sets a value indicating whether to show past stops.
        /// If this flag is true, stops that have a lower stop index than the current stop will be
        /// put into Ximple rows with negative indexes.
        /// </summary>
        [DefaultValue(false)]
        public override bool ShowPastStops { get; set; }

        /// <summary>
        /// Gets or sets the index to hide the next stop
        /// </summary>
        public int HideNextStopForIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether protran has to delete the route information
        /// considering also the run's value, or not. Default value is true;
        /// </summary>
        public bool DeleteRoute { get; set; }
    }
}
