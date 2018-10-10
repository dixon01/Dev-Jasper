// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationEngineBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationEngineBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Configuration.Update.Application;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Files.Writable;
    using Gorba.Motion.Update.Core.Utility;

    using NLog;

    /// <summary>
    /// Base class for <see cref="IInstallationEngine"/> implementations.
    /// </summary>
    public abstract class InstallationEngineBase : IInstallationEngine
    {
        /// <summary>
        /// The NLog logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly IResourceService resourceService;

        private readonly Dictionary<ResourceId, ResourceInfo> resources = new Dictionary<ResourceId, ResourceInfo>();

        private UpdateState state;

        private IPartProgressMonitor monitor;

        private bool forceExecution;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationEngineBase"/> class.
        /// </summary>
        protected InstallationEngineBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);

            this.resourceService = MessageDispatcher.Instance.GetService<IResourceService>();
            this.State = UpdateState.Unknown;
        }

        /// <summary>
        /// Event that is fired when the <see cref="IInstallationEngine.State"/> changed.
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Gets or sets the current update state.
        /// </summary>
        public UpdateState State
        {
            get
            {
                return this.state;
            }

            protected set
            {
                if (this.state == value)
                {
                    return;
                }

                this.state = value;
                this.RaiseStateChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the update command that is being installed.
        /// </summary>
        protected UpdateCommand Command { get; private set; }

        /// <summary>
        /// Gets the installation root directory.
        /// </summary>
        protected IWritableDirectoryInfo InstallationRoot { get; private set; }

        /// <summary>
        /// Gets the file system used for the implementation.
        /// </summary>
        protected IWritableFileSystem FileSystem
        {
            get
            {
                return this.InstallationRoot.FileSystem;
            }
        }

        /// <summary>
        /// Gets the file utility to be used for copying, moving and deleting file system structures.
        /// </summary>
        protected FileUtility FileUtility { get; private set; }

        /// <summary>
        /// Gets the restart applications configuration.
        /// </summary>
        protected RestartApplicationsConfig RestartApplications { get; private set; }

        /// <summary>
        /// Runs the installation. This method can be long-running, so it should be
        /// executed in a separate thread.
        /// </summary>
        /// <param name="host">
        /// The host of the installation.
        /// </param>
        /// <returns>
        /// A flag indicating if the installation was completed or if only part of it was done.
        /// </returns>
        public abstract bool Install(IInstallationHost host);

        /// <summary>
        /// Rolls back anything that was previously done by <see cref="Install"/>.
        /// This method is only called in case the installation failed.
        /// </summary>
        /// <param name="host">
        /// The host of the installation.
        /// </param>
        public abstract void Rollback(IInstallationHost host);

        /// <summary>
        /// Configures this engine.
        /// </summary>
        /// <param name="command">
        ///     The command being executed.
        /// </param>
        /// <param name="installationRoot">
        ///     The installation root directory.
        /// </param>
        /// <param name="resourceList">
        ///     The list of resources to be used during the installation.
        /// </param>
        /// <param name="restartApplicationsConfig">
        ///     The configuration for the applications to be restarted based on the dependencies
        /// </param>
        /// <param name="progressMonitor">
        ///     The progress monitor; only 0.25 to 1.0 should be used.
        /// </param>
        internal void Configure(
            UpdateCommand command,
            IWritableDirectoryInfo installationRoot,
            IList<ResourceInfo> resourceList,
            RestartApplicationsConfig restartApplicationsConfig,
            IPartProgressMonitor progressMonitor)
        {
            this.Command = command;
            this.InstallationRoot = installationRoot;
            this.FileUtility = new FileUtility(this.FileSystem);
            this.monitor = progressMonitor;
            this.RestartApplications = restartApplicationsConfig;
            foreach (var resource in resourceList)
            {
                this.resources[resource.Id] = resource;
            }
        }

        /// <summary>
        /// Raises the <see cref="StateChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseStateChanged(EventArgs e)
        {
            var handler = this.StateChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Sets the progress of the installation.
        /// </summary>
        /// <param name="value">
        /// The progress value from 0.0 to 1.0.
        /// </param>
        /// <param name="note">
        /// The note.
        /// </param>
        protected void Progress(double value, string note)
        {
            this.CheckCancelled();

            this.monitor.Progress(value, note);
        }

        /// <summary>
        /// Checks if the installation has been cancelled through the <see cref="IPartProgressMonitor"/>.
        /// </summary>
        /// <exception cref="InstallationCancelledException">
        /// if the installation was cancelled.
        /// </exception>
        protected void CheckCancelled()
        {
            if (this.monitor.IsCancelled && !this.forceExecution)
            {
                throw new InstallationCancelledException();
            }
        }

        /// <summary>
        /// Exports the referenced resource to the given file path.
        /// </summary>
        /// <param name="id">
        /// The resource id.
        /// </param>
        /// <param name="path">
        /// The full file path of where to export the resource to.
        /// </param>
        protected void ExportResource(ResourceId id, string path)
        {
            this.Logger.Trace("Exporting resource {0} to '{1}'", id, path);
            this.resourceService.ExportResource(this.resources[id], path);

            // TODO: verify the resource was exported properly
            this.CheckCancelled();
        }

        /// <summary>
        /// Copies the new files for a given <see cref="folder"/> structure
        /// the given <see cref="destination"/> directory.
        /// </summary>
        /// <param name="folder">
        /// The folder to get the files for.
        /// </param>
        /// <param name="destination">
        /// The destination root directory.
        /// </param>
        /// <param name="filter">
        /// The filter to apply to all items in the <see cref="folder"/>.
        /// Items are only handled if the filter returns true for it.
        /// Can be null.
        /// </param>
        protected void GetNewFiles(UpdateFolder folder, string destination, Predicate<UpdateSubNode> filter)
        {
            if (filter == null)
            {
                filter = item => true;
            }

            this.Logger.Trace("Getting new files to {0}", destination);
            this.FileSystem.CreateDirectory(destination);

            foreach (var item in folder.Items)
            {
                this.CheckCancelled();
                if (!filter(item) || item.Action == ActionType.Delete)
                {
                    continue;
                }

                var path = Path.Combine(destination, item.Name);

                var subFolder = item as UpdateFolder;
                if (subFolder != null)
                {
                    this.GetNewFiles(subFolder, path, filter);
                    continue;
                }

                var file = item as UpdateFile;
                if (file != null)
                {
                    try
                    {
                        this.ExportResource(file.ResourceId, path);
                    }
                    catch (KeyNotFoundException e)
                    {
                        throw new InstallationCancelledException($"Resource with ID {file.ResourceId} isn't available.", e);
                    }
                    catch (Exception e)
                    {
                        throw new InstallationCancelledException($"Couldn't export resource with ID {file.ResourceId}", e);
                    }
                }
            }
        }

        /// <summary>
        /// Moves all old files from the <see cref="source"/> to the <see cref="destination"/>
        /// to create a backup according to the specified files in <see cref="folder"/>.
        /// </summary>
        /// <param name="folder">
        /// The folder for which to create the backup.
        /// </param>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="destination">
        /// The destination directory.
        /// </param>
        /// <param name="filter">
        /// The filter to apply to all items in the <see cref="folder"/>.
        /// Items are only handled if the filter returns true for it.
        /// Can be null.
        /// </param>
        protected void BackupOldFiles(
            UpdateFolder folder, string source, string destination, Predicate<UpdateSubNode> filter)
        {
            if (filter == null)
            {
                filter = item => true;
            }

            this.MoveFiles(folder, source, destination, item => filter(item) && item.Action != ActionType.Create);
        }

        /// <summary>
        /// Installs all new files defined in <see cref="folder"/> from the <see cref="source"/>
        /// directory to the <see cref="destination"/> directory.
        /// </summary>
        /// <param name="folder">
        /// The folder structure to install.
        /// </param>
        /// <param name="source">
        /// The source directory.
        /// </param>
        /// <param name="destination">
        /// The destination directory.
        /// </param>
        /// <param name="filter">
        /// The filter to apply to all items in the <see cref="folder"/>.
        /// Items are only handled if the filter returns true for it.
        /// Can be null.
        /// </param>
        protected void InstallNewFiles(
            UpdateFolder folder, string source, string destination, Predicate<UpdateSubNode> filter)
        {
            if (filter == null)
            {
                filter = item => true;
            }

            this.MoveFiles(folder, source, destination, item => filter(item) && item.Action != ActionType.Delete);
        }

        /// <summary>
        /// Rolls back all changes done to files and directories defined in <see cref="folder"/>
        /// by using the <see cref="backup"/> directory to <see cref="destination"/>.
        /// </summary>
        /// <param name="folder">
        /// The folder structure to install.
        /// </param>
        /// <param name="backup">
        /// The backup directory created with <see cref="BackupOldFiles"/>.
        /// </param>
        /// <param name="destination">
        /// The destination directory (where the backup files should be placed).
        /// </param>
        /// <param name="filter">
        /// The filter to apply to all items in the <see cref="folder"/>.
        /// Items are only handled if the filter returns true for it.
        /// Can be null.
        /// </param>
        protected void RollbackBackupFiles(
            UpdateFolder folder, string backup, string destination, Predicate<UpdateSubNode> filter)
        {
            if (filter == null)
            {
                filter = item => true;
            }

            this.forceExecution = true;
            try
            {
                this.DeleteFiles(folder, destination, item => filter(item) && item.Action != ActionType.Delete);
                this.MoveFiles(folder, backup, destination, filter);
            }
            finally
            {
                this.forceExecution = false;
            }
        }

        private bool MoveFiles(
            UpdateFolder folder, string source, string destination, Predicate<UpdateSubNode> filter)
        {
            IWritableDirectoryInfo sourceDirectory;
            if (!this.FileSystem.TryGetDirectory(source, out sourceDirectory))
            {
                return false;
            }

            this.Logger.Trace("Moving files from '{0}' to '{1}'", source, destination);
            this.FileSystem.CreateDirectory(destination);
            var canDelete = folder.Items.Count == sourceDirectory.GetFileSystemInfos().Length;
            foreach (var item in folder.Items)
            {
                this.CheckCancelled();
                if (!filter(item))
                {
                    canDelete = false;
                    continue;
                }

                var sourcePath = Path.Combine(source, item.Name);
                var destinationPath = Path.Combine(destination, item.Name);

                var subFolder = item as UpdateFolder;
                if (subFolder != null)
                {
                    if (!this.MoveFiles(subFolder, sourcePath, destinationPath, filter))
                    {
                        canDelete = false;
                    }

                    continue;
                }

                var file = item as UpdateFile;
                if (file != null)
                {
                    IWritableFileInfo fileInfo;
                    if (this.FileSystem.TryGetFile(sourcePath, out fileInfo))
                    {
                        this.Logger.Trace("Moving file '{0}' to '{1}'", sourcePath, destinationPath);
                        fileInfo.Attributes = FileAttributes.Normal;
                        fileInfo.MoveTo(destinationPath);
                    }
                }
            }

            if (canDelete)
            {
                this.Logger.Trace("Deleting directory '{0}'", sourceDirectory.FullName);
                sourceDirectory.Delete();
            }

            return canDelete;
        }

        private void DeleteFiles(UpdateFolder folder, string source, Predicate<UpdateSubNode> filter)
        {
            if (filter == null)
            {
                filter = item => true;
            }

            IWritableDirectoryInfo sourceDirectory;
            if (!this.FileSystem.TryGetDirectory(source, out sourceDirectory))
            {
                return;
            }

            this.Logger.Trace("Deleting new files from {0}", source);

            foreach (var item in folder.Items)
            {
                this.CheckCancelled();
                if (!filter(item))
                {
                    continue;
                }

                var path = Path.Combine(source, item.Name);

                var subFolder = item as UpdateFolder;
                if (subFolder != null)
                {
                    this.DeleteFiles(subFolder, path, filter);
                    continue;
                }

                var file = item as UpdateFile;
                if (file != null)
                {
                    IWritableFileInfo fileInfo;
                    if (this.FileSystem.TryGetFile(path, out fileInfo))
                    {
                        this.Logger.Trace("Deleting file {0}", path);
                        fileInfo.Delete();
                    }
                }
            }

            if (sourceDirectory.GetFileSystemInfos().Length == 0)
            {
                this.Logger.Trace("Deleting directory {0}", sourceDirectory.FullName);
                sourceDirectory.Delete();
            }
        }
    }
}