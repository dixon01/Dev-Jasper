// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceController.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implementation for the <see cref="IResourceController"/>.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Drawing.Text;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Windows.Threading;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.AccessControl;
    using Gorba.Center.Common.ServiceModel.Resources;
    using Gorba.Center.Common.Wpf.Core;
    using Gorba.Center.Common.Wpf.Framework.Controllers;
    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels.Converters;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Extensions;
    using Gorba.Center.Media.Core.History;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.ProjectManagement;
    using Gorba.Center.Media.Core.Properties;
    using Gorba.Center.Media.Core.Resources;
    using Gorba.Center.Media.Core.ViewModels;
    using Gorba.Center.Media.Core.ViewModels.CommandParameters;
    using Gorba.Common.Configuration.Infomedia.Presentation;
    using Gorba.Common.Formats.AlphaNT.Bitmaps;
    using Gorba.Common.Formats.AlphaNT.Fonts;
    using Gorba.Common.Update.ServiceModel;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Implementation for the <see cref="IResourceController"/>.
    /// </summary>
    public class ResourceController : IResourceController
    {
        private const int ThumbnailWidth = 300;
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly Lazy<object> LazyShell32Object = new Lazy<object>(CreateShellObject);

        private static readonly Type ShellAppType = Type.GetTypeFromProgID("Shell.Application");
        private static readonly Lazy<IExtendedResourceProvider> LazyThumbnails =
            new Lazy<IExtendedResourceProvider>(CreateThumbnailsResourceProvider);

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="parentController">
        /// The parent controller.
        /// </param>
        /// <param name="mediaShell">
        /// The media shell.
        /// </param>
        /// <param name="commandRegistry">
        /// The command registry.
        /// </param>
        public ResourceController(
            IMediaShellController parentController,
            IMediaShell mediaShell,
            ICommandRegistry commandRegistry)
        {
            this.MediaShell = mediaShell;
            this.ParentController = parentController;
            this.CommandRegistry = commandRegistry;

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.AddResource,
                new RelayCommand<AddResourceParameters>(this.AddResources, this.HasWritePermission));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.DeleteResource, new RelayCommand<string>(this.DeleteResource));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.SelectResource,
                new RelayCommand<SelectResourceParameters>(this.SelectResource));

            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.AddResourceReference,
                new RelayCommand<AddResourceReferenceParameters>(this.AddResourceReference, this.HasWritePermission));
            commandRegistry.RegisterCommand(
                CommandCompositionKeys.Project.DeleteResourceReference,
                new RelayCommand<RemoveMediaReferenceParameters>(
                    this.DeleteResourceReference, this.HasWritePermission));
        }

        /// <summary>
        /// Gets the media shell.
        /// </summary>
        public IMediaShell MediaShell { get; private set; }

        /// <summary>
        /// Gets or sets the parent controller.
        /// </summary>
        public IMediaShellController ParentController { get; set; }

        /// <summary>
        /// Gets the file system.
        /// </summary>
        protected static IExtendedResourceProvider Thumbnails
        {
            get
            {
                return LazyThumbnails.Value;
            }
        }

        private static object Shell32Object
        {
            get
            {
                return LazyShell32Object.Value;
            }
        }

        private ICommandRegistry CommandRegistry { get; set; }

        /// <summary>
        /// Asynchronously uploads a resource to the server.
        /// </summary>
        /// <param name="resource">
        /// The resource to upload.
        /// </param>
        /// <returns>
        /// The <see cref="Resource"/> from server.
        /// </returns>
        public async Task<Resource> UploadResourceAsync(Resource resource)
        {
            var extractedResource =
                this.ParentController.Shell.MediaApplicationState.ProjectManager.GetResource(resource.Hash);

            using (
                var channelScope =
                    this.ParentController.ParentController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                using (var stream = extractedResource.OpenRead())
                {
                    resource.Length = stream.Length;
                    var streamedResource = new ResourceUploadRequest { Content = stream, Resource = resource };
                    await channelScope.Channel.UploadAsync(streamedResource);
                }
            }

            return resource;
        }

        /// <summary>
        /// Verifies that the resource exists on the system (both definition and valid content).
        /// </summary>
        /// <param name="hash">The hash of the resource.</param>
        /// <returns>
        /// <c>true</c> if the resource exists on the server (both definition and valid content. Otherwise,
        /// <c>false</c>.
        /// </returns>
        public async Task<bool> TestResourceAsync(string hash)
        {
            using (
                var channelScope =
                    this.ParentController.ParentController.ConnectionController.CreateChannelScope<IResourceService>())
            {
                return await channelScope.Channel.TestResourceAsync(hash);
            }
        }

        /// <summary>
        /// Gets the thumbnail resource of the given <paramref name="thumbnailHash"/>.
        /// </summary>
        /// <param name="thumbnailHash">
        /// The thumbnail hash.
        /// </param>
        /// <param name="thumbnailResource">
        /// The thumbnail resource.
        /// </param>
        /// <returns>
        /// <c>true</c> if the resource was found; otherwise, <c>false</c>.
        /// </returns>
        public bool GetVideoThumbnail(string thumbnailHash, out IResource thumbnailResource)
        {
            if (string.IsNullOrWhiteSpace(thumbnailHash))
            {
                thumbnailResource = null;
                return false;
            }

            return Thumbnails.TryGetResource(thumbnailHash, out thumbnailResource);
        }

        /// <summary>
        /// Decrements the reference count for the resource with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash.</param>
        public void DecrementResourceReferenceCount(string hash)
        {
            var resource = this.GetResourceViewModel(hash);
            if (resource == null)
            {
                MessageBox.Show(
                        MediaStrings.ResourceManager_couldNotFindMedia,
                        MediaStrings.ResourceManager_couldNotFindMediaTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return;
            }

            Logger.Debug("Decrementing usage reference count for object {0}", resource);
            if (resource.ReferencesCount > 0)
            {
                resource.ReferencesCount--;
            }
        }

        /// <summary>
        /// Increments the reference count for the resource with the specified <paramref name="hash"/>.
        /// </summary>
        /// <param name="hash">The hash.</param>
        public void IncrementResourceReferenceCount(string hash)
        {
            var resource = this.GetResourceViewModel(hash);
            if (resource == null)
            {
                MessageBox.Show(
                        MediaStrings.ResourceManager_couldNotFindMedia,
                        MediaStrings.ResourceManager_couldNotFindMediaTitle,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                return;
            }

            Logger.Debug("Incrementing usage reference count for object {0}", resource);
            resource.ReferencesCount++;
        }

        /// <summary>
        /// Ensures that the preview of the resource with the given <paramref name="resourceInfo"/> is in the local
        /// AppData. If it's not already present, it is created.
        /// </summary>
        /// <param name="resourceInfo">The resource info.</param>
        /// <param name="thumbnailResource">The resource containing the thumbnail.</param>
        /// <returns>
        /// <c>true</c> if the preview was successfully created (or was already available), and it is available as
        /// <paramref name="thumbnailResource"/>; otherwise, <c>false</c> (and the value of thumbnailResource is not
        /// meaningful.
        /// </returns>
        public bool EnsurePreview(ResourceInfoDataViewModel resourceInfo, out IResource thumbnailResource)
        {
            if (string.IsNullOrWhiteSpace(resourceInfo.ThumbnailHash))
            {
                return this.TryCreatePreview(resourceInfo, out thumbnailResource);
            }

            return Thumbnails.TryGetResource(resourceInfo.ThumbnailHash, out thumbnailResource)
                   || this.TryCreatePreview(resourceInfo, out thumbnailResource);
        }

        /// <summary>
        /// The update resources led font type. This is support for old projects. New resources get this value set.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdateResourcesLedFontType()
        {
            var wasUpdated = false;

            var fonts =
                this.MediaShell.MediaApplicationState.CurrentProject.Resources
                    .Where(r => r.Type == ResourceType.Font && r.IsLedFont);

            var configuration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            foreach (var font in fonts)
            {
                if (font.LedFontType != LedFontType.Unknown)
                {
                    continue;
                }

                var filePath = Path.Combine(
                    configuration.ResourceSettings.LocalResourcePath,
                    Settings.Default.AppDataResourcesRelativePath,
                    font.Hash + ".rx");

                IFileInfo fileInfo;
                var success = ((IWritableFileSystem)FileSystemManager.Local).TryGetFile(filePath, out fileInfo);
                if (!success)
                {
                    Logger.Error("Could not get file {0} to determine font type.", filePath);
                }
                else
                {
                    font.LedFontType = this.GetLedFontType(fileInfo);
                    wasUpdated = true;
                }
            }

            return wasUpdated;
        }

        private static object CreateShellObject()
        {
            var shell = Activator.CreateInstance(ShellAppType);
            return shell;
        }

        private static void HandleInvalidResource(IFileInfo fileInfo, ResourceType resourceType)
        {
            if (resourceType == ResourceType.Font)
            {
                ShowInvalidFontMessage(fileInfo);
            }
            else if (resourceType == ResourceType.Image || resourceType == ResourceType.Symbol)
            {
                ShowInvalidImageMessage(fileInfo);
            }
            else
            {
                throw new Exception("Unhandled invalid resource.");
            }
        }

        private static void ShowInvalidFontMessage(IFileInfo fileInfo)
        {
            var content = string.Format(MediaStrings.ResourceManager_AddedFontInvalid, fileInfo.FullName);
            MessageBox.Show(content, MediaStrings.ResourceManager_AddedFontInvalidTitle, MessageBoxButton.OK);
        }

        private static void ShowInvalidImageMessage(IFileInfo fileInfo)
        {
            var content = string.Format(MediaStrings.ResourceManager_AddedImageInvalid, fileInfo.FullName);
            MessageBox.Show(content, MediaStrings.ResourceManager_AddedImageInvalidTitle, MessageBoxButton.OK);
        }

        private static IExtendedResourceProvider CreateThumbnailsResourceProvider()
        {
            var provider = new AppDataResourceProvider(Settings.Default.AppDataThumbnailsRelativePath);
            var configuration = ServiceLocator.Current.GetInstance<MediaConfiguration>();
            if (configuration.ResourceSettings != null
                && !string.IsNullOrEmpty(configuration.ResourceSettings.LocalResourcePath)
                && configuration.ResourceSettings.LocalResourcePath
                != Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
            {
                var path = Path.Combine(
                    configuration.ResourceSettings.LocalResourcePath, Settings.Default.ThumbnailsRelativePath);
                provider.SetResourceDirectory(path);
            }

            return provider;
        }

        private static string CreateImagePreview(Stream inputStream)
        {
            Image originalImage = null;
            try
            {
                originalImage = Image.FromStream(inputStream, true, true);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error opening image from stream", exception);
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    new Action(() => MessageBox.Show(
                        MediaStrings.ResourceManager_AddedImageStreamInvalid,
                        MediaStrings.ResourceManager_AddedImageInvalidTitle,
                        MessageBoxButton.OK)));
            }

            Image resizedImage;
            if (originalImage == null)
            {
                resizedImage = new Bitmap(ThumbnailWidth, ThumbnailWidth);
            }
            else
            {
                resizedImage = originalImage.GetThumbnailImage(
                ThumbnailWidth, (ThumbnailWidth * originalImage.Height) / originalImage.Width, null, IntPtr.Zero);
            }

            var tempFileName = Path.GetTempFileName();
            resizedImage.Save(tempFileName, ImageFormat.Png);
            return tempFileName;
        }

        private static string CreateImagePreview(string filename)
        {
            var tempFileName = Path.GetTempFileName();
            var filenameConverter = new ResourceFilenameToImageConverter();
            var bitmapsource = filenameConverter.Convert(filename, null, null, null) as BitmapSource;
            if (bitmapsource == null)
            {
                return tempFileName;
            }

            var pngBitmapEncoder = new PngBitmapEncoder();
            pngBitmapEncoder.Frames.Add(BitmapFrame.Create(bitmapsource));
            using (var stream = new FileStream(tempFileName, FileMode.Create))
            {
                pngBitmapEncoder.Save(stream);
            }

            return tempFileName;
        }

        private string CreateVideoPreview(string hash)
        {
            string tempFileName;
            try
            {
                var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
                var path = resourceManager.GetResourcePath(hash);
                using (var videoDetector = new VideoDetector(path))
                {
                    var source = videoDetector.GetMiddleFrame();
                    var pngBitmapEncoder = new PngBitmapEncoder();
                    pngBitmapEncoder.Frames.Add(BitmapFrame.Create(source));
                    tempFileName = Path.GetTempFileName();
                    using (var stream = new FileStream(tempFileName, FileMode.Create))
                    {
                        pngBitmapEncoder.Save(stream);
                    }
                }

                return tempFileName;
            }
            catch (Exception exception)
            {
                Logger.Error("Error while creating video preview thumbnail. Taking default one.", exception);
            }

            using (
                var stream =
                    this.GetType().Assembly.GetManifestResourceStream(Settings.Default.VideoThumbnailPlaceholder))
            {
                tempFileName = CreateImagePreview(stream);
            }

            return tempFileName;
        }

        private void AddResourceReference(AddResourceReferenceParameters parameters)
        {
            Logger.Debug("Request to add media reference.");
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            var oldElements = new List<DataViewModelBase> { (PoolConfigDataViewModel)parameters.Pool.Clone() };
            dispatcher.Dispatch(
                () =>
                    {
                        parameters.Pool.ResourceReferences.Add(
                            new ResourceReferenceDataViewModel
                                {
                                    DisplayText =
                                        Path.GetFileNameWithoutExtension(
                                            parameters.Media.Filename),
                                    Hash = parameters.Media.Hash,
                                    ResourceInfo = parameters.Media,
                                });
                        parameters.Media.ReferencesCount++;

                        var newElements = new List<DataViewModelBase>
                                              {
                                                  (PoolConfigDataViewModel)parameters.Pool.Clone()
                                              };

                        Action doCallback = () =>
                            {
                                parameters.Media.ReferencesCount++;
                            };

                        Action undoCallback = () =>
                            {
                                parameters.Media.ReferencesCount--;
                            };

                        var historyEntry = new UpdateViewModelHistoryEntry(
                            oldElements,
                            newElements,
                            this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Pools,
                            doCallback,
                            undoCallback,
                            MediaStrings.ProjectController_UpdateMediaPoolHistoryEntryLabel,
                            this.CommandRegistry);
                        this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry, true);
                    });
        }

        private void DeleteResourceReference(RemoveMediaReferenceParameters parameters)
        {
            Logger.Debug("Request to remove media reference.");

            var pool = parameters.Pool;
            var oldElements = new List<DataViewModelBase> { (PoolConfigDataViewModel)parameters.Pool.Clone() };
            foreach (var reference in parameters.References)
            {
                pool.ResourceReferences.Remove(reference);
                this.ParentController.ResourceController.DecrementResourceReferenceCount(reference.Hash);
            }

            var newElements = new List<DataViewModelBase> { (PoolConfigDataViewModel)parameters.Pool.Clone() };

            Action doCallback = () =>
            {
                foreach (var reference in parameters.References)
                {
                    this.ParentController.ResourceController.DecrementResourceReferenceCount(reference.Hash);
                }
            };

            Action undoCallback = () =>
            {
                foreach (var reference in parameters.References)
                {
                    this.ParentController.ResourceController.IncrementResourceReferenceCount(reference.Hash);
                }
            };

            var historyEntry = new UpdateViewModelHistoryEntry(
                oldElements,
                newElements,
                this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Pools,
                doCallback,
                undoCallback,
                MediaStrings.ProjectController_UpdateMediaPoolHistoryEntryLabel,
                this.CommandRegistry);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry, true);
        }

        private IResource GetResource(string hash)
        {
            try
            {
                return this.MediaShell.MediaApplicationState.ProjectManager.GetResource(hash);
            }
            catch (UpdateException exception)
            {
                Logger.ErrorException("Can't find resource with hash '" + hash + "'", exception);
                return null;
            }
        }

        private bool TryCreatePreview(ResourceInfoDataViewModel resourceInfo, out IResource resource)
        {
            var originalResource = this.GetResource(resourceInfo.Hash);
            if (originalResource == null)
            {
                resource = null;
                return false;
            }

            string tempFileName;
            switch (resourceInfo.Type)
            {
                case ResourceType.Image:
                case ResourceType.Symbol:
                    if (resourceInfo.Filename.EndsWith(".egr", StringComparison.InvariantCultureIgnoreCase)
                        || resourceInfo.Filename.EndsWith(".egl", StringComparison.InvariantCultureIgnoreCase)
                        || resourceInfo.Filename.EndsWith(".egf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        tempFileName = CreateImagePreview(resourceInfo.Filename);
                    }
                    else
                    {
                        using (var inputStream = originalResource.OpenRead())
                        {
                            tempFileName = CreateImagePreview(inputStream);
                        }
                    }

                    break;
                case ResourceType.Video:
                    tempFileName = this.CreateVideoPreview(resourceInfo.Hash);
                    break;
                default:
                    Logger.Warn("Resource type preview not supported");
                    resource = null;
                    return false;
            }

            resourceInfo.ThumbnailHash = ResourceHash.Create(tempFileName);
            Thumbnails.AddResource(resourceInfo.ThumbnailHash, tempFileName, true);
            return Thumbnails.TryGetResource(resourceInfo.ThumbnailHash, out resource);
        }

        private async void AddResources(AddResourceParameters parameters)
        {
            Logger.Debug("Request to add a resource.");

            this.MediaShell.IsBusy = true;
            this.MediaShell.IsBusyIndeterminate = false;
            this.MediaShell.CurrentBusyProgress = 0;
            this.MediaShell.TotalBusyProgress = parameters.Resources.Count();
            this.MediaShell.BusyContentTextFormat = MediaStrings.ResourceController_AddResourceBusyFormat;
            var resourceManager = ServiceLocator.Current.GetInstance<IResourceManager>();
            foreach (var resource in parameters.Resources)
            {
                try
                {
                    var fileInfo = resource;
                    await Task.Run(() => this.AddResource(fileInfo, parameters.Type, resourceManager));
                }
                catch (Exception e)
                {
                    var message = string.Format("Error while adding resource '{0}'", resource.Name);
                    Logger.ErrorException(message, e);
                }
            }

            this.MediaShell.ClearBusy();
            if (parameters.Completed.TrySetResult(true))
            {
                return;
            }

            Logger.Debug("Couln't set the Completed flag on parameters");
        }

        private void AddResource(IFileInfo fileInfo, ResourceType resourceType, IResourceManager resourceManager)
        {
            this.MediaShell.CurrentBusyProgress++;
            this.MediaShell.CurrentBusyProgressText = fileInfo.Name;
            resourceManager.CleanupResources();
            if (!resourceManager.CheckUsedDiskSpace(fileInfo.Size)
                || !resourceManager.CheckAvailableDiskSpace(fileInfo))
            {
                resourceManager.CleanupResources(true);
                if (!resourceManager.CheckUsedDiskSpace(fileInfo.Size)
                    || !resourceManager.CheckAvailableDiskSpace(fileInfo))
                {
                    var resourceRootPath = Path.GetPathRoot(resourceManager.GetResourcePath(string.Empty));
                    var message = string.Format(
                        MediaStrings.AddResources_NotEnoughSpace_Message,
                        resourceRootPath,
                        fileInfo.Name);
                    MessageBox.Show(
                        message,
                        MediaStrings.AddResources_NotEnoughSpace_Title,
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    throw new UpdateException(message);
                }
            }

            var success = this.TryAddResource(fileInfo, resourceType);
            if (this.ParentController.MainMenuPrompt.ResourceManagementPrompt.SelectedPool != null && success)
            {
                this.AddResourceReference(fileInfo);
            }
        }

        private void AddResourceReference(IFileInfo file)
        {
            string hash;
            using (var stream = file.OpenRead())
            {
                hash = ResourceHash.Create(stream);
            }

            var resources = this.MediaShell.MediaApplicationState.CurrentProject.Resources;
            var resource = resources.SingleOrDefault(r => r.Hash == hash);
            var parameters = new AddResourceReferenceParameters
            {
                Media = resource,
                Pool = this.ParentController.MainMenuPrompt.ResourceManagementPrompt.SelectedPool,
            };
            this.AddResourceReference(parameters);
        }

        private bool TryAddResource(IFileInfo fileInfo, ResourceType resourceType)
        {
            string hash;
            using (var stream = fileInfo.OpenRead())
            {
                hash = ResourceHash.Create(stream);
            }

            this.MediaShell.MediaApplicationState.ProjectManager.AddResource(hash, fileInfo.FullName, false);
            var resourceInfoDataViewModel =
                this.MediaShell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                    model => model.Hash == hash);
            if (resourceInfoDataViewModel == null)
            {
                var dimension = string.Empty;
                var duration = string.Empty;
                var facename = string.Empty;
                var isLedFont = false;
                var isLedImage = false;
                var ledFontType = LedFontType.Unknown;
                var validResource = this.SetResourceTypeSpecificProperties(
                    fileInfo,
                    resourceType,
                    ref dimension,
                    ref duration,
                    ref facename,
                    ref isLedFont,
                    ref isLedImage,
                    ref ledFontType);

                if (validResource)
                {
                    resourceInfoDataViewModel = new ResourceInfoDataViewModel
                    {
                        Filename = fileInfo.FullName,
                        Hash = hash,
                        Type = resourceType,
                        Dimension = dimension,
                        Duration = duration,
                        Facename = facename,
                        IsLedFont = isLedFont,
                        IsLedImage = isLedImage,
                        Length = fileInfo.Size
                    };
                    this.EnsureUniqueMediaFilename(resourceInfoDataViewModel);
                    this.CreateAddResourceHistoryEntry(resourceInfoDataViewModel);
                }
                else
                {
                    HandleInvalidResource(fileInfo, resourceType);
                    return false;
                }
            }
            else
            {
                if (resourceInfoDataViewModel.Length == 0)
                {
                    resourceInfoDataViewModel.Length = fileInfo.Size;
                }

                var content = string.Format(
                    MediaStrings.ResourceManager_ResourceAlreadyPresent,
                    Path.GetFileNameWithoutExtension(resourceInfoDataViewModel.Filename),
                    fileInfo.FullName,
                    Environment.NewLine);
                MessageBox.Show(
                    content,
                    MediaStrings.ResourceManager_ResourceAlreadyPresentTitle,
                    MessageBoxButton.OK);
            }

            return true;
        }

        private void CreateAddResourceHistoryEntry(ResourceInfoDataViewModel resourceInfoDataViewModel)
        {
            var dispatcher = ServiceLocator.Current.GetInstance<IDispatcher>();
            dispatcher.Dispatch(
                () =>
                    {
                        var historyEntry = new AddListElementHistoryEntry<ResourceInfoDataViewModel>(
                            this.MediaShell,
                            resourceInfoDataViewModel,
                            this.MediaShell.MediaApplicationState.CurrentProject.Resources,
                            MediaStrings.ProjectController_AddResource);
                        this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
                        IResource thumbnailResource;
                        this.EnsurePreview(resourceInfoDataViewModel, out thumbnailResource);
                    });
        }

        [SuppressMessage(
            "StyleCopPlus.StyleCopPlusRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        private bool SetResourceTypeSpecificProperties(
            IFileInfo fileInfo,
            ResourceType resourceType,
            ref string dimension,
            ref string duration,
            ref string facename,
            ref bool isLedFont,
            ref bool isLedImage,
            ref LedFontType ledFontType)
        {
            var validResource = true;
            switch (resourceType)
            {
                case ResourceType.Image:
                case ResourceType.Symbol:
                    if (fileInfo.Name.EndsWith(".egr", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var bitmap = new EgrBitmap(fileInfo.FullName);
                            dimension = string.Format("{0} x {1}", bitmap.Width, bitmap.Height);
                            isLedImage = true;
                        }
                        catch (Exception e)
                        {
                            Logger.ErrorException(string.Format("Cold not open egr file '{0}'.", fileInfo.FullName), e);
                            validResource = false;
                        }

                        break;
                    }

                    if (fileInfo.Name.EndsWith(".egl", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var bitmap = new EglBitmap(fileInfo.FullName);
                            dimension = string.Format("{0} x {1}", bitmap.Width, bitmap.Height);
                            isLedImage = true;
                        }
                        catch (Exception e)
                        {
                            Logger.ErrorException(string.Format("Cold not open egl file '{0}'.", fileInfo.FullName), e);
                            validResource = false;
                        }

                        break;
                    }

                    if (fileInfo.Name.EndsWith(".egf", StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            var bitmap = new EgfBitmap(fileInfo.FullName);
                            dimension = string.Format("{0} x {1}", bitmap.Width, bitmap.Height);
                            isLedImage = true;
                        }
                        catch (Exception e)
                        {
                            Logger.ErrorException(string.Format("Cold not open egf file '{0}'.", fileInfo.FullName), e);
                            validResource = false;
                        }

                        break;
                    }

                    dimension = fileInfo.GetDimensions(ShellAppType, Shell32Object);
                    break;
                case ResourceType.Video:
                    duration = fileInfo.GetDuration(ShellAppType, Shell32Object);
                    dimension = fileInfo.GetVideoDimensions(ShellAppType, Shell32Object);
                    break;
                case ResourceType.Audio:
                    duration = fileInfo.GetDuration(ShellAppType, Shell32Object);
                    break;
                case ResourceType.Font:
                    facename = this.GetFontFaceName(fileInfo.FullName, out isLedFont);
                    if (facename == string.Empty)
                    {
                        validResource = false;
                    }

                    if (isLedFont)
                    {
                        try
                        {
                            ledFontType = this.GetLedFontType(fileInfo);
                        }
                        catch (Exception e)
                        {
                            Logger.Error("Could not determine file type of font: {0}", fileInfo.FullName);
                            ledFontType = LedFontType.Unknown;
                        }
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException("resourceType");
            }

            return validResource;
        }

        /// <summary>
        /// The get led font type by reading the file.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <returns>
        /// The <see cref="LedFontType"/>.
        /// </returns>
        /// <exception cref="FileFormatException">
        /// Reading might fail.
        /// </exception>
        private LedFontType GetLedFontType(IFileInfo fileInfo)
        {
            var stream = fileInfo.OpenRead();
            var reader = new BinaryReader(stream);

            try
            {
                var characterCount = (reader.ReadByte() & 0x80) == 0 ? 96 : 224; // 0
                var bytesPerCharacterRow = reader.ReadByte(); // 1
                if (bytesPerCharacterRow <= 0 || bytesPerCharacterRow >= 64)
                {
                    // just some arbitrary limit
                    throw new FileFormatException("Unsupported bytes per character: " + bytesPerCharacterRow);
                }

                reader.ReadByte(); // 2
                Encoding.ASCII.GetString(reader.ReadBytes(8).ToArray(), 0, 8); // 3..10
                var endMarker = reader.ReadByte(); // 11

                var result = LedFontType.Unknown;
                switch (endMarker)
                {
                    case 0x43:
                        var identifier = this.ReadLsbUInt16(reader);  // 13 and 14
                        switch (identifier)
                        {
                            case 0x5801:
                                result = LedFontType.CUxFont;
                                break;

                            default:
                                result = LedFontType.FonUnicodeChines;
                                break;
                        }

                        break;

                    case 0x41:  // 'A' endMarker in font file for arabic font
                    case 0x48:  // 'H' endMarker in font file for hebrew font
                        if (endMarker == 0x41)
                        {
                            result = LedFontType.FonUnicodeArab;
                        }
                        else
                        {
                            result = LedFontType.FonUnicodeHebrew;
                        }

                        break;

                    default:
                        if (endMarker != 0 && endMarker != 0xFF && endMarker != 0xFE)
                        {
                            throw new FileFormatException("Unknown end of header: 0x" + endMarker.ToString("X2"));
                        }

                        if (characterCount == 96)
                        {
                            result = LedFontType.FntFont;
                        }
                        else
                        {
                            result = LedFontType.FonFont;
                        }

                        break;
                }

                return result;
            }
            finally
            {
                stream.Close();
                reader.Close();
            }
        }

        private ushort ReadLsbUInt16(BinaryReader reader)
        {
            var b1 = reader.ReadByte();
            var b2 = reader.ReadByte();
            if (b1 < 0 || b2 < 0)
            {
                throw new EndOfStreamException();
            }

            return (ushort)(b1 | (b2 << 8));
        }

        private string GetFontFaceName(string fullFilename, out bool isLedFont)
        {
            var facename = string.Empty;
            var extension = Path.GetExtension(fullFilename);
            isLedFont = false;

            if (extension != null && (extension.Equals(".fnt", StringComparison.InvariantCultureIgnoreCase)
                || extension.Equals(".fon", StringComparison.InvariantCultureIgnoreCase)
                || extension.Equals(".cux", StringComparison.InvariantCultureIgnoreCase)))
            {
                using (var stream = File.OpenRead(fullFilename))
                {
                    try
                    {
                        // ReSharper disable once UnusedVariable
                        var font = new FontFile(stream, false);
                        facename = font.Name;
                    }
                    catch (InvalidDataException ex)
                    {
                        var errorMessage = string.Format("Error while loading font {0}", fullFilename);
                        Logger.ErrorException(errorMessage, ex);
                        facename = string.Empty;
                    }
                }

                isLedFont = true;
                return facename;
            }

            if (extension == null
                || (!extension.Equals(".ttf", StringComparison.InvariantCultureIgnoreCase)
                    && !extension.Equals(".otf", StringComparison.InvariantCultureIgnoreCase)))
            {
                return facename;
            }

            var privateFontCollection = new PrivateFontCollection();

            try
            {
                privateFontCollection.AddFontFile(fullFilename);
            }
            catch (Exception)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(
                    new Action(
                        () =>
                        {
                            var content = string.Format(
                                MediaStrings.ResourceManager_CouldNotOpenFont,
                                fullFilename);
                            MessageBox.Show(content, MediaStrings.ResourceManager_CouldNotOpenFontTitle);
                        }));
            }

            if (privateFontCollection.Families.Length == 1)
            {
                var font = privateFontCollection.Families[0];
                facename = font.Name;
            }

            return facename;
        }

        private void EnsureUniqueMediaFilename(ResourceInfoDataViewModel resourceInfoDataViewModel)
        {
            var filenameWithoutExtension = Path.GetFileNameWithoutExtension(resourceInfoDataViewModel.Filename);
            if (filenameWithoutExtension == string.Empty)
            {
                throw new Exception("File name without extension is empty.");
            }

            var isUnique = this.IsFilenameWithoutExtensionUnique(
                filenameWithoutExtension, resourceInfoDataViewModel.Type);

            if (!isUnique)
            {
                var uniqueFilename = this.CreateUniqueFilenameWithoutExtension(
                    filenameWithoutExtension,
                    resourceInfoDataViewModel.Type) + Path.GetExtension(resourceInfoDataViewModel.Filename);

                var filePath = Path.GetDirectoryName(resourceInfoDataViewModel.Filename);
                if (filePath == null)
                {
                    throw new Exception(
                        string.Format("Could not get path form resource {0}", resourceInfoDataViewModel.Filename));
                }

                var newFilename = Path.Combine(filePath, uniqueFilename);
                resourceInfoDataViewModel.Filename = newFilename;
            }
        }

        private string CreateUniqueFilenameWithoutExtension(string filenameWithoutExtension, ResourceType type)
        {
            int index = 1;
            var newFileName = filenameWithoutExtension + string.Format(Settings.Default.DuplicatedMediaPostfix, index);

            while (!this.IsFilenameWithoutExtensionUnique(newFileName, type))
            {
                index++;
                newFileName = filenameWithoutExtension + string.Format(Settings.Default.DuplicatedMediaPostfix, index);
            }

            return newFileName;
        }

        private bool IsFilenameWithoutExtensionUnique(string filenameWithoutExtension, ResourceType type)
        {
            var isUnique = true;
            foreach (var resource in this.MediaShell.MediaApplicationState.CurrentProject.Resources)
            {
                if (resource.Type != type)
                {
                    continue;
                }

                var storedFilenameWithoutExtension = Path.GetFileNameWithoutExtension(resource.Filename);
                if (storedFilenameWithoutExtension == string.Empty)
                {
                    throw new Exception("File name without extension is empty for a resource.");
                }

                if (!filenameWithoutExtension.Equals(storedFilenameWithoutExtension))
                {
                    continue;
                }

                isUnique = false;
                break;
            }

            return isUnique;
        }

        private void DeleteResource(string hash)
        {
            if (string.IsNullOrEmpty(hash))
            {
                throw new ArgumentNullException("hash");
            }

            var resourceInfoDataViewModel =
                this.MediaShell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                    model => model.Hash == hash);
            if (resourceInfoDataViewModel == null)
            {
                return;
            }

            if (resourceInfoDataViewModel.IsUsed)
            {
                throw new UpdateException("The resource is used. It can't be deleted.");
            }

            Action onAfterDelete = () =>
            {
                if (resourceInfoDataViewModel.Type != ResourceType.Font)
                {
                    this.RemoveMediaFromRecentlyUsed(resourceInfoDataViewModel);
                    return;
                }

                var name = Path.GetFileName(resourceInfoDataViewModel.Filename);
                if (name != null)
                {
                    var infomediaFontPath = Path.Combine(Settings.Default.RelativeFontsFolderPath, name);
                    var fontDataViewModel =
                        this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Fonts.FirstOrDefault(
                            f => f.Path.Value == infomediaFontPath);
                    if (fontDataViewModel != null)
                    {
                        this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Fonts.Remove(
                            fontDataViewModel);
                    }
                }
            };

            Action onAfterUndoDelete = () =>
            {
                if (resourceInfoDataViewModel.Type != ResourceType.Font)
                {
                    return;
                }

                var name = Path.GetFileName(resourceInfoDataViewModel.Filename);
                if (name != null)
                {
                    var infomediaFontPath = Path.Combine(Settings.Default.RelativeFontsFolderPath, name);
                    var fontDataViewModel = new FontConfigDataViewModel(this.MediaShell)
                    {
                        Path = { Value = infomediaFontPath },
                        ScreenType =
                        {
                            Value = resourceInfoDataViewModel.IsLedFont
                                ? PhysicalScreenType.LED : PhysicalScreenType.TFT
                        }
                    };
                    this.MediaShell.MediaApplicationState.CurrentProject.InfomediaConfig.Fonts.Add(fontDataViewModel);
                }
            };

            var historyEntry = new DeleteListElementEntry<ResourceInfoDataViewModel>(
                this.MediaShell,
                resourceInfoDataViewModel,
                this.MediaShell.MediaApplicationState.CurrentProject.Resources,
                onAfterDelete,
                onAfterUndoDelete,
                MediaStrings.ProjectController_DeleteResource);
            this.ParentController.ChangeHistoryController.AddHistoryEntry(historyEntry);
        }

        private void RemoveMediaFromRecentlyUsed(ResourceInfoDataViewModel resourceInfoDataViewModel)
        {
            var state = this.MediaShell.MediaApplicationState as MediaApplicationState;
            if (state == null)
            {
                throw new Exception("No state available.");
            }

            state.CurrentProjectRecentMediaResources.Remove(resourceInfoDataViewModel);
        }

        private void SelectResource(SelectResourceParameters selectionParameters)
        {
            if (!string.IsNullOrEmpty(selectionParameters.CurrentSelectedResourceHash))
            {
                this.IncrementResourceReferenceCount(selectionParameters.CurrentSelectedResourceHash);
            }

            if (!string.IsNullOrEmpty(selectionParameters.PreviousSelectedResourceHash))
            {
                this.DecrementResourceReferenceCount(selectionParameters.PreviousSelectedResourceHash);
            }
        }

        private ResourceInfoDataViewModel GetResourceViewModel(string hash)
        {
            return
                this.MediaShell.MediaApplicationState.CurrentProject.Resources.SingleOrDefault(
                    model => model.Hash == hash);
        }

        private bool HasWritePermission(object obj)
        {
            return this.MediaShell.PermissionController.HasPermission(
                Permission.Write,
                DataScope.MediaConfiguration);
        }
    }
}
