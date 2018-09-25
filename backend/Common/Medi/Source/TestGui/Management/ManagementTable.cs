// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementTable.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementTable type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui.Management
{
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// An object representing a table node in the management tree.
    /// </summary>
    internal class ManagementTable : IListSource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementTable"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        public ManagementTable(IManagementTableProvider provider)
        {
            this.Provider = provider;
        }

        /// <summary>
        /// Gets the wrapped provider.
        /// </summary>
        public IManagementTableProvider Provider { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the collection is a collection of <see cref="T:System.Collections.IList"/> objects.
        /// </summary>
        /// <returns>
        /// true if the collection is a collection of <see cref="T:System.Collections.IList"/> objects; otherwise, false.
        /// </returns>
        public bool ContainsListCollection
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IList"/> that can be bound to a data source from an object that does not implement an <see cref="T:System.Collections.IList"/> itself.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IList"/> that can be bound to a data source from the object.
        /// </returns>
        public IList GetList()
        {
            return this.Provider.Rows.Select(row => new ManagementTableRow(row)).ToList();
        }

        private class ManagementTableRow : ManagementPropertyCollection
        {
            private readonly IEnumerable<ManagementProperty> row;

            public ManagementTableRow(IEnumerable<ManagementProperty> row)
                : base(string.Empty)
            {
                this.row = row;
            }

            protected override IEnumerable<ManagementProperty> GetProperties()
            {
                return this.row;
            }
        }
    }
}