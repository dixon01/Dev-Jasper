// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PackageVersionDataController.partial.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PackageVersionDataController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.Entities.Software
{
    using System;
    using System.IO;
    using System.Threading.Tasks;

    using Gorba.Center.Admin.Core.DataViewModels.Software;
    using Gorba.Center.Admin.SoftwareDescription;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;

    /// <summary>
    /// Specific implementation of <see cref="PackageVersionDataController"/>.
    /// </summary>
    public partial class PackageVersionDataController
    {
        // ReSharper disable RedundantAssignment
        partial void PostCreateEntity(PackageVersionDataViewModel dataViewModel)
        {
            dataViewModel.RootFolders.Add(new FolderItem { Name = "Progs" });
        }

        partial void PostSetupReferenceProperties(ref Func<PackageVersionDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PostSetupReferencePropertiesAsync;
        }

        private Task PostSetupReferencePropertiesAsync(PackageVersionDataViewModel dataViewModel)
        {
            if (string.IsNullOrEmpty(dataViewModel.Structure.XmlData.Xml))
            {
                dataViewModel.Structure.XmlData = new XmlData(new SoftwarePackageDescriptor());
            }

            return Task.FromResult(0);
        }

        partial void PostStartEditEntity(PackageVersionDataViewModel dataViewModel)
        {
            var packageDescriptor = dataViewModel.Structure.XmlData.Deserialize() as SoftwarePackageDescriptor;
            if (packageDescriptor == null || packageDescriptor.Version == null
                || packageDescriptor.Version.Structure == null)
            {
                return;
            }

            dataViewModel.RootFolders.Clear();
            foreach (var folder in packageDescriptor.Version.Structure.Folders)
            {
                var item = new FolderItem();
                dataViewModel.RootFolders.Add(item);
                this.CreateFolderItems(item, folder);
            }
        }

        private void CreateFolderItems(FolderItem folderItem, FolderUpdate folderUpdate)
        {
            folderItem.Name = folderUpdate.Name;
            foreach (var fileSystemUpdate in folderUpdate.Items)
            {
                var fileUpdate = fileSystemUpdate as FileUpdate;
                if (fileUpdate != null)
                {
                    var fileItem = new FileItem { Hash = fileUpdate.Hash, Name = fileUpdate.Name };
                    folderItem.Children.Add(fileItem);
                    continue;
                }

                var subFolderUpdate = fileSystemUpdate as FolderUpdate;
                if (subFolderUpdate == null)
                {
                    continue;
                }

                var subFolderItem = new FolderItem();
                folderItem.Children.Add(subFolderItem);
                this.CreateFolderItems(subFolderItem, subFolderUpdate);
            }
        }

        // ReSharper disable once RedundantAssignment
        partial void PreSaveEntity(ref Func<PackageVersionDataViewModel, Task> asyncMethod)
        {
            asyncMethod = this.PreSaveEntityAsync;
        }

        private async Task PreSaveEntityAsync(PackageVersionDataViewModel dataViewModel)
        {
            var folderStructure = new UpdateFolderStructure();
            using (
                var resourceService = this.DataController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                foreach (var folderItem in dataViewModel.RootFolders)
                {
                    var folderUpdate = new FolderUpdate();
                    folderStructure.Folders.Add(folderUpdate);
                    await this.CreateFolderUpdatesAsync(folderUpdate, folderItem, resourceService);
                }
            }

            var package = new SoftwarePackageDescriptor
                              {
                                  Description = dataViewModel.Package.SelectedEntity.Description,
                                  Name = dataViewModel.Package.SelectedEntity.ProductName,
                                  PackageId = dataViewModel.Package.SelectedEntity.PackageId,
                                  Version =
                                      new SoftwareVersion
                                          {
                                              Description = dataViewModel.Description,
                                              VersionNumber = dataViewModel.SoftwareVersion,
                                              Structure = folderStructure
                                          }
                              };
            dataViewModel.Structure.XmlData = new XmlData(package);
        }

        private async Task CreateFolderUpdatesAsync(
            FolderUpdate folderUpdate,
            FolderItem folderItem,
            ChannelScope<IResourceService> resourceService)
        {
            folderUpdate.Name = folderItem.Name;
            foreach (var fileSystemItem in folderItem.Children)
            {
                var fileItem = fileSystemItem as FileItem;
                if (fileItem != null)
                {
                    if (string.IsNullOrEmpty(fileItem.Hash))
                    {
                        await this.UploadFileAsync(resourceService, fileItem);
                    }

                    var fileUpdate = new FileUpdate { Hash = fileItem.Hash, Name = fileItem.Name };
                    folderUpdate.Items.Add(fileUpdate);
                    continue;
                }

                var subFolderItem = fileSystemItem as FolderItem;
                if (subFolderItem == null)
                {
                    continue;
                }

                var subFolderUpdate = new FolderUpdate();
                folderUpdate.Items.Add(subFolderUpdate);
                await this.CreateFolderUpdatesAsync(subFolderUpdate, subFolderItem, resourceService);
            }
        }

        private async Task UploadFileAsync(ChannelScope<IResourceService> resourceService, FileItem fileItem)
        {
            // the file was added by the user
            var fileName = fileItem.OriginalFileName;
            fileItem.Hash = ResourceHash.Create(fileName);
            var found = await resourceService.Channel.GetAsync(fileItem.Hash);
            if (found != null)
            {
                return;
            }

            using (var input = File.OpenRead(fileName))
            {
                await
                    resourceService.Channel.UploadAsync(
                        new ResourceUploadRequest
                            {
                                Content = input,
                                Resource =
                                    new Resource
                                        {
                                            Hash = fileItem.Hash,
                                            Length = input.Length,
                                            UploadingUser = this.ApplicationState.CurrentUser,
                                            OriginalFilename = Path.GetFileName(fileName),
                                            MimeType = "application/octet-stream" // TODO: figure out!
                                        }
                            });
            }
        }
    }
}