// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountInfo.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AccountInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.Utility
{
    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels;

    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.File;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// The account info.
    /// </summary>
    public class AccountInfo : ViewModelBase
    {
        private string accountConnectionString;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountInfo"/> class.
        /// </summary>
        /// <param name="storageConnectionString">
        /// The storage connection string.
        /// </param>
        public AccountInfo(string storageConnectionString = "UseDevelopmentStorage=true")
        {
            this.AccountConnectionString = storageConnectionString;
        }

        /// <summary>
        /// Gets or sets the account connection string.
        /// </summary>
        public string AccountConnectionString
        {
            get
            {
                return this.accountConnectionString;
            }

            set
            {
                if (value == this.accountConnectionString)
                {
                    return;
                }

                this.accountConnectionString = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Creates a table client.
        /// </summary>
        /// <returns>
        /// The <see cref="CloudTableClient"/>.
        /// </returns>
        public CloudTableClient CreateTableClient()
        {
            var storageAccount = this.GetCloudStorageAccount();
            return storageAccount.CreateCloudTableClient();
        }

        /// <summary>
        /// Creates a cloud blob client.
        /// </summary>
        /// <returns>
        /// The <see cref="CloudBlobClient"/>.
        /// </returns>
        public CloudBlobClient CreateCloudBlobClient()
        {
            var storageAccount = this.GetCloudStorageAccount();
            return storageAccount.CreateCloudBlobClient();
        }

        /// <summary>
        /// Gets a cloud blob container.
        /// </summary>
        /// <returns>
        /// The <see cref="CloudBlobContainer"/>.
        /// </returns>
        public CloudBlobContainer GetCloudBlobContainer()
        {
            var blobStorage = this.CreateCloudBlobClient();
            var container = blobStorage.GetContainerReference("resources");
            return container;
        }

        /// <summary>
        /// Gets a cloud file client.
        /// </summary>
        /// <returns>
        /// The <see cref="CloudFileClient"/>.
        /// </returns>
        public CloudFileClient GetCloudFileClient()
        {
            var storage = this.GetCloudStorageAccount();
            return storage.CreateCloudFileClient();
        }

        /// <summary>
        /// Gets a cloud file share.
        /// </summary>
        /// <returns>
        /// The <see cref="CloudFileDirectory"/>.
        /// </returns>
        public CloudFileDirectory GetCloudFileShare()
        {
            var client = this.GetCloudFileClient();
            return client.GetShareReference("resources").GetRootDirectoryReference();
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            var storageAccount = CloudStorageAccount.Parse(this.AccountConnectionString);
            return storageAccount;
        }
    }
}