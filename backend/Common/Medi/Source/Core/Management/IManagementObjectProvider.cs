// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManagementObjectProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManagementObjectProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that want to provide management information about an object.
    /// Most likely you can use one of the predefined implementations
    /// instead of creating your own implementation.
    /// </summary>
    public interface IManagementObjectProvider : IManagementProvider
    {
        /// <summary>
        /// Gets all <see cref="ManagementProperty"/> objects for this node.
        /// </summary>
        IEnumerable<ManagementProperty> Properties { get; }

        /// <summary>
        /// Get a property by its name.
        /// </summary>
        /// <param name="name">
        /// The name of the property to be found.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        ManagementProperty GetProperty(string name);
    }
}