// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WmiManagementProvider.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the WmiManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Wmi
{
    using System.Collections.Generic;

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
        /// Gets all properties. This implementation returns an
        /// empty enumerable.
        /// </summary>
        public override IEnumerable<ManagementProperty> Properties
        {
            get
            {
                yield return
                    new ManagementProperty<string>(
                        "CompactFramework",
                        "WMI is not supported under .NET Compact Framework",
                        true);
            }
        }
    }
}
