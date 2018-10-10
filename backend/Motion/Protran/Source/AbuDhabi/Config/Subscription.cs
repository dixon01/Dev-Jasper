// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Subscription.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.AbuDhabi.Config
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Motion.Protran.AbuDhabi.Config.DataItems;

    /// <summary>
    /// Representation in XML style of an object Subscription.
    /// </summary>
    [Serializable]
    public class Subscription : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        public Subscription()
        {
            this.Cyclic = -1;
            this.OnChange = string.Empty;
        }

        /// <summary>
        /// Gets or sets the "onChange" attribute that
        /// belongs to this subscription.
        /// </summary>
        public string OnChange { get; set; }
        
        /// <summary>
        /// Gets or sets the "cyclic" attribute that
        /// belongs to this subscription.
        /// </summary>
        public int Cyclic { get; set; }

        /// <summary>
        /// Gets or sets the data items that are requested by this subscription.
        /// </summary>
        public List<DataItemConfig> DataItems { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.DataItems = new List<DataItemConfig>();

            this.OnChange = reader.GetAttribute("OnChange") ?? string.Empty;
            var cyclic = reader.GetAttribute("Cyclic");
            int cyc;
            this.Cyclic = cyclic != null && int.TryParse(cyclic, out cyc) ? cyc : -1;

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            var serializer = new DataItemConfigXmlSerializer();
            while (reader.NodeType == XmlNodeType.Element)
            {
                var dataItem = serializer.Deserialize(reader);
                if (dataItem != null)
                {
                    this.DataItems.Add(dataItem);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
