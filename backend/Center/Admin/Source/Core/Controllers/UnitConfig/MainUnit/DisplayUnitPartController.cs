// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayUnitPartController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.MainUnit
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    using Gorba.Center.Admin.Core.Extensions;
    using Gorba.Center.Admin.Core.Models;
    using Gorba.Center.Admin.Core.Models.UnitConfig;
    using Gorba.Center.Admin.Core.Resources;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Init;
    using Gorba.Center.Admin.Core.ViewModels.UnitConfig.Parts;
    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Utils;
    using Gorba.Center.Common.Wpf.Client.Controllers;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Interaction;
    using Gorba.Center.Common.Wpf.Framework.Interaction.FileDialog;

    using Microsoft.Practices.ServiceLocation;

    using Telerik.Windows.Controls;
    using Telerik.Windows.Documents.Fixed.FormatProviders;
    using Telerik.Windows.Documents.Fixed.FormatProviders.Pdf;
    using Telerik.Windows.Documents.UI;

    /// <summary>
    /// The IBIS control part controller.
    /// </summary>
    public class DisplayUnitPartController : PartControllerBase<DisplayUnitPartViewModel>
    {
        private const string UnitIndexKey = "UnitIndex";

        private const string IsStaticContentSelectedKey = "IsStaticContentSelected";

        private const string IsDynamicContentSelectedKey = "IsDynamicContentSelected";

        private const string StaticContentFileNameKey = "StaticContentFileName";

        private const string DynamicContentUrlKey = "DynamicContentUrl";

        private const string StaticContentHashKey = "StaticContentHash";

        private decimal unitIndex;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisplayUnitPartController"/> class.
        /// </summary>
        /// <param name="parent">
        /// The parent.
        /// </param>
        /// <param name="unitIndex">
        /// The unit Index.
        /// </param>
        public DisplayUnitPartController(CategoryControllerBase parent, int unitIndex)
            : base(UnitConfigKeys.MainUnit.DisplayUnit + unitIndex, parent)
        {
            this.unitIndex = unitIndex;
        }

        /// <summary>
        /// The load.
        /// </summary>
        /// <param name="partData">
        /// The part data.
        /// </param>
        public override void Load(UnitConfigPart partData)
        {
            this.ViewModel.UnitIndex = this.unitIndex; // this value is not to be edited
            this.ViewModel.Editor.IsStaticContentSelected = partData.GetValue(true, IsStaticContentSelectedKey);
            this.ViewModel.Editor.StaticContentFileName = partData.GetValue(string.Empty, StaticContentFileNameKey);

            this.ViewModel.Editor.IsDynamicContentSelected = partData.GetValue(false, IsDynamicContentSelectedKey);
            this.ViewModel.Editor.DynamicContentUrl = partData.GetValue(string.Empty, DynamicContentUrlKey);
            this.ViewModel.Editor.StaticContentHash = partData.GetValue(string.Empty, StaticContentHashKey);
            this.ViewModel.ClearDirty();

            if (!string.IsNullOrEmpty(this.ViewModel.Editor.StaticContentHash))
            {
                this.LoadPreview();
            }
        }

        /// <summary>
        /// Saves the unit config data from this part controller to the given object.
        /// </summary>
        /// <param name="partData">
        /// The configuration data for this part.
        /// </param>
        public override void Save(UnitConfigPart partData)
        {
            partData.SetValue(this.unitIndex, UnitIndexKey);
            partData.SetValue(this.ViewModel.Editor.IsStaticContentSelected, IsStaticContentSelectedKey);

            partData.SetValue(Path.GetFileName(this.ViewModel.Editor.StaticContentFileName), StaticContentFileNameKey);
            partData.SetValue(this.ViewModel.Editor.IsDynamicContentSelected, IsDynamicContentSelectedKey);
            partData.SetValue(this.ViewModel.Editor.DynamicContentUrl, DynamicContentUrlKey);
            if (!this.ViewModel.Editor.IsStaticContentSelected)
            {
                partData.SetValue(string.Empty, StaticContentHashKey);
            }

            if (this.ViewModel.Editor.IsStaticContentSelected
                && !string.IsNullOrEmpty(this.ViewModel.Editor.StaticContentFileName))
            {
                try
                {
                    var hash = this.EnsureStaticContentIsUploadedAsync().Result;

                    // clould not access/upload local file for upload
                    if (string.IsNullOrEmpty(hash))
                    {
                        this.HandleUploadError(partData);
                    }

                    partData.SetValue(hash, StaticContentHashKey);
                }
                catch (Exception)
                {
                    this.HandleUploadError(partData);
                }
            }
        }

        /// <summary>
        /// Creates and initializes the view model.
        /// </summary>
        /// <returns>
        /// The <see cref="MultiEditorPartViewModel"/>.
        /// </returns>
        protected override DisplayUnitPartViewModel CreateViewModel()
        {
            var viewModel = new DisplayUnitPartViewModel();
            viewModel.UnitIndex = this.unitIndex;
            viewModel.IsVisible = true;
            viewModel.DisplayName = string.Format(AdminStrings.UnitConfig_MainUnit_DisplayUnit, viewModel.UnitIndex);
            viewModel.Description = AdminStrings.UnitConfig_MainUnit_DisplayUnit_Description;

            viewModel.Editor.IsStaticContentSelected = true;
            viewModel.Editor.ChooseStaticContentFileCommand = new RelayCommand(
                this.ChooseStaticContentFile,
                this.CanChooseStaticContentFile);

            viewModel.Editor.PreviewDynamicContentInBrowserCommand = new RelayCommand(
                this.PreviewDynamicContentInBrowser,
                this.CanPreviewDynamicContentInBrowser);

            viewModel.Editor.PropertyChanged += (s, e) => this.UpdateErrors();

            return viewModel;
        }

        /// <summary>
        /// Raises the <see cref="PartControllerBase.ViewModelUpdated"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected override void RaiseViewModelUpdated(EventArgs e)
        {
            base.RaiseViewModelUpdated(e);
            this.UpdateErrors();
        }

        private static bool IsValidUrl(string url)
        {
            try
            {
                Uri uriResult;
                return Uri.TryCreate(url, UriKind.Absolute, out uriResult)
                       && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            }
            catch
            {
                return false;
            }
        }

        private static ImageSource ConvertPdfToImage(Stream stream)
        {
            var factory = new ThumbnailFactory();
            var settings = new FormatProviderSettings(ReadingMode.OnDemand);
            var provider = new PdfFormatProvider(stream, settings);
            var doc = provider.Import();
            var pdfViewer = new RadPdfViewer { Document = doc };
            var page = pdfViewer.Document.Pages[0];

            return factory.CreateThumbnail(page, page.Size);
        }

        private bool CanPreviewDynamicContentInBrowser(object o)
        {
            Uri result;
            return this.ViewModel.Editor.IsDynamicContentSelected
                   && Uri.TryCreate(this.ViewModel.Editor.DynamicContentUrl, UriKind.Absolute, out result);
        }

        private void PreviewDynamicContentInBrowser()
        {
            try
            {
                Process.Start(new ProcessStartInfo(this.ViewModel.Editor.DynamicContentUrl));
            }
            catch (System.Exception)
            {
                MessageBox.Show(
                AdminStrings.UnitConfig_DisplayUnit_ErrorShowingUrl,
                AdminStrings.UnitConfig_DisplayUnit_ErrorShowingUrlTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            }
        }

        private void HandleUploadError(UnitConfigPart partData)
        {
            var message = string.Format(
                AdminStrings.UnitConfig_DisplayUnit_ErrorUploadingStaticResource,
                this.ViewModel.Editor.StaticContentFileName);
            MessageBox.Show(
                message,
                AdminStrings.UnitConfig_DisplayUnit_ErrorUploadingStaticResourceTitle,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            this.Logger.Error("UnitConfig DisplayUnit checkin: Could not uplpad static content resource: {0}",
                this.ViewModel.Editor.StaticContentFileName);

            // save resetted value so after checkin/reload we get a empty file name
            partData.SetValue(string.Empty, StaticContentFileNameKey);
            this.RaiseViewModelUpdated(null);
        }

        private void ChooseStaticContentFile()
        {
            var interaction = new OpenFileDialogInteraction
            {
                AddExtension = true,
                FileName = this.ViewModel.Editor.StaticContentFileName,
                Filter = AdminStrings.UnitConfig_MainUnit_DisplayUnit_StaticContentFile_DialogFilter,
                Title = AdminStrings.UnitConfig_MainUnit_DisplayUnit_StaticContentFile_DialogTitle
            };

            InteractionManager<OpenFileDialogInteraction>.Current.Raise(interaction, this.ImportStaticContentFile);
        }

        private void ImportStaticContentFile(OpenFileDialogInteraction interaction)
        {
            if (!interaction.Confirmed)
            {
                return;
            }

            try
            {
                this.ViewModel.Editor.StaticContentFileName = interaction.FileName;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.GetType().Name, MessageBoxButton.OK, MessageBoxImage.Error);
                this.Logger.Error(ex, "Couldn't import unit static context file from " + interaction.FileName);
            }

            this.LoadPreview();

            this.UpdateErrors();
        }

        private void LoadPreview()
        {
            if (this.IsStaticContentFilenameLocal())
            {
                this.LoadLocalPreview();
            }
            else
            {
                this.LoadRemotePreview();
            }
        }

        private void LoadRemotePreview()
        {
            var connectionController = this.Parent.Parent.DataController.ConnectionController;

            using (var contentResourceService = connectionController.CreateChannelScope<IContentResourceService>())
            {
                var hash = this.ViewModel.Editor.StaticContentHash;
                var exists =
                    contentResourceService.Channel.TestContentResourceAsync(hash, HashAlgorithmTypes.xxHash64).Result;
                if (!exists)
                {
                    this.ViewModel.Editor.CurrentStaticImageSource = null;
                    this.ViewModel.Editor.PreviewImageIsValid = false;
                }

                var downloadRequest = new ContentResourceDownloadRequest()
                                          {
                                              Hash = hash,
                                              HashType = HashAlgorithmTypes.xxHash64
                                          };
                var imageResource = contentResourceService.Channel.DownloadAsync(downloadRequest).Result;

                try
                {
                    if (Path.GetExtension(imageResource.Resource.OriginalFilename) == ".pdf")
                    {
                        using (var stream = new MemoryStream())
                        {
                            imageResource.Content.CopyTo(stream);
                            this.ViewModel.Editor.CurrentStaticImageSource = ConvertPdfToImage(stream);
                        }
                    }
                    else
                    {
                        var bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.StreamSource = imageResource.Content;
                        bitmap.EndInit();
                        this.ViewModel.Editor.CurrentStaticImageSource = bitmap;
                    }

                    this.ViewModel.Editor.PreviewImageIsValid = true;
                }
                catch (Exception ex)
                {
                    this.ViewModel.Editor.CurrentStaticImageSource = null;
                    this.ViewModel.Editor.PreviewImageIsValid = false;
                    this.Logger.Error(ex,
                        "Could not create bitmap from remote resource "
                        + this.ViewModel.Editor.StaticContentFileName);
                }
            }
        }

        private void LoadLocalPreview()
        {
            try
            {
                if (!string.IsNullOrEmpty(this.ViewModel.Editor.StaticContentFileName)
                    && Path.GetExtension(this.ViewModel.Editor.StaticContentFileName) == ".pdf")
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (Stream input = File.OpenRead(this.ViewModel.Editor.StaticContentFileName))
                        {
                            input.CopyTo(stream);
                        }

                        this.ViewModel.Editor.CurrentStaticImageSource = ConvertPdfToImage(stream);
                    }
                }
                else
                {
                    var bitmap = new BitmapImage(new Uri(this.ViewModel.Editor.StaticContentFileName));
                    this.ViewModel.Editor.CurrentStaticImageSource = bitmap;
                }

                this.ViewModel.Editor.PreviewImageIsValid = true;
            }
            catch (Exception ex)
            {
                this.ViewModel.Editor.CurrentStaticImageSource = null;
                this.ViewModel.Editor.PreviewImageIsValid = false;
                this.Logger.Error(ex,"Could not create bitmap from file " + this.ViewModel.Editor.StaticContentFileName);
            }
        }

        private bool CanChooseStaticContentFile(object obj)
        {
            return this.ViewModel.Editor.IsStaticContentSelected;
        }

        private async Task<string> EnsureStaticContentIsUploadedAsync()
        {
            if (this.StaticContentIsUpToDate())
            {
                return this.ViewModel.Editor.StaticContentHash;
            }

            return await this.UploadStaticContentFileAsync().ConfigureAwait(false);
        }

        private bool StaticContentIsUpToDate()
        {
            var connectionController = this.Parent.Parent.DataController.ConnectionController;
            var currentHash = this.ViewModel.Editor.StaticContentHash;
            if (string.IsNullOrEmpty(currentHash))
            {
                return false;
            }

            using (var contentResourceService = connectionController.CreateChannelScope<IContentResourceService>())
            {
                var resource = contentResourceService.Channel.GetAsync(currentHash, HashAlgorithmTypes.xxHash64).Result;
                if (resource != null)
                {
                    // we have a hash but we migth have another file selcted
                    var currentSelectedFileHash = this.CreateXxHashOfFile(this.ViewModel.Editor.StaticContentFileName);
                    if (string.IsNullOrEmpty(currentSelectedFileHash))
                    {
                        // file is not pressent on the file system, if it has a path there is a error
                        if (!this.IsStaticContentFilenameLocal())
                        {
                            // current hash is loaded and filename is also loaded
                            return true;
                        }

                        // file is not accessable
                        throw new Exception(
                            string.Format(
                                "Can not access selected static content file: {0}",
                                this.ViewModel.Editor.StaticContentFileName));
                    }

                    if (resource.Hash.Equals(currentSelectedFileHash))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsStaticContentFilenameLocal()
        {
            if (string.IsNullOrEmpty(this.ViewModel.Editor.StaticContentFileName))
            {
                return false;
            }

            return !string.IsNullOrEmpty(Path.GetDirectoryName(this.ViewModel.Editor.StaticContentFileName));
        }

        private async Task<string> UploadStaticContentFileAsync()
        {
            var connectionController = this.Parent.Parent.DataController.ConnectionController;

            var contenthash = this.CreateXxHashOfFile(this.ViewModel.Editor.StaticContentFileName);
            if (contenthash.Equals(string.Empty))
            {
                throw new Exception(string.Format(
                        "Can not access selected file for upload: {0}", this.ViewModel.Editor.StaticContentFileName));
            }

            try
            {
                await this.UploadFileAsync(contenthash, connectionController).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                this.Logger.Error(exception, "Error uploading a static content resource");
                return string.Empty;
            }

            return contenthash;
        }

        private string CreateXxHashOfFile(string file)
        {
            // we only have a filename loaded, path is missing, so we can trust the loaded hash
            if (!File.Exists(file))
            {
                return string.Empty;
            }

            string contenthash;
            using (var fileStream = File.OpenRead(file))
            {
                contenthash = ContentResourceHash.Create(fileStream, HashAlgorithmTypes.xxHash64);
            }

            return contenthash;
        }

        private async Task UploadFileAsync(string hash, IConnectionController connectionController)
        {
            using (var contentResourceService = connectionController.CreateChannelScope<IContentResourceService>())
            {
                var existingContent =
                    await
                    contentResourceService.Channel.GetAsync(hash, HashAlgorithmTypes.xxHash64).ConfigureAwait(false);
                if (existingContent != null)
                {
                    return;
                }

                using (var content = File.OpenRead(this.ViewModel.Editor.StaticContentFileName))
                {
                    var applicationState = ServiceLocator.Current.GetInstance<IAdminApplicationState>();
                    var newResource = new ContentResource()
                    {
                        Description = this.CreateDescription(),
                        Hash = hash,
                        HashAlgorithmType = HashAlgorithmTypes.xxHash64,
                        Length = content.Length,
                        MimeType = "application/octet-stream",
                        OriginalFilename = Path.GetFileName(this.ViewModel.Editor.StaticContentFileName),
                        UploadingUser = applicationState.CurrentUser
                    };

                    await
                        contentResourceService.Channel.UploadAsync(
                            new ContentResourceUploadRequest { Content = content, Resource = newResource })
                            .ConfigureAwait(false);
                }
            }
        }

        private string CreateDescription()
        {
            return string.Format(
                "Created for unit configuration {0}, version {1}",
                this.Parent.Parent.UnitConfiguration.Document.Name,
                this.Parent.Parent.VersionNumber);
        }

        private void UpdateErrors()
        {
            var errorState = ErrorState.Ok;
            if (this.ViewModel.Editor.IsStaticContentSelected
                && string.IsNullOrEmpty(this.ViewModel.Editor.StaticContentFileName))
            {
                errorState = this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing;
            }

            this.ViewModel.Editor.SetError("StaticContentFileName", errorState, AdminStrings.Errors_TextNotWhitespace);

            if (this.ViewModel.Editor.IsDynamicContentSelected
                && string.IsNullOrEmpty(this.ViewModel.Editor.DynamicContentUrl))
            {
                errorState = this.ViewModel.WasVisited ? ErrorState.Error : ErrorState.Missing;
            }
            else
            {
                errorState = ErrorState.Ok;
            }

            this.ViewModel.Editor.SetError("DynamicContentUrl", errorState, AdminStrings.Errors_TextNotWhitespace);

            if (this.ViewModel.Editor.IsDynamicContentSelected
                && !IsValidUrl(this.ViewModel.Editor.DynamicContentUrl))
            {
                errorState = ErrorState.Error;
            }
            else
            {
                errorState = ErrorState.Ok;
            }

            this.ViewModel.Editor.SetError("DynamicContentUrl", errorState, AdminStrings.Errors_InvalidUrl);
        }
    }
}
