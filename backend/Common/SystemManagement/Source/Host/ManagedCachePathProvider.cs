// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedCachePathProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagedCachePathProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host
{
    using System;

    using Gorba.Common.Configuration.Core;
    using Gorba.Common.SystemManagement.Host.Path;

    /// <summary>
    /// <see cref="CachePathProvider"/> for applications using <see cref="Gorba.Common.SystemManagement.Host.Path"/>.
    /// This provider stores cache files for config files located in <code>Progs</code>, Config and Presentation to
    /// the Data directory.
    /// </summary>
    internal class ManagedCachePathProvider : CachePathProvider
    {
        private const StringComparison DefaultComparison = StringComparison.InvariantCultureIgnoreCase;

        private readonly string configPath;
        private readonly string progsPath;
        private readonly string presentationPath;

        private readonly bool useDefault;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedCachePathProvider"/> class.
        /// </summary>
        public ManagedCachePathProvider()
        {
            this.configPath = PathManager.Instance.GetPath(FileType.Config, string.Empty)
                              + System.IO.Path.DirectorySeparatorChar;
            this.progsPath = PathManager.Instance.GetPath(FileType.Application, string.Empty)
                             + System.IO.Path.DirectorySeparatorChar;
            this.presentationPath = PathManager.Instance.GetPath(FileType.Presentation, string.Empty)
                                    + System.IO.Path.DirectorySeparatorChar;

            this.useDefault = this.configPath == this.progsPath;
        }

        /// <summary>
        /// Gets the full path to the cache file from the given full path to the config file.
        /// </summary>
        /// <param name="configFilePath">
        /// The full config file path.
        /// </param>
        /// <returns>
        /// The full cache file path.
        /// </returns>
        public override string GetCacheFilePath(string configFilePath)
        {
            if (this.useDefault)
            {
                return configFilePath + CachePathProvider.CacheFileExtension;
            }

            if (!System.IO.Path.IsPathRooted(configFilePath))
            {
                return PathManager.Instance.CreatePath(
                    FileType.Data, configFilePath + CachePathProvider.CacheFileExtension);
            }

            if (configFilePath.StartsWith(this.configPath, DefaultComparison))
            {
                configFilePath = configFilePath.Substring(this.configPath.Length);
            }
            else if (configFilePath.StartsWith(this.progsPath, DefaultComparison))
            {
                configFilePath = configFilePath.Substring(this.progsPath.Length);
            }
            else if (configFilePath.StartsWith(this.presentationPath, DefaultComparison))
            {
                configFilePath = configFilePath.Substring(this.presentationPath.Length);
            }
            else
            {
                return configFilePath + CachePathProvider.CacheFileExtension;
            }

            return PathManager.Instance.CreatePath(
                FileType.Data, configFilePath + CachePathProvider.CacheFileExtension);
        }
    }
}