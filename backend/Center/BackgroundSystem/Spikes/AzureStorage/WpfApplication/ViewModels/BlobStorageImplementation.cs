// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobStorageImplementation.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlobStorageImplementation type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the blob storage.
    /// </summary>
    public class BlobStorageImplementation : StorageImplementationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobStorageImplementation"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        public BlobStorageImplementation(Shell shell)
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
                return "blobs";
            }
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
            return new BlobResource(this.AccountInfo, entity.RowKey, entity.OriginalFileName, entity.Length);
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
            var container = this.GetCloudBlobContainer();
            await container.CreateIfNotExistsAsync();
            var pageBlob = container.GetBlockBlobReference(entity.RowKey);
            await pageBlob.UploadFromFileAsync(fullPath, FileMode.Open);
        }

        /// <summary>
        /// Clears the storage.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected override async Task ClearStorageAsync()
        {
            var container = this.GetCloudBlobContainer();
            await container.DeleteIfExistsAsync();
        }
    }
}