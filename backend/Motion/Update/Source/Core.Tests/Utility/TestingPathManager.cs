// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestingPathManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TestingPathManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Tests.Utility
{
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.SystemManagement.Host.Path;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Common.Utility.FilesTesting;

    /// <summary>
    /// <see cref="PathManager"/> that can be used for testing things with a
    /// <see cref="TestingFileSystem"/>.
    /// </summary>
    public class TestingPathManager : PathManager
    {
        private readonly Dictionary<FileType, string> typeDirectories = new Dictionary<FileType, string>
                                                                        {
                                                                            { FileType.Application, "Progs" },
                                                                            { FileType.Config, "Config" },
                                                                            { FileType.Log, "Log" },
                                                                            { FileType.Data, "Data" },
                                                                            { FileType.Presentation, "Presentation" },
                                                                        };

        private readonly IWritableDirectoryInfo root;

        private readonly string applicationName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestingPathManager"/> class.
        /// </summary>
        /// <param name="root">
        /// The root directory of the installation.
        /// </param>
        /// <param name="applicationName">
        /// The application name.
        /// The application will "reside" in <code>${root}\Progs\${applicationName}</code>.
        /// </param>
        public TestingPathManager(IWritableDirectoryInfo root, string applicationName)
        {
            this.root = root;
            this.applicationName = applicationName;
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

            return null;
        }

        /// <summary>
        /// Gets the installation path.
        /// </summary>
        /// <returns>
        /// The full path to the installation root directory.
        /// </returns>
        public override string GetInstallationRoot()
        {
            return this.root.FullName;
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
            return new TestingPathManager(this.root, appName);
        }

        private bool Exists(string path)
        {
            IFileInfo file;
            IDirectoryInfo dir;
            return this.root.FileSystem.TryGetFile(path, out file)
                   || this.root.FileSystem.TryGetDirectory(path, out dir);
        }

        private string GetDefaultPath(FileType type, string filename)
        {
            var path = Path.Combine(this.root.FullName, this.typeDirectories[type]);
            if (type != FileType.Presentation && type != FileType.Log)
            {
                // Log and presentation files are a special case: they are not divided into
                // sub-directories but rather contain the files directly
                path = Path.Combine(path, this.applicationName);
            }

            return Path.Combine(path, filename);
        }

        private void CreateDirectory(string filename)
        {
            if (filename.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                this.root.FileSystem.CreateDirectory(filename);
            }
            else
            {
                var path = Path.GetDirectoryName(filename);
                if (path != null)
                {
                    this.root.FileSystem.CreateDirectory(path);
                }
            }
        }
    }
}
