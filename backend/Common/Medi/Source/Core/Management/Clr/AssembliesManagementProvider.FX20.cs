// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AssembliesManagementProvider.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AssembliesManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.Clr
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// Management provider that returns all assemblies loaded by the current app domain.
    /// </summary>
    internal partial class AssembliesManagementProvider : ManagementProviderBase
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
        /// Gets all loaded assemblies nicely wrapped inside management providers.
        /// </summary>
        public override IEnumerable<IManagementProvider> Children
        {
            get
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                var providers = new List<IManagementProvider>(assemblies.Length);
                foreach (var asm in assemblies)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(asm.Location))
                        {
                            continue;
                        }
                    }
                    catch
                    {
                        continue;
                    }

                    var version = asm.GetName().Version.ToString();
                    var provider = new ModifiableManagementObjectProvider(Path.GetFileName(asm.Location), this);
                    var fileVersion = FileVersionInfo.GetVersionInfo(asm.Location);
                    provider.AddProperty(new ManagementProperty<string>("FileVersion", fileVersion.FileVersion, true));
                    provider.AddProperty(new ManagementProperty<string>("Version", version, true));
                    providers.Add(provider);
                }

                providers.Sort((a, b) => StringComparer.InvariantCultureIgnoreCase.Compare(a.Name, b.Name));
                return providers;
            }
        }
    }
}
