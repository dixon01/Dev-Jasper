// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IsiPut.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IsiPut type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Isi.Messages
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// ISI protocol message IsiPut.
    /// </summary>
    public class IsiPut : IsiMessageBase, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IsiPut"/> class.
        /// <see cref="IsiId"/> is set to -1.
        /// </summary>
        public IsiPut()
        {
            this.IsiId = -1;

            this.Items = new List<DataItem>();
        }

        /// <summary>
        /// Gets or sets the IsiId of this message.
        /// By default the value is -1.
        /// </summary>
        [DefaultValue(-1)]
        public int IsiId { get; set; }

        /// <summary>
        /// Gets the items of this put message.
        /// </summary>
        public List<DataItem> Items { get; private set; }

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

            if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals("IsiId"))
            {
                this.IsiId = reader.ReadElementContentAsInt();
            }

            while (reader.NodeType == XmlNodeType.Element)
            {
                var name = reader.Name;
                var value = reader.ReadElementContentAsString();

                this.Items.Add(new DataItem { Name = name, Value = value });
            }

            reader.ReadEndElement(); // our root element
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            if (this.IsiId != -1)
            {
                writer.WriteElementString("IsiId", this.IsiId.ToString(CultureInfo.InvariantCulture));
            }

            foreach (var item in this.Items)
            {
                writer.WriteElementString(item.Name, item.Value);
            }
        }
    }
}
