// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagementObject.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagementObject type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.TestGui.Management
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management;

    /// <summary>
    /// An object representing an object node in the management tree.
    /// </summary>
    internal class ManagementObject : ManagementPropertyCollection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagementObject"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider to wrap.
        /// </param>
        public ManagementObject(IManagementObjectProvider provider)
            : base(provider.Name)
        {
            this.Provider = provider;
        }

        /// <summary>
        /// Gets the wrapped provider.
        /// </summary>
        public IManagementObjectProvider Provider { get; private set; }

        /// <summary>
        /// Gets the list of properties.
        /// </summary>
        /// <returns>
        /// the list of properties.
        /// </returns>
        protected override IEnumerable<ManagementProperty> GetProperties()
        {
            return this.Provider.Properties;
        }
    }
}