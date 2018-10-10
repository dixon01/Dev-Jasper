// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalMessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LocalMessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Gioom.Core.Messages;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;

    /// <summary>
    /// The message handler for communication between different <see cref="GioomClient"/>s.
    /// </summary>
    internal class LocalMessageHandler : MessageHandler
    {
        private readonly GioomClient client;

        private readonly Dictionary<PortChangeHandlerKey, PortChangeHandler> portChangeHandlers =
            new Dictionary<PortChangeHandlerKey, PortChangeHandler>();

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalMessageHandler"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher used for sending and receiving messages.
        /// </param>
        /// <param name="client">
        /// The client for which this message handler is responsible.
        /// </param>
        public LocalMessageHandler(IMessageDispatcher messageDispatcher, GioomClient client)
            : base(messageDispatcher)
        {
            this.client = client;

            this.MessageDispatcher.Subscribe<QueryPortsRequest>(this.HandleQueryPortsRequest);

            this.MessageDispatcher.Subscribe<PortChangeRegistration>(this.HandlePortChangeRegistration);
            this.MessageDispatcher.Subscribe<PortChangeRequest>(this.HandlePortChangeRequest);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public override void Dispose()
        {
            this.MessageDispatcher.Unsubscribe<QueryPortsRequest>(this.HandleQueryPortsRequest);

            this.MessageDispatcher.Unsubscribe<PortChangeRegistration>(this.HandlePortChangeRegistration);
            this.MessageDispatcher.Unsubscribe<PortChangeRequest>(this.HandlePortChangeRequest);
        }

        private static PortInfo CreatePortInfo(IPort port)
        {
            return new PortInfo
                       {
                           Name = port.Info.Name,
                           CanRead = port.Info.CanRead,
                           CanWrite = port.Info.CanWrite,
                           ValidValues = ValuesInfoBase.From(port.Info.ValidValues),
                           Value = port.Info.CanRead ? port.IntegerValue : 0
                       };
        }

        private void HandleQueryPortsRequest(object sender, MessageEventArgs<QueryPortsRequest> e)
        {
            var response = new QueryPortsResponse { RequestId = e.Message.RequestId };
            lock (this.client.LocalPorts)
            {
                if (string.IsNullOrEmpty(e.Message.Name))
                {
                    foreach (var port in this.client.LocalPorts.Values)
                    {
                        response.Ports.Add(CreatePortInfo(port));
                    }
                }
                else
                {
                    IPort port;
                    if (this.client.LocalPorts.TryGetValue(e.Message.Name, out port))
                    {
                        response.Ports.Add(CreatePortInfo(port));
                    }
                }
            }

            this.Logger.Trace("Found {0} ports for '{1}'", response.Ports.Count, e.Message.Name);
            this.MessageDispatcher.Send(e.Source, response);
        }

        private void HandlePortChangeRegistration(object sender, MessageEventArgs<PortChangeRegistration> e)
        {
            IPort port;
            if (!this.client.LocalPorts.TryGetValue(e.Message.Name, out port))
            {
                return;
            }

            var key = new PortChangeHandlerKey(e.Message.Name, e.Source);
            PortChangeHandler handler;
            if (!this.portChangeHandlers.TryGetValue(key, out handler))
            {
                handler = new PortChangeHandler(port, e.Source, this.MessageDispatcher);
                this.portChangeHandlers.Add(key, handler);
                handler.Disposing += (s, ev) => this.portChangeHandlers.Remove(key);

                this.MessageDispatcher.Send(
                    e.Source,
                    new PortChangeNotification { Name = port.Info.Name, Value = port.IntegerValue });
            }

            handler.SetTimeout(e.Message.Timeout);
        }

        private void HandlePortChangeRequest(object sender, MessageEventArgs<PortChangeRequest> e)
        {
            IPort port;
            if (!this.client.LocalPorts.TryGetValue(e.Message.Name, out port))
            {
                return;
            }

            port.IntegerValue = e.Message.Value;
        }

        private class PortChangeHandler : IDisposable
        {
            private readonly IPort port;

            private readonly MediAddress address;

            private readonly IMessageDispatcher messageDispatcher;

            private long timeout;

            public PortChangeHandler(IPort port, MediAddress address, IMessageDispatcher messageDispatcher)
            {
                this.port = port;
                this.address = address;
                this.messageDispatcher = messageDispatcher;

                this.timeout = long.MaxValue;
                this.port.ValueChanged += this.PortOnValueChanged;
            }

            public event EventHandler Disposing;

            public void SetTimeout(TimeSpan interval)
            {
                this.timeout = TimeProvider.Current.TickCount + (long)interval.TotalMilliseconds;
            }

            public void Dispose()
            {
                this.port.ValueChanged -= this.PortOnValueChanged;

                var handler = this.Disposing;
                if (handler != null)
                {
                    handler(this, EventArgs.Empty);
                }
            }

            private void PortOnValueChanged(object sender, EventArgs e)
            {
                if (this.timeout < TimeProvider.Current.TickCount)
                {
                    this.Dispose();
                    return;
                }

                this.messageDispatcher.Send(
                    this.address,
                    new PortChangeNotification { Name = this.port.Info.Name, Value = this.port.IntegerValue });
            }
        }

        private class PortChangeHandlerKey : IEquatable<PortChangeHandlerKey>
        {
            private readonly string name;

            private readonly MediAddress source;

            public PortChangeHandlerKey(string name, MediAddress source)
            {
                this.name = name;
                this.source = source;
            }

            public bool Equals(PortChangeHandlerKey other)
            {
                return other != null && other.name == this.name && other.source.Equals(this.source);
            }

            public override bool Equals(object obj)
            {
                return this.Equals(obj as PortChangeHandlerKey);
            }

            public override int GetHashCode()
            {
                return this.name.GetHashCode() ^ this.source.GetHashCode();
            }
        }
    }
}