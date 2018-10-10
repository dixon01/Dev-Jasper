// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManageableTable.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManageableTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System.Collections.Generic;
    
    /// <summary>
    /// Simple interface that can be implemented by types that want to provide 
    /// management information about a table.
    /// To create a management provider for this object, just call
    /// <see cref="ManagementProviderFactory.CreateManagementProvider"/> with an
    /// <see cref="IManageableTable"/> object as an argument.
    /// Implementations should always implement this type explicitly because those
    /// methods should never be called by anybody else but the management framework.
    /// </summary>
    public interface IManageableTable : IManageable
    {
        /// <summary>
        /// Gets all rows of <see cref="ManagementProperty"/> objects for this object.
        /// </summary>
        /// <returns>
        /// all rows.
        /// </returns>
        IEnumerable<List<ManagementProperty>> GetRows();
    }
}