// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListTypeSchema.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ListTypeSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    /// <summary>
    /// Type schema for lists and arrays.
    /// </summary>
    public class ListTypeSchema : ITypeSchema
    {
        /// <summary>
        /// Gets or sets the type name of this schema.
        /// </summary>
        public TypeName TypeName { get; set; }

        /// <summary>
        /// Gets or sets the schema of the items.
        /// </summary>
        public ITypeSchema ItemSchema { get; set; }
    }
}
