// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementResponse.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// For internal use only.
    /// The response to a <see cref="ManagementRequest"/>.
    /// </summary>
    public class ManagementResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementResponse"/> class.
        /// </summary>
        public ManagementResponse()
        {
            this.ChildInfos = new List<ChildInfo>();
        }

        /// <summary>
        /// The possible types a child can have.
        /// </summary>
        public enum ChildType
        {
            /// <summary>
            /// Child is an implementation of <see cref="IManagementProvider"/>
            /// </summary>
            ManagementProvider,

            /// <summary>
            /// Child is an implementation of <see cref="IManagementObjectProvider"/>
            /// </summary>
            ManagementObjectProvider,

            /// <summary>
            /// Child is an implementation of <see cref="IManagementTableProvider"/>
            /// </summary>
            ManagementTableProvider
        }

        /// <summary>
        /// Gets or sets the request ID.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Gets or sets the list of properties.
        /// This value is only set if the represented object is an object with properties.
        /// </summary>
        public List<Property> Properties { get; set; }

        /// <summary>
        /// Gets or sets the list of rows.
        /// This value is only set if the represented object is a table.
        /// </summary>
        public List<List<Property>> Rows { get; set; }

        /// <summary>
        /// Gets or sets the list of all children.
        /// </summary>
        public List<ChildInfo> ChildInfos { get; set; }

        /// <summary>
        /// Information about a child.
        /// </summary>
        public class ChildInfo
        {
            /// <summary>
            /// Gets or sets the name of the child.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the type of the child.
            /// </summary>
            public ChildType Type { get; set; }
        }

        /// <summary>
        /// For internal use only.
        /// Serializable management property.
        /// </summary>
        public class Property : IXmlSerializable
        {
            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether this property is read-only.
            /// </summary>
            public bool ReadOnly { get; set; }

            /// <summary>
            /// Gets or sets the value.
            /// </summary>
            public object Value { get; set; }

            XmlSchema IXmlSerializable.GetSchema()
            {
                return null;
            }

            void IXmlSerializable.ReadXml(XmlReader reader)
            {
                reader.MoveToContent();

                var valueType = reader.GetAttribute("valueType");
                if (valueType == null)
                {
                    throw new TypeLoadException("Payload type not defined");
                }

                var type = TypeFactory.Instance[valueType];
                if (type == null)
                {
                    throw new TypeLoadException("Payload type not found: " + valueType);
                }

                reader.ReadStartElement(); // our root element

                this.Name = reader.ReadElementString("Name");
                this.ReadOnly = bool.Parse(reader.ReadElementString("ReadOnly"));

                var parse = GetParseMethod(type);
                if (parse != null)
                {
                    this.Value = parse.Invoke(null, new object[] { reader.ReadElementString("Value") });
                }
                else
                {
                    var ser = new XmlSerializer(type);
                    this.Value = ser.Deserialize(reader);
                }

                reader.ReadEndElement(); // our root element
            }

            void IXmlSerializable.WriteXml(XmlWriter writer)
            {
                var valueType = this.Value.GetType();
                writer.WriteAttributeString("valueType", valueType.FullName);

                writer.WriteElementString("Name", this.Name);
                writer.WriteElementString("ReadOnly", this.ReadOnly.ToString(CultureInfo.InvariantCulture));

                if (GetParseMethod(valueType) != null)
                {
                    writer.WriteElementString("Value", this.Value.ToString());
                }
                else
                {
                    var ser = new XmlSerializer(valueType);
                    ser.Serialize(writer, this.Value);
                }
            }

            private static MethodInfo GetParseMethod(Type type)
            {
                return type.GetMethod(
                    "Parse", BindingFlags.Public | BindingFlags.Static, null, new[] { typeof(string) }, null);
            }
        }
    }
}
