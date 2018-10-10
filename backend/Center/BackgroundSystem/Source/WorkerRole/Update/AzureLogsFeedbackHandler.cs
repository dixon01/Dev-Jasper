// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureLogsFeedbackHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The handler for feedback logs temporary stored in an Azure Storage.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.WorkerRole.Update
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Core.Update.Azure;
    using Gorba.Common.Update.ServiceModel.Common;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using NLog;

    /// <summary>
    /// The handler for feedback logs temporary stored in an Azure Storage.
    /// </summary>
    public class AzureLogsFeedbackHandler : LogsFeedbackHandlerBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CloudStorageAccount storageAccount;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureLogsFeedbackHandler"/> class.
        /// </summary>
        public AzureLogsFeedbackHandler()
        {
            this.storageAccount = CloudStorageAccount.Parse(
                 CloudConfigurationManager.GetSetting(
                     Common.Azure.PredefinedAzureItems.Settings.StorageConnectionString));
            var container = this.storageAccount.CreateCloudBlobClient().GetContainerReference("logs");
            container.CreateIfNotExists();
        }

        /// <summary>
        /// Gets all the log files from the directory defined in <see cref="LogsFeedbackHandlerBase.Configure"/>.
        /// </summary>
        /// <returns>
        /// The list of log files related to the unit.
        /// </returns>
        public override async Task<IReceivedLogFile[]> FindLogFilesAsync()
        {
            var container = this.storageAccount.CreateCloudBlobClient().GetContainerReference("logs");
            var logFiles = new List<AzureReceivedLogFile>();
            var unitDirectories =
                (await ListBlobsAsync(container.ListBlobsSegmentedAsync)).OfType<CloudBlobDirectory>();
            foreach (var directory in unitDirectories)
            {
                var unitName = directory.Prefix.Trim('/');
                var logs = (await ListBlobsAsync(directory.ListBlobsSegmentedAsync)).OfType<CloudBlockBlob>();
                foreach (var logBlob in logs)
                {
                    if (logBlob.Name.EndsWith(
                        FileDefinitions.LogFileExtension,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        Logger.Trace("Found new log file: {0}", logBlob.Uri);
                        logFiles.Add(new AzureReceivedLogFile(logBlob, unitName));
                    }
                }
            }

            return logFiles.ToArray();
        }

        /// <summary>
        /// Deletes the given log file from the storage.
        /// </summary>
        /// <param name="logFile">
        /// The log file, must be one returned by <see cref="LogsFeedbackHandlerBase.FindLogFilesAsync"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> to await.
        /// </returns>
        public override async Task DeleteLogFileAsync(IReceivedLogFile logFile)
        {
            var azureLogFile = (AzureReceivedLogFile)logFile;
            Logger.Trace("Deleting log file {0}", azureLogFile.Uri);
            await azureLogFile.DeleteIfExistsAsync();
        }

        private static async Task<IEnumerable<IListBlobItem>> ListBlobsAsync(
            Func<BlobContinuationToken, Task<BlobResultSegment>> listSegmentedAsync)
        {
            BlobContinuationToken continuationToken = null;
            var results = new List<IListBlobItem>();
            do
            {
                var response = await listSegmentedAsync(continuationToken);
                continuationToken = response.ContinuationToken;
                results.AddRange(response.Results);
            }
            while (continuationToken != null);

            return results;
        }

        private class AzureReceivedLogFile : IReceivedLogFile
        {
            private readonly CloudBlockBlob logBlob;

            public AzureReceivedLogFile(CloudBlockBlob logBlob, string unitName)
            {
                this.logBlob = logBlob;
                this.UnitName = unitName;
            }

            public string UnitName { get; private set; }

            public Uri Uri
            {
                get
                {
                    return this.logBlob.Uri;
                }
            }

            public string FileName
            {
                get
                {
                    return Path.GetFileName(this.Uri.AbsolutePath);
                }
            }

            public void CopyTo(string filePath)
            {
                Logger.Trace("Copying file from {0} to {1}", this.Uri, filePath);
                this.logBlob.DownloadToFile(filePath, FileMode.Create);
            }

            public Stream OpenRead()
            {
                Logger.Trace("Opening file for reading: {0}", this.Uri);
                return this.logBlob.OpenRead();
            }

            public Task DeleteIfExistsAsync()
            {
                return this.logBlob.DeleteIfExistsAsync();
            }
        }
    }
}
