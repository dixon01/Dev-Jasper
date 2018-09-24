// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System;

    /// <summary>
    /// Serializer for <see cref="DateTime"/> objects.
    /// </summary>
    internal partial class DateTimeSchemaSerializer : GeneratedSchemaSerializerBase<DateTime>
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
        public override void Serialize(DateTime obj, BecWriter writer, GeneratedSerializationContext context)
        {
            writer.WriteInt64(obj.Ticks);
            writer.WriteByte((byte)obj.Kind);
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
        public override DateTime Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            var ticks = reader.ReadInt64();
            var kind = reader.ReadByte();
            return new DateTime(ticks, (DateTimeKind)kind);
        }
    }
}