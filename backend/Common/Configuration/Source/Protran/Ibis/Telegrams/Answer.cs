// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Answer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Answer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.ComponentModel;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// Container of all the information to send an
    /// answer to the IBIS master.
    /// </summary>
    [Serializable]
    public class Answer : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Answer"/> class.
        /// </summary>
        public Answer()
        {
            this.Telegram = null;
        }

        /// <summary>
        /// Gets or sets the XML element called Symbol.
        /// </summary>
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public TelegramConfig Telegram { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            if (reader.NodeType == XmlNodeType.Element)
            {
                var serializer = new TelegramConfigXmlSerializer();
                this.Telegram = serializer.Deserialize(reader);
            }

            reader.MoveToContent();
            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.Telegram == null)
            {
                return;
            }

            var serializer = new TelegramConfigXmlSerializer();
            serializer.Serialize(writer, this.Telegram);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return this.Telegram == null ? string.Empty : this.Telegram.Name;
        }
    }
}
