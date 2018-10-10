// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramConfigXmlSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramConfigXmlSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// Serializer class for TelegramConfig objects which allows
    /// the root tag to be different than the actual class.
    /// Like this we can use tags like &lt;DS001&gt; which still use the
    /// <see cref="TelegramConfig"/> base configuration class.
    /// </summary>
    internal class TelegramConfigXmlSerializer
    {
        /// <summary>
        /// Deserializes a <see cref="TelegramConfig"/> from an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A config object or null if the deserialized object didn't implement TelegramConfig.
        /// </returns>
        public TelegramConfig Deserialize(XmlReader reader)
        {
            var name = reader.Name;

            var typeName = string.Format("{0}.{1}Config", this.GetType().Namespace, reader.Name);
            var type = Type.GetType(typeName, false, true) ?? typeof(SimpleTelegramConfig);
            var ser = new XmlSerializer(type);

            var outerXml = reader.ReadOuterXml();

            var regex = new Regex(string.Format(@"(</?){0}(\W)", name), RegexOptions.IgnoreCase);
            outerXml = regex.Replace(outerXml, string.Format("$1{0}$2", type.Name));

            var telegram = ser.Deserialize(new StringReader(outerXml)) as TelegramConfig;
            if (telegram == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(telegram.Name))
            {
                telegram.Name = name;
            }

            return telegram;
        }

        /// <summary>
        /// Serializes a <see cref="TelegramConfig"/> to an <see cref="XmlWriter"/>.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        /// <param name="telegram">
        /// The telegram config object to be serialized.
        /// </param>
        public void Serialize(XmlWriter writer, TelegramConfig telegram)
        {
            var type = telegram.GetType();
            var ser = new XmlSerializer(type);
            var output = new StringWriter();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false };
            var xml = XmlWriter.Create(output, settings);
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            ser.Serialize(xml, telegram, namespaces);

            var regex = new Regex(string.Format(@"(</?){0}(\W)", type.Name), RegexOptions.IgnoreCase);
            var xmlString = regex.Replace(output.ToString(), string.Format("$1{0}$2", telegram.Name));

            writer.WriteNode(XmlReader.Create(new StringReader(xmlString)), false);
        }
    }
}