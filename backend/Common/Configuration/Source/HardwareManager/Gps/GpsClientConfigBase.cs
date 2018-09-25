// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpsClientConfigBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GpsClientConfigBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Gps
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// The base class for all GPS receiver configurations.
    /// </summary>
    [Serializable]
    public abstract class GpsClientConfigBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpsClientConfigBase"/> class.
        /// </summary>
        protected GpsClientConfigBase()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this GPS receiver is enabled.
        /// </summary>
        [XmlAttribute("Enabled")]
        public bool Enabled { get; set; }
    }
}