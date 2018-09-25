// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   This class is not meant for use outside this assembly.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Transcoder.Xml;

    /// <summary>
    /// This class is not meant for use outside this assembly.
    /// </summary>
    [Serializable]
    public class MediMessage : IMessage, IXmlSerializable
    {
        /// <summary>
        /// Gets or sets the source address from where this message comes.
        /// </summary>
        public MediAddress Source { get; set; }

        /// <summary>
        /// Gets or sets the destination address to where this message goes.
        /// </summary>
        public MediAddress Destination { get; set; }

        /// <summary>
        /// Gets or sets the payload.
        /// This can be any object, the only restriction is, it has to be
        /// serializable by the codec used to transmit this message
        /// (e.g. BEC or XML).
        /// </summary>
        public object Payload { get; set; }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format(
                "MediMessage[Source={0},Destination={1},Payload={2}]", this.Source, this.Destination, this.Payload);
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            var payloadType = reader.GetAttribute("payloadType");
            if (payloadType == null)
            {
                throw new TypeLoadException("Payload type not defined");
            }

            reader.ReadStartElement(); // our root element

            this.Source = ReadMediAddress(reader, "Source");
            this.Destination = ReadMediAddress(reader, "Destination");

            var typeName = new TypeName(payloadType);
            if (typeName.IsKnown)
            {
                var ser = new XmlSerializer(typeName.Type);
                this.Payload = ser.Deserialize(reader);
            }
            else
            {
                this.Payload = new UnknownXmlObject(typeName, reader);
            }

            reader.ReadEndElement(); // our root element
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var typeName = TypeName.GetNameFor(this.Payload);
            writer.WriteAttributeString("payloadType", typeName.FullName);

            WriteMediAddress(writer, "Source", this.Source);
            WriteMediAddress(writer, "Destination", this.Destination);

            if (typeName.IsKnown)
            {
                var ser = new XmlSerializer(typeName.Type);
                ser.Serialize(writer, this.Payload);
            }
            else
            {
                var unknown = this.Payload as UnknownXmlObject;
                if (unknown != null)
                {
                    writer.WriteRaw(unknown.Xml);
                }
            }
        }

        private static MediAddress ReadMediAddress(XmlReader reader, string tag)
        {
            var addr = new MediAddress();

            reader.ReadStartElement(tag);
            addr.Unit = reader.ReadElementString("Unit");
            addr.Application = reader.ReadElementString("App");
            reader.ReadEndElement();

            return addr;
        }

        private static void WriteMediAddress(XmlWriter writer, string tag, MediAddress addr)
        {
            writer.WriteStartElement(tag);
            writer.WriteElementString("Unit", addr.Unit);
            writer.WriteElementString("App", addr.Application);
            writer.WriteEndElement(); // tag
        }
    }
}
