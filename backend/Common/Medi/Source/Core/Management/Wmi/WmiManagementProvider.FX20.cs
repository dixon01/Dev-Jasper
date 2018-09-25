// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiManagementProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WmiManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Wmi
{
    using System;
    using System.Collections.Generic;
    using System.Management;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// Management provider that queries the Windows Management Interface.
    /// </summary>
    internal partial class WmiManagementProvider : ManagementObjectProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiManagementProvider"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public WmiManagementProvider(IManagementProvider parent)
            : base("WMI", parent)
        {
        }

        /// <summary>
        /// Gets all children. This implementation returns an
        /// empty enumerable.
        /// </summary>
        public override IEnumerable<IManagementProvider> Children
        {
            get
            {
                var clazz = new ManagementClass(new ManagementScope("root"), new ManagementPath("__namespace"), null);
                var namespaces = clazz.GetInstances();
                var providers = new List<IManagementProvider>(namespaces.Count);
                foreach (ManagementObject ns in namespaces)
                {
                    providers.Add(new WmiNamespaceProvider(ns["Name"].ToString(), this));
                }

                providers.Sort((a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a.Name, b.Name));

                return providers;
            }
        }
    }
}
