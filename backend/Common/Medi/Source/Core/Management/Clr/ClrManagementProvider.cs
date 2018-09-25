// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClrManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ClrManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Clr
{
    using System;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// The management provider for Common Language Runtime information.
    /// Provides a list of loaded assemblies as well as the CLR version.
    /// </summary>
    internal sealed class ClrManagementProvider : ModifiableManagementObjectProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClrManagementProvider"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent in the management tree.
        /// </param>
        public ClrManagementProvider(IManagementProvider parent)
            : base("Clr", parent)
        {
            this.AddChild(new AssembliesManagementProvider(this));

            this.AddProperty(new ManagementProperty<string>("Version", Environment.Version.ToString(), true));
        }
    }
}
