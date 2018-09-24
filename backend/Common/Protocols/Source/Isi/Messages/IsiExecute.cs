// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiExecute.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiExecute type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// ISI protocol message IsiExecute.
    /// </summary>
    public class IsiExecute : IsiMessageBase, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsiExecute"/> class.
        /// </summary>
        public IsiExecute()
        {
            this.Items = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the items of this execute message.
        /// </summary>
        public Dictionary<string, string> Items { get; private set; }

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

            reader.ReadStartElement(); // our root element

            while (reader.NodeType == XmlNodeType.Element)
            {
                var name = reader.Name;
                var value = reader.ReadElementContentAsString();
                this.Items.Add(name, value);
            }

            reader.ReadEndElement(); // our root element
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var item in this.Items)
            {
                writer.WriteElementString(item.Key, item.Value);
            }
        }
    }
}
