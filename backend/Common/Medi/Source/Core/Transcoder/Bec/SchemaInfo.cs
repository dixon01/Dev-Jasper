// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaInfo.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec
{
    using Gorba.Common.Medi.Core.Transcoder.Bec.Engine;
    using Gorba.Common.Medi.Core.Transcoder.Bec.Schema;

    /// <summary>
    /// Holds information about a schema and its serializer.
    /// </summary>
    internal class SchemaInfo
    {
        /// <summary>
        /// Gets or sets the id for the schema.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        public ITypeSchema Schema { get; set; }

        /// <summary>
        /// Gets or sets the serializer for the schema.
        /// </summary>
        public ISchemaSerializer Serializer { get; set; }
    }
}
