// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv453MessageSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv453MessageSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Non-generic serializer for a certain VDV 453 message type.
    /// </summary>
    public class Vdv453MessageSerializer
    {
        private readonly Encoding encoding = Encoding.GetEncoding("iso-8859-1");

        private readonly XmlSerializer serializer;

        private readonly XmlSerializerNamespaces xmlNamespaces = new XmlSerializerNamespaces();

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv453MessageSerializer"/> class.
        /// </summary>
        /// <param name="messageType">
        /// The type of the message.
        /// </param>
        /// <param name="elementName">
        /// XML element name
        /// </param>
        /// <param name="xmlNameSpace">
        /// XML namespace
        /// </param>
        public Vdv453MessageSerializer(Type messageType, string elementName, string xmlNameSpace)
        {
            if (string.IsNullOrEmpty(elementName) || string.IsNullOrEmpty(xmlNameSpace))
            {
                this.xmlNamespaces.Add(string.Empty, string.Empty);

                // Create the Serializer
                this.serializer = new XmlSerializer(messageType);
                return;
            }
            
            this.xmlNamespaces.Add("vdv453", xmlNameSpace);

            // Create an XmlAttributes to override the default root element.
            var xmlAttributes = new XmlAttributes();

            // Create an XmlRootAttribute and set its element name and namespace.
            var xmlRootAttribute = new XmlRootAttribute();
            xmlRootAttribute.ElementName = elementName;
            xmlRootAttribute.Namespace = xmlNameSpace;
            xmlRootAttribute.IsNullable = false;

            // Set the XmlRoot property to the XmlRoot object.
            xmlAttributes.XmlRoot = xmlRootAttribute;
            var xmlAttributeOverrides = new XmlAttributeOverrides();

            // Add the XmlAttributes object to the XmlAttributeOverrides object.
            xmlAttributeOverrides.Add(messageType, xmlAttributes);

            // Create the Serializer
            this.serializer = new XmlSerializer(messageType, xmlAttributeOverrides);
        }

        /// <summary>
        /// Serialization procedure.
        /// </summary>
        /// <param name="message">
        /// The message to be serialized.
        /// </param>
        /// <param name="omitXmlDeclaration">
        /// indicates whether to omit the XML declaration
        /// </param>
        /// <returns>
        /// serialized message string
        /// </returns>
        public string Serialize(Vdv453Message message, bool omitXmlDeclaration)
        {
            // Create the XML writer settings and set the encoding
            var xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Encoding = this.encoding;
            xmlWriterSettings.OmitXmlDeclaration = omitXmlDeclaration;

            // we have to use MemoryStream because StringWriter doesn't support different encodings than UTF-16
            using (var memory = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(memory, xmlWriterSettings))
                {
                    this.serializer.Serialize(xmlWriter, message, this.xmlNamespaces);
                }

                return this.encoding.GetString(memory.ToArray());
            }
        }

        /// <summary>
        /// Deserialization procedure.
        /// </summary>
        /// <param name="xmlReader">
        /// XML text reader object
        /// </param>
        /// <returns>
        /// deserialized message object
        /// </returns>
        public Vdv453Message Deserialize(XmlTextReader xmlReader)
        {
            return (Vdv453Message)this.serializer.Deserialize(xmlReader);
        }
    }
}