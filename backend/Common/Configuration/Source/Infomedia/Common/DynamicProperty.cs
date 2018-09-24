// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicProperty.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicProperty type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Common
{
    using System;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Dynamic property that is evaluated at run-time.
    /// </summary>
    [Serializable]
    public class DynamicProperty : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProperty"/> class.
        /// </summary>
        public DynamicProperty()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicProperty"/> class.
        /// </summary>
        /// <param name="evaluation">
        /// The evaluation to be set for <see cref="Evaluation"/>.
        /// </param>
        public DynamicProperty(EvalBase evaluation)
        {
            this.Evaluation = evaluation;
        }

        /// <summary>
        /// Gets or sets the evaluation configuration.
        /// </summary>
        public EvalBase Evaluation { get; set; }

        /// <summary>
        /// This method is reserved and should not be used. When implementing the
        /// IXmlSerializable interface, you should return null from this method,
        /// and instead, if specifying a custom schema is required, apply the
        /// <see cref="T:System.Xml.Serialization.XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Xml.Schema.XmlSchema"/> that describes the XML
        /// representation of the object that is produced by the
        /// <see cref="M:System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter)"/>
        /// method and consumed by the
        /// <see cref="M:System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream
        /// from which the object is deserialized. </param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.ReadXmlAttributes(reader);

            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();
            reader.MoveToContent();

            while (reader.NodeType == XmlNodeType.Element || reader.NodeType == XmlNodeType.Text)
            {
                if (!this.ReadXmlElement(reader.Name, reader))
                {
                    this.Evaluation = EvalSerializer.Deserialize(reader);
                }

                reader.MoveToContent();
            }

            reader.MoveToContent();
            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream
        /// to which the object is serialized. </param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            this.WriteXmlAttributes(writer);
            this.WriteXmlElements(writer);
            if (this.Evaluation != null)
            {
                EvalSerializer.Serialize(this.Evaluation, writer);
            }
        }

        /// <summary>
        /// Reads a <see cref="DynamicProperty"/> from the given XML reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A new <see cref="DynamicProperty"/> deserialized from the given reader.
        /// </returns>
        internal static DynamicProperty ReadFromXml(XmlReader reader)
        {
            var property = new DynamicProperty();
            ((IXmlSerializable)property).ReadXml(reader);
            return property;
        }

        /// <summary>
        /// Writes the given <see cref="DynamicProperty"/> to the given writer
        /// using the given XML element name. If the <see cref="property"/> is
        /// null, nothing is written.
        /// </summary>
        /// <param name="elementName">
        /// The XML element name.
        /// </param>
        /// <param name="property">
        /// The property.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal static void WriteToXml(string elementName, DynamicProperty property, XmlWriter writer)
        {
            if (property == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            var serializable = (IXmlSerializable)property;
            serializable.WriteXml(writer);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Reads all XML attributes when de-serializing.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        protected virtual void ReadXmlAttributes(XmlReader reader)
        {
        }

        /// <summary>
        /// Reads the given element when de-serializing.
        /// </summary>
        /// <param name="elementName">
        /// The element name.
        /// </param>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// True if the element has been handled.
        /// </returns>
        protected virtual bool ReadXmlElement(string elementName, XmlReader reader)
        {
            return false;
        }

        /// <summary>
        /// Writes all XML attributes when serializing.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected virtual void WriteXmlAttributes(XmlWriter writer)
        {
        }

        /// <summary>
        /// Writes all XML elements when serializing.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected virtual void WriteXmlElements(XmlWriter writer)
        {
        }
    }
}
