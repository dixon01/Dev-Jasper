// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IManagementTableProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IManagementTableProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface for types that want to provide management information about a table.
    /// Most likely you can use one of the predefined implementations
    /// instead of creating your own implementation.
    /// </summary>
    public interface IManagementTableProvider : IManagementProvider
    {
        /// <summary>
        /// Gets all rows of this table.
        /// Each row is a list of properties where the property name is the name of the column.
        /// </summary>
        IEnumerable<List<ManagementProperty>> Rows { get; }

        /// <summary>
        /// Get a row by its index.
        /// </summary>
        /// <param name="index">
        /// The index from zero on which to find the row.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        List<ManagementProperty> GetRow(int index);

        /// <summary>
        /// Get a property by the row index and and the property name.
        /// </summary>
        /// <param name="rowIndex">
        /// The index from zero on which to find the row.
        /// </param>
        /// <param name="columnName">
        /// The name of the column to return the value in the row.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        ManagementProperty GetCell(int rowIndex, string columnName);
    }
}