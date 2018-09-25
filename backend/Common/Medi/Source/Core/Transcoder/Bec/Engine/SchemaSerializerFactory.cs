// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaSerializerFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaSerializerFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Engine
{
    /// <summary>
    /// A base class for all factories for schema serializer classes.
    /// </summary>
    internal abstract partial class SchemaSerializerFactory
    {
        /// <summary>
        /// Creates a serializer for a given schema information.
        /// </summary>
        /// <param name="schemaInfo">
        /// The schema information.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <returns>
        /// A serializer for the given schema.
        /// </returns>
        public abstract ISchemaSerializer CreateSchemaSerializer(SchemaInfo schemaInfo, SerializationContext context);
    }
}