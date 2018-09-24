// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaMemberSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaMemberSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection
{
    using System.Reflection;

    /// <summary>
    /// Serializer for members of a <see cref="Gorba.Common.Medi.Core.Transcoder.Bec.Schema.BecSchema"/>.
    /// </summary>
    internal class SchemaMemberSerializer
    {
        private readonly Getter getter;
        private readonly Setter setter;

        private readonly ISchemaSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaMemberSerializer"/> class
        /// for a non-existant member.
        /// </summary>
        /// <param name="serializer">
        /// The serializer for the type of the member.
        /// </param>
        public SchemaMemberSerializer(ISchemaSerializer serializer)
            : this(p => null, (p, v) => { }, serializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaMemberSerializer"/> class
        /// for a field.
        /// </summary>
        /// <param name="field">
        /// The field which will be read/written.
        /// </param>
        /// <param name="serializer">
        /// The serializer for the type of the field.
        /// </param>
        public SchemaMemberSerializer(FieldInfo field, ISchemaSerializer serializer)
            : this(field.GetValue, field.SetValue, serializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaMemberSerializer"/> class
        /// for a property.
        /// </summary>
        /// <param name="property">
        /// The property which will be read/written.
        /// </param>
        /// <param name="serializer">
        /// The serializer for the type of the property.
        /// </param>
        public SchemaMemberSerializer(PropertyInfo property, ISchemaSerializer serializer)
            : this(p => property.GetValue(p, null), (p, v) => property.SetValue(p, v, null), serializer)
        {
        }

        private SchemaMemberSerializer(Getter getter, Setter setter, ISchemaSerializer serializer)
        {
            this.getter = getter;
            this.setter = setter;
            this.serializer = serializer;
        }

        /// <summary>
        /// Delegate to get a member value from its parent.
        /// </summary>
        /// <param name="parent">
        /// The parent object.
        /// </param>
        /// <returns>
        /// The value of the member.
        /// </returns>
        private delegate object Getter(object parent);

        /// <summary>
        /// Delegate to set a member value to its parent.
        /// </summary>
        /// <param name="parent">
        /// The parent object.
        /// </param>
        /// <param name="value">
        /// The value to be set to the member.
        /// </param>
        private delegate void Setter(object parent, object value);

        /// <summary>
        /// Serializes the member of the given object to the given stream.
        /// </summary>
        /// <param name="parent">
        /// The parent of the member to be serialized.
        /// </param>
        /// <param name="writer">
        /// The writer to which the member will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public void Serialize(object parent, BecWriter writer, SerializationContext context)
        {
            this.serializer.Serialize(this.getter(parent), writer, context);
        }

        /// <summary>
        /// Deserializes the member of the given object from the given stream.
        /// </summary>
        /// <param name="parent">
        /// The parent of the member to be deserialized.
        /// </param>
        /// <param name="reader">
        /// The reader from which the member will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public void Deserialize(object parent, BecReader reader, SerializationContext context)
        {
            this.setter(parent, this.serializer.Deserialize(reader, context));
        }
    }
}
