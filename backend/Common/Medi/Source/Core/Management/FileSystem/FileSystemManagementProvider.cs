// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileSystemManagementProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileSystemManagementProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Management.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using Gorba.Common.Medi.Core.Management.Provider;

    /// <summary>
    /// Management provider for file system information.
    /// </summary>
    public partial class FileSystemManagementProvider : ManagementObjectProviderBase
    {
        public static readonly string RootName = "FileSystem";

        public static readonly string TypePropertyName = "Type";
        public static readonly string LastModifiedPropertyName = "Modified";
        public static readonly string SizePropertyName = "Size";

        public static readonly string TypeValueFile = "File";
        public static readonly string TypeValueDirectory = "Directory";

        private readonly string path;

        private FileSystemManagementProvider(string path, string name, IManagementProvider parent)
            : base(name, parent)
        {
            this.path = path;
        }

        /// <summary>
        /// Gets all file system entries if this is a directory.
        /// </summary>
        public override IEnumerable<IManagementProvider> Children
        {
            get
            {
                return this.GetChildren();
            }
        }

        /// <summary>
        /// Gets the properties: Type, Size and Modified
        /// </summary>
        public override IEnumerable<ManagementProperty> Properties
        {
            get
            {
                bool isDirectory = Directory.Exists(this.path);
                yield return
                    new ManagementProperty<string>(
                        TypePropertyName,
                        isDirectory ? TypeValueDirectory : TypeValueFile,
                        true);

                if (isDirectory)
                {
                    string modified;

                    try
                    {
                        var info = new DirectoryInfo(this.path);
                        modified = info.LastWriteTime.ToString("G", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        yield break;
                    }

                    yield return new ManagementProperty<string>(LastModifiedPropertyName, modified, true);
                }
                else
                {
                    long size;
                    string modified;

                    try
                    {
                        var info = new FileInfo(this.path);
                        size = info.Length;
                        modified = info.LastWriteTime.ToString("G", CultureInfo.InvariantCulture);
                    }
                    catch (Exception)
                    {
                        yield break;
                    }

                    yield return new ManagementProperty<long>(SizePropertyName, size, true);
                    yield return new ManagementProperty<string>(LastModifiedPropertyName, modified, true);
                }
            }
        }

        private IEnumerable<IManagementProvider> GetChildren()
        {
            if (!Directory.Exists(this.path))
            {
                yield break;
            }

            FileSystemManagementProvider child;

            string[] children;
            try
            {
                children = Directory.GetDirectories(this.path);
            }
            catch (Exception)
            {
                children = null;
            }

            if (children != null)
            {
                foreach (var dir in children)
                {
                    try
                    {
                        child = new FileSystemManagementProvider(dir, Path.GetFileName(dir), this);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    yield return child;
                }
            }

            try
            {
                children = Directory.GetFiles(this.path);
            }
            catch (Exception)
            {
                children = null;
            }

            if (children != null)
            {
                foreach (var file in children)
                {
                    try
                    {
                        child = new FileSystemManagementProvider(file, Path.GetFileName(file), this);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    yield return child;
                }
            }
        }
    }
}
