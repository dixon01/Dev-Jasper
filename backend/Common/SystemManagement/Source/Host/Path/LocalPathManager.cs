// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalPathManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalPathManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host.Path
{
    using System.IO;

    using Gorba.Common.Utility.Compatibility;

    /// <summary>
    /// Path manager that looks for files in the directory of the application.
    /// </summary>
    internal abstract class LocalPathManager : PathManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocalPathManager"/> class.
        /// </summary>
        protected LocalPathManager()
        {
            var entryAssemblyPath = ApplicationHelper.GetEntryAssemblyLocation();
            this.ApplicationDirectory = entryAssemblyPath != null
                                            ? Path.GetDirectoryName(entryAssemblyPath)
                                            : ApplicationHelper.CurrentDirectory;
        }

        /// <summary>
        /// Gets the application directory.
        /// This property is set in the constructor and never changes.
        /// </summary>
        protected string ApplicationDirectory { get; private set; }

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
        public override string GetPath(FileType type, string filename)
        {
            if (Path.IsPathRooted(filename))
            {
                return this.Exists(filename) ? filename : null;
            }

            var path = Path.Combine(this.ApplicationDirectory, filename);
            return this.Exists(path) ? path : null;
        }

        /// <summary>
        /// Gets the path to the installation directory root.
        /// </summary>
        /// <returns>
        /// The full path to the installation root directory.
        /// </returns>
        public override string GetInstallationRoot()
        {
            throw new DirectoryNotFoundException("Installation root doesn't exist on a local system");
        }

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
        public override string CreatePath(FileType type, string filename)
        {
            if (Path.IsPathRooted(filename))
            {
                this.CreateDirectory(filename);
                return filename;
            }

            var path = Path.Combine(this.ApplicationDirectory, filename);
            this.CreateDirectory(path);
            return path;
        }

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
        public override PathManager GetPathManager(string applicationName)
        {
            throw new DirectoryNotFoundException("Can't find installation root");
        }

        /// <summary>
        /// Creates the directory of the given file name.
        /// If the <see cref="filename"/> ends with a '\', this method will create the
        /// entire path, otherwise it will create the parent directory of the file.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        protected void CreateDirectory(string filename)
        {
            if (filename.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                Directory.CreateDirectory(filename);
            }
            else
            {
                var path = Path.GetDirectoryName(filename);
                if (path != null)
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        /// <summary>
        /// Checks whether the given path is valid and exists.
        /// It can point either to a file or a directory.
        /// </summary>
        /// <param name="path">
        /// The path to check.
        /// </param>
        /// <returns>
        /// True if the path exists; it can be either a file or a directory.
        /// </returns>
        protected bool Exists(string path)
        {
            return !string.IsNullOrEmpty(path) && (File.Exists(path) || Directory.Exists(path));
        }
    }
}