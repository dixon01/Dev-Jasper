// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IModifiableManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IModifiableManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System;

    /// <summary>
    /// Interface for a modifiable management provider that allows to
    /// add properties and children.
    /// </summary>
    public interface IModifiableManagementProvider : IManagementProvider
    {
        /// <summary>
        /// Adds a child to this node.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <exception cref="ArgumentException">
        /// A provider with the same name already exists in this provider.
        /// </exception>
        void AddChild(IManagementProvider child);

        /// <summary>
        /// Removes a child from this node.
        /// </summary>
        /// <param name="child">
        /// The child.
        /// </param>
        /// <exception cref="ArgumentException">
        /// If the child doesn't belong to this provider.
        /// </exception>
        void RemoveChild(IManagementProvider child);

        /// <summary>
        /// Gets a descendant for a given path.
        /// If the descendant or any intermediate node doesn't exist,
        /// and <see cref="createIfNecessary"/> is set to true, a new
        /// <see cref="IModifiableManagementProvider"/> is created.
        /// </summary>
        /// <param name="createIfNecessary">
        /// If this is set to true, a new provider will be created if necessary.
        /// </param>
        /// <param name="path">
        /// The path as an array of path elements.
        /// </param>
        /// <returns>
        /// The <see cref="IManagementProvider"/> if necessary it will be created.
        /// </returns>
        IManagementProvider GetDescendant(bool createIfNecessary, params string[] path);
    }
}