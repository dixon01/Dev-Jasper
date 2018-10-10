// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownListSchemaSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownListSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Schema serializer for an <see cref="IList{T}"/> or array of an unknown type.
    /// </summary>
    internal class UnknownListSchemaSerializer : ISchemaSerializer
    {
        private const int NullList = -1;

        private readonly ListTypeSchema schema;

        private readonly ISchemaSerializer itemSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownListSchemaSerializer"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="itemSerializer">
        /// The serializer for the individual items.
        /// </param>
        public UnknownListSchemaSerializer(ListTypeSchema schema, ISchemaSerializer itemSerializer)
        {
            this.schema = schema;
            this.itemSerializer = itemSerializer;
        }

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
        public void Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            if (obj == null)
            {
                writer.WriteInt32(NullList);
                return;
            }

            var unknown = obj as UnknownBecList;
            if (unknown == null)
            {
                throw new ArgumentException("Can only serialize unknown lists", "obj");
            }

            writer.WriteInt32(unknown.Items.Count);
            foreach (var item in unknown.Items)
            {
                this.itemSerializer.Serialize(item, writer, context);
            }
        }

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
        public object Deserialize(BecReader reader, SerializationContext context)
        {
            int count = reader.ReadInt32();

            if (count == NullList)
            {
                return null;
            }

            var list = new UnknownBecList(this.schema);
            for (int i = 0; i < count; i++)
            {
                list.Items.Add(this.itemSerializer.Deserialize(reader, context));
            }

            return list;
        }
    }
}
