// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectFile.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectFile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    using Gorba.Center.Media.Core.Models;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core.IO;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// Defines a Media project file.
    /// </summary>
    internal class ProjectFile : IResourceProvider
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<ResourceMetadata> resources = new List<ResourceMetadata>();

        private IWritableFileInfo projectFile;

        private ProjectFile(IWritableFileInfo projectFile)
        {
            this.projectFile = projectFile;
        }

        /// <summary>
        /// Defines the possible changes done to a resource.
        /// </summary>
        private enum ResourceChangeType
        {
            /// <summary>
            /// No change.
            /// </summary>
            None = 0,

            /// <summary>
            /// The resource was added.
            /// </summary>
            Added = 1,

            /// <summary>
            /// The resource was removed.
            /// </summary>
            Removed = 2
        }

        /// <summary>
        /// Gets or sets the <see cref="MediaProject"/> object.
        /// </summary>
        /// <value>
        /// The <see cref="MediaProject"/> object.
        /// </value>
        public MediaProjectDataModel MediaProject { get; set; }

        /// <summary>
        /// Gets the name of the file (without path).
        /// </summary>
        /// <value>
        /// The name of the file (without path).
        /// </value>
        public string FileName
        {
            get
            {
                return this.projectFile.Name;
            }
        }

        /// <summary>
        /// Gets the full name of the file (including path).
        /// </summary>
        public string FullFileName
        {
            get
            {
                return this.projectFile.FullName;
            }
        }

        /// <summary>
        /// Creates a new project file at the given <paramref name="filename"/>
        /// This will only create an empty file.
        /// </summary>
        /// <param name="filename">
        /// The full filename where to store the project file.
        /// </param>
        /// <returns>
        /// The <see cref="ProjectFile"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">The filename is null.</exception>
        public static ProjectFile CreateNew(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentNullException("filename");
            }

            var projectFile = new ProjectFile(((IWritableFileSystem)FileSystemManager.Local).CreateFile(filename));
            return projectFile;
        }

        /// <summary>
        /// Loads a <see cref="ProjectFile"/> from the given file.
        /// </summary>
        /// <param name="filename">
        /// The full filename where to find the project file.
        /// </param>
        /// <returns>
        /// The fully loaded <see cref="ProjectFile"/>.
        /// </returns>
        public static ProjectFile Load(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                throw new ArgumentException("The filename parameter must be a non-empty string", "filename");
            }

            var projectFile = new ProjectFile(((IWritableFileSystem)FileSystemManager.Local).GetFile(filename));
            projectFile.Load();
            return projectFile;
        }

        /// <summary>
        /// Gets a resource for a given hash.
        /// </summary>
        /// <param name="hash">The hash.</param>
        /// <returns>
        /// The <see cref="IResource"/>.
        /// </returns>
        /// <exception cref="UpdateException">
        /// If the resource couldn't be found or is otherwise invalid.
        /// </exception>
        public IResource GetResource(string hash)
        {
            var existingResource = this.resources.SingleOrDefault(r => r.Hash == hash);
            if (existingResource == null)
            {
                throw new UpdateException(string.Format("Can't find the resource with hash '{0}'", hash));
            }

            return new FileResource(existingResource, this);
        }

        /// <summary>
        /// Adds a resource to the provider.
        /// </summary>
        /// <param name="hash">The expected hash of the resource file (the name from where it was copied).</param>
        /// <param name="resourceFile">The full resource file path.</param>
        /// <param name="deleteFile">
        /// A flag indicating whether the <see cref="resourceFile"/> should be deleted after being registered.
        /// </param>
        /// <exception cref="UpdateException">
        /// If the resource file doesn't match the given hash.
        /// </exception>
        public void AddResource(string hash, string resourceFile, bool deleteFile)
        {
            throw new NotSupportedException("This provider is read-only");
        }

        /// <summary>
        /// Asynchronously saves this project file to the location specified by the FileName property.
        /// The file format is the following:
        /// - Header [sizeof(long)]: pointer to serialized project object
        /// - Resources [variable size]: serialized resources
        /// - Serialized project object [variable size]: Xml serialization of this object
        /// If the file doesn't exist, it is created and resources are added ordered by decreasing size.
        /// If the file exists, the location of the first removed resource (based on the previously saved file) is the
        /// starting point to add:
        /// - new resources
        /// - existing resources that were located after the first removed one
        /// All additional resources are ordered by decreasing size.
        /// </summary>
        /// <param name="localResourceProvider">
        /// The local resource provider.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The local resource provider is null
        /// </exception>
        /// <exception cref="ArgumentException">
        /// This object is passed as local resource provider
        /// </exception>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task SaveAsync(IResourceProvider localResourceProvider)
        {
            if (localResourceProvider == null)
            {
                throw new ArgumentNullException("localResourceProvider");
            }

            if (localResourceProvider == this)
            {
                throw new ArgumentException("Can't use myself as the local resource provider", "localResourceProvider");
            }

            if (this.MediaProject == null)
            {
                throw new InvalidOperationException("Can't save file if the MediaProject property is not set");
            }

            Logger.Info("Saving the project to path '{0}'", this.projectFile.FullName);

            // TODO: use Backup again if we think this makes sense
            ////this.Backup();
            try
            {
                var changes = this.EvaluateChanges(localResourceProvider).ToList();
                var firstModification = FindFirstModificationAndSortChanges(changes);

                // toWrite will only contain the resources that are to be written to the file
                var toWrite = changes.Where(c => c.ChangeType != ResourceChangeType.Removed).ToList();

                // update all resource offsets
                var offset = FileStructure.ResourceSegmentBase;
                offset = UpdateOffsets(toWrite, offset);

                var metadata = new ProjectFileMetadata
                                   {
                                       MediaProject = this.MediaProject,
                                       Resources = toWrite.Select(r => r.Metadata).ToList()
                                   };

                using (var fileStream = this.projectFile.OpenAppend())
                {
                    using (var writer = new BinaryWriter(fileStream))
                    {
                        // write the pointer to the serialized object
                        writer.Seek(0, SeekOrigin.Begin);
                        writer.Write(offset);
                        writer.Flush();
                        var currentOffset = firstModification >= 0 && firstModification < toWrite.Count
                                                ? toWrite[firstModification].Metadata.Offset
                                                : offset;
                        fileStream.Seek(currentOffset, SeekOrigin.Begin);

                        // write all changes (if needed)
                        foreach (var change in toWrite.Where(r => r.Metadata.Offset >= currentOffset))
                        {
                            await WriteResourceAsync(localResourceProvider, change.Metadata, fileStream);
                        }

                        SerializeToXml(metadata, fileStream);

                        Logger.Info(
                            "File '{0}' successfully saved (size: {1})", this.projectFile.FullName, fileStream.Length);
                    }
                }

                this.resources.Clear();
                this.resources.AddRange(metadata.Resources);
            }
            catch (UpdateException)
            {
                this.Restore();
                throw;
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while saving the project file", exception);
                this.Restore();
            }

            this.DeleteBackupIfExists();
        }

        private static void SerializeToXml(ProjectFileMetadata metadata, Stream fileStream)
        {
            var xmlSerializer = new XmlSerializer(metadata.GetType());
            xmlSerializer.Serialize(fileStream, metadata);
            fileStream.SetLength(fileStream.Position);
        }

        private static long UpdateOffsets(IEnumerable<ResourceChange> toWrite, long offset)
        {
            foreach (var change in toWrite)
            {
                change.Metadata.Offset = offset;
                offset += change.Metadata.Size;
            }

            return offset;
        }

        private static int FindFirstModificationAndSortChanges(List<ResourceChange> changes)
        {
            var firstModification = changes.FindIndex(c => c.ChangeType != ResourceChangeType.None);
            if (firstModification >= 0)
            {
                changes.Sort(firstModification, changes.Count - firstModification, new ResourceSizeComparer());
            }

            return firstModification;
        }

        private static async Task WriteResourceAsync(
            IResourceProvider resourceProvider,
            ResourceMetadata metadata,
            Stream output)
        {
            var resource = resourceProvider.GetResource(metadata.Hash);
            using (var input = resource.OpenRead())
            {
                await input.CopyToAsync(output);
            }
        }

        private void Load()
        {
            ProjectFileMetadata metadata;
            using (var fileStream = this.projectFile.OpenRead())
            {
                using (var reader = new BinaryReader(fileStream))
                {
                    var serializedObjectPointer = reader.ReadInt64();
                    fileStream.Seek(serializedObjectPointer, SeekOrigin.Begin);

                    var serializer = new XmlSerializer(typeof(ProjectFileMetadata));
                    metadata = (ProjectFileMetadata)serializer.Deserialize(fileStream);
                }
            }

            this.MediaProject = metadata.MediaProject;
            this.resources.AddRange(metadata.Resources);
        }

        private void Restore()
        {
            Logger.Trace("Restoring backup for file '{0}'", this.projectFile.FullName);
            IWritableFileInfo backupFile;
            if (!this.TryGetBackupPath(out backupFile))
            {
                Logger.Debug("Couldn't find backup to restore for file '{0}'", this.projectFile.FullName);
                return;
            }

            var backupPath = backupFile.FullName;
            var originalPath = this.projectFile.FullName;
            this.projectFile.Delete();
            this.projectFile = backupFile.MoveTo(originalPath);
            Logger.Info("Restored backup '{0}' to file '{1}'", backupPath, originalPath);
        }

        private bool TryGetBackupPath(out IWritableFileInfo backupFileInfo)
        {
            var backupFilePath = this.GetBackupFilePath();
            return this.projectFile.FileSystem.TryGetFile(backupFilePath, out backupFileInfo);
        }

        private string GetBackupFilePath()
        {
            var backupFileName = this.projectFile.Name + this.projectFile.Extension + ".bak";
            var backupFilePath = Path.Combine(this.projectFile.Directory.FullName, backupFileName);
            return backupFilePath;
        }

        private void Backup()
        {
            var backupPath = this.GetBackupFilePath();
            this.projectFile.CopyTo(backupPath);

            Logger.Info("Created backup of file '{0}' at '{1}'", this.projectFile.FullName, backupPath);
        }

        private void DeleteBackupIfExists()
        {
            IWritableFileInfo backupFile;
            if (!this.TryGetBackupPath(out backupFile))
            {
                Logger.Debug("Backup for file '{0}' not found", this.projectFile.FullName);
                return;
            }

            Logger.Debug("Deleting existing backup at '{0}'", this.projectFile.FullName);
            backupFile.Delete();
        }

        private IEnumerable<ResourceChange> EvaluateChanges(IResourceProvider localResourceProvider)
        {
            if (this.resources.Count == 0)
            {
                return this.MediaProject.Resources.Select(r => this.CreateResourceChange(localResourceProvider, r));
            }

            var resourceDictionary = this.MediaProject.Resources.ToDictionary(info => info.Hash, info => info);

            // searching unchanged or removed resources
            var changes =
                this.resources.Select(
                    resource =>
                    new ResourceChange
                        {
                            ChangeType =
                                resourceDictionary.Remove(resource.Hash)
                                    ? ResourceChangeType.None
                                    : ResourceChangeType.Removed,
                            Metadata = resource
                        }).ToList();

            // adding added resources
            changes.AddRange(
                resourceDictionary.Values.Select(
                    resource => this.CreateResourceChange(localResourceProvider, resource)));

            return changes;
        }

        private ResourceChange CreateResourceChange(IResourceProvider localResourceProvider, ResourceInfo resource)
        {
            var file = this.resources.SingleOrDefault(r => r.Hash == resource.Hash);
            long size;
            if (file == null)
            {
                var fileInfo = localResourceProvider.GetResource(resource.Hash);
                using (var stream = fileInfo.OpenRead())
                {
                    size = stream.Length;
                }
            }
            else
            {
                size = file.Size;
            }

            return new ResourceChange
                       {
                           ChangeType = ResourceChangeType.Added,
                           Metadata = new ResourceMetadata { Hash = resource.Hash, Size = size }
                       };
        }

        /// <summary>
        /// Defines the structure of the file.
        /// </summary>
        private static class FileStructure
        {
            /// <summary>
            /// The offset (0-based) where the area containing the resources is starting.
            /// </summary>
            public const long ResourceSegmentBase = sizeof(long);
        }

        private class ResourceSizeComparer : IComparer<ResourceChange>
        {
            public int Compare(ResourceChange x, ResourceChange y)
            {
                return x.Metadata.Size.CompareTo(y.Metadata.Size);
            }
        }

        /// <summary>
        /// Defines a resource in the project file.
        /// </summary>
        private class FileResource : IResource
        {
            private readonly ProjectFile owner;

            public FileResource(ResourceMetadata metadata, ProjectFile owner)
            {
                this.owner = owner;
                this.Metadata = metadata;
            }

            public ResourceMetadata Metadata { get; private set; }

            /// <summary>
            /// Gets the unique MD5 hash of this resource.
            /// </summary>
            public string Hash
            {
                get
                {
                    return this.Metadata.Hash;
                }
            }

            /// <summary>
            /// Copies this resource to the given path.
            /// </summary>
            /// <param name="filePath">The full file path where to copy the resource to.</param>
            public void CopyTo(string filePath)
            {
                var destination = (IWritableFileInfo)FileSystemManager.Local.GetFile(filePath);
                using (var contentStream = this.OpenRead())
                {
                    using (var outputStream = destination.OpenWrite())
                    {
                        contentStream.CopyTo(outputStream);
                    }
                }
            }

            /// <summary>
            /// Opens this resource for reading.
            /// </summary>
            /// <returns>
            /// A stream that allows reading the resource.
            /// </returns>
            public Stream OpenRead()
            {
                var fileStream = this.owner.projectFile.OpenRead();
                fileStream.Seek(this.Metadata.Offset, SeekOrigin.Begin);
                return new ResourceStream(fileStream, this.Metadata.Size);
            }
        }

        /// <summary>
        /// Defines the stream used to read a resource. This stream closes the base stream.
        /// </summary>
        private class ResourceStream : ContentStream
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ResourceStream"/> class.
            /// </summary>
            /// <param name="baseStream">The underlying stream.</param>
            /// <param name="contentLength">The content length.</param>
            public ResourceStream(Stream baseStream, long contentLength)
                : base(baseStream, contentLength)
            {
            }

            /// <summary>
            /// Closes the current stream and releases any resources (such as sockets and file handles)
            /// associated with the current stream.
            /// </summary>
            public override void Close()
            {
                this.BaseStream.Close();
            }
        }

        /// <summary>
        /// Defines the information needed to handle the resource during save process.
        /// </summary>
        private class ResourceChange
        {
            /// <summary>
            /// Gets or sets the resource.
            /// </summary>
            /// <value>
            /// The resource.
            /// </value>
            public ResourceMetadata Metadata { get; set; }

            /// <summary>
            /// Gets or sets the type of the change.
            /// </summary>
            /// <value>
            /// The type of the change.
            /// </value>
            public ResourceChangeType ChangeType { get; set; }
        }
    }
}