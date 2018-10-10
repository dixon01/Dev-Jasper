// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageableManagementProvider.cs" company="Gorba AG">
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
    /// It queries the given manageable object for properties and children.
    /// </summary>
    public class ManageableManagementProvider : ManagementProviderBase
    {
        private readonly IManageable manageable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageableManagementProvider"/> class.
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
        public ManageableManagementProvider(string name, IManageable manageable, IManagementProvider parent)
            : base(name, parent)
        {
            this.manageable = manageable;
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