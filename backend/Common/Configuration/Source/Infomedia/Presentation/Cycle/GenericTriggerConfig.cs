// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericTriggerConfig.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GenericTrigger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Presentation.Cycle
{
    using System.Collections.Generic;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Infomedia.Common;
    using Gorba.Common.Configuration.Infomedia.Eval;

    /// <summary>
    /// Trigger that listens on coordinates.
    /// This class is used for event cycles.
    /// </summary>
    public partial class GenericTriggerConfig : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericTriggerConfig"/> class.
        /// </summary>
        /// <param name="evals">
        /// The evaluations.
        /// </param>
        public GenericTriggerConfig(params GenericEval[] evals)
        {
            this.Coordinates = new List<GenericEval>(evals);
        }

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

            while (reader.NodeType == XmlNodeType.Element)
            {
                var coordinate = EvalSerializer.Deserialize(reader) as GenericEval;
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
                EvalSerializer.Serialize(coordinate, writer);
            }
        }

        /// <summary>
        /// Reads a <see cref="GenericTriggerConfig"/> from the given XML reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// A new <see cref="GenericTriggerConfig"/> deserialized from the given reader.
        /// </returns>
        internal static GenericTriggerConfig ReadFromXml(XmlReader reader)
        {
            var trigger = new GenericTriggerConfig();
            ((IXmlSerializable)trigger).ReadXml(reader);
            return trigger;
        }

        /// <summary>
        /// Writes the given <see cref="DynamicProperty"/> to the given writer
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
        internal static void WriteToXml(string elementName, GenericTriggerConfig trigger, XmlWriter writer)
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