// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System.Collections;

    /// <summary>
    /// Serializer for lists (classes implementing <see cref="IList"/>).
    /// </summary>
    /// <typeparam name="TList">
    /// The type of the list.
    /// </typeparam>
    /// <typeparam name="TItem">
    /// The type of the items in the list.
    /// </typeparam>
    internal partial class ListSchemaSerializer<TList, TItem> : GeneratedSchemaSerializerBase<TList>
        where TList : class, IList, new()
    {
        private const int NullList = -1;

        private readonly ISchemaSerializer itemSerializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListSchemaSerializer{TList,TItem}"/> class.
        /// </summary>
        /// <param name="itemSerializer">
        /// The serializer for the individual items.
        /// </param>
        public ListSchemaSerializer(ISchemaSerializer itemSerializer)
        {
            this.itemSerializer = itemSerializer;
        }

        /// <summary>
        /// Serializes a list by first writing the count as an int
        /// (or -1 if the list is null) followed by the items serialized
        /// by the item serializer given in the constructor.
        /// </summary>
        /// <param name="list">
        /// The list to serialize.
        /// </param>
        /// <param name="writer">
        /// The writer to which the list will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public override void Serialize(TList list, BecWriter writer, GeneratedSerializationContext context)
        {
            if (list == null)
            {
                writer.WriteInt32(NullList);
                return;
            }

            writer.WriteInt32(list.Count);
            foreach (var item in list)
            {
                this.itemSerializer.Serialize(item, writer, context.Context);
            }
        }

        /// <summary>
        /// Deserializes a list by first reading the count as an int
        /// (or -1 if the list was null) followed by the items deserialized
        /// by the item serializer given in the constructor.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the list will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized list or null.
        /// </returns>
        public override TList Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            int length = reader.ReadInt32();
            if (length == NullList)
            {
                return null;
            }

            var list = new TList();
            for (int i = 0; i < length; i++)
            {
                list.Add((TItem)this.itemSerializer.Deserialize(reader, context.Context));
            }

            return list;
        }
    }
}
