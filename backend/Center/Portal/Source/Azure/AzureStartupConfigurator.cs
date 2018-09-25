// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureStartupConfigurator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Portal.Azure
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Portal.Host;
    using Gorba.Center.Portal.Host.Services;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;

    using Owin;

    using PredefinedAzureItems = Gorba.Center.Common.Azure.PredefinedAzureItems;

    /// <summary>
    /// Configures the portal for Azure.
    /// </summary>
    public class AzureStartupConfigurator : StartupConfigurator.DefaultStartupConfigurator
    {
        private const string ContainerName = "wwwroot";

        /// <summary>
        /// Configures the application.
        /// </summary>
        /// <param name="appBuilder">
        /// The app builder.
        /// </param>
        protected override void ConfigureInternal(IAppBuilder appBuilder)
        {
            var currentDomain = AppDomain.CurrentDomain.BaseDirectory;
            var webSite = Path.Combine(currentDomain, "WebSite");
            var dir = new DirectoryInfo(webSite);
            if (!dir.Exists)
            {
                Trace.TraceError("Directory '{0}' doesn't exist", dir.FullName);
            }

            DependencyResolver.Current.Register<IContentResourceStorageClient>(new AzureContentResourceStorageClient());
            var appData = (IWritableDirectoryInfo)FileSystemManager.Local.GetDirectory(dir.FullName);

            var directories = appData.GetDirectories();
            var staticContent = directories.SingleOrDefault(d => d.Name == "StaticContent");
            if (staticContent == null)
            {
                this.Download(appData);
                staticContent = directories.SingleOrDefault(d => d.Name == "StaticContent");
                if (staticContent == null)
                {
                    throw new Exception("Can't find the static content");
                }
            }

            appBuilder.Use<ClickOnceMiddleware>();
            this.ConfigurePredefinedMiddlewares(appBuilder, staticContent, appData);
        }

        private void HandleBlock(IWritableDirectoryInfo directoryInfo, CloudBlockBlob blockBlob)
        {
            var fileName = Path.GetFileName(blockBlob.Name);

            // ReSharper disable once AssignNullToNotNullAttribute
            var filePath = Path.Combine(directoryInfo.FullName, fileName);
            this.Logger.Debug("Downloading '{0}' to '{1}'", blockBlob.Name, filePath);
            blockBlob.DownloadToFile(filePath, FileMode.Create);
        }

        private void HandleDirectory(IWritableDirectoryInfo parent, CloudBlobDirectory directory)
        {
            var path = new DirectoryInfo(directory.Prefix);
            var fullDirectoryPath = Path.Combine(parent.FullName, path.Name);
            IWritableDirectoryInfo directoryInfo;
            var exists = parent.FileSystem.TryGetDirectory(fullDirectoryPath, out directoryInfo);
            if (!exists)
            {
                directoryInfo.FileSystem.CreateDirectory(directoryInfo.FullName);
            }

            foreach (var blob in directory.ListBlobs())
            {
                var blockBlob = blob as CloudBlockBlob;
                if (blockBlob != null)
                {
                    this.HandleBlock(directoryInfo, blockBlob);
                    continue;
                }

                var dir = blob as CloudBlobDirectory;
                if (dir != null)
                {
                    this.HandleDirectory(directoryInfo, dir);
                }
            }
        }

        private void Download(IWritableDirectoryInfo appData)
        {
            this.Logger.Info("Folder '{0}' doesn't exist. Creating it.", appData.FullName);
            var staticContentPath = Path.Combine(appData.FullName, "StaticContent");
            IWritableDirectoryInfo directoryInfo;
            var exists = appData.FileSystem.TryGetDirectory(staticContentPath, out directoryInfo);
            if (exists)
            {
                directoryInfo.Delete();
            }

            directoryInfo = appData.FileSystem.CreateDirectory(staticContentPath);
            var storageConnectionString =
                CloudConfigurationManager.GetSetting(
                PredefinedAzureItems.Settings.StorageConnectionString);
            var account = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = new CloudBlobClient(account.BlobStorageUri, account.Credentials);
            var container = blobClient.GetContainerReference(ContainerName);
            foreach (var blob in container.ListBlobs(string.Empty, blobListingDetails: BlobListingDetails.Metadata))
            {
                var blockBlob = blob as CloudBlockBlob;
                if (blockBlob != null)
                {
                    this.HandleBlock(directoryInfo, blockBlob);
                    continue;
                }

                var dir = blob as CloudBlobDirectory;
                if (dir != null)
                {
                    this.HandleDirectory(directoryInfo, dir);
                }
            }
        }
    }
}