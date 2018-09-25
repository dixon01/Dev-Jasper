// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagedPathManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ManagedPathManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Host.Path
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The path manager for the managed directory layout.
    /// This layout is used with system manager.
    /// The files are structured as follows:
    /// <code>
    /// + Root
    ///   + Progs
    ///      + [application name]
    ///        - *.exe
    ///        - *.dll
    ///   + Config
    ///      + [application name]
    ///        - *.config
    ///   + Data
    ///      + [application name]
    ///        - *.xml
    ///        - *.db
    ///   + Logs
    ///      - [application name].log
    ///   + Presentation
    ///      - *.*
    ///   + Database
    ///      - *.*
    /// </code>
    /// </summary>
    internal class ManagedPathManager : LocalPathManager
    {
        private readonly Dictionary<FileType, string> typeDirectories = new Dictionary<FileType, string>
                                                                        {
                                                                            { FileType.Application, "Progs" },
                                                                            { FileType.Config, "Config" },
                                                                            { FileType.Log, "Log" },
                                                                            { FileType.Data, "Data" },
                                                                            { FileType.Presentation, "Presentation" },
                                                                            { FileType.Database, "Database" },
                                                                        };

        private readonly string applicationName;
        private readonly string installationRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedPathManager"/> class.
        /// </summary>
        public ManagedPathManager()
            : this(null)
        {
        }

        private ManagedPathManager(string applicationName)
        {
            this.applicationName = applicationName ?? Path.GetFileName(this.ApplicationDirectory);
            this.installationRoot = Path.GetDirectoryName(this.ApplicationDirectory);
            if (this.installationRoot != null)
            {
                this.installationRoot = Path.GetDirectoryName(this.installationRoot);
            }

            if (this.installationRoot == null)
            {
                this.installationRoot = this.ApplicationDirectory;
            }
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
        public override string GetPath(FileType type, string filename)
        {
            if (Path.IsPathRooted(filename))
            {
                return this.Exists(filename) ? filename : null;
            }

            var path = this.GetDefaultPath(type, filename);
            if (this.Exists(path))
            {
                return path;
            }

            return base.GetPath(type, filename);
        }

        /// <summary>
        /// Gets the path to the installation directory root.
        /// </summary>
        /// <returns>
        /// The full path to the installation root directory.
        /// </returns>
        public override string GetInstallationRoot()
        {
            return this.installationRoot;
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

            var path = this.GetDefaultPath(type, filename);

            this.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Gets a path manager for the given application.
        /// This can be used to figure out paths for other applications
        /// installed in the same installation root.
        /// </summary>
        /// <param name="appName">
        /// The application name.
        /// </param>
        /// <returns>
        /// A new <see cref="PathManager"/> to handle paths from the given application.
        /// </returns>
        public override PathManager GetPathManager(string appName)
        {
            return new ManagedPathManager(appName);
        }

        private string GetDefaultPath(FileType type, string filename)
        {
            var path = Path.Combine(this.installationRoot, this.typeDirectories[type]);
            if (type != FileType.Presentation && type != FileType.Database && type != FileType.Log)
            {
                // Database, log and presentation files are a special case: they are not divided into
                // sub-directories but rather contain the files directly
                path = Path.Combine(path, this.applicationName);
            }

            return Path.Combine(path, filename);
        }
    }
}