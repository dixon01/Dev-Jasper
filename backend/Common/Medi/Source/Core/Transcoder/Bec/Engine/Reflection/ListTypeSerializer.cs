// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListTypeSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListTypeSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection
{
    using System;
    using System.Collections;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Serializer for lists (classes implementing <see cref="IList"/>) and arrays.
    /// </summary>
    internal class ListTypeSerializer : ISchemaSerializer
    {
        private const int NullList = -1;

        private readonly ListTypeSchema schema;

        private readonly ISchemaSerializer itemSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListTypeSerializer"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="itemSerializer">
        /// The serializer for the individual items.
        /// </param>
        public ListTypeSerializer(ListTypeSchema schema, ISchemaSerializer itemSerializer)
        {
            this.schema = schema;
            this.itemSerializer = itemSerializer;
        }

        /// <summary>
        /// Serializes a list or array by first writing the count as an int
        /// (or -1 if the list/array is null) followed by the items serialized
        /// by the item serializer given in the constructor.
        /// </summary>
        /// <param name="obj">
        /// The list or array to serialize.
        /// </param>
        /// <param name="writer">
        /// The writer to which the list/array will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public void Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            var list = obj as IList;
            if (list == null)
            {
                writer.WriteInt32(NullList);
                return;
            }

            writer.WriteInt32(list.Count);

            foreach (var item in list)
            {
                this.itemSerializer.Serialize(item, writer, context);
            }
        }

        /// <summary>
        /// Deserializes a list or array by first reading the count as an int
        /// (or -1 if the list/array was null) followed by the items deserialized
        /// by the item serializer given in the constructor.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the list/array will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized list or array or null.
        /// </returns>
        public object Deserialize(BecReader reader, SerializationContext context)
        {
            int count = reader.ReadInt32();

            if (count == NullList)
            {
                return null;
            }

            if (this.schema.TypeName.Type.IsArray)
            {
                var elementType = this.schema.TypeName.Type.GetElementType();
                if (elementType == null)
                {
                    throw new NotSupportedException("Array doesn't have an element type");
                }

                var array = Array.CreateInstance(elementType, count);

                for (int i = 0; i < count; i++)
                {
                    array.SetValue(this.itemSerializer.Deserialize(reader, context), i);
                }

                return array;
            }

            // ok, it's probably a normal IList implementation that supports Add()
            var list = Activator.CreateInstance(this.schema.TypeName.Type) as IList;
            if (list == null)
            {
                throw new NotSupportedException("Expected IList");
            }

            for (int i = 0; i < count; i++)
            {
                list.Add(this.itemSerializer.Deserialize(reader, context));
            }

            return list;
        }
    }
}