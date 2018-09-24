// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISessionId.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ISessionId type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Network
{
    using System;

    /// <summary>
    /// Interface to be implemented by session IDs.
    /// Subclasses must override the <see cref="object.Equals(object)"/>
    /// and <see cref="object.GetHashCode"/> methods to be
    /// hash table compliant.
    /// </summary>
    public interface ISessionId : IEquatable<ISessionId>
    {
    }
}
