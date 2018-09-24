// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultTypeSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    /// <summary>
    /// Serializer for all types not handled by another serializer.
    /// This serializer uses an int to define the type that is following
    /// and then uses that type's serializer to (de)serialize the object.
    /// A type ID of 0 means null.
    /// </summary>
    internal class DefaultTypeSerializer : ISchemaSerializer
    {
        private static DefaultTypeSerializer instance = new DefaultTypeSerializer();

        private DefaultTypeSerializer()
        {
        }

        /// <summary>
        /// Gets the single instance of this class.
        /// </summary>
        public static DefaultTypeSerializer Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Serializes the object by first writing its schema ID (or 0 if obj is null)
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
        public void Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            if (obj == null)
            {
                writer.WriteInt32(0);
                return;
            }

            var schemaInfo = context.Engine.GetSchemaInfo(obj, context);
            writer.WriteInt32(schemaInfo.Id);
            schemaInfo.Serializer.Serialize(obj, writer, context);
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
        public object Deserialize(BecReader reader, SerializationContext context)
        {
            int id = reader.ReadInt32();
            if (id == 0)
            {
                return null;
            }

            var schemaInfo = context.SchemaMapper[id];
            return schemaInfo.Serializer.Deserialize(reader, context);
        }
    }
}