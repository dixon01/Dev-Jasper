// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutWebmediaElement.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LayoutWebmediaElement type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Webmedia
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Layout;

    /// <summary>
    /// A webmedia element that shows multiple (regular) elements in the given frame (or full-screen).
    /// </summary>
    public class LayoutWebmediaElement : WebmediaElementBase, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LayoutWebmediaElement"/> class.
        /// </summary>
        public LayoutWebmediaElement()
        {
            this.Elements = new List<GraphicalElementBase>();
        }

        /// <summary>
        /// Gets or sets the list of elements in this layout.
        /// </summary>
        public List<GraphicalElementBase> Elements { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Elements.Clear();

            this.Name = reader.GetAttribute("Name");

            var frame = reader.GetAttribute("Frame");
            if (frame != null)
            {
                this.Frame = XmlConvert.ToInt32(frame);
            }

            var duration = reader.GetAttribute("Duration");
            if (duration != null)
            {
                this.DurationXml = duration;
            }

            var enabled = reader.GetAttribute("Enabled");
            if (enabled != null)
            {
                this.Enabled = XmlConvert.ToBoolean(enabled);
            }

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();

            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element)
            {
                if (reader.Name == "Enabled")
                {
                    this.EnabledProperty = DynamicProperty.ReadFromXml(reader);
                    reader.MoveToContent();
                    continue;
                }

                var element = ElementSerializer.Deserialize(reader) as GraphicalElementBase;
                if (element != null)
                {
                    this.Elements.Add(element);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            writer.WriteAttributeString("Name", this.Name);
            writer.WriteAttributeString("Frame", XmlConvert.ToString(this.Frame));
            writer.WriteAttributeString("Duration", this.DurationXml);
            writer.WriteAttributeString("Enabled", XmlConvert.ToString(this.Enabled));

            DynamicProperty.WriteToXml("Enabled", this.EnabledProperty, writer);
            foreach (var element in this.Elements)
            {
                ElementSerializer.Serialize(element, writer);
            }
        }
    }
}
