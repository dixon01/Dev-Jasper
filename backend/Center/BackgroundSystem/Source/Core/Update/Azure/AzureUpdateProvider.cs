// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AzureUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AzureUpdateProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Update.Azure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Gorba.Center.Common.ServiceModel;
    using Gorba.Center.Common.ServiceModel.ChangeTracking;
    using Gorba.Center.Common.ServiceModel.ChangeTracking.Units;
    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Update.AzureClient;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Implements an update provider which is supposed to work in an Azure environment.
    /// For exchanging messages with unit (sending update commands, handling feedback) it used
    /// <see cref="MessageDispatcher"/>.
    /// </summary>
    public class AzureUpdateProvider : UpdateProviderBase<AzureUpdateProviderConfig>
    {
        private readonly ITimer sendTimer = TimerFactory.Current.CreateTimer("AzureUpdateProvider");

        private readonly TaskCompletionSource<bool> waitUnitsLoaded = new TaskCompletionSource<bool>();

        private readonly Dictionary<string, UnitCommandHandler> commandHandlers =
            new Dictionary<string, UnitCommandHandler>();

        private IUnitChangeTrackingManager unitChangeTrackingManager;

        private LogsFeedbackHandlerBase logsFeedbackHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureUpdateProvider"/> class.
        /// </summary>
        /// <param name="backgroundSystemId">
        /// The background system id.
        /// </param>
        public AzureUpdateProvider(string backgroundSystemId)
        {
            this.BackgroundSystemId = backgroundSystemId;
        }

        /// <summary>
        /// Gets the list of handled commandHandlers. One of the unit name might be a wildcard
        /// (<see cref="UpdateComponentBase.UnitWildcard"/>) to tell the user of this
        /// class that the sink is interested in all updates.
        /// </summary>
        public override IEnumerable<string> HandledUnits
        {
            get
            {
                yield return UpdateComponentBase.UnitWildcard;
            }
        }

        /// <summary>
        /// Gets the current background system identifier.
        /// </summary>
        public string BackgroundSystemId { get; private set; }

        /// <summary>
        /// Configures the update provider
        /// </summary>
        /// <param name="config">
        /// Update provider configuration
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        public override void Configure(UpdateProviderConfigBase config, IUpdateContext context)
        {
            base.Configure(config, context);
            this.unitChangeTrackingManager = DependencyResolver.Current.Get<IUnitChangeTrackingManager>();
            this.unitChangeTrackingManager.Added += this.OnUnitAdded;
            this.unitChangeTrackingManager.Removed += this.OnUnitRemoved;
            this.logsFeedbackHandler = LogsFeedbackHandlerProvider.Current.Create();
            this.sendTimer.Interval = this.Config.RetryInterval;
            this.sendTimer.AutoReset = true;
            this.sendTimer.Elapsed += this.OnSendTimer;
        }

        /// <summary>
        /// Handles the update commands by forwarding them.
        /// </summary>
        /// <param name="commands">
        ///     The update commands.
        /// </param>
        /// <param name="progressMonitor">
        ///     The progress monitor that observes the upload of the update command.
        /// </param>
        public override async void HandleCommands(IEnumerable<UpdateCommand> commands, IProgressMonitor progressMonitor)
        {
            try
            {
                this.Logger.Debug("Handling commands");
                var result = await this.waitUnitsLoaded.Task;
                if (!result)
                {
                    this.Logger.Debug("Units initialization failed. Commands not handled");
                    return;
                }

                foreach (var command in commands.OrderBy(c => c.UpdateId.UpdateIndex))
                {
                    UnitCommandHandler unit;
                    lock (this.commandHandlers)
                    {
                        if (!this.commandHandlers.TryGetValue(command.UnitId.UnitName, out unit))
                        {
                            this.Logger.Debug(
                                "Unit {0} not found. Command {1} cannot be sent",
                                command.UnitId.UnitName,
                                command);
                            continue;
                        }
                    }

                    unit.EnqueueCommand(command);
                }
            }
            catch (Exception exception)
            {
                this.Logger.Error("Error while handling commands {0}", exception);
            }
        }

        /// <summary>
        /// Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        /// True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            // This provider is always available
            return true;
        }

        /// <summary>
        /// Implementation of the Start method.
        /// </summary>
        protected override void DoStart()
        {
            this.Logger.Info("Starting");
            MessageDispatcher.Instance.Subscribe<UpdateStateInfo>(this.HandleUpdateStateInfo);
            this.sendTimer.Enabled = true;
            this.LoadUnits();
        }

        /// <summary>
        /// Implementation of the Stop method.
        /// </summary>
        protected override void DoStop()
        {
            this.Logger.Info("Stopping");
            this.sendTimer.Enabled = false;
            MessageDispatcher.Instance.Unsubscribe<UpdateStateInfo>(this.HandleUpdateStateInfo);
        }

        private void OnUnitAdded(object sender, ReadableModelEventArgs<UnitReadableModel> e)
        {
            lock (this.commandHandlers)
            {
                this.commandHandlers.Add(e.Model.Name, new UnitCommandHandler(e.Model));
            }
        }

        private void OnUnitRemoved(object sender, ReadableModelEventArgs<UnitReadableModel> e)
        {
            UnitCommandHandler handler;
            lock (this.commandHandlers)
            {
                if (!this.commandHandlers.TryGetValue(e.Model.Name, out handler))
                {
                    return;
                }

                this.commandHandlers.Remove(e.Model.Name);
            }

            handler.Dispose();
        }

        private async void LoadUnits()
        {
            try
            {
                var handlers =
                    (await this.unitChangeTrackingManager.QueryAsync()).Select(model => new UnitCommandHandler(model));
                lock (this.commandHandlers)
                {
                    foreach (var unit in handlers)
                    {
                        this.commandHandlers.Add(unit.Model.Name, unit);
                    }

                    this.waitUnitsLoaded.TrySetResult(true);
                }
            }
            catch (Exception exception)
            {
                this.waitUnitsLoaded.TrySetResult(false);
                this.Logger.Error("Error while loading command handlers {0}", exception);
                throw;
            }
        }

        private async void OnSendTimer(object sender, EventArgs eventArgs)
        {
            try
            {
                this.Logger.Trace("Retry to send remaining update commands at defined interval");
                this.ResendUpdateCommands();
                await this.CheckForLogsAsync();
            }
            catch (Exception exception)
            {
                this.Logger.Error("Error during timer handling {0}", exception);
            }
        }

        private void HandleUpdateStateInfo(object sender, MessageEventArgs<UpdateStateInfo> e)
        {
            Logger.Trace("HandleUpdateStateInfo Received state {0}", e.Message);
            if (!string.Equals(this.BackgroundSystemId, e.Message.UpdateId.BackgroundSystemGuid))
            {
                this.Logger.Debug(
                    "Received an update state info from a different BackgroundSystemGuid: {0}",
                    e.Message.UpdateId.BackgroundSystemGuid);
                return;
            }

            var ack = new UpdateStateInfoAck
                          {
                              UnitId = e.Message.UnitId,
                              UpdateId = e.Message.UpdateId,
                              UpdateState = e.Message.State
                          };
            MessageDispatcher.Instance.Send(e.Source, ack);

            this.RaiseFeedbackReceived(new FeedbackEventArgs(new IReceivedLogFile[0], new[] { e.Message }, new IReceivedLogFile[0]));
        }

        private void ResendUpdateCommands()
        {
            List<UnitCommandHandler> handlers;
            lock (this.commandHandlers)
            {
                handlers = this.commandHandlers.Values.ToList();
            }

            foreach (var handler in handlers)
            {
                handler.ResendCommands();
            }
        }

        private async Task CheckForLogsAsync()
        {
            if (!this.IsAvailable || this.logsFeedbackHandler == null)
            {
                return;
            }

            try
            {
                this.Logger.Trace("Getting logs from Azure storage");
                var logFiles = await this.logsFeedbackHandler.FindLogFilesAsync();
                this.RaiseFeedbackReceived(new FeedbackEventArgs(logFiles, new UpdateStateInfo[0], new IReceivedLogFile[0]));

                foreach (var logFile in logFiles)
                {
                    await this.logsFeedbackHandler.DeleteLogFileAsync(logFile);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Error("Couldn't download logs {0}", ex);
            }
        }
    }
}