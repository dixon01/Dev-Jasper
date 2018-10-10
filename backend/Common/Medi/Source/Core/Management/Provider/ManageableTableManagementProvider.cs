// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageableTableManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Management provider for <see cref="IManageableTable" /> objects.
//   It queries the given manageable object for properties and children.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// Management provider for <see cref="IManageableTable"/> objects.
    /// It queries the given manageable table for rows and children.
    /// </summary>
    public class ManageableTableManagementProvider : ManagementTableProviderBase
    {
        private readonly IManageableTable manageable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageableTableManagementProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of this node.
        /// </param>
        /// <param name="manageable">
        /// The manageable object to be represented.
        /// </param>
        /// <param name="parent">
        /// The parent of this node.
        /// </param>
        public ManageableTableManagementProvider(string name, IManageableTable manageable, IManagementProvider parent)
            : base(name, parent)
        {
            this.manageable = manageable;
        }

        /// <summary>
        /// Gets all rows of this table.
        /// Each row is a list of properties where the property name is the name of the column.
        /// </summary>
        public override IEnumerable<List<ManagementProperty>> Rows
        {
            get
            {
                return this.manageable.GetRows();
            }
        }

        /// <summary>
        /// Gets all children from the manageable object given in the constructor.
        /// </summary>
        public override IEnumerable<IManagementProvider> Children
        {
            get
            {
                return this.manageable.GetChildren(this);
            }
        }
    }
}