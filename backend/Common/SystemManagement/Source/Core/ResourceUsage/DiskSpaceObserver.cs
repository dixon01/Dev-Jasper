// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DiskSpaceObserver.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DiskSpaceObserver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Core.ResourceUsage
{
    using System;
    using System.IO;

    using Gorba.Common.Configuration.SystemManager.Limits;
    using Gorba.Common.Protocols.Alarming;
    using Gorba.Common.Utility.Files;

    using NLog;

    /// <summary>
    /// Class observing the space on a single disk and taking
    /// configurable action if the space is exceeded.
    /// </summary>
    public class DiskSpaceObserver
    {
        private const double MegaBytes = 1024 * 1024;
        private const int DiskExceedMaxCount = 3;

        private readonly Logger logger;
        private readonly DiskLimitConfig config;

        private readonly LimitController limitController;

        private int diskSpaceLimitExceededCounter;
        private int diskPercentageLimitExceededCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiskSpaceObserver"/> class.
        /// </summary>
        /// <param name="config">
        /// The config.
        /// </param>
        public DiskSpaceObserver(DiskLimitConfig config)
        {
            this.logger = LogManager.GetLogger(string.Format("{0}-{1}:\\", this.GetType().FullName, config.Path[0]));
            this.config = config;

            this.limitController = new LimitController(config, ApplicationRelaunchAttribute.DiskFull, null);
        }

        /// <summary>
        /// Checks the space left on the disk and executes the next configured action
        /// if needed.
        /// </summary>
        public void Verify()
        {
            IDriveInfo drive;
            long available;
            long total;
            try
            {
                drive = FindDrive(this.config.Path);
                available = drive.AvailableFreeSpace;
                total = drive.TotalSize;

                if (total == 0)
                {
                    throw new InvalidOperationException("Total drive size is zero");
                }
            }
            catch (Exception ex)
            {
                this.logger.Error(ex, "Couldn't get drive information");
                return;
            }

            var freePercentage = (double)available / total;

            this.logger.Trace(
                "Disk usage: {0:0.00}MB / {1:0.00}MB free ({2:0.00}%)",
                available / MegaBytes,
                total / MegaBytes,
                freePercentage * 100);

            if (this.config.FreeSpaceMb != null && available < this.config.FreeSpaceMb * MegaBytes)
            {
                this.diskSpaceLimitExceededCounter++;
                this.logger.Warn(
                    "Free disk space below limit {0:0.00}MB < {1:0.00}MB ({2}/{3})",
                    available / MegaBytes,
                    this.config.FreeSpaceMb,
                    this.diskSpaceLimitExceededCounter,
                    DiskExceedMaxCount);
                if (this.diskSpaceLimitExceededCounter >= DiskExceedMaxCount)
                {
                    this.diskSpaceLimitExceededCounter = 0;
                    this.diskPercentageLimitExceededCounter = 0;
                    this.limitController.ExecuteNextAction(
                        string.Format(
                            "Disk {0} @ {1:0.00}MB free", drive.RootDirectory.FullName, available / MegaBytes),
                        true);
                    return;
                }
            }

            if (this.config.FreeSpacePercentage != null && freePercentage < this.config.FreeSpacePercentage * 0.01)
            {
                this.diskPercentageLimitExceededCounter++;
                this.logger.Warn(
                    "Free disk space below limit {0:0.00}% ({1:0.00}MB / {2:0.00}MB) < {3:0.00}% ({4}/{5})",
                    freePercentage * 100,
                    available / MegaBytes,
                    total / MegaBytes,
                    this.config.FreeSpacePercentage,
                    this.diskPercentageLimitExceededCounter,
                    DiskExceedMaxCount);
                if (this.diskPercentageLimitExceededCounter >= DiskExceedMaxCount)
                {
                    this.diskSpaceLimitExceededCounter = 0;
                    this.diskPercentageLimitExceededCounter = 0;
                    this.limitController.ExecuteNextAction(
                        string.Format("Disk {0} @ {1:0.00}% free", drive.RootDirectory.FullName, freePercentage * 100),
                        true);
                }
            }
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
    }
}