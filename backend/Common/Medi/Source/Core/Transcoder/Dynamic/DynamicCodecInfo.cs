// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicCodecInfo.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicCodecInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Dynamic
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// Codec information for <see cref="DynamicCodecConfig"/>.
    /// </summary>
    public class DynamicCodecInfo : IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the type of the Codec.
        /// </summary>
        public Type CodecType { get; set; }

        /// <summary>
        /// Gets or sets the config for the Codec.
        /// </summary>
        public CodecConfig Config { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var configType = reader.GetAttribute("configType");
            if (configType == null)
            {
                throw new TypeLoadException("Config type not defined");
            }

            var type = TypeFactory.Instance[configType];
            if (type == null)
            {
                throw new TypeLoadException("Config type not found: " + configType);
            }

            reader.ReadStartElement(); // our root element

            this.CodecType = TypeFactory.Instance[reader.ReadElementString("CodecType")];

            var ser = new XmlSerializer(type);
            this.Config = ser.Deserialize(reader) as CodecConfig;

            reader.ReadEndElement(); // our root element
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("configType", this.Config.GetType().FullName);

            writer.WriteElementString("CodecType", this.CodecType.FullName);

            var ser = new XmlSerializer(this.Config.GetType());
            ser.Serialize(writer, this.Config);
        }
    }
}
