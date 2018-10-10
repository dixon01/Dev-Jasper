// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExplicitSchemaSerializer.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ExplicitSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Generate
{
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Serializer for objects that implement <see cref="IBecSerializable"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object that can be (de)serialized by this class.
    /// </typeparam>
    internal partial class ExplicitSchemaSerializer<T> : GeneratedSchemaSerializerBase<T>
        where T : IBecSerializable, new()
    {
        private readonly BecSchema schema;

        private readonly bool isClass;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplicitSchemaSerializer{T}"/> class.
        /// </summary>
        /// <param name="schema">
        /// The schema of the type.
        /// </param>
        public ExplicitSchemaSerializer(BecSchema schema)
        {
            this.schema = schema;
            this.isClass = schema.TypeName.Type.IsClass;
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
        public override void Serialize(T obj, BecWriter writer, GeneratedSerializationContext context)
        {
            if (this.isClass)
            {
                writer.WriteBool(obj != null);
            }

            if (obj != null)
            {
                obj.WriteBec(writer, this.schema);
            }
        }

        /// <summary>
        /// Serializes the given object to the given stream.
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
        public override T Deserialize(BecReader reader, GeneratedSerializationContext context)
        {
            if (this.isClass && !reader.ReadBool())
            {
                return default(T);
            }

            var obj = new T();
            obj.ReadBec(reader, this.schema);
            return obj;
        }
    }
}
