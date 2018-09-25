// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileStorageImplementation.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileStorageImplementation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;

    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.Utility;

    /// <summary>
    /// Defines the file storage.
    /// </summary>
    public class FileStorageImplementation : StorageImplementationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileStorageImplementation"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        public FileStorageImplementation(Shell shell)
            : base(shell)
        {
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public override string Name
        {
            get
            {
                return "files";
            }
        }

        /// <summary>
        /// Creates the account info.
        /// </summary>
        /// <returns>
        /// The <see cref="AccountInfo"/>.
        /// </returns>
        protected override AccountInfo CreateAccountInfo()
        {
            return
                new AccountInfo(
                    "DefaultEndpointsProtocol=https;AccountName=azurestoragespike;AccountKey=8EpbQQD8OJfPAQso1pacCOqmaA"
                    + "+ewmgaj1HXIqJcohriY94kUw4Y7e3QwuA3WATj656oHdTFj2D6WswA6nNT4g==");
        }

        /// <summary>
        /// Creates a resource.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceBase"/>.
        /// </returns>
        protected override ResourceBase CreateResource(ResourceEntity entity)
        {
            return new FileResource(this.AccountInfo, entity.RowKey, entity.OriginalFileName, entity.Length);
        }

        /// <summary>
        /// Uploads a resource.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="fullPath">
        /// The full path.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task UploadResourceAsync(ResourceEntity entity, string fullPath)
        {
            // Get a reference to the file share we created previously.
            var rootDir = this.AccountInfo.GetCloudFileShare();

            var fileName = string.Format("{0}.rx", entity.RowKey);
            var fileInfo = new FileInfo(fullPath);
            var file = rootDir.GetFileReference(fileName);
            if (await file.ExistsAsync())
            {
                MessageBox.Show("File already exists. Deleting it");
                await file.DeleteIfExistsAsync();
            }

            await file.CreateAsync(fileInfo.Length);
            await file.UploadFromFileAsync(fullPath, FileMode.OpenOrCreate);
        }

        /// <summary>
        /// Clears the storage.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ClearStorageAsync()
        {
            var share = this.AccountInfo.GetCloudFileShare();
            var files = share.ListFilesAndDirectories();
            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file.Uri.LocalPath);
                var fileReference = share.GetFileReference(fileName);
                await fileReference.DeleteIfExistsAsync();
                ////share.GetFileReference(file)
            }
        }
    }
}