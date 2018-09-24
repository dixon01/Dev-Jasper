// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementTableProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Simple base implementation of <see cref="IManagementProvider" />.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple base implementation of <see cref="IManagementTableProvider"/>.
    /// </summary>
    public abstract class ManagementTableProviderBase : ManagementProviderBase, IManagementTableProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementTableProviderBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        protected ManagementTableProviderBase(string name, IManagementProvider parent)
            : base(name, parent)
        {
        }

        /// <summary>
        /// Gets all rows of this table.
        /// Each row is a list of properties where the property name is the name of the column.
        /// </summary>
        public virtual IEnumerable<List<ManagementProperty>> Rows
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Get a row by its index.
        /// </summary>
        /// <param name="index">
        /// The index from zero on which to find the row.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        public virtual List<ManagementProperty> GetRow(int index)
        {
            int i = 0;
            foreach (var child in this.Rows)
            {
                if (index == i++)
                {
                    return child;
                }
            }

            return null;
        }

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
        public virtual ManagementProperty GetCell(int rowIndex, string columnName)
        {
            var row = this.GetRow(rowIndex);
            if (row == null)
            {
                return null;
            }

            foreach (var property in row)
            {
                if (property.Name == columnName)
                {
                    return property;
                }
            }

            return null;
        }
    }
}