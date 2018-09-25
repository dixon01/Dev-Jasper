// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssemblyLoaderHelper.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssemblyLoaderHelper type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// Remote object to load assemblies on a different <see cref="AppDomain"/>.
    /// </summary>
    public class AssemblyLoaderHelper : MarshalByRefObject, IAssemblyLoaderHelper
    {
        private readonly List<Assembly> assemblies = new List<Assembly>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyLoaderHelper"/> class.
        /// </summary>
        public AssemblyLoaderHelper()
        {
            AppDomain.CurrentDomain.AssemblyResolve += this.CurrentDomainAssemblyResolve;
        }

        /// <summary>
        /// Loads the given assembly and adds it to the list of assemblies to be searched.
        /// </summary>
        /// <param name="assemblyLocation">
        /// The full path to the assembly file.
        /// </param>
        public void Load(string assemblyLocation)
        {
            this.assemblies.Add(Assembly.LoadFile(assemblyLocation));
        }

        private Assembly CurrentDomainAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return this.assemblies.FirstOrDefault(asm => asm.FullName.Equals(args.Name));
        }
    }
}