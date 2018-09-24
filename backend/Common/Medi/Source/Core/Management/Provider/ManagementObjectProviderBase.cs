// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementObjectProviderBase.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementObjectProviderBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Provider
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple base implementation of <see cref="IManagementProvider"/>.
    /// </summary>
    public abstract class ManagementObjectProviderBase : ManagementProviderBase, IManagementObjectProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementObjectProviderBase"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        protected ManagementObjectProviderBase(string name, IManagementProvider parent)
            : base(name, parent)
        {
        }

        /// <summary>
        /// Gets all properties. This implementation returns an
        /// empty enumerable.
        /// </summary>
        public virtual IEnumerable<ManagementProperty> Properties
        {
            get
            {
                yield break;
            }
        }

        /// <summary>
        /// Get a property by its name.
        /// This implementation searches through all properties
        /// returned by <see cref="Properties"/> to see if one
        /// matches the given name.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <returns>
        /// the property if found, otherwise null.
        /// </returns>
        public virtual ManagementProperty GetProperty(string name)
        {
            foreach (var property in this.Properties)
            {
                if (property.Name.Equals(name))
                {
                    return property;
                }
            }

            return null;
        }
    }
}
