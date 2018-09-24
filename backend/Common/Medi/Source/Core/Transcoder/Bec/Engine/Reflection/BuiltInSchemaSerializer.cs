// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuiltInSchemaSerializer.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BuiltInSchemaSerializer type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine.Reflection
{
    using System;

    /// <summary>
    /// Serializer for built-in types (primitives, <see cref="string"/> and <see cref="Type"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type to be serialized and deserialized.
    /// </typeparam>
    internal class BuiltInSchemaSerializer<T> : ISchemaSerializer
    {
        private readonly Reader reader;

        private readonly Writer writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuiltInSchemaSerializer{T}"/> class.
        /// </summary>
        /// <param name="reader">
        /// The reader used to read the given type from a stream.
        /// </param>
        /// <param name="writer">
        /// The writer used to write the given type to a stream.
        /// </param>
        public BuiltInSchemaSerializer(Reader reader, Writer writer)
        {
            this.reader = reader;
            this.writer = writer;
        }

        /// <summary>
        /// Delegated to read the given type.
        /// </summary>
        /// <param name="reader">
        /// The reader from which the type has to be read.
        /// </param>
        /// <returns>
        /// An object of the given type.
        /// </returns>
        public delegate T Reader(BecReader reader);

        /// <summary>
        /// Delegated to write the given type.
        /// </summary>
        /// <param name="writer">
        /// The writer to which the type has to be written.
        /// </param>
        /// <param name="value">
        /// The value to be written.
        /// </param>
        public delegate void Writer(BecWriter writer, T value);

        /// <summary>
        /// Serializes the given object to the given stream
        /// using the writer delegate provided in the constructor.
        /// </summary>
        /// <param name="obj">
        /// The object to be serialized.
        /// </param>
        /// <param name="becWriter">
        /// The writer to which the object will be serialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        public void Serialize(object obj, BecWriter becWriter, SerializationContext context)
        {
            this.writer(becWriter, (T)obj);
        }

        /// <summary>
        /// Deserializes the given object from the given stream
        /// using the reader delegate provided in the constructor.
        /// </summary>
        /// <param name="becReader">
        /// The reader from which the object will be deserialized.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        /// <returns>
        /// The deserialized object.
        /// </returns>
        public object Deserialize(BecReader becReader, SerializationContext context)
        {
            return this.reader(becReader);
        }
    }
}
