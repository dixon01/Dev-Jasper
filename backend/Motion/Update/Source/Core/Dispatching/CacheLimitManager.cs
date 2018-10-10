// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheLimitManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Dispatching
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;

    using NLog;

    using Math = System.Math;

    /// <summary>
    /// Class to check the limits on the cache folder regularly and perform deletion when limits are reached
    /// </summary>
    public class CacheLimitManager
    {
        private const long MegaBytes = 1024 * 1024;

        private static readonly Logger Logger = LogHelper.GetLogger<CacheLimitManager>();

        private readonly CacheLimitsConfig config;

        private readonly string poolDirectory;

        private readonly string queueDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheLimitManager"/> class.
        /// </summary>
        /// <param name="cacheLimitsConfig">
        /// The folder limit config.
        /// </param>
        /// <param name="poolDirectory">
        /// The pool directory.
        /// </param>
        /// <param name="queueDirectory">
        /// The queue directory.
        /// </param>
        public CacheLimitManager(CacheLimitsConfig cacheLimitsConfig, string poolDirectory, string queueDirectory)
        {
            this.config = cacheLimitsConfig;
            this.poolDirectory = poolDirectory;
            this.queueDirectory = queueDirectory;
        }

        /// <summary>
        /// Performs folder limit operation by checking the number of files in pool directory
        /// or the free space left in the drive in which the application runs.
        /// All feedback and log files over 30 days old are deleted first.
        /// If the limit still exceeds, all log and feedback files are deleted.
        /// </summary>
        public void PerformFolderLimitOperation()
        {
            int numberOfFilesToBeDeleted = 0;
            if (this.config.NumberOfFiles > 0)
            {
                numberOfFilesToBeDeleted = Math.Max(
                    0,
                    Directory.GetFiles(this.poolDirectory).Length - this.config.NumberOfFiles);
            }

            long spaceToBeFreed = 0;
            long available;
            if (this.config.FreeSpaceMb.HasValue && this.TryGetAvailableDiskSpace(out available))
            {
                // ReSharper disable once PossibleInvalidOperationException
                spaceToBeFreed = Math.Max(0, (this.config.FreeSpaceMb.Value * MegaBytes) - available);
            }

            if (numberOfFilesToBeDeleted <= 0 && spaceToBeFreed <= 0)
            {
                return;
            }

            Logger.Info("Number of files to be deleted: {0}", numberOfFilesToBeDeleted);
            Logger.Info("Space to be freed: {0} bytes", spaceToBeFreed);
            this.CheckAndDeletePoolFiles(numberOfFilesToBeDeleted, spaceToBeFreed);
        }

        private static IDriveInfo FindDrive(string path)
        {
            foreach (var drive in FileSystemManager.Local.GetDrives())
            {
                if (path.StartsWith(
                    drive.RootDirectory.FullName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return drive;
                }
            }

            throw new DirectoryNotFoundException("Couldn't find drive for " + path);
        }

        private bool TryGetAvailableDiskSpace(out long available)
        {
            available = 0;
            try
            {
                var entryAssemblyLocation = ApplicationHelper.GetEntryAssemblyLocation();
                if (entryAssemblyLocation != null)
                {
                    IDriveInfo drive = FindDrive(entryAssemblyLocation);
                    available = drive.AvailableFreeSpace;

                    if (drive.TotalSize == 0)
                    {
                        throw new InvalidOperationException("Total drive size is zero");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex, "Couldn't get drive information");
                return false;
            }

            return true;
        }

        private void CheckAndDeletePoolFiles(int numberOfFilesToDelete, long spaceToFreeUp)
        {
            var files = new DirectoryInfo(this.poolDirectory).GetFiles();
            Array.Sort(files, (f1, f2) => f1.CreationTime.CompareTo(f2.CreationTime));
            var filesToDelete = new List<FileInfo>(files.Length);
            var hashesToDelete = new List<string>(files.Length);
            foreach (var file in files)
            {
                filesToDelete.Add(file);
                hashesToDelete.Add(this.GetHash(file.Name));
                numberOfFilesToDelete--;
                spaceToFreeUp -= file.Length;

                if (numberOfFilesToDelete <= 0 && spaceToFreeUp <= 0)
                {
                    break;
                }
            }

            Logger.Trace("Total number of files to be deleted: {0}", filesToDelete.Count);
            this.DeleteReferences(new DirectoryInfo(this.queueDirectory), hashesToDelete);

            foreach (var poolFile in filesToDelete)
            {
                Logger.Trace("Deleting pool file {0}", poolFile.FullName);
                poolFile.Delete();
            }
        }

        private void DeleteReferences(DirectoryInfo directory, List<string> hashesToDelete)
        {
            foreach (var subDirectory in directory.GetDirectories())
            {
                this.DeleteReferences(subDirectory, hashesToDelete);
            }

            foreach (var referenceFile in directory.GetFiles("*.ref"))
            {
                foreach (var hash in hashesToDelete)
                {
                    if (referenceFile.Name.StartsWith(hash))
                    {
                        Logger.Trace("Deleting reference file {0}", referenceFile.FullName);
                        referenceFile.Delete();
                        break;
                    }
                }
            }
        }

        private string GetHash(string fileName)
        {
            var dot = fileName.IndexOf('.');
            return dot < 0 ? fileName : fileName.Substring(0, dot);
        }
    }
}
