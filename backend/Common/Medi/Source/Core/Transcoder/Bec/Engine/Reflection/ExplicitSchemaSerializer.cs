// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplicitSchemaSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExplicitSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection
{
    using System;

    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Serializer for objects that implement <see cref="IBecSerializable"/>.
    /// </summary>
    internal class ExplicitSchemaSerializer : ISchemaSerializer
    {
        private readonly BecSchema schema;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplicitSchemaSerializer"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        public ExplicitSchemaSerializer(BecSchema schema)
        {
            this.schema = schema;
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
            var serializable = obj as IBecSerializable;

            writer.WriteBool(serializable != null);

            if (serializable == null)
            {
                return;
            }

            serializable.WriteBec(writer, this.schema);
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
            if (!reader.ReadBool())
            {
                return null;
            }

            var serializable = Activator.CreateInstance(this.schema.TypeName.Type) as IBecSerializable;

            if (serializable != null)
            {
                serializable.ReadBec(reader, this.schema);
            }

            return serializable;
        }
    }
}