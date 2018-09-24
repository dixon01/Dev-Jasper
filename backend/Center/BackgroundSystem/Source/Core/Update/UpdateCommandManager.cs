// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandManager.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateCommandManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.Filters.Update;
    using Gorba.Center.Common.ServiceModel.Units;
    using Gorba.Center.Common.ServiceModel.Update;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    using UpdateCommandDto = Gorba.Center.Common.ServiceModel.Update.UpdateCommand;
    using UpdateCommandMsg = Gorba.Common.Update.ServiceModel.Messages.UpdateCommand;
    using UpdateState = Gorba.Center.Common.ServiceModel.Update.UpdateState;

    /// <summary>
    /// The update command manager responsible for creating update commands for units and
    /// storing them into the database.
    /// </summary>
    public class UpdateCommandManager
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IUpdateCommandDataService updateCommandDataService;

        private readonly IUpdateFeedbackDataService updateFeedbackDataService;

        private readonly ISystemConfigDataService systemConfigDataService;

        private string backgroundSystemGuid;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandManager"/> class.
        /// </summary>
        public UpdateCommandManager()
        {
            this.updateCommandDataService = DependencyResolver.Current.Get<IUpdateCommandDataService>();
            this.updateFeedbackDataService = DependencyResolver.Current.Get<IUpdateFeedbackDataService>();
            this.systemConfigDataService = DependencyResolver.Current.Get<ISystemConfigDataService>();
        }

        /// <summary>
        /// Creates all required new update commands for the given update group.
        /// </summary>
        /// <param name="updateGroup">
        /// The update group.
        /// It's <see cref="UpdateGroup.UpdateParts"/> must be set correctly.
        /// The UpdatePart's <see cref="UpdatePart.Structure"/> and
        /// <see cref="UpdatePart.InstallInstructions"/> must also be set correctly.
        /// </param>
        /// <returns>
        /// The list of newly created <see cref="UpdateCommandMsg"/> message objects to
        /// be sent to units.
        /// </returns>
        public async Task<IEnumerable<UpdateCommandMsg>> CreateUpdateCommandsAsync(UpdateGroup updateGroup)
        {
            var updates = this.GetCurrentUpdates(updateGroup);
            if (updates.Count == 0)
            {
                return Enumerable.Empty<UpdateCommandMsg>();
            }

            var tasks = updateGroup.Units.Select(unit => this.CreateUpdateCommandsAsync(updates, unit)).ToArray();
            return (await Task.WhenAll(tasks)).SelectMany(c => c.ToList());
        }

        /// <summary>
        /// Creates all required new update commands for the given unit.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// It's <see cref="Unit.UpdateGroup"/> must be set correctly.
        /// The UpdateGroup's <see cref="UpdateGroup.UpdateParts"/> must also be set correctly.
        /// The UpdatePart's <see cref="UpdatePart.Structure"/> and
        /// <see cref="UpdatePart.InstallInstructions"/> must also be set correctly.
        /// </param>
        /// <returns>
        /// The list of newly created <see cref="UpdateCommandMsg"/> message objects to
        /// be sent to the unit.
        /// </returns>
        public async Task<IEnumerable<UpdateCommandMsg>> CreateUpdateCommandsAsync(Unit unit)
        {
            if (unit.UpdateGroup == null)
            {
                throw new ArgumentException("Unit doesn't have an UpdateGroup");
            }

            var updates = this.GetCurrentUpdates(unit.UpdateGroup);
            if (updates.Count == 0)
            {
                return Enumerable.Empty<UpdateCommandMsg>();
            }

            return await this.CreateUpdateCommandsAsync(updates, unit);
        }

        /// <summary>
        /// Adds update state feedback to the database.
        /// </summary>
        /// <param name="stateInfos">
        /// The state information to add.
        /// </param>
        /// <returns>
        /// The async <see cref="Task"/>.
        /// </returns>
        public async Task AddFeedbacksAsync(params UpdateStateInfo[] stateInfos)
        {
            var guid = await this.GetBackgroundSystemIdAsync();
            foreach (
                var indexGroup in
                    stateInfos.Where(s => s.UpdateId.BackgroundSystemGuid == guid).GroupBy(s => s.UpdateId.UpdateIndex))
            {
                var commands =
                    (await
                     this.updateCommandDataService.QueryAsync(
                         UpdateCommandQuery.Create().WithUpdateIndex(indexGroup.Key).IncludeUnit())).ToList();
                foreach (var unitGroup in indexGroup.GroupBy(s => s.UnitId.UnitName))
                {
                    var command = commands.FirstOrDefault(c => c.Unit.Name == unitGroup.Key);
                    if (command == null)
                    {
                        Logger.Warn("Couldn't find unit {0} to add feedback", unitGroup.Key);
                        continue;
                    }

                    foreach (var updateStateInfo in unitGroup)
                    {
                        await this.AddFeedbackAsync(command, updateStateInfo);
                    }
                }
            }
        }

        private IList<GroupUpdate> GetCurrentUpdates(UpdateGroup updateGroup)
        {
            if (updateGroup.UpdateParts == null)
            {
                throw new ArgumentException("UpdateGroup doesn't contain UpdateParts");
            }

            if (updateGroup.UpdateParts.Count == 0)
            {
                Logger.Warn("Couldn't find any update parts for {0}", updateGroup.Name);
                return new GroupUpdate[0];
            }

            var now = TimeProvider.Current.UtcNow;
            var times =
                updateGroup.UpdateParts.Select(p => p.Start)
                    .Union(updateGroup.UpdateParts.Select(p => p.End))
                    .Distinct()
                    .OrderBy(t => t).ToList();
            var firstIndex = times.FindIndex(t => t > now);
            if (firstIndex < 0)
            {
                Logger.Debug("Couldn't find any valid updates for {0}", updateGroup.Name);
                return new GroupUpdate[0];
            }

            if (firstIndex > 0)
            {
                // we want the time before "now"
                firstIndex--;
            }

            var updates = new List<GroupUpdate>(times.Count - firstIndex);
            for (var i = firstIndex; i < times.Count - 1; i++)
            {
                var time = times[i];
                var update = new GroupUpdate(time);
                updates.Add(update);
                foreach (var part in
                    updateGroup.UpdateParts.Where(p => time >= p.Start && time < p.End)
                        .OrderByDescending(p => p.Id)
                        .GroupBy(p => p.Type).Select(g => g.First()))
                {
                    update.IncludedParts.Add(part);
                    update.Structure.Include((UpdateFolderStructure)part.Structure.Deserialize());
                    var instructions = part.InstallInstructions.Deserialize() as InstallationInstructions;
                    if (instructions != null)
                    {
                        update.InstallInstructions.Include(instructions);
                    }
                }
            }

            return updates;
        }

        private async Task<IEnumerable<UpdateCommandMsg>> CreateUpdateCommandsAsync(
            IList<GroupUpdate> updates, Unit unit)
        {
            var query = UpdateCommandQuery.Create().WithUnit(unit).IncludeCommand().OrderByUpdateIndex();
            var existingCommands =
                (await this.updateCommandDataService.QueryAsync(query)).Select(
                    c => (UpdateCommandMsg)c.Command.Deserialize()).ToList();
            var updateIndex = existingCommands.Select(c => c.UpdateId.UpdateIndex).LastOrDefault();

            var existingIndex = existingCommands.FindLastIndex(c => updates[0].Matches(c));

            var commands = new List<UpdateCommandMsg>();
            for (int i = 0; i < updates.Count; i++)
            {
                var update = updates[i];
                if (existingIndex >= 0 && existingIndex < existingCommands.Count)
                {
                    if (update.Matches(existingCommands[existingIndex]))
                    {
                        existingIndex++;
                        continue;
                    }

                    if (i > 0 && existingCommands[existingIndex].ActivateTime < update.ActivateTime)
                    {
                        // special case: the current command would be later than the one we are
                        // overwriting, therefore we must add the previous command to the list as well
                        commands.Add(await this.CreateUpdateCommandAsync(updates[i - 1], unit, ++updateIndex));
                    }
                }

                // from now on, ignore existing commands, if one is wrong, they are all wrong
                existingIndex = -1;
                commands.Add(await this.CreateUpdateCommandAsync(update, unit, ++updateIndex));
            }

            if (existingIndex >= 0 && existingIndex != existingCommands.Count)
            {
                // special case: there are more existing commands than we computed, so we need to
                // delete them by sending the last found command with a new update index,
                // this will overwrite the last command plus all later commands
                commands.Add(await this.CreateUpdateCommandAsync(updates.Last(), unit, ++updateIndex));
            }

            return commands;
        }

        private async Task<UpdateCommandMsg> CreateUpdateCommandAsync(GroupUpdate update, Unit unit, int updateIndex)
        {
            var commandDto = new UpdateCommandDto
                                 {
                                     Feedbacks = new List<UpdateFeedback>(),
                                     IncludedParts = new List<UpdatePart>(),
                                     Unit = unit,
                                     UpdateIndex = updateIndex,
                                     WasInstalled = false
                                 };
            update.IncludedParts.ForEach(commandDto.IncludedParts.Add);

            var guid = await this.GetBackgroundSystemIdAsync();

            // TODO: pre and post installations could be skipped once they are done
            // (i.e. only add it to the first command with that RunCommands defined)
            var command = new UpdateCommandMsg
                              {
                                  ActivateTime = update.ActivateTime,
                                  UnitId = new UnitId { UnitName = unit.Name },
                                  UpdateId = new UpdateId(guid, updateIndex),
                                  Folders = update.Structure.Folders,
                                  InstallAfterBoot = update.InstallAfterBoot,
                                  PreInstallation = update.PreInstallation,
                                  PostInstallation = update.PostInstallation
                              };
            commandDto.Command = new XmlData(command);
            await this.updateCommandDataService.AddAsync(commandDto);
            return command;
        }

        private async Task<string> GetBackgroundSystemIdAsync()
        {
            if (this.backgroundSystemGuid != null)
            {
                return this.backgroundSystemGuid;
            }

            var config = (await this.systemConfigDataService.QueryAsync()).Single();
            return this.backgroundSystemGuid = config.SystemId.ToString();
        }

        private async Task AddFeedbackAsync(UpdateCommandDto command, UpdateStateInfo updateStateInfo)
        {
            // we parse through a string so we don't cast by integer value (which might be wrong)
            UpdateState state;
            if (!Enum.TryParse(updateStateInfo.State.ToString(), out state))
            {
                state = UpdateState.Unknown;
                Logger.Warn("Received unknown state from unit: {0}", updateStateInfo.State);
            }

            // update the command first, because otherwise the feedback update below might overwrite the command
            if (state == UpdateState.Transferred && !command.WasTransferred)
            {
                command.WasTransferred = true;
                command = await this.updateCommandDataService.UpdateAsync(command);
            }
            else if ((state == UpdateState.Installed || state == UpdateState.Ignored
                      || state == UpdateState.PartiallyInstalled) && !command.WasInstalled)
            {
                // in the above states, we know the command was installed (or ignored, which is the same for us)
                command.WasInstalled = true;
                command.WasTransferred = true;
                command = await this.updateCommandDataService.UpdateAsync(command);
            }

            var feedback = new UpdateFeedback
                               {
                                   State = state,
                                   Timestamp = updateStateInfo.TimeStamp,
                                   UpdateCommand = command,
                                   Feedback = new XmlData(updateStateInfo)
                               };
            await this.updateFeedbackDataService.AddAsync(feedback);
        }

        private class GroupUpdate
        {
            public GroupUpdate(DateTime activateTime)
            {
                this.ActivateTime = activateTime;
                this.IncludedParts = new List<UpdatePart>();
                this.Structure = new UpdateFolderStructure();
                this.InstallInstructions = new InstallationInstructions();
            }

            public DateTime ActivateTime { get; private set; }

            public List<UpdatePart> IncludedParts { get; private set; }

            public UpdateFolderStructure Structure { get; private set; }

            public InstallationInstructions InstallInstructions { get; private set; }

            public bool InstallAfterBoot
            {
                get
                {
                    return this.InstallInstructions.InstallAfterBoot.HasValue
                           && this.InstallInstructions.InstallAfterBoot.Value;
                }
            }

            public RunCommands PreInstallation
            {
                get
                {
                    if (this.InstallInstructions.PreInstallation == null
                        || this.InstallInstructions.PreInstallation.Items.Count == 0)
                    {
                        return null;
                    }

                    return this.InstallInstructions.PreInstallation;
                }
            }

            public RunCommands PostInstallation
            {
                get
                {
                    if (this.InstallInstructions.PostInstallation == null
                        || this.InstallInstructions.PostInstallation.Items.Count == 0)
                    {
                        return null;
                    }

                    return this.InstallInstructions.PostInstallation;
                }
            }

            public bool Matches(UpdateCommandMsg updateCommand)
            {
                return this.ActivateTime.Equals(updateCommand.ActivateTime)
                       && this.Structure.Equals(new UpdateFolderStructure { Folders = updateCommand.Folders });
            }
        }
    }
}