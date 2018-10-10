// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProtocolConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProtocolConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Core
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// General protocol configuration settings
    /// </summary>
    [Serializable]
    public class ProtocolConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProtocolConfig"/> class.
        /// </summary>
        public ProtocolConfig()
        {
            this.Enabled = true;
        }

        /// <summary>
        /// Gets or sets Name.
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this protocol is enabled.
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }
    }
}
