// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateComponentBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateComponentBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel.Common
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Compatibility;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Base class for all classes that implement <see cref="IUpdateComponent"/>.
    /// </summary>
    public abstract class UpdateComponentBase : IUpdateComponent
    {
        /// <summary>
        /// The string used to say that we are interested in all unit update commands.
        /// </summary>
        public static readonly string UnitWildcard = "*";

        private bool available;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateComponentBase"/> class.
        /// </summary>
        protected UpdateComponentBase()
        {
            this.Logger = LogManager.GetLogger(this.GetType().FullName);
        }

        /// <summary>
        /// Event that is fired when <see cref="IsAvailable"/> changes.
        /// </summary>
        public virtual event EventHandler IsAvailableChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the repository is available
        /// (e.g. the USB stick is plugged in or the FTP server is reachable).
        /// </summary>
        public virtual bool IsAvailable
        {
            get
            {
                this.IsAvailable = this.CheckAvailable();
                return this.available;
            }

            protected set
            {
                if (this.available == value)
                {
                    return;
                }

                this.available = value;
                this.RaiseIsAvailableChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Gets the unique name of this sink.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the NLog logger.
        /// </summary>
        protected Logger Logger { get; set; }

        /// <summary>
        /// Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        /// True if the repository is available.
        /// </returns>
        protected abstract bool CheckAvailable();

        /// <summary>
        /// Raises the <see cref="IsAvailableChanged"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        protected virtual void RaiseIsAvailableChanged(EventArgs e)
        {
            var handler = this.IsAvailableChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Gets all files for the given command including all files in:
        /// - <see cref="UpdateCommand.Folders"/>
        /// - <see cref="UpdateCommand.PreInstallation"/> items
        /// - <see cref="UpdateCommand.PostInstallation"/> items
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// An enumeration over the <see cref="FileUpdate"/>s found in the command.
        /// </returns>
        protected IEnumerable<FileUpdate> GetAllFiles(UpdateCommand command)
        {
            var root = new FolderUpdate();
            foreach (var folder in command.Folders)
            {
                root.Items.Add(folder);
            }

            this.AddRunCommands(root, command.PreInstallation);
            this.AddRunCommands(root, command.PostInstallation);
            var files = new List<FileUpdate>();
            this.GetAllFiles(root, files);
            return files;
        }

        /// <summary>
        /// Creates an <see cref="UpdateStateInfo"/> with the state
        /// <see cref="UpdateState.TransferFailed"/> for the given update command.
        /// </summary>
        /// <param name="updateCommand">
        /// The update command.
        /// </param>
        /// <param name="reason">
        /// The reason for the error.
        /// </param>
        /// <returns>
        /// The newly created <see cref="UpdateStateInfo"/>.
        /// </returns>
        protected UpdateStateInfo CreateTransferFailedFeedback(UpdateCommand updateCommand, string reason)
        {
            this.Logger.Info(
                "Created TransferFailed feedback for {0} of {1}",
                updateCommand.UpdateId.UpdateIndex,
                updateCommand.UpdateId.BackgroundSystemGuid);

            var feedback = new UpdateStateInfo
                               {
                                   UnitId = updateCommand.UnitId,
                                   TimeStamp = TimeProvider.Current.UtcNow,
                                   State = UpdateState.TransferFailed,
                                   UpdateId = updateCommand.UpdateId,
                                   Description =
                                       string.Format(
                                           "From {0} @ {1}: {2}",
                                           this.GetType().Name,
                                           ApplicationHelper.MachineName,
                                           reason)
                               };

            return feedback;
        }

        private void AddRunCommands(FolderUpdate root, RunCommands commands)
        {
            if (commands == null)
            {
                return;
            }

            root.Items.AddRange(commands.Items);
        }

        private void GetAllFiles(FolderUpdate folder, List<FileUpdate> files)
        {
            foreach (var item in folder.Items)
            {
                var subFolder = item as FolderUpdate;
                if (subFolder != null)
                {
                    this.GetAllFiles(subFolder, files);
                    continue;
                }

                var file = item as FileUpdate;
                if (file != null)
                {
                    files.Add(file);
                }
            }
        }
    }
}