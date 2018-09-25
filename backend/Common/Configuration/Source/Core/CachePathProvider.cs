// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CachePathProvider.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the CachePathProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.Core
{
    using System;

    /// <summary>
    /// Provider for the location of a cache file.
    /// This class allows to store the .cache file in a specific location.
    /// </summary>
    public abstract class CachePathProvider
    {
        /// <summary>
        /// The cache file extension (.cache).
        /// </summary>
        protected static readonly string CacheFileExtension = ".cache";

        private static CachePathProvider current;

        static CachePathProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets or sets the current cache path provider instance.
        /// </summary>
        public static CachePathProvider Current
        {
            get
            {
                return current;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                current = value;
            }
        }

        /// <summary>
        /// Resets the <see cref="Current"/> value to the default provider.
        /// </summary>
        public static void Reset()
        {
            Current = new DefaultCachePathProvider();
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
        public abstract string GetCacheFilePath(string configFilePath);

        private class DefaultCachePathProvider : CachePathProvider
        {
            public override string GetCacheFilePath(string configFilePath)
            {
                return configFilePath + CachePathProvider.CacheFileExtension;
            }
        }
    }
}
