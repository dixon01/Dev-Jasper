// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PortForwardingService.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the PortForwardingService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Ports
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;
    using Gorba.Common.Medi.Core.Network;
    using Gorba.Common.Medi.Core.Services;
    using Gorba.Common.Medi.Ports.Config;
    using Gorba.Common.Medi.Ports.Forwarder;
    using Gorba.Common.Medi.Ports.Messages;
    using Gorba.Common.Medi.Ports.Stream;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;
    using Gorba.Common.Utility.Core.Factory;

    using NLog;

    /// <summary>
    /// The port forwarding service implementation.
    /// </summary>
    internal class PortForwardingService
        : IPortForwardingService, IConfigurable<PortForwardingServiceConfig>, IServiceImpl
    {
        private static readonly Logger Logger = LogHelper.GetLogger<PortForwardingService>();

        private readonly Dictionary<string, PortForwarding> forwardings = new Dictionary<string, PortForwarding>();

        private readonly Dictionary<string, ForwarderInfo> forwarders = new Dictionary<string, ForwarderInfo>();

        private IMessageDispatcherImpl dispatcher;

        private int nextId;

        /// <summary>
        /// Initializes a new instance of the <see cref="PortForwardingService"/> class.
        /// </summary>
        public PortForwardingService()
        {
            this.nextId = new Random().Next(short.MaxValue);
        }

        /// <summary>
        /// Begins to create a port forwarding from a given address to another address.
        /// </summary>
        /// <param name="firstAddress">
        /// The first address.
        /// </param>
        /// <param name="firstConfig">
        /// The first forwarding config.
        /// </param>
        /// <param name="secondAddress">
        /// The second address.
        /// </param>
        /// <param name="secondConfig">
        /// The second forwarding config.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="IPortForwardingService.EndCreateForwarding"/>.
        /// </returns>
        public IAsyncResult BeginCreateForwarding(
            MediAddress firstAddress,
            ForwardingEndPointConfig firstConfig,
            MediAddress secondAddress,
            ForwardingEndPointConfig secondConfig,
            AsyncCallback callback,
            object state)
        {
            var result = new SimpleAsyncResult<PortForwarding>(callback, state);

            var id = string.Format("{0}<{1:X4}>", this.dispatcher.LocalAddress, Interlocked.Increment(ref this.nextId));
            var createFirst = new CreateForwardingMessage
                                  {
                                      LocalId = id + 'A',
                                      RemoteId = id + 'B',
                                      RemoteAddress = secondAddress,
                                      Config = firstConfig
                                  };
            var createSecond = new CreateForwardingMessage
                                   {
                                       LocalId = id + 'B',
                                       RemoteId = id + 'A',
                                       RemoteAddress = firstAddress,
                                       Config = secondConfig
                                   };

            var forwarding = new PortForwarding(
                id, firstAddress, firstConfig, secondAddress, secondConfig, result, this);
            lock (this.forwardings)
            {
                this.forwardings.Add(id, forwarding);
            }

            try
            {
                if (!this.dispatcher.Send(this.dispatcher.LocalAddress, firstAddress, createFirst, null))
                {
                    throw new KeyNotFoundException("Couldn't find first remote address " + firstAddress);
                }

                if (!this.dispatcher.Send(this.dispatcher.LocalAddress, secondAddress, createSecond, null))
                {
                    throw new KeyNotFoundException("Couldn't find second remote address " + firstAddress);
                }
            }
            catch (Exception ex)
            {
                lock (this.forwardings)
                {
                    this.forwardings.Remove(id);
                }

                result.CompleteException(ex, true);
            }

            return result;
        }

        /// <summary>
        /// Begins to create a port forwarding from the local node to another address.
        /// </summary>
        /// <param name="localConfig">
        /// The local forwarding config.
        /// </param>
        /// <param name="remoteAddress">
        /// The remote address.
        /// </param>
        /// <param name="remoteConfig">
        /// The remote forwarding config.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="IPortForwardingService.EndCreateForwarding"/>.
        /// </returns>
        public IAsyncResult BeginCreateForwarding(
            ForwardingEndPointConfig localConfig,
            MediAddress remoteAddress,
            ForwardingEndPointConfig remoteConfig,
            AsyncCallback callback,
            object state)
        {
            return this.BeginCreateForwarding(
                this.dispatcher.LocalAddress, localConfig, remoteAddress, remoteConfig, callback, state);
        }

        /// <summary>
        /// Begins to connect to a server reachable from a remote Medi node.
        /// </summary>
        /// <param name="remoteAddress">
        /// The remote address of the Medi node that is used for port forwarding.
        /// </param>
        /// <param name="remoteConfig">
        /// The configuration how to connect to the TCP server reachable from the remote note.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndConnect"/>.
        /// </returns>
        public IAsyncResult BeginConnect(
            MediAddress remoteAddress,
            TcpClientEndPointConfig remoteConfig,
            AsyncCallback callback,
            object state)
        {
            return this.BeginCreateForwarding(
                new StreamForwardingConfig(),
                remoteAddress,
                remoteConfig,
                callback,
                state);
        }

        /// <summary>
        /// Finishes the asynchronous call to <see cref="BeginConnect"/>.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned from <see cref="BeginConnect"/>.
        /// </param>
        /// <returns>
        /// The newly created <see cref="Stream"/> which allows to read and write to the remote server.
        /// </returns>
        public System.IO.Stream EndConnect(IAsyncResult ar)
        {
            var forwarding = this.EndCreateForwarding(ar);
            var config = (StreamForwardingConfig)forwarding.FirstConfig;
            config.Forwarder.Closing += (s, e) => forwarding.Dispose();
            config.Forwarder.Connect();
            return config.Forwarder.Stream;
        }

        IPortForwarding IPortForwardingService.EndCreateForwarding(IAsyncResult ar)
        {
            return this.EndCreateForwarding(ar);
        }

        void IConfigurable<PortForwardingServiceConfig>.Configure(PortForwardingServiceConfig config)
        {
            // nothing to do for now
        }

        void IServiceImpl.Start(IMessageDispatcherImpl disp)
        {
            this.dispatcher = disp;

            this.dispatcher.Subscribe<CreateForwardingMessage>(this.HandleCreateForwarding);
            this.dispatcher.Subscribe<CreateForwardingResultMessage>(this.HandleCreateForwardingResult);
            this.dispatcher.Subscribe<RemoveForwardingMessage>(this.HandleRemoveForwarding);

            this.dispatcher.RoutingTable.Updated += this.RoutingTableOnUpdated;
        }

        void IServiceImpl.Stop()
        {
            this.dispatcher.Unsubscribe<CreateForwardingMessage>(this.HandleCreateForwarding);
            this.dispatcher.Unsubscribe<CreateForwardingResultMessage>(this.HandleCreateForwardingResult);
            this.dispatcher.Unsubscribe<RemoveForwardingMessage>(this.HandleRemoveForwarding);

            this.dispatcher.RoutingTable.Updated -= this.RoutingTableOnUpdated;
        }

        /// <summary>
        /// Finishes the asynchronous call to .
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned from one of the <code>BeginCreateForwarding</code> methods.
        /// </param>
        /// <returns>
        /// The newly created <see cref="IPortForwarding"/> which can be used to close the forwarding again.
        /// </returns>
        private PortForwarding EndCreateForwarding(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<PortForwarding>;
            if (result == null)
            {
                throw new ArgumentException("Provide result from BeginCreateForwarding() as argument", "ar");
            }

            if (!result.IsCompleted)
            {
                result.AsyncWaitHandle.WaitOne();
            }

            return result.Value;
        }

        private void RemoveForwarding(PortForwarding forwarding)
        {
            lock (this.forwardings)
            {
                if (!this.forwardings.Remove(forwarding.Id))
                {
                    Logger.Warn("Trying to remove unknown forwarding: {0}", forwarding.Id);
                    return;
                }
            }

            Logger.Info("Removing forwarding {0}", forwarding.Id);
            this.dispatcher.Send(forwarding.FirstAddress, new RemoveForwardingMessage { Id = forwarding.Id + 'A' });
            this.dispatcher.Send(forwarding.SecondAddress, new RemoveForwardingMessage { Id = forwarding.Id + 'B' });
        }

        private void HandleCreateForwarding(object sender, MessageEventArgs<CreateForwardingMessage> e)
        {
            var result = new CreateForwardingResultMessage { Id = e.Message.LocalId };
            try
            {
                var forwarder = ConfigImplementationFactory.CreateFromConfig<IPortForwarder>(e.Message.Config);
                lock (this.forwarders)
                {
                    this.forwarders.Add(
                        e.Message.LocalId,
                        new ForwarderInfo(e.Message.LocalId, forwarder, e.Message.RemoteAddress));
                }

                Logger.Debug("Created forwarder for {0}", e.Message.LocalId);
                result.ResultingConfig = forwarder.Start(
                    this.dispatcher, e.Message.LocalId, e.Message.RemoteId, e.Message.RemoteAddress);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex, "Couldn't create forwarder " + e.Message.LocalId);
                result.ErrorMessage = string.Format("{0}: {1}", ex.GetType().Name, ex.Message);
            }

            this.dispatcher.Send(e.Source, result);
        }

        private void HandleCreateForwardingResult(object sender, MessageEventArgs<CreateForwardingResultMessage> e)
        {
            PortForwarding forwarding;

            lock (this.forwardings)
            {
                var id = e.Message.Id.Substring(0, e.Message.Id.Length - 1);
                bool isFirst = e.Message.Id.EndsWith("A");
                if (!this.forwardings.TryGetValue(id, out forwarding))
                {
                    Logger.Warn(
                        "Got result for unknown forwarding creation {0}: '{1}'", e.Message.Id, e.Message.ErrorMessage);
                    return;
                }

                if (isFirst)
                {
                    forwarding.FirstErrorMessage = e.Message.ErrorMessage ?? string.Empty;
                    forwarding.FirstConfig = e.Message.ResultingConfig ?? forwarding.FirstConfig;
                }
                else
                {
                    forwarding.SecondErrorMessage = e.Message.ErrorMessage ?? string.Empty;
                    forwarding.SecondConfig = e.Message.ResultingConfig ?? forwarding.SecondConfig;
                }

                if (forwarding.FirstErrorMessage == null || forwarding.SecondErrorMessage == null)
                {
                    // we haven't received both results yet, let's wait for the second one
                    return;
                }
            }

            try
            {
                if (forwarding.FirstErrorMessage.Length > 0)
                {
                    throw new InvalidOperationException(forwarding.FirstErrorMessage);
                }

                if (forwarding.SecondErrorMessage.Length > 0)
                {
                    throw new InvalidOperationException(forwarding.SecondErrorMessage);
                }
            }
            catch (Exception ex)
            {
                forwarding.Result.CompleteException(ex, false);
                return;
            }

            forwarding.Result.Complete(forwarding, false);
        }

        private void HandleRemoveForwarding(object sender, MessageEventArgs<RemoveForwardingMessage> e)
        {
            ForwarderInfo info;
            lock (this.forwarders)
            {
                if (!this.forwarders.TryGetValue(e.Message.Id, out info))
                {
                    return;
                }

                this.forwarders.Remove(e.Message.Id);
            }

            info.Forwarder.Stop();
        }

        private void RoutingTableOnUpdated(object sender, RouteUpdatesEventArgs e)
        {
            // remove all forwards for the application that just disconnected
            var updates = new List<RouteUpdate>(e.Updates);
            var deletes = new List<ForwarderInfo>();
            lock (this.forwarders)
            {
                foreach (var update in updates)
                {
                    if (update.Added || updates.FindIndex(i => i.Address.Equals(update.Address) && i.Added) >= 0)
                    {
                        // address was not removed or [removed and added in the same transaction], we keep the locks
                        continue;
                    }

                    foreach (var info in this.forwarders.Values)
                    {
                        if (info.RemotePeer.Equals(update.Address))
                        {
                            deletes.Add(info);
                        }
                    }
                }
            }

            lock (this.forwarders)
            {
                foreach (var delete in deletes)
                {
                    if (this.forwarders.Remove(delete.Id))
                    {
                        delete.Forwarder.Stop();
                    }
                }
            }
        }

        private class ForwarderInfo
        {
            public ForwarderInfo(string id, IPortForwarder forwarder, MediAddress remotePeer)
            {
                this.Id = id;
                this.Forwarder = forwarder;
                this.RemotePeer = remotePeer;
            }

            public string Id { get; private set; }

            public IPortForwarder Forwarder { get; private set; }

            public MediAddress RemotePeer { get; private set; }
        }

        private class PortForwarding : IPortForwarding
        {
            private PortForwardingService owner;

            public PortForwarding(
                string id,
                MediAddress firstAddress,
                ForwardingEndPointConfig firstConfig,
                MediAddress secondAddress,
                ForwardingEndPointConfig secondConfig,
                SimpleAsyncResult<PortForwarding> result,
                PortForwardingService owner)
            {
                this.owner = owner;

                this.Id = id;
                this.FirstAddress = firstAddress;
                this.FirstConfig = firstConfig;
                this.SecondAddress = secondAddress;
                this.SecondConfig = secondConfig;
                this.Result = result;
            }

            public string Id { get; private set; }

            public MediAddress FirstAddress { get; private set; }

            public MediAddress SecondAddress { get; private set; }

            public ForwardingEndPointConfig FirstConfig { get; set; }

            public ForwardingEndPointConfig SecondConfig { get; set; }

            public SimpleAsyncResult<PortForwarding> Result { get; private set; }

            public string FirstErrorMessage { get; set; }

            public string SecondErrorMessage { get; set; }

            public void Close()
            {
                var own = this.owner;
                if (own == null)
                {
                    return;
                }

                this.owner = null;
                own.RemoveForwarding(this);
            }

            public void Dispose()
            {
                this.Close();
            }
        }
    }
}
