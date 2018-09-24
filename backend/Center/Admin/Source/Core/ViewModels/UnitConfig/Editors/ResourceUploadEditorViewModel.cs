// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceUploadEditorViewModel.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceUploadEditorViewModel type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.ViewModels.UnitConfig.Editors
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Common.Client;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    using HashUtility = Gorba.Common.Update.ServiceModel.Resources.ResourceHash;

    /// <summary>
    /// The view model for a file upload control that will create resources in the Background System.
    /// </summary>
    public class ResourceUploadEditorViewModel : EditorViewModelBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ChannelScope<IResourceService> resourceService;

        private string resourceHash;

        private bool isLoading;

        private ImageSource previewImage;

        private string fileFilters;

        private string filename;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceUploadEditorViewModel"/> class.
        /// </summary>
        /// <param name="resourceService">
        /// A <see cref="ChannelScope{T}"/> for the resource service.
        /// </param>
        public ResourceUploadEditorViewModel(ChannelScope<IResourceService> resourceService)
        {
            this.resourceService = resourceService;
            this.FileFilters = AdminStrings.FileDialog_AllFilesFilter;
            this.UploadFileCommand = new RelayCommand(this.UploadFile);
        }

        /// <summary>
        /// Gets the upload file command.
        /// </summary>
        public ICommand UploadFileCommand { get; private set; }

        /// <summary>
        /// Gets or sets the resource hash.
        /// </summary>
        public string ResourceHash
        {
            get
            {
                return this.resourceHash;
            }

            set
            {
                if (!this.SetProperty(ref this.resourceHash, value, () => this.ResourceHash))
                {
                    return;
                }

                this.MakeDirty();
                this.UpdateResource(value);
            }
        }

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public string Filename
        {
            get
            {
                return this.filename ?? this.ResourceHash;
            }

            private set
            {
                this.SetProperty(ref this.filename, value, () => this.Filename);
            }
        }

        /// <summary>
        /// Gets the preview image source.
        /// </summary>
        public ImageSource PreviewImage
        {
            get
            {
                return this.previewImage;
            }

            private set
            {
                this.SetProperty(ref this.previewImage, value, () => this.PreviewImage);
            }
        }

        /// <summary>
        /// Gets or sets the file filters.
        /// </summary>
        public string FileFilters
        {
            get
            {
                return this.fileFilters;
            }

            set
            {
                this.SetProperty(ref this.fileFilters, value, () => this.FileFilters);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this editor is loading (i.e. uploading or downloading a resource).
        /// </summary>
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            private set
            {
                this.SetProperty(ref this.isLoading, value, () => this.IsLoading);
            }
        }

        private async void UpdateResource(string hash)
        {
            this.Filename = null;
            this.PreviewImage = null;
            this.ClearErrors("Resource");
            if (string.IsNullOrEmpty(hash))
            {
                return;
            }

            try
            {
                this.IsLoading = true;
                var resource = await this.resourceService.Channel.GetAsync(hash);
                if (hash != this.resourceHash)
                {
                    return;
                }

                if (resource != null
                    && (resource.ThumbnailHash != null
                    || (resource.MimeType != null && resource.MimeType.StartsWith("image/"))))
                {
                    var response =
                        await
                        this.resourceService.Channel.DownloadAsync(
                            new ResourceDownloadRequest { Hash = resource.ThumbnailHash ?? hash });
                    if (hash != this.resourceHash)
                    {
                        return;
                    }

                    if (this.filename == null)
                    {
                        this.Filename = resource.OriginalFilename;
                    }

                    using (var input = response.Content)
                    {
                        var tcs = new TaskCompletionSource<int>();
                        var bitmap = new BitmapImage();
                        bitmap.DownloadCompleted += (s, e) => tcs.TrySetResult(0);
                        bitmap.DownloadFailed += (s, e) => tcs.TrySetException(e.ErrorException);
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.CreateOptions = BitmapCreateOptions.None;
                        bitmap.StreamSource = input;
                        bitmap.EndInit();

                        await tcs.Task;
                        bitmap.Freeze();

                        this.PreviewImage = bitmap;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't update resource for hash " + hash);
                this.SetError("Resource", ErrorState.Error, ex.Message);
            }

            this.IsLoading = false;
        }

        private void UploadFile()
        {
            var interaction = new OpenFileDialogInteraction
                                  {
                                      Filter = this.FileFilters,
                                      MultiSelect = false,
                                      Title = this.Label
                                  };

            InteractionManager<OpenFileDialogInteraction>.Current.Raise(interaction, this.FileSelected);
        }

        private async void FileSelected(OpenFileDialogInteraction interaction)
        {
            if (!interaction.Confirmed)
            {
                return;
            }

            this.IsLoading = true;
            this.ClearErrors("Resource");
            try
            {
                var selectedFilename = Path.GetFileName(interaction.FileName);
                this.Filename = selectedFilename;
                var hash = await Task.Run(() => HashUtility.Create(interaction.FileName));
                var found = await this.resourceService.Channel.GetAsync(hash);
                if (found != null)
                {
                    this.IsLoading = false;
                    this.ResourceHash = found.Hash;
                    return;
                }

                var applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
                using (var input = File.OpenRead(interaction.FileName))
                {
                    var request = new ResourceUploadRequest
                                      {
                                          Content = input,
                                          Resource =
                                              new Resource
                                                  {
                                                      Hash = hash,
                                                      Length = input.Length,
                                                      UploadingUser = applicationState.CurrentUser,
                                                      Description = "Uploaded manually from Unit Configurator",
                                                      OriginalFilename = selectedFilename
                                                  }
                                      };
                    if (interaction.FileName.EndsWith(".png", StringComparison.InvariantCultureIgnoreCase))
                    {
                        request.Resource.MimeType = "image/png";
                    }
                    else
                    {
                        request.Resource.MimeType = "image/jpeg";
                    }

                    var result = await this.resourceService.Channel.UploadAsync(request);
                    this.IsLoading = false;
                    this.ResourceHash = result.Resource.Hash;
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't upload resource");
                this.SetError("Resource", ErrorState.Error, ex.Message);
                this.IsLoading = false;
            }
        }
    }
}
