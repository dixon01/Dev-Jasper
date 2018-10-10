// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TypeSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TypeSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using System;

    /// <summary>
    /// Serializer for <see cref="Type"/> objects.
    /// </summary>
    internal partial class TypeSchemaSerializer : GeneratedSchemaSerializerBase<Type>
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
        public override void Serialize(Type obj, BecWriter writer, GeneratedSerializationContext context)
        {
            writer.WriteString(obj == null ? null : obj.FullName);
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
        public override Type Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            string name = reader.ReadString();
            return name == null ? null : TypeFactory.Instance[name];
        }
    }
}
