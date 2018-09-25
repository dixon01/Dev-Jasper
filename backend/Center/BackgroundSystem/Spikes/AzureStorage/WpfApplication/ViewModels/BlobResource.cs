// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlobResource.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BlobResource type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.Utility;

    /// <summary>
    /// Defines a blob resource.
    /// </summary>
    public class BlobResource : ResourceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BlobResource"/> class.
        /// </summary>
        /// <param name="accountInfo">
        /// The account info.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <param name="originalFileName">
        /// The original file name.
        /// </param>
        /// <param name="length">
        /// The length.
        /// </param>
        public BlobResource(AccountInfo accountInfo, string hash, string originalFileName, long length)
            : base(accountInfo, hash, originalFileName, length)
        {
        }

        /// <summary>
        /// Downloads a resource.
        /// </summary>
        /// <param name="open">
        /// The open.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected override async Task DownloadResourceAsync(bool open = false)
        {
            var client = this.AccountInfo.GetCloudBlobContainer();
            var blockBlobReference = client.GetBlockBlobReference(this.Hash);
            var fullPath = this.GetFullTempPath();
            await blockBlobReference.DownloadToFileAsync(fullPath, FileMode.CreateNew);
            if (!open)
            {
                return;
            }

            Process.Start(fullPath);
        }
    }
}