// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateStateInfoFactory.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateStateInfoFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Resources;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Files;

    using NLog;

    /// <summary>
    /// Factory for <see cref="UpdateStateInfo"/> objects.
    /// </summary>
    public class UpdateStateInfoFactory
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UpdateStateInfoFactory>();

        private readonly UpdateCommand updateCommand;

        private readonly IDirectoryInfo installationRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStateInfoFactory"/> class.
        /// </summary>
        /// <param name="updateCommand">
        /// The update command.
        /// </param>
        public UpdateStateInfoFactory(UpdateCommand updateCommand)
            : this(updateCommand, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateStateInfoFactory"/> class.
        /// </summary>
        /// <param name="updateCommand">
        /// The update command.
        /// </param>
        /// <param name="installationRoot">
        /// The installation root.
        /// </param>
        public UpdateStateInfoFactory(UpdateCommand updateCommand, IDirectoryInfo installationRoot)
        {
            this.updateCommand = updateCommand;
            this.installationRoot = installationRoot;
        }

        /// <summary>
        /// Creates the feedback for the given state.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <param name="description">
        /// The description; mandatory if the <see cref="state"/> is an error state.
        /// </param>
        /// <returns>
        /// The <see cref="UpdateStateInfo"/>. It will contain the folder structure if required.
        /// </returns>
        public UpdateStateInfo CreateFeedback(UpdateState state, string description)
        {
            var unitId = new UnitId(this.updateCommand.UnitId.UnitName);
            var updateId = new UpdateId(
                this.updateCommand.UpdateId.BackgroundSystemGuid, this.updateCommand.UpdateId.UpdateIndex);

            Logger.Info(
                "Created feedback (State={0}) for {1} of {2}",
                state,
                updateId.UpdateIndex,
                updateId.BackgroundSystemGuid);

            var feedback = new UpdateStateInfo
            {
                UnitId = unitId,
                TimeStamp = TimeProvider.Current.UtcNow,
                State = state,
                UpdateId = updateId,
                Description = description,
            };

            if (state == UpdateState.Installed)
            {
                this.AddFolderFeedback(feedback);
            }
            else if (state == UpdateState.PartiallyInstalled)
            {
                if (this.installationRoot == null)
                {
                    throw new NotSupportedException("Installation root is required for " + state);
                }

                this.AddFolderFeedback(feedback, this.installationRoot);
            }

            return feedback;
        }

        private FolderUpdateInfo AddFolderFeedback(UpdateStateInfo feedback)
        {
            var commandRoot = new FolderUpdate();
            foreach (var folder in this.updateCommand.Folders)
            {
                commandRoot.Items.Add(folder);
            }

            var feedbackRoot = new FolderUpdateInfo();
            this.CreateFolderFeedback(feedbackRoot, commandRoot);

            foreach (var item in feedbackRoot.Items)
            {
                feedback.Folders.Add((FolderUpdateInfo)item);
            }

            return feedbackRoot;
        }

        private void CreateFolderFeedback(FolderUpdateInfo feedback, FolderUpdate folder)
        {
            foreach (var item in folder.Items)
            {
                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    var subFeedback = new FolderUpdateInfo { Name = item.Name, State = ItemUpdateState.UpToDate };
                    feedback.Items.Add(subFeedback);
                    this.CreateFolderFeedback(subFeedback, subFolder);
                    continue;
                }

                var file = item as FileUpdate;
                if (file != null)
                {
                    var fileFeedback = new FileUpdateInfo
                                           {
                                               Name = item.Name,
                                               State = ItemUpdateState.UpToDate,
                                               Hash = file.Hash,
                                               ExpectedHash = file.Hash
                                           };
                    feedback.Items.Add(fileFeedback);
                }
            }
        }

        private void AddFolderFeedback(UpdateStateInfo feedback, IDirectoryInfo rootDirectory)
        {
            var feedbackRoot = this.AddFolderFeedback(feedback);
            var factory = new UpdateSetFactory(rootDirectory, this.updateCommand, null);
            var updateSet = factory.CreateUpdateSet();

            var updateSetRoot = new UpdateFolder(null);
            foreach (var folder in updateSet.Folders)
            {
                updateSetRoot.Items.Add(folder);
            }

            this.UpdateFolderFeedback(feedbackRoot, updateSetRoot);
        }

        private void UpdateFolderFeedback(FolderUpdateInfo feedback, UpdateFolder folder)
        {
            foreach (var item in folder.Items)
            {
                var subFolder = item as UpdateFolder;
                if (subFolder != null)
                {
                    var subFeedback =
                        feedback.Items.Find(i => i.Name == item.Name && i is FolderUpdateInfo) as FolderUpdateInfo;
                    if (subFeedback == null)
                    {
                        subFeedback = new FolderUpdateInfo { Name = item.Name };
                        feedback.Items.Add(subFeedback);
                    }

                    this.UpdateFolderFeedback(subFeedback, subFolder);
                    switch (subFolder.Action)
                    {
                        case ActionType.Create:
                            subFeedback.State = ItemUpdateState.NotCreated;
                            break;
                        case ActionType.Delete:
                            subFeedback.State = ItemUpdateState.NotDeleted;
                            break;
                        default: // i.e. ActionType.Update
                            subFeedback.State =
                                subFeedback.Items.Find(
                                    i =>
                                    i.State == ItemUpdateState.UpToDate ||
                                    i.State == ItemUpdateState.PartiallyUpdated) == null
                                        ? ItemUpdateState.NotUpdated
                                        : ItemUpdateState.PartiallyUpdated;
                            break;
                    }

                    continue;
                }

                var file = item as UpdateFile;
                if (file != null)
                {
                    var fileFeedback =
                        feedback.Items.Find(i => i.Name == item.Name && i is FileUpdateInfo) as FileUpdateInfo;
                    if (fileFeedback == null)
                    {
                        fileFeedback = new FileUpdateInfo { Name = item.Name };
                        feedback.Items.Add(fileFeedback);
                    }

                    switch (file.Action)
                    {
                        case ActionType.Create:
                            fileFeedback.State = ItemUpdateState.NotCreated;
                            fileFeedback.Hash = null;
                            fileFeedback.ExpectedHash = file.ResourceId.Hash;
                            break;
                        case ActionType.Delete:
                            fileFeedback.State = ItemUpdateState.NotDeleted;
                            fileFeedback.Hash = this.CreateLocalHash(file);
                            fileFeedback.ExpectedHash = null;
                            break;
                        default: // i.e. ActionType.Update
                            fileFeedback.State = ItemUpdateState.NotUpdated;
                            fileFeedback.Hash = this.CreateLocalHash(file);
                            fileFeedback.ExpectedHash = file.ResourceId.Hash;
                            break;
                    }
                }
            }
        }

        private string CreateLocalHash(UpdateFile file)
        {
            var pathItems = new List<string>();
            for (UpdateSubNode item = file; item != null && item.Name != null; item = item.Parent)
            {
                pathItems.Add(item.Name);
            }

            pathItems.Reverse();

            var path = new StringBuilder(this.installationRoot.FullName);
            foreach (var item in pathItems)
            {
                path.Append(item).Append(Path.DirectorySeparatorChar);
            }

            path.Length--;

            try
            {
                using (var input = this.installationRoot.FileSystem.GetFile(path.ToString()).OpenRead())
                {
                    return ResourceHash.Create(input);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't calculate hash of path={0}", path);
                return null;
            }
        }
    }
}
