// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    /// <summary>
    /// Serializer for all types not handled by another serializer.
    /// This class is just a wrapper around <see cref="DefaultTypeSerializer"/>
    /// used because all serializer classes in the Generated namespace have to inherit
    /// from <see cref="GeneratedSchemaSerializerBase{T}"/>.
    /// </summary>
    internal partial class DefaultSchemaSerializer : GeneratedSchemaSerializerBase<object>
    {
        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static readonly DefaultSchemaSerializer Instance = new DefaultSchemaSerializer();

        private readonly DefaultTypeSerializer serializer;

        private DefaultSchemaSerializer()
        {
            this.serializer = DefaultTypeSerializer.Instance;
        }

        /// <summary>
        /// Serializes the object by first writing its schema ID (or 0 if <see cref="obj"/> is null)
        /// and then using the object's type's serializer to serialize the object itself.
        /// </summary>
        /// <param name="obj">
        /// The object to be serialized. Can be null.
        /// </param>
        /// <param name="writer">
        /// The writer to which the object will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public override void Serialize(object obj, BecWriter writer, GeneratedSerializationContext context)
        {
            this.serializer.Serialize(obj, writer, context.Context);
        }

        /// <summary>
        /// Deserializes an object by first reading its schema ID (or 0 if null was serialized)
        /// and then using the object's type's serializer to deserialize the object itself.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the object will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized object or null if the schema ID is 0.
        /// </returns>
        public override object Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            return this.serializer.Deserialize(reader, context.Context);
        }
    }
}