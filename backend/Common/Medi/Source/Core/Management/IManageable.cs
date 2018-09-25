// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageable.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManageable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple interface that can be implemented by types that want to provide management information.
    /// To create a management provider for this object, just call
    /// <see cref="ManagementProviderFactory.CreateManagementProvider"/> with an
    /// <see cref="IManageable"/> object as an argument.
    /// Implementations should always implement this type explicitly because those
    /// methods should never be called by anybody else but the management framework.
    /// </summary>
    public interface IManageable
    {
        /// <summary>
        /// Gets all management children for this object.
        /// </summary>
        /// <param name="parent">
        /// The parent provider object.
        /// </param>
        /// <returns>
        /// all children.
        /// </returns>
        IEnumerable<IManagementProvider> GetChildren(IManagementProvider parent);
    }
}
