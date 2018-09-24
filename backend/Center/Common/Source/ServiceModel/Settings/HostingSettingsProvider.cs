// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HostingSettingsProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HostingSettingsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Settings
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Provider for the BackgroundSystem settings.
    /// </summary>
    public abstract class HostingSettingsProvider
    {
        static HostingSettingsProvider()
        {
            Reset();
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static HostingSettingsProvider Current { get; private set; }

        /// <summary>
        /// Resets the current provider to the default.
        /// </summary>
        public static void Reset()
        {
            Current = DefaultHostingSettingsProvider.Instance;
        }

        /// <summary>
        /// Sets the provided instance as the current one.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void Set(HostingSettingsProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets the settings
        /// </summary>
        /// <param name="path">Optional path to the settings file.</param>
        /// <returns>
        /// The <see cref="HostingSettings"/>.
        /// </returns>
        public abstract HostingSettings GetSettings(string path = null);

        private class DefaultHostingSettingsProvider : HostingSettingsProvider
        {
            static DefaultHostingSettingsProvider()
            {
                Instance = new DefaultHostingSettingsProvider();
            }

            public static DefaultHostingSettingsProvider Instance { get; private set; }

            public override HostingSettings GetSettings(string path = null)
            {
                try
                {
                    return new HostingSettings(
                        ConfigurationManager.AppSettings["ResourcesPath"],
                        ConfigurationManager.AppSettings["ContentResourcesPath"]);
                }
                catch (Exception exception)
                {
                    throw new ConfigurationErrorsException("Couldn't load BackgroundSystem settings", exception);
                }
            }
        }
    }
}