// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchemaMember.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchemaMember type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    /// <summary>
    /// Member (property or field) of a schema.
    /// </summary>
    public class SchemaMember
    {
        /// <summary>
        /// Gets or sets the schema of the member.
        /// </summary>
        public ITypeSchema Schema { get; set; }

        /// <summary>
        /// Gets or sets the name of the member.
        /// </summary>
        public string Name { get; set; }
    }
}
