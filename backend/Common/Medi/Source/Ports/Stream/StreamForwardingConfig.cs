// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamForwardingConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamForwardingConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports.Stream
{
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Ports.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Dummy configuration for creating a local forwarding stream.
    /// This config should never be transmitted over Medi.
    /// </summary>
    [Implementation(typeof(StreamForwarder))]
    internal class StreamForwardingConfig : ForwardingEndPointConfig
    {
        /// <summary>
        /// Gets or sets the forwarder being used with the stream.
        /// </summary>
        [XmlIgnore]
        public StreamForwarder Forwarder { get; set; }
    }
}