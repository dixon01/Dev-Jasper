// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BuiltInTypeSchema.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BuiltInTypeSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    /// <summary>
    /// Type schema for built-in types (primitives, 
    /// <see cref="string"/> and <see cref="System.Type"/>).
    /// </summary>
    public class BuiltInTypeSchema : ITypeSchema
    {
        /// <summary>
        /// Gets or sets the type name of this schema.
        /// </summary>
        public TypeName TypeName { get; set; }
    }
}
