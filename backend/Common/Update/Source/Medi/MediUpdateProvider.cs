// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediUpdateProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediUpdateProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.Medi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Configuration.Update.Providers;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Resources;
    using Gorba.Common.Medi.Core.Utility;
    using Gorba.Common.Update.Medi.Messages;
    using Gorba.Common.Update.ServiceModel.Common;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Update.ServiceModel.Providers;
    using Gorba.Common.Update.ServiceModel.Utility;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// Update provider that sends updates and receives feedback over the Medi network.
    /// </summary>
    public class MediUpdateProvider : UpdateProviderBase<MediUpdateProviderConfig>
    {
        private const string RegistrationFileName = "registrations.xml";

        private readonly ReadWriteLock clientsLock = new ReadWriteLock();

        private readonly Dictionary<string, List<ClientRegistration>> registeredClients =
            new Dictionary<string, List<ClientRegistration>>();

        private readonly IResourceService resourceService;

        private string tempDirectory;

        private ResendHandler<UpdateCommand> commandResendHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediUpdateProvider"/> class.
        /// </summary>
        public MediUpdateProvider()
        {
            this.resourceService = MessageDispatcher.Instance.GetService<IResourceService>();
        }

        /// <summary>
        /// Gets the list of handled units. One of the unit name might be a wildcard
        /// (<see cref="UpdateComponentBase.UnitWildcard"/>) to tell the user of this
        /// class that the sink is interested in all updates.
        /// </summary>
        public override IEnumerable<string> HandledUnits
        {
            get
            {
                return this.registeredClients.Keys;
            }
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
        public override void Configure(UpdateProviderConfigBase config, IUpdateContext context)
        {
            base.Configure(config, context);

            this.tempDirectory = Path.Combine(context.TemporaryDirectory, Path.Combine("MediProvider", this.Name));
            Directory.CreateDirectory(this.tempDirectory);
            this.Logger.Trace($" {MethodBase.GetCurrentMethod().Name} Created temp directory for Medi Update : {this.tempDirectory} ");
            this.LoadRegistrations();

            this.commandResendHandler = new ResendHandler<UpdateCommand>(Path.Combine(this.tempDirectory, "Commands"));
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
        public override void HandleCommands(IEnumerable<UpdateCommand> commands, IProgressMonitor progressMonitor)
        {
            this.Logger.Info($"Handling  {commands.Count()} Commnds");
            foreach (var command in commands)
            {
                this.Logger.Info($"Handling Commnd {command}");
                this.HandleCommand(command);
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
            this.Logger.Info($"Found {this.registeredClients.Count} registered medi update provider clients");
            return this.registeredClients.Count > 0;
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateProviderBase{TConfig}.Start"/> method.
        /// </summary>
        protected override void DoStart()
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            MessageDispatcher.Instance.Subscribe<UpdateRegistration>(this.HandleUpdateRegistration);
            MessageDispatcher.Instance.Subscribe<UpdateStateInfo>(this.HandleUpdateStateInfo);
            MessageDispatcher.Instance.Subscribe<UpdateCommandAck>(this.HandleUpdateCommandAck);
            this.resourceService.FileReceived += this.ResourceServiceOnFileReceived;

            this.commandResendHandler.Start();
        }

        /// <summary>
        /// Implementation of the <see cref="UpdateProviderBase{TConfig}.Stop"/> method.
        /// </summary>
        protected override void DoStop()
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            this.commandResendHandler.Stop();

            MessageDispatcher.Instance.Unsubscribe<UpdateRegistration>(this.HandleUpdateRegistration);
            MessageDispatcher.Instance.Unsubscribe<UpdateStateInfo>(this.HandleUpdateStateInfo);
            MessageDispatcher.Instance.Unsubscribe<UpdateCommandAck>(this.HandleUpdateCommandAck);
            this.resourceService.FileReceived -= this.ResourceServiceOnFileReceived;
        }

        private void HandleCommand(UpdateCommand command)
        {
            this.Logger.Debug(
                "Slaves Getting all resources of update {0} for {1} from {2}",
                command.UpdateId.UpdateIndex,
                command.UnitId.UnitName,
                command.UpdateId.BackgroundSystemGuid);
            var state = new GetResourceState(command);
            var allResources = new List<string>(this.GetAllResourceHashes(command));
            foreach (var hash in allResources)
            {
                state.Resources[hash] = null;
            }

            if (state.Resources.Count == 0)
            {
                this.ForwardCommand(command, state.Resources);
                return;
            }

            this.Logger.Trace("Getting {0} resource informations", state.Resources.Count);

            foreach (var hash in allResources)
            {
                this.resourceService.BeginGetResource(new ResourceId(hash), this.GotResource, state);
            }
        }

        private void GotResource(IAsyncResult ar)
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            var state = ar.AsyncState as GetResourceState;
            if (state == null)
            {
                this.Logger.Warn("Couldn't finish GetResource(), state missing");
                return;
            }

            try
            {
                var resourceInfo = this.resourceService.EndGetResource(ar);
                lock (state)
                {
                    state.Resources[resourceInfo.Id.Hash] = resourceInfo;
                }
            }
            catch (Exception ex)
            {
                this.NotifyFailedStatus(
                    state.Command, "Couldn't get resources: " + ex.Message);
                this.Logger.Warn(ex, "Couldn't finish GetResource()");
                state.Failed = true;
            }

            if (state.Failed)
            {
                return;
            }

            lock (state)
            {
                foreach (var info in state.Resources.Values)
                {
                    if (info == null)
                    {
                        // we don't have yet all resources
                        this.Logger.Info($"We Don't Have Yet All Resources");
                        return;
                    }
                }
            }

            this.Logger.Debug("Got all resources, forwarding command");
            this.ForwardCommand(state.Command, state.Resources);
        }

        private void ForwardCommand(UpdateCommand command, IDictionary<string, ResourceInfo> resources)
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            using (this.clientsLock.AcquireReadLock())
            {
                foreach (var client in this.registeredClients)
                {
                    if (client.Key != UpdateComponentBase.UnitWildcard
                        && !client.Key.Equals(command.UnitId.UnitName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }

                    foreach (var registration in client.Value)
                    {
                        if (!registration.IsValid)
                        {
                            continue;
                        }

                        this.Logger.Debug(
                            "Sending resources for command {0} from {1} to {2}",
                            command.UpdateId.UpdateIndex,
                            command.UpdateId.BackgroundSystemGuid,
                            registration.Address);

                        foreach (var resource in resources.Values)
                        {
                            this.Logger.Trace("Sending resource {0} to {1}", resource.Id, registration.Address);
                            this.resourceService.SendResource(resource, registration.Address);
                        }

                        this.Logger.Debug(
                            "Sending command {0} from {1} to {2}",
                            command.UpdateId.UpdateIndex,
                            command.UpdateId.BackgroundSystemGuid,
                            registration.Address);
                        this.commandResendHandler.Send(registration.Address, command);
                    }
                }
            }
        }

        private void HandleUpdateRegistration(object sender, MessageEventArgs<UpdateRegistration> e)
        {
            this.Logger.Debug("Got UpdateRegistration({0}) from {1}", e.Message.RegistrationId, e.Source);
            var hasNewRegistration = false;
            using (this.clientsLock.AcquireWriteLock())
            {
                foreach (var unitName in e.Message.UnitNames)
                {
                    this.Logger.Trace(" - {0}", unitName);
                    ClientRegistration registration;
                    List<ClientRegistration> registrations;
                    if (!this.registeredClients.TryGetValue(unitName, out registrations))
                    {
                        registrations = new List<ClientRegistration>();
                        this.registeredClients.Add(unitName, registrations);
                    }
                    else
                    {
                        registration = registrations.Find(r => r.Address.Equals(e.Source));
                        if (registration != null)
                        {
                            registration.Revalidate();
                            continue;
                        }
                    }

                    registration = new ClientRegistration(e.Source, this.Config.RegistrationTimeOut);
                    registrations.Add(registration);
                    hasNewRegistration = true;
                }

                this.SaveRegistrations();
            }

            MessageDispatcher.Instance.Send(
                e.Source,
                new UpdateRegistrationAck
                    {
                        RegistrationId = e.Message.RegistrationId,
                        ValidityTime = this.Config.RegistrationTimeOut
                    });

            if (hasNewRegistration)
            {
                this.commandResendHandler.ResendAll(e.Source);
            }

            this.IsAvailable = true;
        }

        private void LoadRegistrations()
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            try
            {
                List<ClientRegistrationPersistence> persistence;
                var fileName = Path.Combine(this.tempDirectory, RegistrationFileName);
                var serializer = new XmlSerializer(typeof(List<ClientRegistrationPersistence>));
                using (var input = File.OpenRead(fileName))
                {
                    persistence = (List<ClientRegistrationPersistence>)serializer.Deserialize(input);
                }

                using (this.clientsLock.AcquireWriteLock())
                {
                    foreach (var registration in persistence)
                    {
                        List<ClientRegistration> registrations;
                        if (!this.registeredClients.TryGetValue(registration.UnitName, out registrations))
                        {
                            registrations = new List<ClientRegistration>();
                            this.registeredClients.Add(registration.UnitName, registrations);
                        }

                        registrations.Add(new ClientRegistration(registration, this.Config.RegistrationTimeOut));
                    }
                }
                foreach (var registeredClient in this.registeredClients)
                {
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + $"Registered Client Info Key = {registeredClient.Key} , Value = {registeredClient.Value}");
                }
               
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't load registrations");
            }
        }

        private void SaveRegistrations()
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            try
            {
                var persistence = new List<ClientRegistrationPersistence>();
                foreach (var pair in this.registeredClients)
                {
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + $"Registered Client Info Key = {pair.Key} , Value = {pair.Value}");
                    foreach (var registration in pair.Value)
                    {
                        persistence.Add(registration.Persist(pair.Key));
                    }
                }

                foreach (var registeredClient in this.registeredClients)
                {
                    this.Logger.Info(MethodBase.GetCurrentMethod().Name + $"Registered Client Info Key = {registeredClient.Key} , Value = {registeredClient.Value}");
                }
                var fileName = Path.Combine(this.tempDirectory, RegistrationFileName);
                this.Logger.Info(MethodBase.GetCurrentMethod().Name + $"Filename {fileName}");

               var serializer = new XmlSerializer(typeof(List<ClientRegistrationPersistence>));
                using (var output = File.Create(fileName))
                {
                    serializer.Serialize(output, persistence);
                }
            }
            catch (Exception ex)
            {
                this.Logger.Warn(ex, "Couldn't save registrations");
            }
        }

        private void HandleUpdateStateInfo(object sender, MessageEventArgs<UpdateStateInfo> e)
        {
            this.Logger.Debug(
                "Received UpdateStateInfo({4}) for command {0} of {1} from {2} via {3}",
                e.Message.UpdateId.UpdateIndex,
                e.Message.UpdateId.BackgroundSystemGuid,
                e.Message.UnitId.UnitName,
                e.Source,
                e.Message.State);
            this.RaiseFeedbackReceived(new FeedbackEventArgs(new IReceivedLogFile[0], new[] { e.Message }, new IReceivedLogFile[0]));

            MessageDispatcher.Instance.Send(
                e.Source,
                new UpdateStateInfoAck
                    {
                        State = e.Message.State,
                        TimeStamp = e.Message.TimeStamp,
                        UnitId = e.Message.UnitId,
                        UpdateId = e.Message.UpdateId
                    });
        }

        private void HandleUpdateCommandAck(object sender, MessageEventArgs<UpdateCommandAck> e)
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            if (this.commandResendHandler.Remove(
                e.Source, c => c.UnitId.Equals(e.Message.UnitId) && c.UpdateId.Equals(e.Message.UpdateId)))
            {
                this.Logger.Trace("Got ack for {0}", e.Message);
            }
            else
            {
                this.Logger.Warn("Got unknown ack for {0}", e.Message);
            }
        }

        private void ResourceServiceOnFileReceived(object sender, FileReceivedEventArgs e)
        {
            this.Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Enter");
            if (!e.OriginalFileName.EndsWith(
                FileDefinitions.LogFileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            this.Logger.Debug("Received logfile '{0}' from {1}", e.OriginalFileName, e.Source);
            this.RaiseFeedbackReceived(
                new FeedbackEventArgs(new IReceivedLogFile[] { new ReceivedMediLogFile(e) }, new UpdateStateInfo[0], new IReceivedLogFile[0]));
        }

        /// <summary>
        /// Do not use this class outside the owning class; it is only public to support XML serialization.
        /// Persistence for <see cref="ClientRegistration"/>.
        /// </summary>
        public class ClientRegistrationPersistence
        {
            /// <summary>
            /// Gets or sets the timeout in UTC.
            /// </summary>
            [XmlIgnore]
            public DateTime TimeoutUtc { get; set; }

            /// <summary>
            /// Gets or sets the timeout in UTC in an XML serializable format.
            /// </summary>
            [XmlAttribute("TimeoutUtc")]
            public string TimeoutXml
            {
                get
                {
                    return XmlConvert.ToString(this.TimeoutUtc, XmlDateTimeSerializationMode.Utc);
                }

                set
                {
                    this.TimeoutUtc = XmlConvert.ToDateTime(value, XmlDateTimeSerializationMode.Utc);
                }
            }

            /// <summary>
            /// Gets or sets the unit name.
            /// </summary>
            [XmlAttribute]
            public string UnitName { get; set; }

            /// <summary>
            /// Gets or sets the Medi address of the unit.
            /// </summary>
            public MediAddress Address { get; set; }
        }

        private class ClientRegistration
        {
            private readonly TimeSpan timeout;

            private long timeoutTickCounter;

            public ClientRegistration(MediAddress address, TimeSpan timeout)
            {
                this.timeout = timeout;
                this.Address = address;

                this.Revalidate();
            }

            public ClientRegistration(ClientRegistrationPersistence persistence, TimeSpan timeout)
            {
                this.timeout = timeout;
                this.Address = persistence.Address;

                var timeLeft = persistence.TimeoutUtc - TimeProvider.Current.UtcNow;
                this.timeoutTickCounter = TimeProvider.Current.TickCount + (long)timeLeft.TotalMilliseconds;
            }

            public MediAddress Address { get; private set; }

            public bool IsValid
            {
                get
                {
                    return TimeProvider.Current.TickCount < this.timeoutTickCounter;
                }
            }

            public void Revalidate()
            {
                this.timeoutTickCounter = TimeProvider.Current.TickCount + (long)this.timeout.TotalMilliseconds;
            }

            public ClientRegistrationPersistence Persist(string unitName)
            {
                var persistence = new ClientRegistrationPersistence();
                persistence.UnitName = unitName;
                persistence.Address = this.Address;
                persistence.TimeoutUtc = TimeProvider.Current.UtcNow
                                         + TimeSpan.FromMilliseconds(
                                             this.timeoutTickCounter - TimeProvider.Current.TickCount);
                return persistence;
            }
        }

        private class GetResourceState
        {
            public GetResourceState(UpdateCommand command)
            {
                this.Resources = new Dictionary<string, ResourceInfo>();
                this.Command = command;
            }

            public IDictionary<string, ResourceInfo> Resources { get; private set; }

            public UpdateCommand Command { get; private set; }

            public bool Failed { get; set; }
        }
    }
}
