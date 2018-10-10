// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileCheck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileCheck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Utility.Core
{
    using System;
    using System.IO;
    using System.Security.Cryptography;

    /// <summary>
    /// Helper class that verifies if a given file has changed.
    /// </summary>
    public class FileCheck
    {
        private readonly FileInfo fileInfo;

        private readonly Mode mode;

        private DateTime lastModified;

        private long length = -1;

        private string hash;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCheck"/> class.
        /// The algorithm will only check if the file exists and if its last modified timestamp has changed.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        public FileCheck(string filename)
            : this(filename, Mode.LastModified)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCheck"/> class.
        /// </summary>
        /// <param name="filename">
        /// The filename.
        /// </param>
        /// <param name="mode">
        /// The <see cref="Mode"/> in which the algorithm should run.
        /// The algorithm will check the given attributes and always also check if the file exists.
        /// </param>
        public FileCheck(string filename, Mode mode)
        {
            this.fileInfo = new FileInfo(filename);
            this.mode = mode;
            this.CheckChanged();
        }

        /// <summary>
        /// The mode with which the file should be checked for changes.
        /// A combination of values is always possible.
        /// </summary>
        [Flags]
        public enum Mode
        {
            /// <summary>
            /// The <see cref="FileSystemInfo.LastWriteTime"/> is checked.
            /// </summary>
            LastModified = 1,

            /// <summary>
            /// The <see cref="FileInfo.Length"/> is checked.
            /// </summary>
            Length = 2,

            /// <summary>
            /// The MD5 hash of the file is computed and compared.
            /// Be aware that this is a pretty resource consuming mode.
            /// </summary>
            Hash = 4
        }

        /// <summary>
        /// Gets a value indicating whether the file exists.
        /// This flag is updated on construction and every time <see cref="CheckChanged"/> is called.
        /// </summary>
        public bool Exists { get; private set; }

        /// <summary>
        /// Checks if the file has changed according to the algorithm defined in .
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CheckChanged()
        {
            this.fileInfo.Refresh();

            if (!this.fileInfo.Exists)
            {
                // if it doesn't exist, then it has changed if it existed
                var existed = this.Exists;
                this.Exists = false;
                return existed;
            }

            var changed = !this.Exists;
            this.Exists = true;

            if ((this.mode & Mode.LastModified) != 0)
            {
                changed |= this.fileInfo.LastWriteTime != this.lastModified;
                this.lastModified = this.fileInfo.LastWriteTime;
            }

            if ((this.mode & Mode.Length) != 0)
            {
                changed |= this.fileInfo.Length != this.length;
                this.length = this.fileInfo.Length;
            }

            if ((this.mode & Mode.Hash) != 0)
            {
                using (var md5 = MD5.Create())
                {
                    using (var input = this.fileInfo.OpenRead())
                    {
                        var newHash = BitConverter.ToString(md5.ComputeHash(input));
                        changed |= newHash != this.hash;
                        this.hash = newHash;
                    }
                }
            }

            return changed;
        }
    }
}
