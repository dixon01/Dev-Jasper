// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IOPortConfig.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IOPortConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.AudioRenderer
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Configuration for a single I/O port.
    /// </summary>
    [Serializable]
    public class IOPortConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IOPortConfig"/> class.
        /// </summary>
        public IOPortConfig()
        {
            this.Value = 8;
        }

        /// <summary>
        /// Gets or sets the unit name where the I/O resides.
        /// If this is left empty, the I/O is searched on the current unit only.
        /// </summary>
        [XmlAttribute]
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the application name where the I/O resides.
        /// If this is left empty, the I/O is searched in all applications
        /// (on the configured <see cref="Unit"/>).
        /// </summary>
        [XmlAttribute]
        public string Application { get; set; }

        /// <summary>
        /// Gets or sets the port name of the I/O.
        /// </summary>
        [XmlAttribute("Name")]
        public string PortName { get; set; }

        /// <summary>
        /// Gets or sets the port volume value.
        /// </summary>
        [XmlAttribute("Value")]
        public int Value { get; set; }
    }
}