// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITypeSchema.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITypeSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    /// <summary>
    /// Interface to be implemented by all type schemas.
    /// You should never have to implement this yourself,
    /// just use the existing type schemas.
    /// </summary>
    public interface ITypeSchema
    {
        /// <summary>
        /// Gets the type name of this schema.
        /// </summary>
        TypeName TypeName { get; }
    }
}
