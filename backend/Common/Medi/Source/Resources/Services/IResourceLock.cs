// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceLock.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IResourceLock type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Services
{
    using System;

    using Gorba.Common.Medi.Core.Resources;

    /// <summary>
    /// Resource lock that can be acquired by calling
    /// <see cref="ResourceServiceBase.AcquireLock"/>.
    /// This class can also be used as an <see cref="IDisposable"/>
    /// inside a <code>using</code> directive.
    /// </summary>
    internal interface IResourceLock : IDisposable
    {
        /// <summary>
        /// Gets the resource id.
        /// </summary>
        ResourceId Id { get; }

        /// <summary>
        /// Releases the lock.
        /// </summary>
        void Release();
    }
}