// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Connection configuration for DS021a
    /// </summary>
    [Serializable]
    public class ConnectionConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionConfig"/> class.
        /// </summary>
        public ConnectionConfig()
        {
            this.Enabled = false;
            this.LineNumberFormat = "{0}";
            this.ShowForNextStopOnly = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether connection handling is enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the generic usage of the
        /// connection information for the destination.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedFor { get; set; }

        /// <summary>
        /// Gets or sets the generic usage of the
        /// connection information for the stop for which this connection is.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForStopName { get; set; }

        /// <summary>
        /// Gets or sets the generic usage of the
        /// connection information for the line number.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForLineNumber { get; set; }

        /// <summary>
        /// Gets or sets the generic usage of the
        /// connection information for the departure time.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GenericUsage UsedForDepartureTime { get; set; }

        /// <summary>
        /// Gets or sets LineNumberFormat.
        /// </summary>
        public string LineNumberFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether connection information
        /// is to be shown only for the immediately next stop.
        /// </summary>
        public bool ShowForNextStopOnly { get; set; }
    }
}