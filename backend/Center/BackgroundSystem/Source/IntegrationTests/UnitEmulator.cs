// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnitEmulator.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UnitEmulator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Transcoder.Bec;
    using Gorba.Common.Medi.Core.Transport.Tcp;
    using Gorba.Common.Update.AzureClient;
    using Gorba.Common.Update.ServiceModel.Messages;
    using Gorba.Common.Utility.IntegrationTests;

    using NLog;

    /// <summary>
    /// The unit emulator emulates units within the context of an integration test.
    /// </summary>
    public class UnitEmulator : IDisposable
    {
        private readonly Dictionary<string, IMessageDispatcher> dispatchers =
            new Dictionary<string, IMessageDispatcher>();

        private readonly Dictionary<UpdateId, ReceivedMessage<UpdateCommand>> receivedCommands =
            new Dictionary<UpdateId, ReceivedMessage<UpdateCommand>>(new UpdateIdComparer());

        private readonly Dictionary<UpdateId, List<ReceivedMessage<UpdateStateInfoAck>>> receivedAcks =
            new Dictionary<UpdateId, List<ReceivedMessage<UpdateStateInfoAck>>>(new UpdateIdComparer());

        private readonly List<UnitEmulatorExpectation> expectations = new List<UnitEmulatorExpectation>();

        static UnitEmulator()
        {
            NLog.Config.SimpleConfigurator.ConfigureForConsoleLogging(LogLevel.Debug);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitEmulator"/> class.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public UnitEmulator(IntegrationTestContext context)
        {
            this.Context = context;
        }

        /// <summary>
        /// Gets or sets the context.
        /// </summary>
        public IntegrationTestContext Context { get; set; }

        /// <summary>
        /// Adds a new emulated unit.
        /// </summary>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        public void Add(string unitName)
        {
            var expectation = new UnitEmulatorExpectation
                                  {
                                      SendInstalled = true,
                                      SendTransferred = true,
                                      SendTransferring = true,
                                      Unit = unitName
                                  };
                                  this.expectations.Add(expectation);
        }

        /// <summary>
        /// Starts the emulator.
        /// </summary>
        public void Start()
        {
            this.Context.Info("Started");
            var units = this.expectations.Select(expectation => expectation.Unit).Distinct();
            foreach (var unitName in units)
            {
                this.CreateDispatcher(unitName);
            }
        }

        /// <summary>
        /// Stops the emulator. Disconnects all units.
        /// </summary>
        public void Stop()
        {
            this.Context.Info("Stopped");
            foreach (var dispatcher in this.dispatchers.Values)
            {
                dispatcher.Dispose();
            }

            this.dispatchers.Clear();
        }

        /// <summary>
        /// Sends the 'Transferring' feedback for all received update commands.
        /// </summary>
        public void SendTransferring()
        {
            foreach (var receivedCommand in this.receivedCommands.Values)
            {
                var feedback = CreateFeedback(receivedCommand, UpdateState.Transferring);
                IMessageDispatcher dispatcher;
                if (!this.TryGetDispatcher(receivedCommand, out dispatcher))
                {
                    continue;
                }

                dispatcher.Send(receivedCommand.Source, feedback);
            }
        }

        /// <summary>
        /// Sends the 'Transferred' feedback for all received update commands.
        /// </summary>
        public void SendTransferred()
        {
            foreach (var receivedCommand in this.receivedCommands.Values)
            {
                var feedback = CreateFeedback(receivedCommand, UpdateState.Transferred);
                IMessageDispatcher dispatcher;
                if (!this.TryGetDispatcher(receivedCommand, out dispatcher))
                {
                    continue;
                }

                dispatcher.Send(receivedCommand.Source, feedback);
            }
        }

        /// <summary>
        /// Sends the 'Installed' feedback for all received update commands.
        /// </summary>
        public void SendInstalled()
        {
            foreach (var receivedCommand in this.receivedCommands.Values)
            {
                var feedback = CreateFeedback(receivedCommand, UpdateState.Installed);
                IMessageDispatcher dispatcher;
                if (!this.TryGetDispatcher(receivedCommand, out dispatcher))
                {
                    continue;
                }

                dispatcher.Send(receivedCommand.Source, feedback);
            }
        }

        /// <summary>
        /// Verifies the expectations.
        /// </summary>
        public void Verify()
        {
            var result = this.VerifyState(UpdateState.Transferring);
            result = result && this.VerifyState(UpdateState.Transferred);
            result = result && this.VerifyState(UpdateState.Installed);
            if (result)
            {
                this.Context.Info("Verification succeeded");
                return;
            }

            this.Context.Fail("Verification failed");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (true)
            {
                // dispose unmanaged resources here
            }

            this.Stop();
        }

        private static UpdateStateInfo CreateFeedback(
            ReceivedMessage<UpdateCommand> receivedCommand,
            UpdateState updateState)
        {
            var feedback = new UpdateStateInfo
                               {
                                   State = updateState,
                                   UnitId = new UnitId(receivedCommand.Destination.Unit),
                                   UpdateId = receivedCommand.Message.UpdateId
                               };
            return feedback;
        }

        private bool VerifyState(UpdateState state)
        {
            var exists = this.receivedAcks.Values.Any(v => v.Any(message => message.Message.UpdateState == state));
            if (exists)
            {
                this.Context.Debug("{0} received", state);
                return true;
            }

            this.Context.Error("{0} ack not sent", state);
            return false;
        }

        private bool TryGetDispatcher(ReceivedMessage<UpdateCommand> receivedCommand, out IMessageDispatcher dispatcher)
        {
            if (this.dispatchers.ContainsKey(receivedCommand.Destination.Unit))
            {
                dispatcher = this.dispatchers[receivedCommand.Destination.Unit];
                return true;
            }

            dispatcher = null;
            return false;
        }

        private void CreateDispatcher(string unitName)
        {
            var config = new MediConfig
                             {
                                 Peers =
                                     {
                                         new ClientPeerConfig
                                             {
                                                 Codec = new BecCodecConfig(),
                                                 Transport = new TcpTransportClientConfig()
                                             }
                                     }
                             };
            var configurator = new ObjectConfigurator(config, unitName, "AzureUpdateClient");
            var dispatcher = MessageDispatcher.Create(configurator);
            dispatcher.Subscribe<UpdateCommand>(this.OnUpdateCommandMessage);
            dispatcher.Subscribe<UpdateStateInfoAck>(this.OnUpdateStateInfoAckMessage);
            this.dispatchers.Add(unitName, dispatcher);
        }

        private void OnUpdateCommandMessage(
            object sender,
            MessageEventArgs<UpdateCommand> updateCommandMessageEventArgs)
        {
            if (this.receivedCommands.ContainsKey(updateCommandMessageEventArgs.Message.UpdateId))
            {
                this.Context.Debug(
                    "Discarded message for update{0} (already present)",
                    updateCommandMessageEventArgs.Message.UpdateId);
                return;
            }

            this.Context.Info("Received message: {0}", updateCommandMessageEventArgs.Message);
            this.receivedCommands.Add(
                updateCommandMessageEventArgs.Message.UpdateId,
                new ReceivedMessage<UpdateCommand>(
                    updateCommandMessageEventArgs.Destination,
                    updateCommandMessageEventArgs.Source,
                    updateCommandMessageEventArgs.Message));
            this.SendTransferring();
            this.SendTransferred();
            this.SendInstalled();
        }

        private void OnUpdateStateInfoAckMessage(
            object sender,
            MessageEventArgs<UpdateStateInfoAck> updateCommandMessageEventArgs)
        {
            this.Context.Info("Received message: {0}", updateCommandMessageEventArgs.Message);
            lock (this.receivedAcks)
            {
                List<ReceivedMessage<UpdateStateInfoAck>> list;
                if (!this.receivedAcks.TryGetValue(updateCommandMessageEventArgs.Message.UpdateId, out list))
                {
                    list = new List<ReceivedMessage<UpdateStateInfoAck>>();
                    this.receivedAcks.Add(updateCommandMessageEventArgs.Message.UpdateId, list);
                }

                list.Add(
                    new ReceivedMessage<UpdateStateInfoAck>(
                        updateCommandMessageEventArgs.Destination,
                        updateCommandMessageEventArgs.Source,
                        updateCommandMessageEventArgs.Message));
            }
        }

        private class ReceivedMessage<T>
        {
            public ReceivedMessage(MediAddress destination, MediAddress source, T message)
            {
                this.Destination = destination;
                this.Source = source;
                this.Message = message;
            }

            public MediAddress Destination { get; private set; }

            public MediAddress Source { get; private set; }

            public T Message { get; private set; }
        }

        private class UpdateIdComparer : IEqualityComparer<UpdateId>
        {
            public bool Equals(UpdateId x, UpdateId y)
            {
                if (x == null)
                {
                    return y == null;
                }

                if (y == null)
                {
                    return false;
                }

                return x.BackgroundSystemGuid.Equals(y.BackgroundSystemGuid) && x.UpdateIndex == y.UpdateIndex;
            }

            public int GetHashCode(UpdateId obj)
            {
                return obj.BackgroundSystemGuid.GetHashCode() ^ obj.UpdateIndex;
            }
        }
    }
}
