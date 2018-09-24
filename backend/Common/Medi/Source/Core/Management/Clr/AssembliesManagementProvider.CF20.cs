// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssembliesManagementProvider.CF20.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssembliesManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Clr
{
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// Management provider that returns all assemblies loaded by the current app domain.
    /// </summary>
    internal partial class AssembliesManagementProvider : ManagementObjectProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AssembliesManagementProvider"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        public AssembliesManagementProvider(IManagementProvider parent)
            : base("Assemblies", parent)
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
                        "Listing the loaded assemblies is not supported under .NET Compact Framework",
                        true);
            }
        }
    }
}
