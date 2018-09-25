// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GpioConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Mgi
{
    using System;
    using System.Collections.Generic;
    using System.Xml.Serialization;

    /// <summary>
    /// The configuration for the GPIO
    /// </summary>
    [Serializable]
    public class GpioConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GpioConfig"/> class.
        /// </summary>
        public GpioConfig()
        {
            this.Pins = new List<PinConfig>();
        }

        /// <summary>
        /// Gets or sets the pins.
        /// </summary>
        [XmlElement("Pin")]
        public List<PinConfig> Pins { get; set; }
    }
}
