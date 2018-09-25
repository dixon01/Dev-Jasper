// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicCodecConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicCodecConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Dynamic
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the dynamic Codec.
    /// </summary>
    [Implementation(typeof(DynamicMessageCodec))]
    public class DynamicCodecConfig : CodecConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicCodecConfig"/> class.
        /// By default it supports two Codecs with their default configuration:
        /// XML and BEC.
        /// </summary>
        public DynamicCodecConfig()
        {
            this.SupportedCodecs = new List<DynamicCodecInfo>
            {
                new DynamicCodecInfo { CodecType = typeof(Xml.XmlMessageCodec), Config = new Xml.XmlCodecConfig() },
                new DynamicCodecInfo { CodecType = typeof(Bec.BecMessageCodec), Config = new Bec.BecCodecConfig() }
            };
        }

        /// <summary>
        /// Gets or sets the list of supported codecs.
        /// </summary>
        public List<DynamicCodecInfo> SupportedCodecs { get; set; }
    }
}
