// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GuidSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GuidSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System;

    /// <summary>
    /// Serializer for <see cref="Guid"/> objects.
    /// </summary>
    internal partial class GuidSchemaSerializer : GeneratedSchemaSerializerBase<Guid>
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
        public override void Serialize(Guid obj, BecWriter writer, GeneratedSerializationContext context)
        {
            writer.WriteBytes(obj.ToByteArray());
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
        public override Guid Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            return new Guid(reader.ReadBytes());
        }
    }
}