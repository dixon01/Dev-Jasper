// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EvalSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EvalSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Infomedia.Eval
{
    using System;
    using System.Diagnostics;
    using System.Xml;
    using System.Xml.Serialization;

    /// <summary>
    /// XML serializer helper for <see cref="EvalBase"/> objects.
    /// </summary>
    internal static class EvalSerializer
    {
        /// <summary>
        /// Deserializes a config by looking at the XML element name and
        /// finding the right class to deserialize.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <returns>
        /// the new config. Never null.
        /// </returns>
        public static EvalBase Deserialize(XmlReader reader)
        {
            reader.MoveToContent();

            if (reader.NodeType == XmlNodeType.Text)
            {
                return new ConstantEval { Value = reader.ReadString().Trim() };
            }

            var typeName = string.Format("{0}.{1}Eval", typeof(EvalBase).Namespace, reader.Name);
            var type = Type.GetType(typeName, true, false);
            Debug.Assert(type != null, "Type should never be null");

            var ser = new XmlSerializer(type);
            return (EvalBase)ser.Deserialize(reader);
        }

        /// <summary>
        /// Serializes a config using the given writer.
        /// </summary>
        /// <param name="eval">
        /// The config.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public static void Serialize(EvalBase eval, XmlWriter writer)
        {
            var constant = eval as ConstantEval;
            if (constant != null)
            {
                writer.WriteString(constant.Value);
                return;
            }

            var ser = new XmlSerializer(eval.GetType());
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            ser.Serialize(writer, eval, namespaces);
        }
    }
}