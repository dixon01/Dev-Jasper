// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyResolver.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAssemblyResolver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Export
{
    /// <summary>
    /// Callback interface used by <see cref="ConfigAdjusterProxy"/> to resolve unknown assemblies.
    /// </summary>
    internal interface IAssemblyResolver
    {
        /// <summary>
        /// Tries resolving the given assembly name.
        /// </summary>
        /// <param name="assemblyName">
        /// The (full) assembly name.
        /// </param>
        /// <returns>
        /// The path to the (temporary) DLL corresponding to the <paramref name="assemblyName"/>.
        /// </returns>
        string Resolve(string assemblyName);
    }
}