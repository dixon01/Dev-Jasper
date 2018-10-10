// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSyncConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSyncConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// The time sync config.
    /// </summary>
    [Serializable]
    public class TimeSyncConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether time-sync is enabled.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the generic usage for the date.
        /// </summary>
        public GenericUsage DateUsedFor { get; set; }

        /// <summary>
        /// Gets or sets the generic usage for the time.
        /// </summary>
        public GenericUsage TimeUsedFor { get; set; }
    }
}