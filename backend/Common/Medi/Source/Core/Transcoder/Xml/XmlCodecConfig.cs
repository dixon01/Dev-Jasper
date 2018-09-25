// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlCodecConfig.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlCodecConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Xml
{
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Codec configuration for XML codec.
    /// </summary>
    [Implementation(typeof(XmlMessageCodec))]
    public class XmlCodecConfig : CodecConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlCodecConfig"/> class.
        /// The default value of <see cref="Encoding"/> is set to "UTF-8".
        /// </summary>
        public XmlCodecConfig()
        {
            this.Encoding = System.Text.Encoding.UTF8.WebName;
        }

        /// <summary>
        /// Gets or sets the encoding.
        /// This should be the body name of an encoding supported by .NET,
        /// see <see cref="System.Text.Encoding.GetEncoding(string)"/>.
        /// </summary>
        public string Encoding { get; set; }
    }
}
