// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AudioOutputElementDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The audio output element data model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    /// <summary>
    /// The audio output element data model.
    /// </summary>
    public partial class AudioOutputElementDataModel : IXmlSerializable, IEnumerable<LayoutElementDataModelBase>
    {
        /// <summary>
        /// Gets or sets the elements.
        /// </summary>
        public List<PlaybackElementDataModelBase> Elements { get; set; }

        IEnumerator<LayoutElementDataModelBase> IEnumerable<LayoutElementDataModelBase>.GetEnumerator()
        {
            return this.Elements.ConvertAll(e => (LayoutElementDataModelBase)e).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.Elements.GetEnumerator();
        }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Elements.Clear();

            var attrEnabled = reader.GetAttribute("Enabled");
            if (attrEnabled != null)
            {
                this.Enabled = XmlConvert.ToBoolean(attrEnabled);
            }

            var attrVolume = reader.GetAttribute("Volume");
            if (attrVolume != null)
            {
                this.Volume = XmlConvert.ToInt32(attrVolume);
            }

            var attrPriority = reader.GetAttribute("Priority");
            if (attrPriority != null)
            {
                this.Priority = XmlConvert.ToInt32(attrPriority);
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
                    this.EnabledProperty = DynamicPropertyDataModel.ReadFromXml(reader);
                    reader.MoveToContent();
                    continue;
                }

                if (reader.Name == "Volume")
                {
                    this.VolumeProperty = DynamicPropertyDataModel.ReadFromXml(reader);
                    reader.MoveToContent();
                    continue;
                }

                var element =
                    DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Layout") as
                        PlaybackElementDataModelBase;
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
            if (this.Enabled != true)
            {
                writer.WriteAttributeString("Enabled", XmlConvert.ToString(this.Enabled));
            }

            writer.WriteAttributeString("Volume", XmlConvert.ToString(this.Volume));

            writer.WriteAttributeString("Priority", XmlConvert.ToString(this.Priority));

            DynamicPropertyDataModel.WriteToXml("Enabled", this.EnabledProperty, writer);

            DynamicPropertyDataModel.WriteToXml("Volume", this.VolumeProperty, writer);

            foreach (var element in this.Elements)
            {
                DataModelSerializer.Serialize(element, writer);
            }
        }

        partial void AdditionalInitialization()
        {
            this.Elements = new List<PlaybackElementDataModelBase>();
        }
    }
}
