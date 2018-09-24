// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.ProjectManagement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Gorba.Center.Common.Wpf.Framework.DataViewModels;
    using Gorba.Center.Media.Core.Configuration;
    using Gorba.Center.Media.Core.DataViewModels;
    using Gorba.Center.Media.Core.DataViewModels.Layout;
    using Gorba.Center.Media.Core.DataViewModels.Presentation;
    using Gorba.Center.Media.Core.DataViewModels.Presentation.Section;
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Center.Media.Core.Models;
    using Gorba.Center.Media.Core.Models.Options;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;
    using Gorba.Common.Utility.Files.Writable;

    using Microsoft.Practices.ServiceLocation;

    using NLog;

    /// <summary>
    /// Defines the component responsible to cleanup resources.
    /// </summary>
    public class ResourceManager : IResourceManager
    {
        private const string ProjectsAppdataRelativePath = @"Gorba\Center\Media\Projects";
        private const string ProjectsRelativePath = @"Projects";
        private const string ResourcesAppdataRelativePath = @"Gorba\Center\Media\Resources";
        private const string ResourcesRelativePath = @"Resources";
        private const string ThumbnailsAppdataRelativePath = @"Gorba\Center\Media\Thumbnails";

        private const string ThumbnailsRelativePath = @"Thumbnails";
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ResourceSettings cleanupPolicy;

        private readonly Lazy<AnalogClockElementReferenceManager> lazyAnalogClockElementManager =
            new Lazy<AnalogClockElementReferenceManager>(() => new AnalogClockElementReferenceManager());

        private readonly Lazy<AudioElementReferenceManager> lazyAudioElementManager =
            new Lazy<AudioElementReferenceManager>(() => new AudioElementReferenceManager());

        private readonly Lazy<IWritableFileSystem> lazyFileSystem;

        private readonly Lazy<ImageElementReferenceManager> lazyImageElementManager =
            new Lazy<ImageElementReferenceManager>(() => new ImageElementReferenceManager());

        private readonly Lazy<TextElementReferenceManager> lazyTextElementManager =
            new Lazy<TextElementReferenceManager>(() => new TextElementReferenceManager());

        private readonly Lazy<TextualReplacementElementReferenceManager> lazyTextualReplacementElementManager =
            new Lazy<TextualReplacementElementReferenceManager>(() => new TextualReplacementElementReferenceManager());

        private readonly Lazy<VideoElementReferenceManager> lazyVideoElementManager =
            new Lazy<VideoElementReferenceManager>(() => new VideoElementReferenceManager());

        private readonly Lazy<PoolReferenceManager> lazyPoolManager =
            new Lazy<PoolReferenceManager>(() => new PoolReferenceManager());

        private readonly Lazy<ImageSectionReferenceManager> lazyImageSectionManager =
            new Lazy<ImageSectionReferenceManager>(() => new ImageSectionReferenceManager());

        private readonly Lazy<VideoSectionReferenceManager> lazyVideoSectionManager =
            new Lazy<VideoSectionReferenceManager>(() => new VideoSectionReferenceManager());

        private readonly string projectsPath;
        private readonly string resourceRoot;
        private readonly string resourcesPath;
        private readonly string thumbnailsPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceManager"/> class.
        /// </summary>
        /// <param name="resourceSettings">
        /// The resource settings.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="resourceSettings"/> is null.
        /// </exception>
        public ResourceManager(ResourceSettings resourceSettings)
        {
            if (resourceSettings == null)
            {
                throw new ArgumentNullException("resourceSettings");
            }

            this.cleanupPolicy = resourceSettings;
            var state = ServiceLocator.Current.GetInstance<IMediaApplicationState>();

            if (state.Options != null)
            {
                var resourceCategory = state.Options.Categories.FirstOrDefault(c => c is LocalResourceOptionCategory);
                if (resourceCategory != null)
                {
                    var resourceGroup = resourceCategory.Groups.FirstOrDefault(g => g is LocalResourceOptionGroup);
                    if (resourceGroup != null)
                    {
                        this.cleanupPolicy.RemoveLocalResourceAfter =
                            ((LocalResourceOptionGroup)resourceGroup).RemoveLocalResourcesAfter;
                    }
                }
            }

            this.resourceRoot = string.IsNullOrEmpty(resourceSettings.LocalResourcePath)
                                    ? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)
                                    : this.cleanupPolicy.LocalResourcePath;
            if (this.resourceRoot == Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
            {
                this.thumbnailsPath = Path.Combine(this.resourceRoot, ThumbnailsAppdataRelativePath);
                this.resourcesPath = Path.Combine(this.resourceRoot, ResourcesAppdataRelativePath);
                this.projectsPath = Path.Combine(this.resourceRoot, ProjectsAppdataRelativePath);
            }
            else
            {
                this.thumbnailsPath = Path.Combine(this.resourceRoot, ThumbnailsRelativePath);
                this.resourcesPath = Path.Combine(this.resourceRoot, ResourcesRelativePath);
                this.projectsPath = Path.Combine(this.resourceRoot, ProjectsRelativePath);
            }

            this.lazyFileSystem = new Lazy<IWritableFileSystem>(this.GetFileSystem);
        }

        /// <summary>
        /// Gets the analog clock element manager.
        /// </summary>
        public AnalogClockElementReferenceManager AnalogClockElementManager
        {
            get
            {
                return this.lazyAnalogClockElementManager.Value;
            }
        }

        /// <summary>
        /// Gets the audio element manager.
        /// </summary>
        public AudioElementReferenceManager AudioElementManager
        {
            get
            {
                return this.lazyAudioElementManager.Value;
            }
        }

        /// <summary>
        /// Gets the image element manager.
        /// </summary>
        public ImageElementReferenceManager ImageElementManager
        {
            get
            {
                return this.lazyImageElementManager.Value;
            }
        }

        /// <summary>
        /// Gets the pool reference manager.
        /// </summary>
        public PoolReferenceManager PoolManager
        {
            get
            {
                return this.lazyPoolManager.Value;
            }
        }

        /// <summary>
        /// Gets the image section reference manager.
        /// </summary>
        public ImageSectionReferenceManager ImageSectionManager
        {
            get
            {
                return this.lazyImageSectionManager.Value;
            }
        }

        /// <summary>
        /// Gets the video section reference manager.
        /// </summary>
        public VideoSectionReferenceManager VideoSectionManager
        {
            get
            {
                return this.lazyVideoSectionManager.Value;
            }
        }

        /// <summary>
        /// Gets the text element manager.
        /// </summary>
        public TextElementReferenceManager TextElementManager
        {
            get
            {
                return this.lazyTextElementManager.Value;
            }
        }

        /// <summary>
        /// Gets the textual replacement element manager.
        /// </summary>
        public TextualReplacementElementReferenceManager TextualReplacementElementManager
        {
            get
            {
                return this.lazyTextualReplacementElementManager.Value;
            }
        }

        /// <summary>
        /// Gets the video element manager.
        /// </summary>
        public VideoElementReferenceManager VideoElementManager
        {
            get
            {
                return this.lazyVideoElementManager.Value;
            }
        }

        private IWritableFileSystem FileSystem
        {
            get
            {
                return this.lazyFileSystem.Value;
            }
        }

        /// <summary>
        /// Checks the remaining available disk space.
        /// </summary>
        /// <param name="additionalFile">
        /// The file that should be added. If set, the available disk space is calculated including that file.
        /// </param>
        /// <returns>
        /// <c>true</c> if there is enough available disk space; <c>false</c> otherwise.
        /// </returns>
        public bool CheckAvailableDiskSpace(IFileInfo additionalFile = null)
        {
            var size = 0L;
            if (additionalFile != null)
            {
                size = additionalFile.Size;
            }

            return this.CheckAvailableDiskSpace(size);
        }

        /// <summary>
        /// Checks the remaining available disk space.
        /// </summary>
        /// <param name="additionalFilesSize">
        /// The additional size of all files that will be created locally.
        /// </param>
        /// <returns>
        /// <c>true</c> if there is enough available disk space; <c>false</c> otherwise.
        /// </returns>
        public bool CheckAvailableDiskSpace(long additionalFilesSize)
        {
            var driveRoot = Path.GetPathRoot(this.resourceRoot);
            if (driveRoot != null)
            {
                var driveInfo =
                    this.FileSystem.GetDrives()
                        .SingleOrDefault(
                            d =>
                            d.RootDirectory.FullName.Equals(driveRoot, StringComparison.InvariantCultureIgnoreCase));
                if (driveInfo != null)
                {
                    var availableSpace = driveInfo.AvailableFreeSpace - this.cleanupPolicy.MinRemainingDiskSpace
                                         - additionalFilesSize;
                    if (availableSpace > 0)
                    {
                        return true;
                    }

                    return false;
                }
            }

            throw new DirectoryNotFoundException(@"No valid drive root.");
        }

        /// <summary>
        /// Checks if the used disk space by resources is lower than the configured MaxUsedDiskSpace.
        /// </summary>
        /// <param name="additionalFilesSize">
        /// The additional size of all files that will be created locally.
        /// </param>
        /// <returns>
        /// <c>true</c> if the used space is lower; <c>false</c> otherwise.
        /// </returns>
        public bool CheckUsedDiskSpace(long additionalFilesSize = 0)
        {
            double directorySize = 0;
            try
            {
                IDirectoryInfo thumbnailDirectory;
                if (FileSystemManager.Local.TryGetDirectory(this.thumbnailsPath, out thumbnailDirectory))
                {
                    directorySize = thumbnailDirectory.GetFiles()
                                                      .Aggregate(directorySize, (current, file) => current + file.Size);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while getting the Thumbnails directory", exception);
            }

            try
            {
                IDirectoryInfo resourceDirectory;
                if (FileSystemManager.Local.TryGetDirectory(this.resourcesPath, out resourceDirectory))
                {
                    directorySize += resourceDirectory.GetFiles()
                                                      .Aggregate(
                                                          directorySize,
                                                          (current, resourceInfo) => current + resourceInfo.Size);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while getting the Resources directory", exception);
            }

            directorySize += additionalFilesSize;
            return !(directorySize >= this.cleanupPolicy.MaxUsedDiskSpace);
        }

        /// <summary>
        /// Deletes local resources which are not used in the current project and the file is older than the configured
        /// store duration.
        /// </summary>
        /// <param name="deleteAll">
        /// If set to <c>true</c>, all local resources will be deleted except the ones of the current project.
        /// </param>
        public void CleanupResources(bool deleteAll = false)
        {
            Logger.Debug("Cleanup local resources. Root path: '{0}'. Delete all: {1}", this.resourceRoot, deleteAll);
            var applicationState = ServiceLocator.Current.GetInstance<IMediaApplicationState>();
            var currentProjectResources = (applicationState.CurrentProject != null)
                                              ? applicationState.CurrentProject.Resources.Select(
                                                  r =>
                                                  new ResourceInfo
                                                      {
                                                          Filename = r.Hash + ".rx",
                                                          ThumbnailHash = r.ThumbnailHash + ".rx"
                                                      })
                                              : Enumerable.Empty<ResourceInfo>();

            try
            {
                IWritableDirectoryInfo resourceDirectory;
                if (this.FileSystem.TryGetDirectory(this.resourcesPath, out resourceDirectory))
                {
                    var resourceFileHashes =
                        resourceDirectory.GetFiles()
                                         .Where(
                                             f => currentProjectResources.All(resource => resource.Filename != f.Name))
                                         .Select(file => file.FullName);
                    this.CleanupFiles(resourceFileHashes, deleteAll);
                }
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while cleaning project resources", exception);
            }

            IWritableDirectoryInfo thumbnailDirectory;
            if (!this.FileSystem.TryGetDirectory(this.thumbnailsPath, out thumbnailDirectory))
            {
                return;
            }

            try
            {
                var thumbnailHashes =
                    thumbnailDirectory.GetFiles()
                                      .Where(
                                          f =>
                                          currentProjectResources.All(
                                              resourceInfo => resourceInfo.ThumbnailHash != f.Name))
                                      .Select(file => file.FullName);
                this.CleanupFiles(thumbnailHashes, deleteAll);
            }
            catch (Exception exception)
            {
                Logger.ErrorException("Error while cleaning thumbnails", exception);
            }
        }

        /// <summary>
        /// Gets the local projects path.
        /// </summary>
        /// <returns>
        /// The full path to the locally stored projects.
        /// </returns>
        public string GetLocalProjectsPath()
        {
            return this.projectsPath;
        }

        /// <summary>
        /// Gets the full path for the given resource hash.
        /// </summary>
        /// <param name="hash">
        /// The MD5 hash of a resource.
        /// </param>
        /// <returns>
        /// The full path to the resource.
        /// </returns>
        public string GetResourcePath(string hash)
        {
            return Path.Combine(this.resourcesPath, hash + ".rx");
        }

        /// <summary>
        /// Gets the full path for the given thumbnail hash.
        /// </summary>
        /// <param name="hash">
        /// The MD5 hash of a thumbnail.
        /// </param>
        /// <returns>
        /// The full path to the thumbnail.
        /// </returns>
        public string GetThumbnailPath(string hash)
        {
            return Path.Combine(this.thumbnailsPath, hash + ".rx");
        }

        /// <summary>
        /// The set references for project.
        /// </summary>
        /// <param name="infomediaConfig">
        /// The infomedia config.
        /// </param>
        public void SetReferencesForProject(MediaProjectDataViewModel infomediaConfig)
        {
            foreach (var replacement in infomediaConfig.Replacements)
            {
                this.HandleElement(replacement);
            }

            foreach (var layout in infomediaConfig.InfomediaConfig.Layouts)
            {
                foreach (var reference in layout.CycleSectionReferences)
                {
                    this.HandleReference(reference);
                }

                foreach (var resolution in layout.Resolutions)
                {
                    foreach (var element in resolution.Elements)
                    {
                        var output = element as AudioOutputElementDataViewModel;
                        if (output == null)
                        {
                            this.HandleElement(element);
                            continue;
                        }

                        foreach (var playback in output.Elements)
                        {
                            this.HandleElement(playback);
                        }
                    }
                }
            }
        }

        private void HandleReference(LayoutCycleSectionRefDataViewModel reference)
        {
            var poolReference = reference.SectionReference as PoolSectionConfigDataViewModel;
            if (poolReference != null)
            {
                this.HandleElement(poolReference);
                return;
            }

            var imageReference = reference.SectionReference as ImageSectionConfigDataViewModel;
            if (imageReference != null)
            {
                this.HandleElement(imageReference);
                return;
            }

            var videoReference = reference.SectionReference as VideoSectionConfigDataViewModel;
            if (videoReference == null)
            {
                return;
            }

            this.HandleElement(videoReference);
        }

        private void CleanupFiles(IEnumerable<string> filePaths, bool deleteAll)
        {
            var cleanupBefore = TimeProvider.Current.Now - this.cleanupPolicy.RemoveLocalResourceAfter;
            foreach (var path in filePaths)
            {
                IWritableFileInfo fileInfo;
                this.FileSystem.TryGetFile(path, out fileInfo);

                if (fileInfo == null)
                {
                    continue;
                }

                if (deleteAll || fileInfo.LastWriteTime < cleanupBefore)
                {
                    Logger.Debug("Delete file {0}", fileInfo.FullName);
                    fileInfo.Delete();
                }
            }
        }

        private IWritableFileSystem GetFileSystem()
        {
            return (IWritableFileSystem)FileSystemManager.Local;
        }

        private void HandleElement(DataViewModelBase element)
        {
            var analogClock = element as AnalogClockElementDataViewModel;
            if (analogClock != null)
            {
                this.AnalogClockElementManager.SetReferences(analogClock);
                return;
            }

            var audio = element as AudioFileElementDataViewModel;
            if (audio != null)
            {
                this.AudioElementManager.SetReferences(audio);
                return;
            }

            var text = element as TextElementDataViewModel;
            if (text != null)
            {
                this.TextElementManager.SetReferences(text);
                return;
            }

            var textualReplacement = element as TextualReplacementDataViewModel;
            if (textualReplacement != null)
            {
                this.TextualReplacementElementManager.SetReferences(textualReplacement);
                return;
            }

            var video = element as VideoElementDataViewModel;
            if (video != null)
            {
                this.VideoElementManager.SetReferences(video);
                return;
            }

            var image = element as ImageElementDataViewModel;
            if (image != null)
            {
                this.ImageElementManager.SetReferences(image);
                return;
            }

            var imageSection = element as ImageSectionConfigDataViewModel;
            if (imageSection != null)
            {
                this.ImageSectionManager.SetReferences(imageSection);
                return;
            }

            var videoSection = element as VideoSectionConfigDataViewModel;
            if (videoSection != null)
            {
                this.VideoSectionManager.SetReferences(videoSection);
                return;
            }

            var poolSection = element as PoolSectionConfigDataViewModel;
            if (poolSection == null)
            {
                return;
            }

            this.PoolManager.SetReferences(poolSection);
        }

        private class ResourceInfo
        {
            /// <summary>
            /// Gets or sets the filename.
            /// </summary>
            public string Filename { get; set; }

            /// <summary>
            /// Gets or sets the thumbnail hash.
            /// </summary>
            public string ThumbnailHash { get; set; }
        }
    }
}