// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MessageHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using Gorba.Common.Gioom.Core.Messages;
    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// The message handler for communication between different <see cref="GioomClientBase"/>s.
    /// </summary>
    internal class MessageHandler : IDisposable
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private readonly Dictionary<int, Action<IList<IPortInfo>>> queryPortsResponseHandlers =
            new Dictionary<int, Action<IList<IPortInfo>>>();

        private int nextQueryPortsId;

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageHandler"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher used for sending messages to other GIOoM clients.
        /// </param>
        public MessageHandler(IMessageDispatcher messageDispatcher)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());

            this.MessageDispatcher = messageDispatcher;

            this.nextQueryPortsId = new Random().Next(short.MaxValue);

            this.MessageDispatcher.Subscribe<QueryPortsResponse>(this.HandleQueryPortsResponse);
        }

        /// <summary>
        /// Gets the message dispatcher.
        /// </summary>
        protected IMessageDispatcher MessageDispatcher { get; private set; }

        /// <summary>
        /// Sends a <see cref="QueryPortsRequest"/> to the given address.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="name">
        /// The name of the port, can be null or empty to get all ports on the given address.
        /// </param>
        /// <param name="handler">
        /// The handler to call whenever new <see cref="QueryPortsResponse"/>s arrive for this request.
        /// If the handler returns true, no further responses will be provided to it.
        /// </param>
        /// <returns>
        /// An object to tell when no more responses should be taken for this request.
        /// The user of this method should call <see cref="IDisposable.Dispose"/> when it is done,
        /// or return false from the <see cref="handler"/>.
        /// </returns>
        public IDisposable SendQueryPortsRequest(MediAddress address, string name, Action<IList<IPortInfo>> handler)
        {
            var id = Interlocked.Increment(ref this.nextQueryPortsId);
            lock (this.queryPortsResponseHandlers)
            {
                this.queryPortsResponseHandlers[id] = handler;
            }

            this.MessageDispatcher.Send(address, new QueryPortsRequest { RequestId = id, Name = name });

            return new Disposer(() => this.RemoveQueryPortsResponseHandler(id));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public virtual void Dispose()
        {
            this.MessageDispatcher.Unsubscribe<QueryPortsResponse>(this.HandleQueryPortsResponse);

            if (this.queryPortsResponseHandlers.Count == 0)
            {
                return;
            }

            List<Action<IList<IPortInfo>>> handlers;
            lock (this.queryPortsResponseHandlers)
            {
                handlers = new List<Action<IList<IPortInfo>>>(this.queryPortsResponseHandlers.Values);
                this.queryPortsResponseHandlers.Clear();
            }

            var empty = new IPortInfo[0];
            foreach (var handler in handlers)
            {
                handler(empty);
            }
        }

        private void HandleQueryPortsResponse(object sender, MessageEventArgs<QueryPortsResponse> e)
        {
            Action<IList<IPortInfo>> handler;
            lock (this.queryPortsResponseHandlers)
            {
                if (!this.queryPortsResponseHandlers.TryGetValue(e.Message.RequestId, out handler))
                {
                    this.Logger.Warn("Couldn't find response handler for request {0}", e.Message.RequestId);
                    return;
                }
            }

            var ports = e.Message.Ports.ConvertAll(p => (IPortInfo)new PortInfoWrapper(p, e.Source));
            handler(ports);
        }

        private void RemoveQueryPortsResponseHandler(int id)
        {
            lock (this.queryPortsResponseHandlers)
            {
                this.queryPortsResponseHandlers.Remove(id);
            }
        }

        private class Disposer : IDisposable
        {
            private readonly ThreadStart action;

            public Disposer(ThreadStart action)
            {
                this.action = action;
            }

            public void Dispose()
            {
                this.action();
            }
        }
    }
}