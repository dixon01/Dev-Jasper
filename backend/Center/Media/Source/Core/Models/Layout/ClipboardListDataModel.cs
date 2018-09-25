// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClipboardListDataModel.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Data Model used to copy a list of layout elements to the clipboard.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Layout
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Presentation;

    /// <summary>
    /// Data Model used to copy a list of layout elements to the clipboard.
    /// </summary>
    [Serializable]
    public class ClipboardListDataModel : DataModelBase, IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClipboardListDataModel"/> class.
        /// </summary>
        public ClipboardListDataModel()
        {
            this.Elements = new List<LayoutElementDataModelBase>();
            this.Resources = new List<ResourceInfo>();
        }

        /// <summary>
        /// Gets or sets the elements to be copied to the Clipboard.
        /// </summary>
        public List<LayoutElementDataModelBase> Elements { get; set; }

        /// <summary>
        /// Gets or sets the resource info for the elements.
        /// </summary>
        public List<ResourceInfo> Resources { get; set; }

        /// <summary>
        /// Gets or sets the screen type.
        /// </summary>
        public PhysicalScreenType ScreenType { get; set; }

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Elements.Clear();
            this.Resources.Clear();
            this.ReadXmlAttributes(reader);
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();
            while (reader.NodeType == XmlNodeType.Element)
            {
                if (!this.ReadXmlElement(reader.Name, reader))
                {
                    var item = DataModelSerializer.Deserialize(reader, "Gorba.Center.Media.Core.Models.Layout");
                    var element = item as LayoutElementDataModelBase;
                    if (element != null)
                    {
                        this.Elements.Add(element);
                    }
                    else
                    {
                        var resource = item as ResourceInfo;
                        if (resource != null)
                        {
                            this.Resources.Add(resource);
                        }
                    }
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlAttributes(writer);
            this.WriteXmlElements(writer);
            foreach (var item in this.Elements)
            {
                DataModelSerializer.Serialize(item, writer);
            }

            foreach (var resource in this.Resources)
            {
                DataModelSerializer.Serialize(resource, writer);
            }
        }

        /// <summary>
        /// Reads the Xml attributes.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
            var attribute = reader.GetAttribute("ScreenType");
            if (attribute != null)
            {
                PhysicalScreenType screenType;
                if (Enum.TryParse(attribute, out screenType))
                {
                    this.ScreenType = screenType;
                }
            }
        }

        /// <summary>
        /// Reads the xml elements. This object always returns false.
        /// </summary>
        /// <param name="elementName">
        /// The element name.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// Always false.
        /// </returns>
        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        /// <summary>
        /// Writes Xml attributes.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
            writer.WriteAttributeString("ScreenType", this.ScreenType.ToString());
        }

        /// <summary>
        /// Writes Xml elements.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected virtual void WriteXmlElements(XmlWriter writer)
        {
        }
    }
}
