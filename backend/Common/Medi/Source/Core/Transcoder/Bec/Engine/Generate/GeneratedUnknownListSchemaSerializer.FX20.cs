// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneratedUnknownListSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GeneratedUnknownListSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Wrapper for <see cref="UnknownListSchemaSerializer"/> used in this namespace.
    /// </summary>
    internal partial class GeneratedUnknownListSchemaSerializer : GeneratedSchemaSerializerBase, ISchemaSerializer
    {
        private readonly UnknownListSchemaSerializer serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneratedUnknownListSchemaSerializer"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema.
        /// </param>
        /// <param name="itemSerializer">
        /// The serializer for the individual items.
        /// </param>
        public GeneratedUnknownListSchemaSerializer(ListTypeSchema schema, GeneratedSchemaSerializerBase itemSerializer)
        {
            this.serializer = new UnknownListSchemaSerializer(schema, itemSerializer);
        }

        void ISchemaSerializer.Serialize(object obj, BecWriter writer, SerializationContext context)
        {
            this.serializer.Serialize(obj, writer, context);
        }

        object ISchemaSerializer.Deserialize(BecReader reader, SerializationContext context)
        {
            return this.serializer.Deserialize(reader, context);
        }
    }
}