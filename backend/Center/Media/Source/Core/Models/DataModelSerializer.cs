// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataModelSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ElementSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Models
{
    using System;
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class used to serialize and deserialize
    /// <see cref="DataModelSerializer"/>s.
    /// </summary>
    internal static class DataModelSerializer
    {
        private static readonly Logger Logger = LogHelper.GetLogger(typeof(DataModelSerializer));

        /// <summary>
        /// Deserializes an element by looking at the XML element name and
        /// finding the right class to deserialize.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        /// <param name="modelNamespace">
        /// The namespace of the class.</param>
        /// <returns>
        /// the new config or null if the type can't be found
        /// </returns>
        public static DataModelBase Deserialize(XmlReader reader, string modelNamespace)
        {
            reader.MoveToContent();
            string typeName;
            if (modelNamespace.Contains("Layout"))
            {
                if (reader.Name == "ResourceInfo")
                {
                    typeName = string.Format("{0}.{1}", modelNamespace.Replace(".Layout", string.Empty), reader.Name);
                }
                else if (reader.Name != "Font")
                {
                    typeName = string.Format("{0}.{1}ElementDataModel", modelNamespace, reader.Name);
                }
                else
                {
                    typeName = string.Format("{0}.{1}DataModel", modelNamespace, reader.Name);
                }
            }
            else if (modelNamespace.Contains("Eval"))
            {
                typeName = string.Format("{0}.{1}EvalDataModel", modelNamespace, reader.Name);
            }
            else if (modelNamespace.EndsWith("Models"))
            {
                typeName = string.Format("{0}.{1}", modelNamespace, reader.Name);
            }
            else
            {
                typeName = string.Format(
                    "{0}.{1}ConfigDataModel",
                    modelNamespace,
                    reader.Name);
            }

            var type = Type.GetType(typeName, false, true);
            var outerXml = reader.ReadOuterXml();
            if (type == null)
            {
                Logger.Warn("Unknown element type '{0}': {1}", typeName, outerXml);
                return null;
            }

            var ser = new XmlSerializer(type);
            return
                ser.Deserialize(new StringReader(outerXml)) as DataModelBase;
        }

        /// <summary>
        /// Serializes the given element using the writer.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public static void Serialize(DataModelBase element, XmlWriter writer)
        {
            var ser = new XmlSerializer(element.GetType());
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            ser.Serialize(writer, element, namespaces);
        }

        /// <summary>
        /// Serializes the given resource info using the writer.
        /// </summary>
        /// <param name="resourceInfo">
        /// The resource info.
        /// </param>
        /// <param name="writer">
        /// The writer.
        /// </param>
        public static void Serialize(ResourceInfo resourceInfo, XmlWriter writer)
        {
            var ser = new XmlSerializer(typeof(ResourceInfo));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            ser.Serialize(writer, resourceInfo, namespaces);
        }
    }
}