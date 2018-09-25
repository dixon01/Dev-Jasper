// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationStateManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationStateManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Framework.Services
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// Loads and persists the application state.
    /// </summary>
    public abstract class ApplicationStateManager
    {
        static ApplicationStateManager()
        {
            ResetCurrent();
        }

        /// <summary>
        /// Gets the current factory.
        /// </summary>
        public static ApplicationStateManager Current { get; private set; }

        /// <summary>
        /// Resets the current factory instance to the default one.
        /// </summary>
        public static void ResetCurrent()
        {
            SetCurrent(DefaultApplicationStateManager.Instance);
        }

        /// <summary>
        /// Sets the current factory instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public static void SetCurrent(ApplicationStateManager instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "The application state manager instance can't be null");
            }

            Current = instance;
        }

        /// <summary>
        /// Loads the <typeparamref name="T"/> object from user state.
        /// </summary>
        /// <typeparam name="T">The type of the state object.</typeparam>
        /// <param name="application">The application.</param>
        /// <param name="name">The name of the object to load.</param>
        /// <param name="knownTypes">The types of application specific option groups</param>
        /// <returns>
        /// The <typeparamref name="T"/> object loaded from user state.
        /// </returns>
        public abstract T Load<T>(string application, string name, Type[] knownTypes = null)
            where T : class;

        /// <summary>
        /// Saves the state in user local directory.
        /// </summary>
        /// <param name="application">The application.</param>
        /// <param name="name">The name of the object to save.</param>
        /// <param name="state">The state to save.</param>
        /// <param name="knownTypes">The types of application specific option groups</param>
        public abstract void Save(string application, string name, object state, Type[] knownTypes = null);

        /// <summary>
        /// Default implementation of the <see cref="ApplicationStateManager"/>.
        /// </summary>
        internal sealed class DefaultApplicationStateManager : ApplicationStateManager
        {
            private const string Extension = ".state";

            private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

            private static readonly Lazy<DefaultApplicationStateManager> LazyInstance =
                new Lazy<DefaultApplicationStateManager>(CreateInstance);

            private DataContractSerializer serializer;

            /// <summary>
            /// Gets the instance.
            /// </summary>
            /// <value>
            /// The instance.
            /// </value>
            public static DefaultApplicationStateManager Instance
            {
                get
                {
                    return LazyInstance.Value;
                }
            }

            /// <summary>
            /// Loads the <typeparamref name="T"/> object from user state.
            /// </summary>
            /// <typeparam name="T">
            /// The type of the state object.
            /// </typeparam>
            /// <param name="application">
            /// The application.
            /// </param>
            /// <param name="name">
            /// The name of the object to load.
            /// </param>
            /// <param name="knownTypes">
            /// The types of application specific option groups
            /// </param>
            /// <returns>
            /// The <typeparamref name="T"/> object loaded from user state.
            /// </returns>
            public override T Load<T>(string application, string name, Type[] knownTypes = null)
            {
                Logger.Debug("Request to load '{0}'", name);
                try
                {
                    var stateFileInfo = GetStatePath(application, name);

                    Logger.Trace("Attempt to load the file name '{0}' from path '{1}'", name, stateFileInfo.FullName);
                    if (this.serializer == null)
                    {
                        if (knownTypes == null)
                        {
                            Logger.Trace("Creating DataContractSerializer without additional types.");
                            this.serializer = new DataContractSerializer(typeof(T));
                        }
                        else
                        {
                            Logger.Trace(
                                "Creating DataContractSerializer with {0} additional types", knownTypes.Length);
                            this.serializer = new DataContractSerializer(typeof(T), knownTypes);
                        }
                    }

                    using (
                        var fileStream = new FileStream(
                            stateFileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        var settings = (T)this.serializer.ReadObject(fileStream);
                        Logger.Info("Returning state from path '{0}': {1}", stateFileInfo.FullName, settings);
                        return settings;
                    }
                }
                catch (Exception exception)
                {
                    throw new ApplicationException(
                        "An error occurred while loading the state. Please check the inner exception for details",
                        exception);
                }
            }

            /// <summary>
            /// Saves the state in user local directory.
            /// </summary>
            /// <param name="application">The application.</param>
            /// <param name="name">The name of the object to save.</param>
            /// <param name="state">The state to save.</param>
            /// <param name="knownTypes">The types of application specific option groups</param>
            public override void Save(string application, string name, object state, Type[] knownTypes = null)
            {
                Logger.Debug("Request to save state {0} with name '{1}'", state, name);
                try
                {
                    var stateFileInfo = GetStatePath(application, name);

                    Logger.Trace("Attempt to save settings '{0}' to path '{1}'", name, stateFileInfo.FullName);
                    if (this.serializer == null)
                    {
                        if (knownTypes == null)
                        {
                            Logger.Trace("Creating DataContractSerializer without additional types.");
                            this.serializer = new DataContractSerializer(state.GetType());
                        }
                        else
                        {
                            Logger.Trace(
                                "Creating DataContractSerializer with {0} additional types", knownTypes.Length);
                            this.serializer = new DataContractSerializer(state.GetType(), knownTypes);
                        }
                    }

                    using (
                        var fileStream = new FileStream(
                            stateFileInfo.FullName, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        this.serializer.WriteObject(fileStream, state);
                        Logger.Info("Saved state to path '{0}'", stateFileInfo.FullName);
                    }
                }
                catch (Exception exception)
                {
                    throw new ApplicationException(
                        "An error occurred while saving the state. Please check the inner exception for details",
                        exception);
                }
            }

            private static DefaultApplicationStateManager CreateInstance()
            {
                return new DefaultApplicationStateManager();
            }

            private static IDirectoryInfo CreateDirectory(string directoryPath)
            {
                Logger.Trace("Creating directory '{0}'", directoryPath);
                var writableFileSystem = FileSystemManager.Local as IWritableFileSystem;
                if (writableFileSystem == null)
                {
                    throw new ApplicationException("The file system is read-only");
                }

                Logger.Debug("Created directory '{0}'", directoryPath);
                return writableFileSystem.CreateDirectory(directoryPath);
            }

            private static IFileInfo CreateFile(string filePath)
            {
                Logger.Trace("Creating file '{0}'", filePath);
                var writableFileSystem = FileSystemManager.Local as IWritableFileSystem;
                if (writableFileSystem == null)
                {
                    throw new ApplicationException("The file system is read-only");
                }

                Logger.Debug("Created file '{0}'", filePath);
                return writableFileSystem.CreateFile(filePath);
            }

            private static IFileInfo GetStatePath(string application, string name)
            {
                Logger.Trace("Getting path for state '{0}' of application '{1}'", name, application);
                var fileName = name + Extension;
                var localApplicationData =
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

                IDirectoryInfo localApplicationDataDirectory;
                if (!FileSystemManager.Local.TryGetDirectory(localApplicationData, out localApplicationDataDirectory))
                {
                    throw new DirectoryNotFoundException("Couldn't find local application data directory");
                }

                Logger.Trace("Local application data directory '{0}'", localApplicationDataDirectory.FullName);

                IDirectoryInfo gorbaDirectory;
                var gorbaDirectoryPath = Path.Combine(localApplicationDataDirectory.FullName, "Gorba");
                if (!FileSystemManager.Local.TryGetDirectory(gorbaDirectoryPath, out gorbaDirectory))
                {
                    gorbaDirectory = CreateDirectory(gorbaDirectoryPath);
                }

                Logger.Trace("Gorba application data directory '{0}'", gorbaDirectory.FullName);

                IDirectoryInfo applicationDirectory;
                var applicationDirectoryPath = Path.Combine(gorbaDirectory.FullName, application);
                if (!FileSystemManager.Local.TryGetDirectory(applicationDirectoryPath, out applicationDirectory))
                {
                    applicationDirectory = CreateDirectory(applicationDirectoryPath);
                }

                Logger.Trace("Application data directory '{0}'", applicationDirectory.FullName);

                var filePath = Path.Combine(applicationDirectory.FullName, fileName);
                IFileInfo fileInfo;
                if (!FileSystemManager.Local.TryGetFile(filePath, out fileInfo))
                {
                    fileInfo = CreateFile(filePath);
                }

                Logger.Info("Returning state path '{0}'", fileInfo.FullName);
                return fileInfo;
            }
        }
    }
}