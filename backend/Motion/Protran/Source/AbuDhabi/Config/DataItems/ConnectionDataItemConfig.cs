// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionDataItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config.DataItems
{
    using System;
    using System.ComponentModel;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Class for data item configurations that refer to connection information that have to be shown on the connection
    /// screen.
    /// </summary>
    [Serializable]
    public abstract class ConnectionDataItemConfig : DataItemConfig
    {
        private const string DefaultTimeFormat = "HH:mm";

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionDataItemConfig"/> class.
        /// </summary>
        protected ConnectionDataItemConfig()
        {
            this.TransportTypeFormat = "{0}";
            this.TimeFormat = DefaultTimeFormat;
        }

        /// <summary>
        /// Gets or sets the used for transfer symbols.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForTransferSymbols { get; set; }

        /// <summary>
        /// Gets or sets the used for connection time.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForConnectionTime { get; set; }

        /// <summary>
        /// Gets or sets the used for connection destination.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForConnectionDestination { get; set; }

        /// <summary>
        /// Gets or sets the used for connection line number.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForConnectionLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the used for connection transport type.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForConnectionTransportType { get; set; }

        /// <summary>
        /// Gets or sets the time format.
        /// </summary>
        public string TimeFormat { get; set; }

        /// <summary>
        /// Gets or sets TransportTypeFormat.
        /// </summary>
        public string TransportTypeFormat { get; set; }
    }
}
