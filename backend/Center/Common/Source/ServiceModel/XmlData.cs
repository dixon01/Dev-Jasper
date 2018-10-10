// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlData.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlData type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    /// <summary>
    /// Structure representing XML serialized data in the database.
    /// </summary>
    [DataContract]
    public struct XmlData
    {
        /// <summary>
        /// The XML structure as a string.
        /// </summary>
        [DataMember]
        public readonly string Xml;

        /// <summary>
        /// The qualified type name of the serialized data (type name + assembly name).
        /// </summary>
        [DataMember]
        public readonly string Type;

        private object deserialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlData"/> struct.
        /// </summary>
        /// <param name="xml">
        /// The xml structure as a string.
        /// </param>
        /// <param name="qualifiedTypeName">
        /// The type qualified type name of the serialized data (type name + assembly name).
        /// </param>
        public XmlData(string xml, string qualifiedTypeName)
        {
            this.Xml = xml;
            this.Type = qualifiedTypeName;
            this.deserialized = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlData"/> struct.
        /// </summary>
        /// <param name="data">
        /// The data object to be serialized.
        /// </param>
        public XmlData(object data)
        {
            this.deserialized = data;
            if (data == null)
            {
                this.Xml = null;
                this.Type = null;
                return;
            }

            var type = data.GetType();
            var serializer = new XmlSerializer(type);
            var writer = new StringWriter();
            serializer.Serialize(writer, data);
            this.Xml = writer.ToString();
            this.Type = GetTypeName(type);
        }

        /// <summary>
        /// The equality operator.
        /// </summary>
        /// <param name="left">
        /// The left operand.
        /// </param>
        /// <param name="right">
        /// The right operand.
        /// </param>
        /// <returns>
        /// true if the two objects are equal.
        /// </returns>
        public static bool operator ==(XmlData left, XmlData right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// The inequality operator.
        /// </summary>
        /// <param name="left">
        /// The left operand.
        /// </param>
        /// <param name="right">
        /// The right operand.
        /// </param>
        /// <returns>
        /// true if the two objects are not equal.
        /// </returns>
        public static bool operator !=(XmlData left, XmlData right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Gets the type name to be used for <see cref="Type"/> for the given type.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The type name including full name and assembly name (but no assembly version).
        /// </returns>
        public static string GetTypeName(Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        /// <summary>
        /// Deserializes the the data contained in <see cref="Xml"/>.
        /// </summary>
        /// <returns>
        /// The deserialized <see cref="object"/>.
        /// </returns>
        public object Deserialize()
        {
            if (this.Xml == null)
            {
                return null;
            }

            if (this.deserialized != null)
            {
                return this.deserialized;
            }

            var type = System.Type.GetType(this.Type);
            Debug.Assert(type != null, "Type can't be null");
            var serializer = new XmlSerializer(type);
            return this.deserialized = serializer.Deserialize(new StringReader(this.Xml));
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type
        /// and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">The object to compare with the current instance. </param>
        public override bool Equals(object obj)
        {
            if (!(obj is XmlData))
            {
                return false;
            }

            var other = (XmlData)obj;
            return other.Xml == this.Xml && other.Type == this.Type;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Xml == null ? 42 : this.Xml.GetHashCode();
        }
    }
}
