// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ArraySchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ArraySchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    /// <summary>
    /// Serializer for arrays.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the array items.
    /// </typeparam>
    internal partial class ArraySchemaSerializer<T> : GeneratedSchemaSerializerBase<T[]>
    {
        private const int NullArray = -1;

        private readonly ISchemaSerializer itemSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArraySchemaSerializer{T}"/> class.
        /// </summary>
        /// <param name="itemSerializer">
        /// The serializer for the individual items.
        /// </param>
        public ArraySchemaSerializer(ISchemaSerializer itemSerializer)
        {
            this.itemSerializer = itemSerializer;
        }

        /// <summary>
        /// Serializes an array by first writing the length as an int
        /// (or -1 if the array is null) followed by the items serialized
        /// by the item serializer given in the constructor.
        /// </summary>
        /// <param name="array">
        /// The array to serialize.
        /// </param>
        /// <param name="writer">
        /// The writer to which the array will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public override void Serialize(T[] array, BecWriter writer, GeneratedSerializationContext context)
        {
            if (array == null)
            {
                writer.WriteInt32(NullArray);
            }
            else
            {
                writer.WriteInt32(array.Length);
                foreach (var item in array)
                {
                    this.itemSerializer.Serialize(item, writer, context.Context);
                }
            }
        }

        /// <summary>
        /// Deserializes an array by first reading the length as an int
        /// (or -1 if the array was null) followed by the items deserialized
        /// by the item serializer given in the constructor.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the array will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized array or null.
        /// </returns>
        public override T[] Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            int length = reader.ReadInt32();
            if (length == NullArray)
            {
                return null;
            }

            var array = new T[length];
            for (int i = 0; i < length; i++)
            {
                array[i] = (T)this.itemSerializer.Deserialize(reader, context.Context);
            }

            return array;
        }
    }
}
