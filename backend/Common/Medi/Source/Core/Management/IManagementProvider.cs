// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that want to provide management information.
    /// Most likely you can use one of the predefined implementations
    /// instead of creating your own implementation.
    /// </summary>
    public interface IManagementProvider : IDisposable
    {
        /// <summary>
        /// Gets the name of this node.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parent of this node.
        /// </summary>
        IManagementProvider Parent { get; }

        /// <summary>
        /// Gets the factory that created this node.
        /// </summary>
        IManagementProviderFactory Factory { get; }

        /// <summary>
        /// Gets all children.
        /// </summary>
        IEnumerable<IManagementProvider> Children { get; }

        /// <summary>
        /// Get a child by its name.
        /// </summary>
        /// <param name="name">
        /// The name of the child to be found.
        /// </param>
        /// <returns>
        /// the child if found, otherwise null.
        /// </returns>
        IManagementProvider GetChild(string name);

        /// <summary>
        /// Gets a descendant for a given path.
        /// </summary>
        /// <param name="path">
        /// The path as an array of path elements.
        /// </param>
        /// <returns>
        /// The <see cref="IManagementProvider"/> or null if the descendant is not found.
        /// </returns>
        IManagementProvider GetDescendant(params string[] path);
    }
}
