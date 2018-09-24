// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISchemaSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    /// <summary>
    /// Interface to be implemented by all schema serializers.
    /// </summary>
    internal interface ISchemaSerializer
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
        void Serialize(object obj, BecWriter writer, SerializationContext context);

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
        object Deserialize(BecReader reader, SerializationContext context);
    }
}