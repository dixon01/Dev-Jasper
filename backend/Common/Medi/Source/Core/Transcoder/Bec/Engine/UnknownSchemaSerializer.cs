// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnknownSchemaSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnknownSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// A schema serializer for an unknown type (i.e. a type that
    /// does not exist on this node, but does exist on the source and the 
    /// destination of the message).
    /// </summary>
    internal class UnknownSchemaSerializer : ISchemaSerializer
    {
        private readonly BecSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnknownSchemaSerializer"/> class.
        /// </summary>
        /// <param name="schema">
        /// The BEC schema.
        /// </param>
        public UnknownSchemaSerializer(BecSchema schema)
        {
            this.schema = schema;
            this.Members = new List<UnknownSchemaMemberSerializer>();
        }

        /// <summary>
        /// Gets a list of all members of the unknown type.
        /// </summary>
        public List<UnknownSchemaMemberSerializer> Members { get; private set; }

        /// <summary>
        /// Serializes the given object to the given stream.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="UnknownBecObject"/> to be serialized.
        /// </param>
        /// <param name="writer">
        /// The writer to which the object will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public void Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            var unknown = obj as UnknownBecObject;
            if (unknown == null)
            {
                throw new ArgumentException("Can only serialize unknown objects", "obj");
            }

            foreach (var member in this.Members)
            {
                member.Serializer.Serialize(unknown[member.Name], writer, context);
            }
        }

        /// <summary>
        /// Deserializes an object from the given stream.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the object will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized placeholder object (an <see cref="UnknownBecObject"/>).
        /// </returns>
        public object Deserialize(BecReader reader, SerializationContext context)
        {
            var obj = new UnknownBecObject(this.schema);
            foreach (var member in this.Members)
            {
                obj[member.Name] = member.Serializer.Deserialize(reader, context);
            }

            return obj;
        }
    }
}