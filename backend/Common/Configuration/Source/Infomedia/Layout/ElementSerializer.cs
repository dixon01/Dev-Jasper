// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ElementSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Layout
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class used to serialize and deserialize <see cref="ElementBase"/>s.
    /// </summary>
    internal static class ElementSerializer
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(ElementSerializer));

        /// <summary>
        /// Deserializes an element by looking at the XML element name and
        /// finding the right class to deserialize.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// the new config or null if the type can't be found
        /// </returns>
        public static ElementBase Deserialize(XmlReader reader)
        {
            reader.MoveToContent();

            var typeName = string.Format("{0}.{1}Element", typeof(ElementBase).Namespace, reader.Name);
            var type = Type.GetType(typeName, false, false);
            var outerXml = reader.ReadOuterXml();
            if (type == null)
            {
                Logger.Warn("Unknown element type '{0}': {1}", typeName, outerXml);
                return null;
            }

            var ser = new XmlSerializer(type);
            return ser.Deserialize(new StringReader(outerXml)) as ElementBase;
        }

        /// <summary>
        /// Serializes the given element using the writer.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public static void Serialize(ElementBase element, XmlWriter writer)
        {
            var ser = new XmlSerializer(element.GetType());
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            ser.Serialize(writer, element, namespaces);
        }
    }
}