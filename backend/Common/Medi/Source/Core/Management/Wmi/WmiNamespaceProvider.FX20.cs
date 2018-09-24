// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiNamespaceProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WmiNamespaceProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Wmi
{
    using System;
    using System.Collections.Generic;
    using System.Management;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// The provider for an entire WMI namespace.
    /// </summary>
    internal partial class WmiNamespaceProvider : ManagementObjectProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiNamespaceProvider"/> class.
        /// </summary>
        /// <param name="name">
        /// The name of the namespace.
        /// </param>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public WmiNamespaceProvider(string name, WmiManagementProvider parent)
            : base(name, parent)
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
                // Perform WMI object query on
                // selected namespace.
                var searcher = new ManagementObjectSearcher(
                    new ManagementScope("root\\" + this.Name), new WqlObjectQuery("SELECT * FROM meta_class"), null);
                var results = searcher.Get();
                var providers = new List<IManagementProvider>(results.Count);
                foreach (ManagementClass wmiClass in results)
                {
                    providers.Add(new WmiClassProvider(wmiClass, this));
                }

                providers.Sort((a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a.Name, b.Name));

                return providers;
            }
        }
    }
}