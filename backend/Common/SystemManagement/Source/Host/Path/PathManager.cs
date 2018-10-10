// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PathManager.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PathManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host.Path
{
    using System.IO;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// The path manager is responsible for finding the right path to an
    /// existing or a new file or directory.
    /// </summary>
    public abstract class PathManager
    {
        private static volatile PathManager instance;

        /// <summary>
        /// Gets the single instance of a subclass of this class
        /// valid for the context the application is running in.
        /// </summary>
        public static PathManager Instance
        {
            get
            {
                if (instance != null)
                {
                    return instance;
                }

                lock (typeof(PathManager))
                {
                    if (instance != null)
                    {
                        return instance;
                    }

                    return instance = CreatePathManager();
                }
            }
        }

        /// <summary>
        /// Changes the <see cref="Instance"/> of this path manager.
        /// This should only be done for testing purposes, otherwise you
        /// should rely on the default <see cref="PathManager"/> returned
        /// by <see cref="get_Instance"/>.
        /// </summary>
        /// <param name="pathManager">
        /// The path manager.
        /// </param>
        public static void ChangeInstance(PathManager pathManager)
        {
            instance = pathManager;
        }

        /// <summary>
        /// Gets the path to an existing file.
        /// </summary>
        /// <param name="type">
        /// The type of file for which you want the path.
        /// </param>
        /// <param name="filename">
        /// The file or directory name.
        /// </param>
        /// <returns>
        /// The full path to the file or null if it was not found.
        /// </returns>
        public abstract string GetPath(FileType type, string filename);

        /// <summary>
        /// Gets the installation path.
        /// </summary>
        /// <returns>
        /// The full path to the installation root directory.
        /// </returns>
        public abstract string GetInstallationRoot();

        /// <summary>
        /// Creates the path to a new or existing file.
        /// If the <see cref="filename"/> ends with a '\', this method will create the
        /// entire path, otherwise it will create the parent directory of the file.
        /// This method will not create the file itself.
        /// </summary>
        /// <param name="type">
        /// The type of file for which you want the path.
        /// </param>
        /// <param name="filename">
        /// The file or directory name.
        /// </param>
        /// <returns>
        /// The full path to the file.
        /// This file might not exist after calling this method.
        /// </returns>
        public abstract string CreatePath(FileType type, string filename);

        /// <summary>
        /// Gets a path manager for the given application.
        /// This can be used to figure out paths for other applications
        /// installed in the same installation root.
        /// </summary>
        /// <param name="applicationName">
        /// The application name.
        /// </param>
        /// <returns>
        /// A new <see cref="PathManager"/> to handle paths from the given application.
        /// </returns>
        public abstract PathManager GetPathManager(string applicationName);

        private static PathManager CreatePathManager()
        {
            var entryAssemblyLocation = ApplicationHelper.GetEntryAssemblyLocation();
            if (entryAssemblyLocation == null)
            {
                return new DevelopmentPathManager();
            }

            var applicationDirectory = Path.GetDirectoryName(entryAssemblyLocation);
            var applicationName = Path.GetFileName(applicationDirectory);

            // special case for development: don't use application name handling
            // if we are running from a development directory
            if (applicationName == "Debug" || applicationName == "Release" || applicationName == "bin" || applicationName == "Out")
            {
                return new DevelopmentPathManager();
            }

            // special case for I&T: don't use application name handling
            // if we are running from the I&T directory
            if (applicationName == "Binaries")
            {
                return new IntegrationTestPathManager();
            }

            return new ManagedPathManager();
        }
    }
}
