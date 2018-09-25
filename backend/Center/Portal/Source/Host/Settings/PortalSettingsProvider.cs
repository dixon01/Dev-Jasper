// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortalSettingsProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortalSettingsProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Settings
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
    using System.IO;
    using System.Linq;

    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Files;

    using Microsoft.WindowsAzure;

    using NLog;

    /// <summary>
    /// The portal settings provider.
    /// </summary>
    public abstract class PortalSettingsProvider
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        static PortalSettingsProvider()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PortalSettingsProvider"/> class.
        /// </summary>
        protected PortalSettingsProvider()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Gets the current provider.
        /// </summary>
        public static PortalSettingsProvider Current { get; private set; }

        /// <summary>
        /// Sets the current provider.
        /// </summary>
        /// <param name="instance">
        /// The instance.
        /// </param>
        /// <exception cref="ArgumentNullException">The instance is null.</exception>
        public static void SetCurrent(PortalSettingsProvider instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            Current = instance;
        }

        /// <summary>
        /// Gets the <see cref="PortalSettings"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="PortalSettings"/>.
        /// </returns>
        public abstract PortalSettings GetSettings();

        /// <summary>
        /// Tries to get a setting value.
        /// </summary>
        /// <param name="name">
        /// The name of the setting.
        /// </param>
        /// <param name="value">
        /// The value, if found.
        /// </param>
        /// <typeparam name="T">The type of the setting.</typeparam>
        /// <returns>
        /// <c>true</c> if the setting was found and contained a valid value; otherwise, <c>false</c>.
        /// </returns>
        protected virtual bool TryGetSetting<T>(string name, out T value)
        {
            var stringValue = this.GetStringValue(name);
            if (string.IsNullOrEmpty(stringValue))
            {
                this.Logger.Debug("Cannot find settings for port '{0}'", name);
                value = default(T);
                return false;
            }

            var typeDescriptor = TypeDescriptor.GetConverter(typeof(T));
            try
            {
                value = (T)typeDescriptor.ConvertFromString(stringValue);
                return true;
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error while converting setting");
                value = default(T);
                return false;
            }
        }

        /// <summary>
        /// Gets the string value for a setting.
        /// </summary>
        /// <param name="settingName">
        /// The name of the setting.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> value.
        /// </returns>
        protected virtual string GetStringValue(string settingName)
        {
            return ConfigurationManager.AppSettings[settingName];
        }

        private static void ResetCurrent()
        {
            SetCurrent(RootDirectoryPortalSettingsProvider.CurrentDirectoryProviderInstance);
        }

        /// <summary>
        /// Provides the settings searching the WebSite directory within the specified root directory.
        /// </summary>
        public class RootDirectoryPortalSettingsProvider : PortalSettingsProvider
        {
            private const string AppDataDirectoryName = "WebSite";

            private static new readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private static readonly Lazy<RootDirectoryPortalSettingsProvider> LazyCurrentDirectoryProviderInstance =
                new Lazy<RootDirectoryPortalSettingsProvider>(() => new RootDirectoryPortalSettingsProvider());

            /// <summary>
            /// Gets the current directory provider instance.
            /// </summary>
            internal static RootDirectoryPortalSettingsProvider CurrentDirectoryProviderInstance
            {
                get
                {
                    return LazyCurrentDirectoryProviderInstance.Value;
                }
            }

            /// <summary>
            /// Gets the <see cref="PortalSettings"/>.
            /// </summary>
            /// <returns>
            /// The <see cref="PortalSettings"/>.
            /// </returns>
            public override PortalSettings GetSettings()
            {
                var appData = this.GetAppDataDirectory();
                var portalSettings = new PortalSettings { AppDataPath = appData.FullName };
                int port;
                if (this.TryGetSetting("HttpPort", out port))
                {
                    portalSettings.HttpPort = port;
                }

                if (this.TryGetSetting("HttpsPort", out port))
                {
                    portalSettings.HttpsPort = port;
                }

                bool enableHttps;
                if (this.TryGetSetting(PredefinedAzureItems.Settings.EnableHttps, out enableHttps))
                {
                    portalSettings.EnableHttps = enableHttps;
                }

                return portalSettings;
            }

            /// <summary>
            /// Gets the directory for the specified path.
            /// </summary>
            /// <param name="directoryFullPath">
            /// The directory full path.
            /// </param>
            /// <returns>
            /// The <see cref="IDirectoryInfo"/>.
            /// </returns>
            /// <exception cref="DirectoryNotFoundException">The directory was not found.</exception>
            protected IDirectoryInfo GetDirectoryInfo(string directoryFullPath)
            {
                IDirectoryInfo directory;
                if (!FileSystemManager.Local.TryGetDirectory(directoryFullPath, out directory))
                {
                    throw new DirectoryNotFoundException(
                        string.Format("The directory '{0}' was not found", directoryFullPath));
                }

                return directory;
            }

            private IDirectoryInfo GetRootDirectory()
            {
                var rootPath = ApplicationHelper.CurrentDirectory;
                return this.GetDirectoryInfo(rootPath);
            }

            private IDirectoryInfo GetAppDataDirectory()
            {
                var directory = this.GetRootDirectory();
                var directoryInfos = directory.GetDirectories();
                var appData = directoryInfos.SingleOrDefault(d => d.Name == AppDataDirectoryName);
                if (appData == null)
                {
                    throw new DirectoryNotFoundException(
                        string.Format(
                            "Directory '{0}' not found under current directory '{1}'",
                            AppDataDirectoryName,
                            ApplicationHelper.CurrentDirectory));
                }

                return appData;
            }
        }
    }
}