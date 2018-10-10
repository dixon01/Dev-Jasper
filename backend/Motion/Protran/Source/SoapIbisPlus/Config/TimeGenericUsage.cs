// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeGenericUsage.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TimeGenericUsage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Config
{
    using System;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Protran.Common;
    using Gorba.Common.Configuration.Protran.Core;

    /// <summary>
    /// Configuration for all Unix timestamp values.
    /// </summary>
    [Serializable]
    public class TimeGenericUsage : GenericUsage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeGenericUsage"/> class.
        /// </summary>
        public TimeGenericUsage()
        {
            this.TimeFormat = "HH:mm";
        }

        /// <summary>
        /// Gets or sets the time format.
        /// Default value: HH:mm.
        /// </summary>
        [XmlAttribute]
        public string TimeFormat { get; set; }
    }
}