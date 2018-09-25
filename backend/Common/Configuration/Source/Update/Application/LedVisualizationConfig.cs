// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LedVisualizationConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LedVisualizationConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Update.Application
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for the Update LED on the PC-2.
    /// </summary>
    [Serializable]
    public class LedVisualizationConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LedVisualizationConfig"/> class.
        /// </summary>
        public LedVisualizationConfig()
        {
            this.Enabled = false;
            this.DefaultFrequency = 1.25;
            this.ErrorFrequency = 5;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the LED should be used.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }

        /// <summary>
        /// Gets or sets the default blink frequency in Hz for update transfers.
        /// Default value is 1.25 Hz meaning a change every 400 ms.
        /// </summary>
        [XmlAttribute]
        public double DefaultFrequency { get; set; }

        /// <summary>
        /// Gets or sets the error blink frequency in Hz for update errors.
        /// Default value is 5 Hz meaning a change every 100 ms.
        /// </summary>
        [XmlAttribute]
        public double ErrorFrequency { get; set; }
    }
}