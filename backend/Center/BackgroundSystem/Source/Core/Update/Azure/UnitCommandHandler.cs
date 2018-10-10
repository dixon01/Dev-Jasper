// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitCommandHandler.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitCommandHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update.Azure
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;

    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Update.AzureClient;
    using Gorba.Common.Update.ServiceModel.Messages;

    using NLog;

    /// <summary>
    /// The unit observer which handles the connection state.
    /// </summary>
    public class UnitCommandHandler : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly Dictionary<UpdateId, UpdateCommand> updateCommands = new Dictionary<UpdateId, UpdateCommand>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitCommandHandler"/> class.
        /// </summary>
        /// <param name="unit">
        /// The unit.
        /// </param>
        public UnitCommandHandler(UnitReadableModel unit)
        {
            this.Model = unit;
            this.Address = new MediAddress(unit.Name, AzureUpdateClient.VirtualApplicationName);
            unit.PropertyChanged += this.OnUnitPropertyChanged;
            MessageDispatcher.Instance.Subscribe<UpdateStateInfo>(this.HandleUpdateStateInfo);
        }

        /// <summary>
        /// Gets the address.
        /// </summary>
        public MediAddress Address { get; private set; }

        /// <summary>
        /// Gets the unit readable model.
        /// </summary>
        public UnitReadableModel Model { get; private set; }

        /// <summary>
        /// Enqueues a command to be sent to the unit.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        public void EnqueueCommand(UpdateCommand command)
        {
            lock (this.updateCommands)
            {
                if (this.updateCommands.ContainsKey(command.UpdateId))
                {
                    Logger.Debug("Update command {0} already added", command);
                    return;
                }

                this.updateCommands.Add(command.UpdateId, command);
            }

            this.Send(command);
        }

        /// <summary>
        /// Tries to resend all commands that are not yet acknowledged.
        /// </summary>
        public void ResendCommands()
        {
            if (!this.Model.IsConnected)
            {
                Logger.Trace("Unit {0} is not connected. Can't resend UpdateCommands", this.Model.Name);
                return;
            }

            List<UpdateCommand> commands;
            lock (this.updateCommands)
            {
                commands = this.updateCommands.Values.OrderBy(c => c.UpdateId.UpdateIndex).ToList();
            }

            Logger.Trace("Resending {0} update command(s) to {1}", commands.Count, this.Model.Name);
            foreach (var command in commands)
            {
                this.Send(command);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Model.PropertyChanged -= this.OnUnitPropertyChanged;
            MessageDispatcher.Instance.Unsubscribe<UpdateStateInfo>(this.HandleUpdateStateInfo);
        }

        /// <summary>
        /// Tries to (re)send the command given in the constructor.
        /// </summary>
        /// <param name="command">
        /// The command to send.
        /// </param>
        private void Send(UpdateCommand command)
        {
            if (!this.Model.IsConnected)
            {
                Logger.Trace("Unit {0} is not connected. Can't send UpdateCommand {1}", this.Model.Name, command);
                return;
            }

            MessageDispatcher.Instance.Send(this.Address, command);
            Logger.Trace("Sent UpdateCommand {0} to unit {1}", command, this.Model.Name);
        }

        private void HandleUpdateStateInfo(object sender, MessageEventArgs<UpdateStateInfo> e)
        {
            if (!this.Model.Name.Equals(e.Message.UnitId.UnitName, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Logger.Trace("Received state for {0}", e.Message.UpdateId);
            lock (this.updateCommands)
            {
                if (this.updateCommands.Remove(e.Message.UpdateId))
                {
                    Logger.Debug("Removed acknowledged command: {0}", e.Message);
                }
            }
        }

        private void OnUnitPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (propertyChangedEventArgs.PropertyName != "IsConnected"
                || !this.Model.IsConnected)
            {
                return;
            }

            this.ResendCommands();
        }
    }
}