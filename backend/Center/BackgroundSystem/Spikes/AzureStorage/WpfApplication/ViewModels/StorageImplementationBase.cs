// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StorageImplementationBase.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StorageImplementationBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.Utility;
    using Gorba.Center.BackgroundSystem.Spikes.AzureStorage.WpfApplication.Utility;

    using Microsoft.Win32;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Table;

    /// <summary>
    /// Defines a base class for storage implementations.
    /// </summary>
    public abstract class StorageImplementationBase : ViewModelBase
    {
        private readonly Lazy<AccountInfo> lazyAccountInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageImplementationBase"/> class.
        /// </summary>
        /// <param name="shell">
        /// The shell.
        /// </param>
        protected StorageImplementationBase(Shell shell)
        {
            this.lazyAccountInfo = new Lazy<AccountInfo>(this.CreateAccountInfo);
            this.UploadResourceCommand = new AwaitableDelegateCommand(this.UploadAsync);
            this.Resources = new ObservableCollection<ResourceBase>();
            this.Shell = shell;
            this.ClearCommand = new AwaitableDelegateCommand(this.ClearAsync);
            this.ReloadCommand = new AwaitableDelegateCommand(this.LoadResourcesAsync);
        }

        /// <summary>
        /// Gets the account info.
        /// </summary>
        public AccountInfo AccountInfo
        {
            get
            {
                return this.lazyAccountInfo.Value;
            }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the reload command.
        /// </summary>
        public ICommand ReloadCommand { get; private set; }

        /// <summary>
        /// Gets the clear command.
        /// </summary>
        public ICommand ClearCommand { get; private set; }

        /// <summary>
        /// Gets the upload resource command.
        /// </summary>
        public ICommand UploadResourceCommand { get; private set; }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        public ObservableCollection<ResourceBase> Resources { get; private set; }

        /// <summary>
        /// Gets the shell.
        /// </summary>
        public Shell Shell { get; private set; }

        /// <summary>
        /// Creates a resource.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="ResourceBase"/>.
        /// </returns>
        protected abstract ResourceBase CreateResource(ResourceEntity entity);

        /// <summary>
        /// Creates an account info.
        /// </summary>
        /// <returns>
        /// The <see cref="AccountInfo"/>.
        /// </returns>
        protected virtual AccountInfo CreateAccountInfo()
        {
            return new AccountInfo();
        }

        /// <summary>
        /// Uploads a resource.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task UploadAsync()
        {
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();
            if (!result.GetValueOrDefault(false))
            {
                return;
            }

            var fileInfo = new FileInfo(openFileDialog.FileName);
            var table = await this.GetTableAsync();
            var hash = ResourceHash.Create(fileInfo.FullName);
            var exists = await this.ResourceExistsAsync(table, hash);
            if (exists)
            {
                MessageBox.Show("Resource already exists");
                return;
            }

            var resource = new ResourceEntity
                               {
                                   PartitionKey = this.Name,
                                   RowKey = hash,
                                   Length = fileInfo.Length,
                                   OriginalFileName = fileInfo.Name
                               };
            await this.UploadResourceAsync(resource, fileInfo.FullName);
            await this.AddResourceAsync(table, resource);
            await this.LoadResourcesAsync();
        }

        /// <summary>
        /// Checks if a resource exists.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="hash">
        /// The hash.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task<bool> ResourceExistsAsync(CloudTable table, string hash)
        {
            var query = TableOperation.Retrieve<ResourceEntity>(this.Name, hash);
            var queryResult = await table.ExecuteAsync(query);
            return queryResult.Result != null;
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
        protected abstract Task UploadResourceAsync(ResourceEntity entity, string fullPath);

        /// <summary>
        /// Adds a resource.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <param name="resource">
        /// The resource.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task AddResourceAsync(CloudTable table, ResourceEntity resource)
        {
            var insertOperation = TableOperation.Insert(resource);
            await table.ExecuteAsync(insertOperation);
        }

        /// <summary>
        /// Loads the resources.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task LoadResourcesAsync()
        {
            var table = await this.GetTableAsync();
            var query = table.ExecuteQuery(new TableQuery<ResourceEntity>());
            var viewModels = query.Select(this.CreateResource).ToList();
            Action<ResourceBase> add = entity =>
                {
                    if (this.Resources.Any(r => r.Hash == entity.Hash))
                    {
                        return;
                    }

                    this.Resources.Add(entity);
                };
            Action updateCollection = () => viewModels.ForEach(add);
            await this.Shell.Dispatcher.InvokeAsync(updateCollection);
            Action cleanupCollection = () =>
                {
                    var except = this.Resources.Where(r => viewModels.All(v => v.Hash != r.Hash));
                    except.ToList().ForEach(r => this.Resources.Remove(r));
                };
            await this.Shell.Dispatcher.InvokeAsync(cleanupCollection);
        }

        /// <summary>
        /// Clears the resources.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task ClearAsync()
        {
            var table = await this.GetTableAsync();
            await this.ClearStorageAsync();
            await this.ClearResourcesAsync(table);
        }

        /// <summary>
        /// Clears the storage.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected abstract Task ClearStorageAsync();

        /// <summary>
        /// Clears the resources from the given table.
        /// </summary>
        /// <param name="table">
        /// The table.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/> that can be awaited.
        /// </returns>
        protected virtual async Task ClearResourcesAsync(CloudTable table)
        {
            await table.DeleteIfExistsAsync();
            await table.CreateAsync();
        }

        /// <summary>
        /// Gets a blob container for the current Account info.
        /// </summary>
        /// <returns>A <see cref="CloudBlobContainer"/> for the current Account info.</returns>
        protected CloudBlobContainer GetCloudBlobContainer()
        {
            var container = this.AccountInfo.GetCloudBlobContainer();
            return container;
        }

        private async Task<CloudTable> GetTableAsync()
        {
            var tableClient = this.AccountInfo.CreateTableClient();

            var table = tableClient.GetTableReference("storagespikeresources");
            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}