// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeItemConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Container of configuration related to time
    /// </summary>
    [Serializable]
    public class TimeItemConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeItemConfig"/> class.
        /// </summary>
        public TimeItemConfig()
        {
            this.Enabled = false;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SOAP data type is to
        /// be handled or not.
        /// </summary>
        [XmlAttribute(AttributeName = "Enabled")]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the stop arrival relative time handling.
        /// </summary>
        public GenericUsage UsedForRelativeTime { get; set; }

        /// <summary>
        /// Gets or sets the stop arrival absolute time handling.
        /// </summary>
        public TimeGenericUsage UsedForAbsoluteTime { get; set; }
    }
}
