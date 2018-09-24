// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedSchemaSerializerBase{T}.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GeneratedSchemaSerializerBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    /// <summary>
    /// This class is not meant for use outside this assembly.
    /// Base class for all serializer classes in the Generated namespace.
    /// Provides two type-safe methods for serialization and deserialization
    /// and implements <see cref="ISchemaSerializer"/> by casting to <see cref="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object that can be (de)serialized by this class.
    /// </typeparam>
    public abstract partial class GeneratedSchemaSerializerBase<T> : GeneratedSchemaSerializerBase, ISchemaSerializer
    {
        /// <summary>
        /// Serializes the given object to the given stream.
        /// </summary>
        /// <param name="obj">
        /// The object to be serialized.
        /// </param>
        /// <param name="writer">
        /// The writer to which the object will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public abstract void Serialize(T obj, BecWriter writer, GeneratedSerializationContext context);

        /// <summary>
        /// Deserializes the given object from the given stream.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the object will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public abstract T Deserialize(BecReader reader, GeneratedSerializationContext context);

        void ISchemaSerializer.Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            this.Serialize((T)obj, writer, new GeneratedSerializationContext(context));
        }

        object ISchemaSerializer.Deserialize(BecReader reader, SerializationContext context)
        {
            return this.Deserialize(reader, new GeneratedSerializationContext(context));
        }
    }
}
