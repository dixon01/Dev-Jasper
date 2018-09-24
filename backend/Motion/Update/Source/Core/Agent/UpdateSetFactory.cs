// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSetFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSetFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;

    using NLog;

    /// <summary>
    /// Factory for <see cref="UpdateSet"/> that computes the difference between
    /// a local directory and the contents requested by an <see cref="UpdateCommand"/>.
    /// </summary>
    public class UpdateSetFactory
    {
        private const StringComparison DefaultStringComparison = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// A file with this name is created next to an EXE when .NET remote debugging is enabled under WinCE.
        /// We need to ignore this because otherwise we always update CF EXE directories (because we would
        /// need to delete that file).
        /// </summary>
        private const string CompactFrameworkDebugFile = "514c36bf-c13e-4091-a3a7-1e566227b20d";

        private static readonly Logger Logger = LogHelper.GetLogger<UpdateSetFactory>();

        private readonly IDirectoryInfo rootDirectory;

        private readonly UpdateCommand command;

        private readonly IPartProgressMonitor progressMonitor;

        private double maxProgress;

        private int progress;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSetFactory"/> class.
        /// </summary>
        /// <param name="rootDirectory">
        /// The root directory to compare with.
        /// </param>
        /// <param name="command">
        /// The update command.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor for this operation.
        /// </param>
        public UpdateSetFactory(
            IDirectoryInfo rootDirectory, UpdateCommand command, IPartProgressMonitor progressMonitor)
        {
            this.rootDirectory = rootDirectory;
            this.command = command;
            this.progressMonitor = progressMonitor ?? new NullProgressMonitor();
        }

        /// <summary>
        /// Creates the <see cref="UpdateSet"/> by checking the difference between
        /// the root directory and the contents requested by the <see cref="UpdateCommand"/>.
        /// </summary>
        /// <returns>
        /// The <see cref="UpdateSet"/> containing all changes.
        /// </returns>
        /// <exception cref="InstallationCancelledException">
        /// if the installation was cancelled from the monitor.
        /// </exception>
        public virtual UpdateSet CreateUpdateSet()
        {
            var updateSet = new UpdateSet();

            var commandRootNode = new FolderUpdate();
            foreach (var folderUpdate in this.command.Folders)
            {
                commandRootNode.Items.Add(folderUpdate);
            }

            this.maxProgress = this.CountFiles(commandRootNode) + 1.0;

            Logger.Debug("Creating update set for {0}", this.rootDirectory.FullName);

            var updateSetRoot = new UpdateFolder(null);
            this.PopulateUpdateSet(
                this.rootDirectory, commandRootNode, updateSetRoot, false, this.rootDirectory.FullName);

            foreach (var item in updateSetRoot.Items)
            {
                updateSet.Folders.Add(item as UpdateFolder);
            }

            return updateSet;
        }

        private int CountFiles(FolderUpdate folder)
        {
            var count = 0;
            foreach (var item in folder.Items)
            {
                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    count += this.CountFiles(subFolder);
                    continue;
                }

                count++;
            }

            return count;
        }

        /// <summary>
        /// Populates the update set tree from the given <see cref="directory"/>.
        /// </summary>
        /// <param name="directory">
        /// the file system directory to look at.
        /// </param>
        /// <param name="commandFolder">
        /// the folder from the update command to compare to.
        /// </param>
        /// <param name="updateSetFolder">
        /// the folder of the update set into which to put the items to be updated.
        /// </param>
        /// <param name="deleteItems">
        /// A flag indicating if items should also be deleted.
        /// In the root level we shouldn't delete any items, since there are
        /// folders (and perhaps files) that are not managed by us (e.g. D:\Data or D:\Logs).
        /// </param>
        /// <param name="path">
        /// The folder path used for logging only.
        /// </param>
        private void PopulateUpdateSet(
            IDirectoryInfo directory,
            FolderUpdate commandFolder,
            UpdateFolder updateSetFolder,
            bool deleteItems,
            string path)
        {
            Logger.Trace("Computing differences in {0}", path);

            var commandItems = new List<FileSystemUpdate>(commandFolder.Items);
            if (directory != null)
            {
                this.AddExistingFiles(directory.GetFiles(), updateSetFolder, deleteItems, commandItems);

                this.AddExistingDirectories(directory.GetDirectories(), updateSetFolder, deleteItems, commandItems);
            }

            this.AddNewItems(updateSetFolder, commandItems, path);
        }

        private void AddExistingDirectories(
            IEnumerable<IDirectoryInfo> directories,
            UpdateFolder folder,
            bool deleteItems,
            List<FileSystemUpdate> commandItems)
        {
            foreach (var dir in directories)
            {
                if (this.progressMonitor.IsCancelled)
                {
                    throw new InstallationCancelledException();
                }

                var subNode = new UpdateFolder(folder) { Name = dir.Name };
                var currentFolder =
                    commandItems.Find(i => i.Name.Equals(dir.Name, DefaultStringComparison)) as FolderUpdate;
                if (currentFolder != null)
                {
                    commandItems.Remove(currentFolder);
                    this.PopulateUpdateSet(dir, currentFolder, subNode, true, dir.FullName);
                    if (subNode.Items.Count == 0)
                    {
                        Logger.Trace("Folder is up to date: {0}", dir.FullName);
                        continue;
                    }

                    Logger.Trace("Found folder to be updated: {0}", dir.FullName);
                    subNode.Action = ActionType.Update;
                }
                else if (deleteItems)
                {
                    Logger.Trace("Found folder to be deleted: {0}", dir.FullName);
                    subNode.Action = ActionType.Delete;
                    this.PopulateUpdateSet(dir, new FolderUpdate(), subNode, true, dir.FullName);
                }
                else
                {
                    continue;
                }

                folder.Items.Add(subNode);
            }
        }

        private void AddExistingFiles(
            IEnumerable<IFileInfo> files, UpdateFolder folder, bool deleteItems, List<FileSystemUpdate> commandItems)
        {
            foreach (var file in files)
            {
                if (this.progressMonitor.IsCancelled)
                {
                    throw new InstallationCancelledException();
                }

                var fileNode = new UpdateFile(folder) { Name = file.Name };
                var currentFile =
                    commandItems.Find(i => i.Name.Equals(file.Name, DefaultStringComparison)) as FileUpdate;
                if (currentFile != null)
                {
                    this.progressMonitor.Progress(
                        (++this.progress) / this.maxProgress, "Create Update Set: " + file.Name);

                    commandItems.Remove(currentFile);
                    var hash = this.CreateHash(file);
                    if (currentFile.Hash == hash)
                    {
                        Logger.Trace("File is up to date: {0}", file.FullName);
                        continue;
                    }

                    Logger.Trace("Found file to be updated: {0}", file.FullName);
                    fileNode.Action = ActionType.Update;
                    fileNode.ResourceId = new ResourceId(currentFile.Hash);
                }
                else if (deleteItems && !file.Name.Equals(CompactFrameworkDebugFile, DefaultStringComparison))
                {
                    Logger.Trace("Found file to be deleted: {0}", file.FullName);
                    fileNode.Action = ActionType.Delete;
                }
                else
                {
                    Logger.Trace("Ignoring file to be deleted: {0}", file.FullName);
                    continue;
                }

                folder.Items.Add(fileNode);
            }
        }

        private void AddNewItems(UpdateFolder updateSetFolder, IEnumerable<FileSystemUpdate> commandItems, string path)
        {
            foreach (var item in commandItems)
            {
                if (this.progressMonitor.IsCancelled)
                {
                    throw new InstallationCancelledException();
                }

                var subPath = Path.Combine(path, item.Name);
                var folder = item as FolderUpdate;
                if (folder != null)
                {
                    var updateFolder = new UpdateFolder(updateSetFolder)
                                           {
                                               Name = item.Name,
                                               Action = ActionType.Create,
                                           };
                    Logger.Trace("Found new folder: {0}", subPath);
                    this.PopulateUpdateSet(null, folder, updateFolder, true, subPath);
                    updateSetFolder.Items.Add(updateFolder);
                    continue;
                }

                var file = item as FileUpdate;
                if (file != null)
                {
                    this.progressMonitor.Progress(
                        (++this.progress) / this.maxProgress,
                        "Create Update Set: " + subPath);
                    Logger.Trace("Found new file: {0}", subPath);
                    var updateFile = new UpdateFile(updateSetFolder)
                                         {
                                             Name = item.Name,
                                             Action = ActionType.Create,
                                             ResourceId = new ResourceId(file.Hash)
                                         };
                    updateSetFolder.Items.Add(updateFile);
                }
            }
        }

        private string CreateHash(IFileInfo file)
        {
            using (var input = file.OpenRead())
            {
                return ResourceHash.Create(input);
            }
        }
    }
}
