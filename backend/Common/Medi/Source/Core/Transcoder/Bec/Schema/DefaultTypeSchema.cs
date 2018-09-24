// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeSchema.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DefaultTypeSchema type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Bec.Schema
{
    /// <summary>
    /// Type schema for all types that are not handled differently.
    /// </summary>
    public class DefaultTypeSchema : ITypeSchema
    {
        private static readonly TypeName ObjectType = TypeName.Of<object>();

        /// <summary>
        /// Gets the type name. This always returns the <see cref="object"/> type name.
        /// </summary>
        public TypeName TypeName
        {
            get
            {
                return ObjectType;
            }
        }
    }
}
