// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTriggerConfigDataModel.partial.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models.Presentation.Cycle
{
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models.Eval;

    /// <summary>
    /// The generic trigger config data model.
    /// </summary>
    public partial class GenericTriggerConfigDataModel : IXmlSerializable
    {
        /// <summary>
        /// This method is reserved and should not be used. When implementing the IXmlSerializable interface,
        /// you should return null (Nothing in Visual Basic) from this method, and instead,
        /// if specifying a custom schema is required, apply the <see cref="XmlSchemaProviderAttribute"/> to the class.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlSchema"/> that describes the XML representation of the object that is produced
        /// by the <see cref="IXmlSerializable.WriteXml(XmlWriter)"/> method and consumed by the
        /// <see cref="IXmlSerializable.ReadXml(XmlReader)"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The <see cref="T:System.Xml.XmlReader"/> stream from which the object is deserialized.
        /// </param>
        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            this.Coordinates.Clear();
            if (reader.IsEmptyElement)
            {
                reader.ReadStartElement();
                return;
            }

            reader.ReadStartElement();

            reader.MoveToContent();
            const string ModelNameSpace = "Gorba.Center.Media.Core.Models.Eval";
            while (reader.NodeType == XmlNodeType.Element)
            {
                var coordinate = DataModelSerializer.Deserialize(reader, ModelNameSpace) as GenericEvalDataModel;
                if (coordinate != null)
                {
                    this.Coordinates.Add(coordinate);
                }

                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The <see cref="T:System.Xml.XmlWriter"/> stream to which the object is serialized.
        /// </param>
        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var coordinate in this.Coordinates)
            {
                DataModelSerializer.Serialize(coordinate, writer);
            }
        }

        /// <summary>
        /// Reads a <see cref="GenericTriggerConfigDataModel"/> from the given XML reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A new <see cref="GenericTriggerConfigDataModel"/> deserialized from the given reader.
        /// </returns>
        internal static GenericTriggerConfigDataModel ReadFromXml(XmlReader reader)
        {
            var trigger = new GenericTriggerConfigDataModel();
            ((IXmlSerializable)trigger).ReadXml(reader);
            return trigger;
        }

        /// <summary>
        /// Writes the given trigger to the given writer
        /// using the given XML element name. If the <see cref="trigger"/> is
        /// null, nothing is written.
        /// </summary>
        /// <param name="elementName">
        /// The XML element name.
        /// </param>
        /// <param name="trigger">
        /// The trigger.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal static void WriteToXml(string elementName, GenericTriggerConfigDataModel trigger, XmlWriter writer)
        {
            if (trigger == null)
            {
                return;
            }

            var serializable = (IXmlSerializable)trigger;
            writer.WriteStartElement(elementName);
            serializable.WriteXml(writer);
            writer.WriteEndElement();
        }
    }
}
