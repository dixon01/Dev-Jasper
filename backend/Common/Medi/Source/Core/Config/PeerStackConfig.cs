// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeerStackConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PeerStackConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Peers;

    /// <summary>
    /// Configuration for <see cref="PeerStackBase{TTransport,TTransportConfig}"/>
    /// </summary>
    /// <typeparam name="TTransportConfig">
    /// The type of the transport configuration.
    /// </typeparam>
    public abstract class PeerStackConfig<TTransportConfig> : PeerConfig
        where TTransportConfig : TransportConfig
    {
        /// <summary>
        /// Gets or sets the codec configuration.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public CodecConfig Codec { get; set; }

        /// <summary>
        /// Gets or sets transport configuration.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TTransportConfig Transport { get; set; }
    }
}
