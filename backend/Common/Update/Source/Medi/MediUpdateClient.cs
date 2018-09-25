// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediUpdateClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediUpdateClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Threading;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Clients;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Update.Medi.Messages;
    using Gorba.Common.Update.ServiceModel.Clients;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Update client that receives updates and sends feedback over the Medi network.
    /// </summary>
    public class MediUpdateClient : UpdateClientBase<MediUpdateClientConfig>
    {
        private readonly IResourceService resourceService;

        private readonly string registrationId;

        private readonly XmlSerializer commandSerializer;

        private readonly List<MediAddress> foundProviders = new List<MediAddress>();

        private readonly ITimer registrationAckTimeoutTimer;
        private readonly ITimer reregisterTimer;

        private bool registered;

        private string tempDirectory;

        private string commandsDirectory;

        private ResendHandler<UpdateStateInfo> stateResendHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediUpdateClient"/> class.
        /// </summary>
        public MediUpdateClient()
        {
            this.resourceService = MessageDispatcher.Instance.GetService<IResourceService>();

            this.registrationId = Guid.NewGuid().ToString();

            this.commandSerializer = new XmlSerializer(typeof(UpdateCommand));

            this.registrationAckTimeoutTimer =
                TimerFactory.Current.CreateTimer(this.GetType().Name + ".RegistrationAckTimeout");
            this.registrationAckTimeoutTimer.AutoReset = false;
            this.registrationAckTimeoutTimer.Interval = TimeSpan.FromSeconds(10);
            this.registrationAckTimeoutTimer.Elapsed += (sender, args) => this.SendRegistration();

            this.reregisterTimer = TimerFactory.Current.CreateTimer(this.GetType().Name + ".Reregistration");
            this.reregisterTimer.AutoReset = true;
            this.reregisterTimer.Elapsed += (sender, args) => this.SendRegistration();
        }

        /// <summary>
        /// Configures the update provider
        /// </summary>
        /// <param name="config">
        /// Update provider configuration
        /// </param>
        /// <param name="context">
        /// The update context.
        /// </param>
        public override void Configure(UpdateClientConfigBase config, IUpdateContext context)
        {
            base.Configure(config, context);

            var dir = context.TemporaryDirectory;
            if (string.IsNullOrEmpty(dir))
            {
                dir = Path.GetTempPath();
            }

            this.tempDirectory = Path.Combine(dir, Path.Combine("MediClient", this.Name));
            this.commandsDirectory = Path.Combine(this.tempDirectory, "Commands");
            Directory.CreateDirectory(this.commandsDirectory);

            this.stateResendHandler = new ResendHandler<UpdateStateInfo>(Path.Combine(this.tempDirectory, "Feedback"));
        }

        /// <summary>
        /// Sends feedback back to the source.
        /// </summary>
        /// <param name="logFiles">
        /// The log files to upload.
        /// </param>
        /// <param name="stateInfos">
        /// The state information objects to upload.
        /// </param>
        /// <param name="progressMonitor">
        /// The progress monitor that observes the upload of update feedback and log files.
        /// </param>
        public override void SendFeedback(
            IEnumerable<IReceivedLogFile> logFiles,
            IEnumerable<UpdateStateInfo> stateInfos,
            IProgressMonitor progressMonitor)
        {
            foreach (var logFile in logFiles)
            {
                this.SendLogFile(logFile);
            }

            foreach (var updateStateInfo in stateInfos)
            {
                this.SendUpdateStateInfo(updateStateInfo);
            }
        }

        /// <summary>
        /// Handle uploading files via Medi. Not implemented.
        /// </summary>
        /// <param name="uploadFiles">The files to upload</param>
        public override void UploadFiles(IList<IReceivedLogFile> uploadFiles)
        {
        }

        /// <summary>
        /// Checks if this client's repository is available.
        /// </summary>
        /// <returns>
        /// True if the repository is available.
        /// </returns>
        protected override bool CheckAvailable()
        {
            return this.registered;
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateClientBase{TConfig}.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            MessageDispatcher.Instance.Subscribe<UpdateRegistrationAck>(this.HandleUpdateRegistrationAck);
            MessageDispatcher.Instance.Subscribe<UpdateCommand>(this.HandleUpdateCommand);
            MessageDispatcher.Instance.Subscribe<UpdateStateInfoAck>(this.HandleUpdateStateInfoAck);

            this.SendRegistration();

            this.stateResendHandler.Start();

            foreach (
                var command in Directory.GetFiles(this.commandsDirectory, "*" + FileDefinitions.UpdateCommandExtension))
            {
                try
                {
                    this.Logger.Info($"processing command {command} in the {this.commandsDirectory}");
                    this.VerifyCommand(command);
                }
                catch (Exception ex)
                {
                    this.Logger.Warn(ex, "Couldn't handle cached update command: {0}", command);
                }
            }
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateClientBase{TConfig}.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            MessageDispatcher.Instance.Unsubscribe<UpdateRegistrationAck>(this.HandleUpdateRegistrationAck);
            MessageDispatcher.Instance.Unsubscribe<UpdateCommand>(this.HandleUpdateCommand);
            MessageDispatcher.Instance.Unsubscribe<UpdateStateInfoAck>(this.HandleUpdateStateInfoAck);

            this.stateResendHandler.Stop();

            this.registrationAckTimeoutTimer.Enabled = false;
            this.reregisterTimer.Enabled = false;
        }

        private void SendRegistration()
        {
            this.Logger.Info( MethodBase.GetCurrentMethod().Name + "Broadcasting registration for all units begin");
            this.IsAvailable = false;

            var registration = new UpdateRegistration { RegistrationId = this.registrationId };

            foreach (var updateSink in this.Context.Sinks)
            {
                foreach (var unit in updateSink.HandledUnits)
                {
                    if (!registration.UnitNames.Contains(unit))
                    {
                        registration.UnitNames.Add(unit);
                    }
                }
            }

            if (registration.UnitNames.Contains(UpdateComponentBase.UnitWildcard))
            {
                // we only need the wildcard
                registration.UnitNames.Clear();
                registration.UnitNames.Add(UpdateComponentBase.UnitWildcard);
                this.Logger.Debug("Broadcasting registration for all units ({0})", this.registrationId);
            }
            else
            {
                this.Logger.Debug(
                    "Broadcasting registration for {0} units ({1})",
                    registration.UnitNames.Count,
                    this.registrationId);
                if (this.Logger.IsTraceEnabled)
                {
                    foreach (var unitName in registration.UnitNames)
                    {
                        this.Logger.Trace(" - {0}", unitName);
                    }
                }
            }

            MessageDispatcher.Instance.Broadcast(registration);
            this.registrationAckTimeoutTimer.Enabled = true;
        }

        private void SendLogFile(IReceivedLogFile logFile)
        {
            var directory = Path.Combine(this.tempDirectory, Guid.NewGuid().ToString());
            Directory.CreateDirectory(directory);
            var file = Path.Combine(directory, logFile.FileName);
            logFile.CopyTo(file);
            var sendCounter = this.foundProviders.Count;
            foreach (var provider in this.foundProviders)
            {
                this.Logger.Debug("Sending log file '{0}' to {1}", file, provider);
                this.resourceService.BeginSendFile(
                    file,
                    provider,
                    ar =>
                        {
                            this.resourceService.EndSendFile(ar);
                            if (Interlocked.Decrement(ref sendCounter) == 0)
                            {
                                File.Delete(file);
                                Directory.Delete(directory);
                            }
                        },
                    null);
            }
        }

        private void SendUpdateStateInfo(UpdateStateInfo updateStateInfo)
        {
            this.Logger.Info(MethodBase.GetCurrentMethod().Name + updateStateInfo);
            foreach (var provider in this.foundProviders)
            {
                this.Logger.Debug(
                    "Sending UpdateStateInfo({4}) for command {0} of {1} from {2} via {3}",
                    updateStateInfo.UpdateId.UpdateIndex,
                    updateStateInfo.UpdateId.BackgroundSystemGuid,
                    updateStateInfo.UnitId.UnitName,
                    provider,
                    updateStateInfo.State);

                this.stateResendHandler.Send(provider, updateStateInfo);
            }
        }

        private void HandleUpdateRegistrationAck(object sender, MessageEventArgs<UpdateRegistrationAck> e)
        {
            if (e.Message.RegistrationId != this.registrationId)
            {
                return;
            }

            if (!this.foundProviders.Contains(e.Source))
            {
                this.Logger.Debug("Got first registration ACK from {0}", e.Source);
                this.foundProviders.Add(e.Source);

                this.stateResendHandler.ResendAll(e.Source);
            }
            else
            {
                this.Logger.Debug("Got registration ACK from {0}", e.Source);
            }

            this.registrationAckTimeoutTimer.Enabled = false;
            this.registered = true;
            this.IsAvailable = true;

            var validityTime = e.Message.ValidityTime;
            this.reregisterTimer.Interval = TimeSpan.FromMilliseconds(validityTime.TotalMilliseconds * 0.9);
            this.reregisterTimer.Enabled = true;
        }

        private void HandleUpdateCommand(object sender, MessageEventArgs<UpdateCommand> e)
        {
            this.Logger.Debug(
                "Received update command {0} from {1}",
                e.Message.UpdateId.UpdateIndex,
                e.Message.UpdateId.BackgroundSystemGuid);
            var file = Path.Combine(this.commandsDirectory, Guid.NewGuid() + FileDefinitions.UpdateCommandExtension);
            using (var output = File.Create(file))
            {
                this.commandSerializer.Serialize(output, e.Message);
            }

            MessageDispatcher.Instance.Send(
                e.Source, new UpdateCommandAck { UnitId = e.Message.UnitId, UpdateId = e.Message.UpdateId });

            this.VerifyCommand(file);
        }

        private void HandleUpdateStateInfoAck(object sender, MessageEventArgs<UpdateStateInfoAck> e)
        {
            if (this.stateResendHandler.Remove(
                e.Source,
                s =>
                    s.State == e.Message.State
                    && s.UpdateId.Equals(e.Message.UpdateId)
                    && s.UnitId.Equals(e.Message.UnitId)
                    && s.TimeStamp.Equals(e.Message.TimeStamp)))
            {
                this.Logger.Trace("Got ack for {0}", e.Message);
            }
            else
            {
                this.Logger.Warn("Got unknown ack for {0}", e.Message);
            }
        }

        private void VerifyCommand(string fileName)
        {
            this.Logger.Debug(MethodBase.GetCurrentMethod().Name + " Verifiying command: {0}", fileName);
            UpdateCommand command;
            using (var input = File.OpenRead(fileName))
            {
                command = (UpdateCommand)this.commandSerializer.Deserialize(input);
            }

            var collector = new ResourceCollector();
            var resourceIds = collector.GetAllResourceHashes(command).ConvertAll(h => new ResourceId(h));
            var state = new ResourceVerificationState(resourceIds, fileName, command);
            if (resourceIds.Count == 0)
            {
                this.ForwardCommand(state);
                return;
            }

            foreach (var resourceId in resourceIds.ToArray())
            {
                this.Logger.Trace(MethodBase.GetCurrentMethod().Name + " Verifiying resource {0}", resourceId);
                this.resourceService.BeginGetResource(resourceId, this.GotResource, state);
            }
        }

        private void GotResource(IAsyncResult ar)
        {
            var state = (ResourceVerificationState)ar.AsyncState;
            var info = this.resourceService.EndGetResource(ar);

            this.Logger.Trace(MethodBase.GetCurrentMethod().Name + " Resource {0} is now available", info.Id);
            lock (state)
            {
                state.ResourceIds.Remove(info.Id);

                if (state.ResourceIds.Count > 0)
                {
                    this.Logger.Warn("{0} resources are still missing", state.ResourceIds.Count);
                    return;
                }
            }

            this.ForwardCommand(state);
        }

        private void ForwardCommand(ResourceVerificationState state)
        {
            this.Logger.Debug(
                "All resources for command {0} from {1} are now available ({2})",
                state.UpdateCommand.UpdateId.UpdateIndex,
                state.UpdateCommand.UpdateId.BackgroundSystemGuid,
                state.TempFileName);

            this.RaiseCommandReceived(new UpdateCommandsEventArgs(state.UpdateCommand));

            File.Delete(state.TempFileName);
        }

        private class ResourceVerificationState
        {
            public ResourceVerificationState(
                List<ResourceId> resourceIds, string tempFileName, UpdateCommand updateCommand)
            {
                this.ResourceIds = resourceIds;
                this.TempFileName = tempFileName;
                this.UpdateCommand = updateCommand;
            }

            public List<ResourceId> ResourceIds { get; private set; }

            public string TempFileName { get; private set; }

            public UpdateCommand UpdateCommand { get; private set; }
        }
    }
}
