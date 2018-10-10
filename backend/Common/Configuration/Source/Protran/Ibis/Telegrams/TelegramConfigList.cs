// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramConfigList.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramConfigList type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Protran.Ibis.Telegrams
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// List of TelegramConfig that can handle subclasses of TelegramConfig
    /// using the root tag.
    /// </summary>
    [Serializable]
    public class TelegramConfigList : List<TelegramConfig>, IXmlSerializable
    {
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Clear();

            reader.MoveToContent();

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            var serializer = new TelegramConfigXmlSerializer();

            while (reader.NodeType == XmlNodeType.Element)
            {
                var telegram = serializer.Deserialize(reader);
                if (telegram != null)
                {
                    this.Add(telegram);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            var serializer = new TelegramConfigXmlSerializer();
            foreach (var telegram in this)
            {
                serializer.Serialize(writer, telegram);
            }
        }
    }
}
