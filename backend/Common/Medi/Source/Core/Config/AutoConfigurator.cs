// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AutoConfigurator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Config
{
    using System;
    using System.IO;
    using System.Reflection;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Configurator that searches for medi.config in app and current directory.
    /// The address will always be:
    /// Unit: MachineName
    /// Application: name of the entry assembly (usually the name of the EXE)
    /// </summary>
    public class AutoConfigurator : FileConfigurator
    {
        private const string MediConfig = "medi.config";

        /// <summary>
        /// Initializes a new instance of the <see cref="AutoConfigurator"/> class.
        /// </summary>
        public AutoConfigurator()
            : base(FindConfigFile())
        {
        }

        private static string FindConfigFile()
        {
            var entryPath = Path.GetDirectoryName(ApplicationHelper.GetEntryAssemblyLocation());
            string path;
            if (entryPath != null)
            {
                path = Path.Combine(entryPath, MediConfig);
                if (File.Exists(path))
                {
                    return path;
                }
            }

            path = Path.Combine(ApplicationHelper.CurrentDirectory, MediConfig);
            if (File.Exists(path))
            {
                return path;
            }

            throw new FileNotFoundException("Couldn't find medi.config in EXE or current directory");
        }
    }
}
