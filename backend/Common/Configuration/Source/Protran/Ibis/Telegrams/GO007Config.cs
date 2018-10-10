// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GO007Config.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GO007Config type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;

    /// <summary>
    /// Container of all the information about a GO007 IBIS telegram.
    /// </summary>
    public class GO007Config : TelegramConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GO007Config"/> class.
        /// </summary>
        public GO007Config()
        {
            this.UsedForDestination = null;
            this.HideLastStop = false;
            this.HideDestinationBelow = 0;
        }

        /// <summary>
        /// Gets or sets the usage of this telegram for transfers.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForTransfers { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last stop in GO007 telegram
        /// as a destination.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForDestination { get; set; }

        /// <summary>
        /// Gets or sets the usage of the last stop in GO007 telegram for destination transfers.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForDestinationTransfers { get; set; }

        /// <summary>
        /// Gets or sets the usage of this telegram's line number.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForLineNumber { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the last stop should be shown or not.
        /// You might want to hide the last stop if you prefer not to show the destination
        /// name twice when approaching the destination.
        /// Default value: false
        /// </summary>
        public bool HideLastStop { get; set; }

        /// <summary>
        /// Gets or sets the threshold below which the destination has to be hidden.
        /// Set this value to one more than the number of stops visible in your
        /// stop list to hide the destination information (see <see cref="UsedForDestination"/>, ...)
        /// if the last stop is visible in the stop list.
        /// Default value: 0 (this feature is disabled)
        /// </summary>
        [DefaultValue(0)]
        public int HideDestinationBelow { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to show past stops.
        /// If this flag is true, stops that have a lower stop index than the current stop will be
        /// put into Ximple rows with negative indexes.
        /// </summary>
        [DefaultValue(false)]
        public bool ShowPastStops { get; set; }
    }
}
