// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAssemblyLoaderHelper.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAssemblyLoaderHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    using System;

    /// <summary>
    /// Interface to load assemblies on a different <see cref="AppDomain"/>.
    /// </summary>
    public interface IAssemblyLoaderHelper
    {
        /// <summary>
        /// Loads the given assembly and adds it to the list of assemblies to be searched.
        /// </summary>
        /// <param name="assemblyLocation">
        /// The full path to the assembly file.
        /// </param>
        void Load(string assemblyLocation);
    }
}