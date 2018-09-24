// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRemoteManagementProviderImpl.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IRemoteManagementProviderImpl type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Remote
{
    using System;

    /// <summary>
    /// Internal interface to be implemented by all <see cref="IRemoteManagementProvider"/>
    /// implementations.
    /// </summary>
    internal interface IRemoteManagementProviderImpl : IRemoteManagementProvider
    {
        /// <summary>
        /// Gets the path elements of this provider.
        /// </summary>
        string[] Path { get; }
    }
}