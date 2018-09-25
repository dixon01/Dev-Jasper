// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureContentResourceStorageClient.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureContentResourceStorageClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Azure
{
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel.Exceptions;
    using Gorba.Center.Portal.Host.Services;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Storage;

    /// <summary>
    /// A content storage client that retrieves the content from Azure (storage).
    /// </summary>
    internal class AzureContentResourceStorageClient : ContentResourceStorageClientBase
    {
        /// <summary>
        /// Retrieves the content stream from the storage.
        /// </summary>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The stream with the content.
        /// </returns>
        /// <exception cref="AzureConfigurationException">The Azure configuration is not correct.</exception>
        protected override async Task<Stream> RetrieveContentAsync(string hash)
        {
            var storageAccount =
               CloudStorageAccount.Parse(
                   CloudConfigurationManager.GetSetting(
                       Common.Azure.PredefinedAzureItems.Settings.StorageConnectionString));

            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference("ContentResources");
            if (!await container.ExistsAsync())
            {
                throw new AzureConfigurationException("The content storage doesn't exist");
            }

            var blockBlobReference = container.GetBlockBlobReference(hash + ".rx");
            var memoryStream = new MemoryStream();
            await blockBlobReference.DownloadToStreamAsync(memoryStream);
            memoryStream.Position = 0;
            return memoryStream;
        }
    }
}