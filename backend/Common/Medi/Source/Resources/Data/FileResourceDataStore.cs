// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileResourceDataStore.cs" company="Gorba AG">
//   Copyright © 2011-2016 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceStore type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files.Writable;

    using NLog;

    /// <summary>
    /// The resource store responsible to keep track of resources.
    /// </summary>
    internal class FileResourceDataStore : IResourceDataStore
    {
        private const long MegaByte = 1024 * 1024;

        private static readonly Logger Logger = LogHelper.GetLogger<FileResourceDataStore>();

        private readonly Dictionary<ResourceId, StoredResource> resources =
            new Dictionary<ResourceId, StoredResource>();

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(StoredResource));

        private IWritableDirectoryInfo baseDirectory;

        private IWritableFileSystem fileSystem;

        private int currentSetIndex = 0;

        private int maxSizeMb = 0;

        /// <summary>
        /// Initializes the store by loading all necessary data.
        /// </summary>
        /// <param name="baseDir">
        /// The base directory.
        /// </param>
        public void Initialize(IWritableDirectoryInfo baseDir)
        {
            this.Initialize(baseDir, 0);
        }

        /// <summary>
        /// Initializes the store by loading all necessary data.
        /// </summary>
        /// <param name="baseDir">
        /// The base directory.
        /// </param>
        /// <param name="maxSizMb">
        /// Maximum size in MB of this store.
        /// </param>
        public void Initialize(IWritableDirectoryInfo baseDir, int maxSizMb)
        {
            Logger.Debug("Initializing resource store");

            this.baseDirectory = baseDir;
            this.fileSystem = this.baseDirectory.FileSystem;
            this.maxSizeMb = maxSizMb;

            // load all .rin files that contain the "resource database"
            var resourcesItems = this.baseDirectory.GetFiles();
            long totalSize = 0;
            foreach (var file in resourcesItems)
            {
                totalSize += file.Size;
                if (!file.Name.EndsWith(".rin"))
                {
                    continue;
                }

                this.currentSetIndex = System.Math.Max(this.LoadResourceInfo(file), this.currentSetIndex);
            }

            Logger.Info(
                 "There are {0} files in Resource directory of total size {1} MBs",
                 resourcesItems.Length,
                 (double)totalSize / MegaByte);

            // load all .rin.bak files for which we haven't loaded the .rin yet;
            // these were read errors or missing .rin files
            foreach (var file in this.baseDirectory.GetFiles())
            {
                if (!file.Name.EndsWith(".rin.bak"))
                {
                    continue;
                }

                var index = file.Name.IndexOf('.');
                if (index < 0)
                {
                    continue;
                }

                var hash = file.Name.Substring(0, index);
                if (this.resources.ContainsKey(new ResourceId(hash)))
                {
                    continue;
                }

                this.currentSetIndex = System.Math.Max(this.LoadResourceInfo(file), this.currentSetIndex);
            }

            Logger.Trace("Previous highest Priority in resources is {0}", this.currentSetIndex);
            this.Purge();
        }

        /// <summary>
        /// Creates a new <see cref="StoredResource"/>, but doesn't add it to the store yet.
        /// Use this method instead of creating <see cref="StoredResource"/> manually.
        /// </summary>
        /// <param name="id">The resource id.</param>
        /// <returns>A new <see cref="StoredResource"/> that can be used with this store.</returns>
        public IStoredResource Create(ResourceId id)
        {
            return new StoredResource(id);
        }

        /// <summary>
        /// Gets the resource for a given id or null if the resource is not found.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="StoredResource"/> for the given id or null if the resource is not found.
        /// </returns>
        public IStoredResource Get(ResourceId id)
        {
            StoredResource resource;
            this.resources.TryGetValue(id, out resource);
            return resource;
        }

        /// <summary>
        /// Adds the given resource to the store.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <exception cref="ArgumentException">
        /// if a resource with the given <see cref="StoredResource.Id"/>
        /// already exists.
        /// </exception>
        public void Add(IStoredResource resource)
        {
            resource.SetIndex = this.currentSetIndex;
            Logger.Debug("Adding {0}", resource);
            this.resources.Add(resource.Id, (StoredResource)resource);
            this.SaveResourceInfo(resource);
        }

        /// <summary>
        /// Updates the information about the given resource in the database.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        public void Update(IStoredResource resource)
        {
            if (!this.resources.ContainsKey(resource.Id))
            {
                Logger.Debug("Not updating since it's not from this store: {0}", resource);
                return;
            }

            resource.SetIndex = this.currentSetIndex;
            Logger.Debug("Updating {0}", resource);
            this.SaveResourceInfo(resource);
        }

        /// <summary>
        /// Removes the given resource from this database.
        /// </summary>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// True if the resource was successfully removed.
        /// </returns>
        public bool Remove(IStoredResource resource)
        {
            if (!this.resources.Remove(resource.Id))
            {
                return false;
            }

            Logger.Debug("Removing {0}", resource);
            try
            {
                this.fileSystem.GetFile(this.GetInfoFilePath(resource.Id, false)).Delete();
                IWritableFileInfo backupFile;
                if (this.fileSystem.TryGetFile(this.GetInfoFilePath(resource.Id, true), out backupFile))
                {
                    backupFile.Delete();
                }
            }
            catch (Exception ex)
            {
                var message = "Couldn't delete resource information for ";
                Logger.Warn(message);
                Logger.Debug("{0} - {1}", message, ex);
            }

            return true;
        }

        /// <summary>
        /// Gets all resources registered in this store.
        /// </summary>
        /// <returns>
        /// All resources.
        /// </returns>
        public IEnumerable<IStoredResource> GetAll()
        {
            foreach (var resource in this.resources.Values)
            {
                yield return resource;
            }
        }

        /// <summary>
        /// Indicates a new set of resources
        /// </summary>
        public void BeginSet()
        {
            this.Purge();
            this.currentSetIndex++;
        }

        /// <summary>
        /// Indicates the end of new set of resources
        /// </summary>
        public void EndSet()
        {
            // this.Purge();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // nothing to do for now
        }

        /// <summary>
        /// Purges the content according to Maximum Size.
        /// </summary>
        internal void Purge()
        {
            if (this.maxSizeMb <= 0)
            {
                return;
            }

            Logger.Info("Purging unnecessary resources...");
            long totalSize = 0;
            long sizeLimit = this.maxSizeMb * MegaByte;
            foreach (var item in this.resources)
            {
                totalSize += item.Value.Size;
            }

            if (totalSize <= sizeLimit)
            {
                return;
            }

            var resourcesSets = new List<KeyValuePair<int, StoredResource>>(this.resources.Count);
            foreach (var item in this.resources)
            {
                var resource = item.Value;
                resourcesSets.Add(new KeyValuePair<int, StoredResource>(resource.SetIndex, resource));
            }

            resourcesSets.Sort((x, y) => x.Key.CompareTo(y.Key));

            long newSize = totalSize;
            IResourceStore storeResource = new FileResourceStore();
            storeResource.Initialize(this.baseDirectory);
            foreach (var item in resourcesSets)
            {
                var resource = item.Value;
                if (newSize <= sizeLimit)
                {
                    break;
                }

                if (item.Key >= this.currentSetIndex)
                {
                    continue;
                }

                newSize -= resource.Size;
                this.Remove(resource);
                lock (storeResource)
                {
                    storeResource.Remove(resource);
                }
            }

            storeResource.Dispose();
            Logger.Info("Freed {0} MBs", (totalSize - newSize) / (double)MegaByte);
        }

        private int LoadResourceInfo(IWritableFileInfo file)
        {
            StoredResource resource;
            try
            {
                using (var reader = file.OpenText())
                {
                    resource = (StoredResource)this.serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                var message = "Couldn't load information from ";
                Logger.Warn(message);
                Logger.Debug("{0} - {1}", message, ex);
                try
                {
                    file.Delete();
                }
                catch (Exception exception)
                {
                    var msg = "Couldn't delete file " + file.FullName;
                    Logger.Warn(msg);
                    Logger.Debug(msg, exception);
                }

                return 0;
            }

            Logger.Trace("Loaded from {0}: {1}", file.FullName, resource);
            this.resources.Add(resource.Id, resource);
            return resource.SetIndex;
        }

        [SuppressMessage("StyleCopPlus.StyleCopPlusRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private void SaveResourceInfo(IStoredResource resource)
        {
            // the write process is as follows:
            // 1. delete backup .rin.bak
            // 2. move .rin to .rin.bak
            // 3. create new .rin
            // 4. delete old .rin.bak
            // 5. copy new .rin to .rin.bak
            var filename = this.GetInfoFilePath(resource.Id, false);
            var backup = this.GetInfoFilePath(resource.Id, true);

            // 1. delete backup .rin.bak
            IWritableFileInfo backupFile;
            if (this.fileSystem.TryGetFile(backup, out backupFile))
            {
                try
                {
                    backupFile.Delete();
                }
                catch (IOException ex)
                {
                    var message = "Couldn't delete backup file: ";
                    Logger.Warn(message);
                    Logger.Debug("{0} - {1}", message, ex);
                }
            }

            // 2. move .rin to .rin.bak
            IWritableFileInfo originalFile;
            if (this.fileSystem.TryGetFile(filename, out originalFile))
            {
                try
                {
                    originalFile.MoveTo(backup);
                }
                catch (IOException ex)
                {
                    // probably the original file didn't exist
                    var message = "Couldn't move original file to backup: " + filename;
                    Logger.Warn(message);
                    Logger.Debug("{0} - {1}", message, ex);
                }
            }

            // 3. create new .rin
            IWritableFileInfo newFile;
            try
            {
                newFile = this.fileSystem.CreateFile(filename);
                using (var stream = newFile.OpenWrite())
                {
                    this.serializer.Serialize(stream, resource);
                }
            }
            catch (Exception ex)
            {
                var message = "Couldn't write resource file: " + filename;
                Logger.Warn(message);
                Logger.Debug("{0} - {1}", message, ex);
                return;
            }

            // 4. delete old .rin.bak
            if (this.fileSystem.TryGetFile(backup, out backupFile))
            {
                try
                {
                    backupFile.Delete();
                }
                catch (IOException ex)
                {
                    var message = "Couldn't delete temporary backup file: " + backup;
                    Logger.Warn(message);
                    Logger.Debug("{0} - {1}", message, ex);
                }
            }

            // 5. copy new .rin to .rin.bak
            try
            {
                newFile.CopyTo(backup);
            }
            catch (IOException ex)
            {
                var message = "Couldn't create backup file: ";
                Logger.Warn(message);
                Logger.Debug("{0} - {1}", message, ex);
            }
        }

        private string GetInfoFilePath(ResourceId id, bool backup)
        {
            return Path.Combine(
                this.baseDirectory.FullName,
                string.Format("{0}.rin{1}", id.Hash, backup ? ".bak" : string.Empty));
        }
    }
}
