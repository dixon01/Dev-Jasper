// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManageableObjectManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManageableObjectManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// Management provider for <see cref="IManageableObject"/> objects.
    /// It queries the given manageable object for properties and children.
    /// </summary>
    public class ManageableObjectManagementProvider : ManagementObjectProviderBase
    {
        private readonly IManageableObject manageable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManageableObjectManagementProvider"/> class.
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
        public ManageableObjectManagementProvider(string name, IManageableObject manageable, IManagementProvider parent)
            : base(name, parent)
        {
            this.manageable = manageable;
        }

        /// <summary>
        /// Gets all properties from the manageable object given in the constructor.
        /// </summary>
        public override IEnumerable<ManagementProperty> Properties
        {
            get
            {
                return this.manageable.GetProperties();
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
