// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecCodecConfig.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecCodecConfig type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using System.ComponentModel;

    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Utility.Core.Factory;

    /// <summary>
    /// Configuration for the BEC Codec.
    /// </summary>
    [Implementation(typeof(BecMessageCodec))]
    public class BecCodecConfig : CodecConfig
    {
        /// <summary>
        /// Possible types of the serializer.
        /// </summary>
        public enum SerializerType
        {
            /// <summary>
            /// Use the default serializer for the current platform.
            /// </summary>
            Default,

            /// <summary>
            /// Use a serializer based on reflection.
            /// </summary>
            Reflection,

            /// <summary>
            /// Use a serializer that is generated using System.Reflection.Emit.
            /// </summary>
            Generated
        }

        /// <summary>
        /// Gets or sets the type of serializer to be used.
        /// Default value is "Default".
        /// </summary>
        [DefaultValue(SerializerType.Default)]
        public SerializerType Serializer { get; set; }
    }
}
