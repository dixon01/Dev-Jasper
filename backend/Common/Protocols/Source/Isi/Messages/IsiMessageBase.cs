// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiMessageBase.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiMessageBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>
    /// Base class for all ISI protocol messages.
    /// </summary>
    public abstract class IsiMessageBase
    {
        /// <summary>
        /// Gets the XML representation of this ISI message
        /// (whitout white spaces, tabs, indentations... as specified by ISI).
        /// </summary>
        /// <returns>The XML representation of this ISI message.</returns>
        public override string ToString()
        {
            var writer = new StringWriter();

            var serializable = this as IXmlSerializable;
            using (var xml = IsiXmlWriterFactory.Create(writer))
            {
                if (serializable != null)
                {
                    xml.WriteStartElement(this.GetType().Name);
                    serializable.WriteXml(xml);
                    xml.WriteEndElement();
                }
                else
                {
                    var serializer = new XmlSerializer(this.GetType());
                    var namespaces = new XmlSerializerNamespaces();
                    namespaces.Add(string.Empty, string.Empty);
                    serializer.Serialize(xml, this, namespaces);
                }
            }

            return writer.ToString();
        }
    }
}
