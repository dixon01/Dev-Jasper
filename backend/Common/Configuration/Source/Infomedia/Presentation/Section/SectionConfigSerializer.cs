// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SectionConfigSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SectionConfigSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation.Section
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Serializer for <see cref="SectionConfigBase"/>.
    /// </summary>
    public static class SectionConfigSerializer
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(SectionConfigSerializer));

        /// <summary>
        /// Deserializes a section by looking at the XML element name and
        /// finding the right class to deserialize.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// the new config or null if the type can't be found
        /// </returns>
        public static SectionConfigBase Deserialize(XmlReader reader)
        {
            var typeName = string.Format("{0}.{1}Config", typeof(SectionConfigBase).Namespace, reader.Name);
            var type = Type.GetType(typeName, false, false);
            var outerXml = reader.ReadOuterXml();
            if (type == null)
            {
                Logger.Warn("Unknown section in cycle configuration '{0}': {1}", typeName, outerXml);
                return null;
            }

            var ser = new XmlSerializer(type);
            return ser.Deserialize(new StringReader(outerXml)) as SectionConfigBase;
        }

        /// <summary>
        /// Serializes the given section using the writer.
        /// </summary>
        /// <param name="section">
        /// The section.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public static void Serialize(SectionConfigBase section, XmlWriter writer)
        {
            var ser = new XmlSerializer(section.GetType());
            ser.Serialize(writer, section);
        }
    }
}