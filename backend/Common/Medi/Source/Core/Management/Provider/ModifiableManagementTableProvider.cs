// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ModifiableManagementTableProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   An <see cref="IModifiableManagementProvider" /> implementation
//   that allows to add and remove children and properties.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// An <see cref="IModifiableManagementProvider"/> implementation
    /// that allows to add and remove children and properties.
    /// </summary>
    public class ModifiableManagementTableProvider : ModifiableManagementProvider, IManagementTableProvider
    {
        private readonly List<List<ManagementProperty>> rows = new List<List<ManagementProperty>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ModifiableManagementTableProvider"/> class. 
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public ModifiableManagementTableProvider(string name, IManagementProvider parent)
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
                return this.rows;
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
            if (index < 0 || index >= this.rows.Count)
            {
                return null;
            }

            return this.rows[index];
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

            return row.Find(p => p.Name == columnName);
        }

        /// <summary>
        /// Adds a row of properties to this provider.
        /// </summary>
        /// <param name="row">
        /// The row.
        /// </param>
        public virtual void AddRow(List<ManagementProperty> row)
        {
            this.rows.Add(new List<ManagementProperty>(row));
        }

        /// <summary>
        /// Clears all properties and children from this node.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            this.rows.Clear();
        }
    }
}