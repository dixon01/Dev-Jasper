// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IResourceManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Gorba.Center.Media.Core.ProjectManagement
{
    using Gorba.Center.Media.Core.DataViewModels.Project;
    using Gorba.Common.Utility.Files;

    /// <summary>
    /// Defines the component responsible to cleanup resources.
    /// </summary>
    public interface IResourceManager
    {
        /// <summary>
        /// Gets the analog clock element manager.
        /// </summary>
        AnalogClockElementReferenceManager AnalogClockElementManager { get; }

        /// <summary>
        /// Gets the audio element manager.
        /// </summary>
        AudioElementReferenceManager AudioElementManager { get; }

        /// <summary>
        /// Gets the image element manager.
        /// </summary>
        ImageElementReferenceManager ImageElementManager { get; }

        /// <summary>
        /// Gets the pool manager.
        /// </summary>
        PoolReferenceManager PoolManager { get; }

        /// <summary>
        /// Gets the text element manager.
        /// </summary>
        TextElementReferenceManager TextElementManager { get; }

        /// <summary>
        /// Gets the textual replacement element manager.
        /// </summary>
        TextualReplacementElementReferenceManager TextualReplacementElementManager { get; }

        /// <summary>
        /// Gets the video element manager.
        /// </summary>
        VideoElementReferenceManager VideoElementManager { get; }

        /// <summary>
        /// Gets the video section reference manager.
        /// </summary>
        VideoSectionReferenceManager VideoSectionManager { get; }

        /// <summary>
        /// Gets the image section reference manager.
        /// </summary>
        ImageSectionReferenceManager ImageSectionManager { get; }

        /// <summary>
        /// Checks the remaining available disk space on the disk used for resources.
        /// </summary>
        /// <param name="additionalFile">
        /// The file that should be added. If set, the available disk space is calculated including that file.
        /// </param>
        /// <returns>
        /// <c>true</c> if there is enough available disk space; <c>false</c> otherwise.
        /// </returns>
        bool CheckAvailableDiskSpace(IFileInfo additionalFile = null);

        /// <summary>
        /// Checks the remaining available disk space.
        /// </summary>
        /// <param name="additionalFilesSize">
        /// The additional size of all files that will be created locally.
        /// </param>
        /// <returns>
        /// <c>true</c> if there is enough available disk space; <c>false</c> otherwise.
        /// </returns>
        bool CheckAvailableDiskSpace(long additionalFilesSize);

        /// <summary>
        /// Checks if the used disk space by resources is lower than the configured MaxUsedDiskSpace.
        /// </summary>
        /// <param name="additionalFilesSize">
        /// The additional size of all files that will be created locally.
        /// </param>
        /// <returns>
        /// <c>true</c> if the used space is lower; <c>false</c> otherwise.
        /// </returns>
        bool CheckUsedDiskSpace(long additionalFilesSize);

        /// <summary>
        /// Deletes local resources which are not used in the current project and the file is older than the configured
        /// store duration.
        /// </summary>
        /// <param name="deleteAll">
        /// If set to <c>true</c>, all local resources will be deleted except the ones of the current project.
        /// </param>
        void CleanupResources(bool deleteAll = false);

        /// <summary>
        /// Gets the local projects path.
        /// </summary>
        /// <returns>
        /// The full path to the locally stored projects.
        /// </returns>
        string GetLocalProjectsPath();

        /// <summary>
        /// Gets the full path for the given resource hash.
        /// </summary>
        /// <param name="hash">
        /// The MD5 hash of a resource.
        /// </param>
        /// <returns>
        /// The full path to the resource.
        /// </returns>
        string GetResourcePath(string hash);

        /// <summary>
        /// Gets the full path for the given thumbnail hash.
        /// </summary>
        /// <param name="hash">
        /// The MD5 hash of a thumbnail.
        /// </param>
        /// <returns>
        /// The full path to the thumbnail.
        /// </returns>
        string GetThumbnailPath(string hash);

        /// <summary>
        /// Sets all references for a project.
        /// To be used during the loading of a project.
        /// </summary>
        /// <param name="infomediaConfig">
        /// The infomedia config.
        /// </param>
        void SetReferencesForProject(MediaProjectDataViewModel infomediaConfig);
    }
}