// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateValet.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Class responsible for managing a parked update
    /// </summary>
    public class UpdateValet
    {
        private static readonly Logger Logger = LogHelper.GetLogger<UpdateValet>();

        private readonly IList<UpdateCommand> parkedUpdateCommands;

        private readonly IDeadlineTimer parkingMeter;

        private bool running;

        private UpdateCommand lastParkedInstallAfterBootUpdate;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateValet"/> class.
        /// </summary>
        /// <param name="parkedUpdateCommands">
        /// The list of parked update commands coming from persistence.
        /// </param>
        public UpdateValet(IList<UpdateCommand> parkedUpdateCommands)
        {
            this.parkedUpdateCommands = parkedUpdateCommands;

            this.parkingMeter = TimerFactory.Current.CreateDeadlineTimer("ParkingMeter");
            this.parkingMeter.Elapsed += this.ParkingMeterOnElapsed;
            this.parkingMeter.TriggerIfPassed = true;
        }

        /// <summary>
        /// Event that is fired when a parked update becomes valid.
        /// </summary>
        public event EventHandler<UpdateCommandsEventArgs> UpdateDeparked;

        /// <summary>
        /// Gets the persisted parked update commands.
        /// </summary>
        public IEnumerable<UpdateCommand> StoredParkedUpdateCommands
        {
            get
            {
                return this.parkedUpdateCommands;
            }
        }

        /// <summary>
        /// Starts the valet.
        /// </summary>
        public void Start()
        {
            if (this.running)
            {
                return;
            }

            this.running = true;

            this.DeparkCommands();
            Logger.Debug("Update Valet has started");
        }

        /// <summary>
        /// Stops the valet.
        /// </summary>
        public void Stop()
        {
            if (!this.running)
            {
                return;
            }

            this.running = false;

            this.parkingMeter.Enabled = false;

            Logger.Debug("Update Valet has stopped");
        }

        /// <summary>
        /// Handles the parked update commands
        /// </summary>
        /// <param name="validatedParkedUpdateCommands">
        /// The validated parked update commands.
        /// </param>
        public void ParkUpdates(List<UpdateCommand> validatedParkedUpdateCommands)
        {
            this.parkingMeter.Enabled = false;
            this.parkedUpdateCommands.Clear();

            if (validatedParkedUpdateCommands.Count == 0)
            {
                Logger.Debug("There are no valid park updates available to park");
                return;
            }

            validatedParkedUpdateCommands.Sort((a, b) => a.ActivateTime.CompareTo(b.ActivateTime));
            validatedParkedUpdateCommands.ForEach(this.parkedUpdateCommands.Add);

            Logger.Debug(
                "Next update command to install is {0} for {1}",
                this.parkedUpdateCommands[0].UpdateId.UpdateIndex,
                this.parkedUpdateCommands[0].UpdateId.BackgroundSystemGuid);
            this.StartTimer();
        }

        private void DeparkCommands()
        {
            this.parkingMeter.Enabled = false;
            var commands = this.DeparkCommandsToInstall();

            this.StartTimer();

            if (commands.Count > 0)
            {
                this.RaiseUpdateDeparked(new UpdateCommandsEventArgs(commands.ToArray()));
            }
        }

        private List<UpdateCommand> DeparkCommandsToInstall()
        {
            if (this.parkedUpdateCommands.Count == 0)
            {
                Logger.Trace("There is no parked update available");
                return new List<UpdateCommand>(1);
            }

            var toInstall = new List<UpdateCommand>(this.parkedUpdateCommands.Count);
            var now = TimeProvider.Current.UtcNow;
            foreach (var updateCommand in this.parkedUpdateCommands)
            {
                if (updateCommand.ActivateTime > now)
                {
                    continue;
                }

                if (updateCommand == this.lastParkedInstallAfterBootUpdate)
                {
                    this.lastParkedInstallAfterBootUpdate = null;
                    continue;
                }

                // Forcefully set the installAfterBoot flag to false else it is stored again for
                // future installation in a cyclic manner
                updateCommand.InstallAfterBoot = false;
                toInstall.Add(updateCommand);
            }

            if (toInstall.Count == 0)
            {
                Logger.Trace("There is no parked update to be installed");
                return toInstall;
            }

            foreach (var updateCommand in toInstall)
            {
                this.parkedUpdateCommands.Remove(updateCommand);
                Logger.Info(
                    "Parked update with index {0} (at {1}) has been deparked",
                    updateCommand.UpdateId.UpdateIndex,
                    updateCommand.ActivateTime);
            }

            return toInstall;
        }

        private void StartTimer()
        {
            this.parkingMeter.Enabled = false;
            if (this.parkedUpdateCommands.Count == 0)
            {
                Logger.Trace("No next parked update available");
                return;
            }

            var now = TimeProvider.Current.UtcNow;
            DateTime? activateTime = null;
            foreach (var parkedUpdateCommand in this.parkedUpdateCommands)
            {
                if (parkedUpdateCommand.ActivateTime <= now && parkedUpdateCommand.InstallAfterBoot)
                {
                    continue;
                }

                activateTime = parkedUpdateCommand.ActivateTime;
                if (parkedUpdateCommand.InstallAfterBoot)
                {
                    this.lastParkedInstallAfterBootUpdate = parkedUpdateCommand;
                }

                break;
            }

            if (activateTime == null)
            {
                return;
            }

            if (activateTime < now)
            {
                activateTime = now + TimeSpan.FromSeconds(1);
            }

            this.parkingMeter.UtcDeadline = (DateTime)activateTime;
            Logger.Info("Starting parking meter to trigger at {0} (UTC)", activateTime);
            this.parkingMeter.Enabled = true;
        }

        private void ParkingMeterOnElapsed(object sender, EventArgs e)
        {
            this.DeparkCommands();
        }

        private void RaiseUpdateDeparked(UpdateCommandsEventArgs eventArgs)
        {
            var handler = this.UpdateDeparked;
            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
    }
}
