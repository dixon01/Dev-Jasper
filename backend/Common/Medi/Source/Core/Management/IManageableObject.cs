// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageableObject.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManageableObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple interface that can be implemented by types that want to provide 
    /// management information about an object.
    /// To create a management provider for this object, just call
    /// <see cref="ManagementProviderFactory.CreateManagementProvider"/> with an
    /// <see cref="IManageableObject"/> object as an argument.
    /// Implementations should always implement this type explicitly because those
    /// methods should never be called by anybody else but the management framework.
    /// </summary>
    public interface IManageableObject : IManageable
    {
        /// <summary>
        /// Gets all <see cref="ManagementProperty"/> objects for this object.
        /// </summary>
        /// <returns>
        /// all properties.
        /// </returns>
        IEnumerable<ManagementProperty> GetProperties();
    }
}