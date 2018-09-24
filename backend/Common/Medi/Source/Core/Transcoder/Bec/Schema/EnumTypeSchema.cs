// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumTypeSchema.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the EnumTypeSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    /// <summary>
    /// Type schema for enum types.
    /// </summary>
    public class EnumTypeSchema : ITypeSchema
    {
        /// <summary>
        /// Gets or sets the type name of this schema.
        /// </summary>
        public TypeName TypeName { get; set; }

        /// <summary>
        /// Gets or sets the schema of the underlying primitive type.
        /// </summary>
        public BuiltInTypeSchema UnderlyingSchema { get; set; }
    }
}