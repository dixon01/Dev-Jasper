// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Vdv453MessageSerializer{T}.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Vdv453MessageSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Messages
{
    using System.Xml;

    /// <summary>
    /// Generic serializer for a certain VDV 453 message type.
    /// </summary>
    /// <typeparam name="T">
    /// The type this serializer understands.
    /// </typeparam>
    public class Vdv453MessageSerializer<T> 
        where T : Vdv453Message
    {
        private readonly Vdv453MessageSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Vdv453MessageSerializer{T}"/> class.
        /// </summary>
        /// <param name="elementName">
        /// XML element name
        /// </param>
        /// <param name="xmlNameSpace">
        /// XML namespace
        /// </param>
        public Vdv453MessageSerializer(string elementName, string xmlNameSpace)
        {
            this.serializer = new Vdv453MessageSerializer(typeof(T), elementName, xmlNameSpace);
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
        public string Serialize(T message, bool omitXmlDeclaration)
        {
            return this.serializer.Serialize(message, omitXmlDeclaration);
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
        public T Deserialize(XmlTextReader xmlReader)
        {
            return (T)this.serializer.Deserialize(xmlReader);
        }
    }
}