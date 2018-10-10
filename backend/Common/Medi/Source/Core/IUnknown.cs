// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IUnknown.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    /// <summary>
    /// Interface to for classes specific to a certain Codec that represent an unknown object.
    /// This is used to be able to deserialize and serialize objects without actually knowing them.
    /// </summary>
    internal interface IUnknown
    {
        /// <summary>
        /// Gets the type name of the object represented by this object.
        /// The TypeName's <see cref="Core.TypeName.IsKnown"/> should return false.
        /// </summary>
        TypeName TypeName { get; }
    }
}
