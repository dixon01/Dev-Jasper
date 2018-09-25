// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BecSchemaSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BecSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Serializer for normal types that have a <see cref="BecSchema"/>.
    /// </summary>
    internal class BecSchemaSerializer : ISchemaSerializer
    {
        private readonly BecSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="BecSchemaSerializer"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        public BecSchemaSerializer(BecSchema schema)
        {
            this.Members = new List<SchemaMemberSerializer>();
            this.schema = schema;
        }

        /// <summary>
        /// Gets the list of member serializer classes.
        /// </summary>
        public List<SchemaMemberSerializer> Members { get; private set; }

        /// <summary>
        /// Serializes the given object to the given stream by
        /// serializing each member to the stream.
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
            foreach (var member in this.Members)
            {
                member.Serialize(obj, writer, context);
            }
        }

        /// <summary>
        /// Deserializes the given object from the given stream
        /// de-serializing each member from the stream.
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
            var obj = Activator.CreateInstance(this.schema.TypeName.Type);
            foreach (var member in this.Members)
            {
                member.Deserialize(obj, reader, context);
            }

            return obj;
        }
    }
}