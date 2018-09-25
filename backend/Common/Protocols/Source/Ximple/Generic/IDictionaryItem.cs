// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDictionaryItem.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IDictionaryItem type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ximple.Generic
{
    /// <summary>
    /// Interface implemented by all items in a dictionary that have a name and an index.
    /// This interface is only used internally for <see cref="Dictionary.GetForNameOrNumber{T}"/>.
    /// </summary>
    internal interface IDictionaryItem
    {
        /// <summary>
        /// Gets the index.
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }
    }
}