// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateSelector.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateSelector type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class responsible for searching the right <see cref="UpdateCommand"/>s to install.
    /// </summary>
    public class UpdateSelector
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UpdateSelector>();

        private readonly List<UpdateCommand> commands;

        private readonly List<UpdateStateInfo> feedback = new List<UpdateStateInfo>();

        private readonly List<UpdateCommand> commandsToInstall = new List<UpdateCommand>();

        private readonly List<UpdateCommand> validatedParkedUpdateCommands = new List<UpdateCommand>();

        private bool sendTransferredStateForParkedUdates;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateSelector"/> class.
        /// </summary>
        /// <param name="commands">
        /// The received commands.
        /// </param>
        public UpdateSelector(IEnumerable<UpdateCommand> commands)
        {
            this.commands = new List<UpdateCommand>(commands);
            this.Feedback = new ReadOnlyCollection<UpdateStateInfo>(this.feedback);
            this.CommandsToInstall = new ReadOnlyCollection<UpdateCommand>(this.commandsToInstall);
            this.ValidatedParkedUpdateCommands =
                new ReadOnlyCollection<UpdateCommand>(this.validatedParkedUpdateCommands);
        }

        /// <summary>
        /// Gets or sets the validated parked update commands which is the list of parked update commands
        /// with valid activate time and update index.
        /// </summary>
        public IList<UpdateCommand> ValidatedParkedUpdateCommands { get; set; }

        /// <summary>
        /// Gets a read-only list of the feedback to send about the commands.
        /// This property is only valid after calling <see cref="Search"/>.
        /// </summary>
        public IList<UpdateStateInfo> Feedback { get; private set; }

        /// <summary>
        /// Gets a read-only list of the commands to actually install
        /// (in the order they have to be installed).
        /// This property is only valid after calling <see cref="Search"/>.
        /// </summary>
        public IList<UpdateCommand> CommandsToInstall { get; private set; }

        /// <summary>
        /// Searches for all commands to install and generates the feedback for all commands.
        /// </summary>
        /// <param name="lastUpdateId">
        /// The last <see cref="UpdateId"/> installed on this system.
        /// </param>
        /// <param name="parkedUpdateCommands">
        /// The parked Update Commands.
        /// </param>
        /// <param name="sendTransferredState">
        /// The flag indicating whether to send the Transferred State for parked updates or not.
        /// </param>
        public void Search(
            UpdateId lastUpdateId,
            IEnumerable<UpdateCommand> parkedUpdateCommands,
            bool sendTransferredState)
        {
            if (this.commands.Count == 0)
            {
                return;
            }

            this.sendTransferredStateForParkedUdates = sendTransferredState;

            this.commands.Sort((a, b) => b.UpdateId.UpdateIndex.CompareTo(a.UpdateId.UpdateIndex));
            var newBackgroundSystemCommands =
                this.commands.FindAll(c => c.UpdateId.BackgroundSystemGuid != lastUpdateId.BackgroundSystemGuid);
            if (newBackgroundSystemCommands.Count > 0)
            {
                var newBackgroundSystemId = newBackgroundSystemCommands[0].UpdateId.BackgroundSystemGuid;
                foreach (var command in this.commands)
                {
                    if (newBackgroundSystemCommands.Contains(command))
                    {
                        continue;
                    }

                    this.feedback.Add(
                        this.CreateFeedback(
                            command, UpdateState.Ignored, "New background system: " + newBackgroundSystemId));
                }

                var newSelector = new UpdateSelector(newBackgroundSystemCommands);
                newSelector.Search(
                    new UpdateId(newBackgroundSystemId, -1),
                    parkedUpdateCommands,
                    this.sendTransferredStateForParkedUdates);
                this.feedback.AddRange(newSelector.Feedback);
                this.commandsToInstall.AddRange(newSelector.CommandsToInstall);
                this.validatedParkedUpdateCommands.AddRange(newSelector.ValidatedParkedUpdateCommands);
                return;
            }

            this.FindCommandsToInstall(lastUpdateId, parkedUpdateCommands);

            // after this the list contains items in order of installation
            this.commandsToInstall.Reverse();
        }

        private static bool HasRunCommands(RunCommands runCommands)
        {
            return runCommands != null && runCommands.Items.Count > 0;
        }

        private void FindCommandsToInstall(UpdateId lastUpdateId, IEnumerable<UpdateCommand> parkedUpdates)
        {
            var parkedUpdateCommands = new List<UpdateCommand>(parkedUpdates);
            var now = TimeProvider.Current.UtcNow;
            foreach (var updateCommand in this.commands)
            {
                if (updateCommand.UpdateId.UpdateIndex == lastUpdateId.UpdateIndex)
                {
                    this.feedback.Add(
                        this.CreateFeedback(updateCommand, UpdateState.Installed, "Update was installed previously"));
                    continue;
                }

                if (updateCommand.UpdateId.UpdateIndex < lastUpdateId.UpdateIndex)
                {
                    var reason = string.Format(
                        "Lower update index: {0} < {1}", updateCommand.UpdateId.UpdateIndex, lastUpdateId.UpdateIndex);
                    this.feedback.Add(this.CreateFeedback(updateCommand, UpdateState.Ignored, reason));
                    continue;
                }

                if (this.commandsToInstall.Count != 0 && !HasRunCommands(updateCommand.PreInstallation)
                    && !HasRunCommands(updateCommand.PostInstallation))
                {
                    var reason = string.Format(
                        "Old update: {0} < {1}",
                        updateCommand.UpdateId.UpdateIndex,
                        this.commandsToInstall[0].UpdateId.UpdateIndex);
                    this.feedback.Add(this.CreateFeedback(updateCommand, UpdateState.Ignored, reason));
                    continue;
                }

                if (updateCommand.ActivateTime <= now && !updateCommand.InstallAfterBoot)
                {
                    Logger.Debug(
                        "Found command for the current BackgroundSystem to be installed: old={0} new={1} for {2}",
                        lastUpdateId.UpdateIndex,
                        updateCommand.UpdateId.UpdateIndex,
                        updateCommand.UpdateId.BackgroundSystemGuid);

                    this.commandsToInstall.Add(updateCommand);
                    if (this.sendTransferredStateForParkedUdates)
                    {
                        this.feedback.Add(this.CreateFeedback(updateCommand, UpdateState.Transferred, null));
                    }

                    this.RemoveLowerIndexedParkedUpdates(parkedUpdateCommands, updateCommand);

                    continue;
                }

                // this update command is a candidate for parking
                string ignoreReason;
                var updateIndexIsSameOrLower = this.VerifyAgainstOldParkedUpdates(
                    parkedUpdateCommands, updateCommand, out ignoreReason);

                if (this.commandsToInstall.Count > 0)
                {
                    var reason = string.Format(
                        "Old parked update: {0} < {1} (installing)",
                        updateCommand.UpdateId.UpdateIndex,
                        this.commandsToInstall[0].UpdateId.UpdateIndex);
                    this.feedback.Add(this.CreateFeedback(updateCommand, UpdateState.Ignored, reason));
                    continue;
                }

                if (updateIndexIsSameOrLower)
                {
                    this.feedback.Add(this.CreateFeedback(updateCommand, UpdateState.Ignored, ignoreReason));
                    continue;
                }

                this.SaveParkedUpdateCommand(parkedUpdateCommands, updateCommand);
            }

            this.ValidatedParkedUpdateCommands = parkedUpdateCommands;
        }

        private void SaveParkedUpdateCommand(List<UpdateCommand> parkedUpdateCommands, UpdateCommand updateCommand)
        {
            parkedUpdateCommands.Add(updateCommand);
            Logger.Debug(
                "Found command for the current BackgroundSystem to be parked: {0} for {1}",
                updateCommand.UpdateId.UpdateIndex,
                updateCommand.UpdateId.BackgroundSystemGuid);
            if (this.sendTransferredStateForParkedUdates)
            {
                this.feedback.Add(this.CreateFeedback(
                    updateCommand, UpdateState.Transferred, "Update has been parked"));
            }
        }

        private void RemoveLowerIndexedParkedUpdates(
            List<UpdateCommand> parkedUpdateCommands, UpdateCommand updateCommand)
        {
            var removedParkedUpdateCommands =
                parkedUpdateCommands.FindAll(x => x.UpdateId.UpdateIndex < updateCommand.UpdateId.UpdateIndex);
            foreach (var removedParkedUpdateCommand in removedParkedUpdateCommands)
            {
                var reason = string.Format(
                    "Old parked update: {0} < {1}",
                    removedParkedUpdateCommand.UpdateId.UpdateIndex,
                    updateCommand.UpdateId.UpdateIndex);
                this.feedback.Add(this.CreateFeedback(removedParkedUpdateCommand, UpdateState.Ignored, reason));
                parkedUpdateCommands.Remove(removedParkedUpdateCommand);
            }
        }

        private bool VerifyAgainstOldParkedUpdates(
            List<UpdateCommand> parkedUpdateCommands, UpdateCommand updateCommand, out string ignoreReason)
        {
            var updateIndexIsSameOrLower = false;
            ignoreReason = string.Empty;
            if (parkedUpdateCommands != null && parkedUpdateCommands.Count > 0)
            {
                var removedParkedUpdateCommands = new List<UpdateCommand>();
                foreach (var parkedUpdateCommand in parkedUpdateCommands)
                {
                    if (updateCommand.UpdateId.UpdateIndex < parkedUpdateCommand.UpdateId.UpdateIndex &&
                        (updateCommand.ActivateTime.CompareTo(parkedUpdateCommand.ActivateTime) >= 0))
                    {
                        updateIndexIsSameOrLower = true;
                        ignoreReason = string.Format(
                            "Old parked update: {0} < {1}",
                            updateCommand.UpdateId.UpdateIndex,
                            parkedUpdateCommand.UpdateId.UpdateIndex);
                        continue;
                    }

                    if (updateCommand.UpdateId.UpdateIndex == parkedUpdateCommand.UpdateId.UpdateIndex)
                    {
                        updateIndexIsSameOrLower = true;
                        ignoreReason = string.Format(
                            "Update was parked previously: {0} and {1}",
                            updateCommand.UpdateId.BackgroundSystemGuid,
                            updateCommand.UpdateId.UpdateIndex);
                        continue;
                    }

                    if (updateCommand.UpdateId.UpdateIndex.CompareTo(parkedUpdateCommand.UpdateId.UpdateIndex) > 0
                        && (updateCommand.ActivateTime.CompareTo(parkedUpdateCommand.ActivateTime) > 0))
                    {
                        continue;
                    }

                    if (updateCommand.ActivateTime.CompareTo(parkedUpdateCommand.ActivateTime) <= 0)
                    {
                        continue;
                    }

                    removedParkedUpdateCommands.Add(parkedUpdateCommand);
                    var reason =
                        string.Format(
                            "Old parked update: {0} < {1} (parked) with activation time {2} same/less than {3}",
                            parkedUpdateCommand.UpdateId.UpdateIndex,
                            updateCommand.UpdateId.UpdateIndex,
                            parkedUpdateCommand.ActivateTime,
                            updateCommand.ActivateTime);
                    this.feedback.Add(this.CreateFeedback(parkedUpdateCommand, UpdateState.Ignored, reason));
                }

                foreach (var removedParkedUpdateCommand in removedParkedUpdateCommands)
                {
                    parkedUpdateCommands.Remove(removedParkedUpdateCommand);
                }

                removedParkedUpdateCommands.Clear();
            }

            return updateIndexIsSameOrLower;
        }

        private UpdateStateInfo CreateFeedback(
            UpdateCommand updateCommand, UpdateState state, string errorReason)
        {
            var factory = new UpdateStateInfoFactory(updateCommand);
            return factory.CreateFeedback(state, errorReason);
        }
    }
}
