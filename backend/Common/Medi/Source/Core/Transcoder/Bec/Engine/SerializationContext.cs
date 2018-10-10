// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationContext.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SerializationContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    /// <summary>
    /// Serialization context containing all information requried by 
    /// the individual serializers.
    /// </summary>
    internal class SerializationContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationContext"/> class.
        /// </summary>
        /// <param name="engine">
        /// The engine that crates this context.
        /// </param>
        /// <param name="stringMapper">
        /// The string mapper used for mapping strings to IDs.
        /// </param>
        /// <param name="schemaMapper">
        /// The schema mapper used for mapping schema information to IDs.
        /// </param>
        public SerializationContext(SerializationEngine engine, IMapper<string> stringMapper, IMapper<SchemaInfo> schemaMapper)
        {
            this.Engine = engine;
            this.StringMapper = stringMapper;
            this.SchemaMapper = schemaMapper;
        }

        /// <summary>
        /// Gets the string mapper used for mapping strings to IDs.
        /// </summary>
        public IMapper<string> StringMapper { get; private set; }

        /// <summary>
        /// Gets the schema mapper used for mapping schema information to IDs.
        /// </summary>
        public IMapper<SchemaInfo> SchemaMapper { get; private set; }

        /// <summary>
        /// Gets the serialization engine that is running this (de)serialization.
        /// </summary>
        public SerializationEngine Engine { get; private set; }
    }
}
