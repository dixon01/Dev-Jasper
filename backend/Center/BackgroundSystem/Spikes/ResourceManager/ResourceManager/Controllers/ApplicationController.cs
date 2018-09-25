// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ApplicationController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ApplicationController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Spikes.ResourceManager.Controllers
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    using Gorba.Center.BackgroundSystem.Spikes.ResourceManager.ViewModels;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.ServiceModel.Security;
    using Gorba.Common.Update.ServiceModel.Resources;

    using Microsoft.Win32;

    /// <summary>
    /// Defines the application controller.
    /// </summary>
    public class ApplicationController
    {
        private const int BufferSize = 2048;

        private UserCredentials userCredentials = new UserCredentials("gorba", "cc2dd14ae78e4d98b351c4f364be00af");

        /// <summary>
        /// The upload operation.
        /// </summary>
        public enum UploadOperation
        {
            /// <summary>
            /// The initialized.
            /// </summary>
            Initialized = 0,

            /// <summary>
            /// The evaluating fingerprint.
            /// </summary>
            EvaluatingFingerprint = 1,

            /// <summary>
            /// The uploading.
            /// </summary>
            Uploading = 2,

            /// <summary>
            /// The succeeded.
            /// </summary>
            Succeeded = 3,

            /// <summary>
            /// The failed.
            /// </summary>
            Failed = 4
        }

        /// <summary>
        /// Uploads a resource.
        /// </summary>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public async Task Upload()
        {
            SetUploadOperation();
            var openFileDialog = new OpenFileDialog();
            var result = openFileDialog.ShowDialog();

            var resource = await this.DoUpload(openFileDialog.FileName);
            if (resource == null)
            {
                SetUploadOperation(UploadOperation.Failed);
                return;
            }

            this.UpdateRemoteResources(resource);
            SetUploadOperation(UploadOperation.Succeeded, 100);
        }

        /// <summary>
        /// Downloads a resource.
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>A <see cref="Task"/> that can be awaited.</returns>
        public async Task Download(string hash)
        {
            var resource = await this.DoDownload(hash);
            if (resource == null)
            {
                return;
            }

            this.UpdateLocalResources(resource);
        }

        /// <summary>
        /// The create credentials.
        /// </summary>
        /// <param name="credentials">
        /// The credentials.
        /// </param>
        public void CreateCredentials(UserCredentials credentials)
        {
            this.userCredentials = credentials;
        }

        private static void SetUploadOperation(
            UploadOperation operation = UploadOperation.Initialized,
            double progress = 0)
        {
            var shell = DependencyResolver.Current.Get<Shell>();
            switch (operation)
            {
                case UploadOperation.Initialized:
                case UploadOperation.EvaluatingFingerprint:
                case UploadOperation.Uploading:
                    shell.Status.Application = "Busy";
                    break;
                case UploadOperation.Succeeded:
                case UploadOperation.Failed:
                    shell.Status.Application = "Ready";
                    break;
                default:
                    throw new ArgumentOutOfRangeException("operation");
            }

            shell.Status.CurrentOperation = operation.ToString();
            shell.Status.Progress = progress;
        }

        private static void UpdateResources(
            Resource resource,
            ResourceSectionViewModel resourcesSection,
            bool allowDownload = false)
        {
            var resourceViewModel = new ResourceViewModel
                                        {
                                            CanDownload = allowDownload,
                                            Filename = resource.OriginalFilename,
                                            Hash = resource.Hash,
                                            MimeType = "data/image"
                                        };

            var scheduler = DependencyResolver.Current.Get<Dispatcher>();
            scheduler.Invoke(
                () =>
                    {
                        try
                        {
                            resourcesSection.Resources.Add(resourceViewModel);
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                    });
        }

        private static string GetResourcesPath()
        {
            var exe = Assembly.GetEntryAssembly().Location;
            var exeInfo = new FileInfo(exe);
            return Path.Combine(exeInfo.Directory.FullName, "Resources");
        }

        private async Task<Resource> DoUpload(string fileName)
        {
            SetUploadOperation(UploadOperation.EvaluatingFingerprint);
            var hash = ResourceHash.Create(fileName);
            SetUploadOperation(UploadOperation.Uploading);
            var name = Path.GetFileName(fileName);
            var resource = new Resource { OriginalFilename = name, Hash = hash };
            try
            {
                using (var channelScope = ChannelScopeFactory<IResourceService>.Current.Create(this.userCredentials))
                {
                    using (var stream = new FileStream(fileName, FileMode.Open))
                    {
                        var streamedResource = new ResourceUploadRequest { Content = stream, Resource = resource };
                        await channelScope.Channel.UploadAsync(streamedResource);
                        SetUploadOperation(UploadOperation.Uploading, 1);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show("Can't upload the resource: " + exception);
                return null;
            }

            return resource;
        }

        private async Task<Resource> DoDownload(string hash)
        {
            var resourcesPath = GetResourcesPath();
            var resourcesDirectoryInfo = new DirectoryInfo(resourcesPath);
            if (!resourcesDirectoryInfo.Exists)
            {
                resourcesDirectoryInfo.Create();
            }

            try
            {
                using (var channelScope = ChannelScopeFactory<IResourceService>.Current.Create(this.userCredentials))
                {
                    var request = new ResourceDownloadRequest { Hash = hash };
                    var resourceDownloadResult = await channelScope.Channel.DownloadAsync(request);
                    var fileName = hash + ".rx";
                    var path = Path.Combine(resourcesPath, fileName);
                    using (var stream = File.OpenWrite(path))
                    {
                        await resourceDownloadResult.Content.CopyToAsync(stream, BufferSize);
                        return resourceDownloadResult.Resource;
                    }
                }
            }
            catch
            {
                return null;
            }
        }

        private void UpdateLocalResources(Resource resource)
        {
            var shell = DependencyResolver.Current.Get<Shell>();
            UpdateResources(resource, shell.LocalResources);
        }

        private void UpdateRemoteResources(Resource resource)
        {
            var shell = DependencyResolver.Current.Get<Shell>();
            UpdateResources(resource, shell.RemoteResources, true);
        }
    }
}