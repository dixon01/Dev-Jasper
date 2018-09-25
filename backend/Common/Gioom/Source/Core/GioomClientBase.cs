// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GioomClientBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the GioomClientBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;
    using System.Collections.Generic;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Utility.Core;
    using Gorba.Common.Utility.Core.Async;

    using NLog;

    /// <summary>
    /// The base class for all Gorba I/O over Medi client.
    /// </summary>
    public abstract class GioomClientBase
    {
        /// <summary>
        /// The logger.
        /// </summary>
        protected readonly Logger Logger;

        private MessageHandler messageHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="GioomClientBase"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher to be used.
        /// </param>
        protected GioomClientBase(IMessageDispatcher messageDispatcher)
        {
            this.Logger = LogHelper.GetLogger(this.GetType());

            this.Dispatcher = messageDispatcher;
        }

        /// <summary>
        /// Gets the message dispatcher used by this class.
        /// </summary>
        protected IMessageDispatcher Dispatcher { get; private set; }

        /// <summary>
        /// Begins the search for ports on the given address.
        /// </summary>
        /// <param name="address">
        /// The address of the node(s) to search for ports.
        /// Wildcards can be used to search in all applications on a given unit.
        /// </param>
        /// <param name="timeout">
        /// The timeout after which this asynchronous method should complete.
        /// This method completes immediately if <see cref="address"/> is the local Medi address.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndFindPorts"/>.
        /// </returns>
        public virtual IAsyncResult BeginFindPorts(
            MediAddress address, TimeSpan timeout, AsyncCallback callback, object state)
        {
            return this.BeginFindPorts(address, null, timeout, false, callback, state);
        }

        /// <summary>
        /// Ends the search for ports started with <see cref="BeginFindPorts"/>.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned by <see cref="BeginFindPorts"/>.
        /// </param>
        /// <returns>
        /// A list of port information. These objects can be used to open the port
        /// (see <see cref="OpenPort"/>).
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the asynchronous result is not one returned by <see cref="BeginFindPorts"/>
        /// </exception>
        public IPortInfo[] EndFindPorts(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<IPortInfo[]>;
            if (result == null)
            {
                throw new ArgumentException("Provide async result from BeginFindPorts()", "ar");
            }

            result.AsyncWaitHandle.WaitOne();
            return result.Value;
        }

        /// <summary>
        /// Opens a port.
        /// </summary>
        /// <param name="info">
        /// The information about the port. You can get this from <see cref="BeginFindPorts"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IPort"/>.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If a local port is used and that port does not exist (anymore).
        /// </exception>
        public virtual IPort OpenPort(IPortInfo info)
        {
            return this.CreateRemotePort(info);
        }

        /// <summary>
        /// Begins to open a port on the given address with the given name.
        /// This method will complete synchronously if the address is the local address.
        /// This asynchronous request times out after 10 seconds if the port was not found.
        /// </summary>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="callback">
        /// The asynchronous callback.
        /// </param>
        /// <param name="state">
        /// The asynchronous state.
        /// </param>
        /// <returns>
        /// The <see cref="IAsyncResult"/> to be used with <see cref="EndOpenPort"/>.
        /// </returns>
        public virtual IAsyncResult BeginOpenPort(
            MediAddress address, string name, AsyncCallback callback, object state)
        {
            var result = new SimpleAsyncResult<IPort>(callback, state);

            this.BeginFindPorts(
                address,
                name,
                TimeSpan.FromSeconds(10),
                true,
                ar =>
                    {
                        var ports = this.EndFindPorts(ar);
                        result.Complete(
                            ports.Length == 1 ? this.CreateRemotePort(ports[0]) : null, ar.CompletedSynchronously);
                    },
                null);

            return result;
        }

        /// <summary>
        /// Ends opening a port started with <see cref="BeginOpenPort"/>.
        /// </summary>
        /// <param name="ar">
        /// The asynchronous result returned by <see cref="BeginOpenPort"/>.
        /// </param>
        /// <returns>
        /// The <see cref="IPort"/> or null if it doesn't exist.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// If the asynchronous result is not one returned by <see cref="BeginOpenPort"/>
        /// </exception>
        public IPort EndOpenPort(IAsyncResult ar)
        {
            var result = ar as SimpleAsyncResult<IPort>;
            if (result == null)
            {
                throw new ArgumentException("Provide async result from BeginOpenPort()", "ar");
            }

            result.WaitForCompletionAndVerify();
            return result.Value;
        }

        /// <summary>
        /// Creates a new <see cref="MessageHandler"/> that will handle all Medi messages related
        /// to this client.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="MessageHandler"/>.
        /// </returns>
        internal abstract MessageHandler CreateMessageHandler();

        /// <summary>
        /// Starts this client.
        /// This method must be called before calling any public methods of this class.
        /// </summary>
        protected void Start()
        {
            this.messageHandler = this.CreateMessageHandler();
        }

        /// <summary>
        /// Closes this client.
        /// </summary>
        protected void Close()
        {
            this.messageHandler.Dispose();
        }

        private IAsyncResult BeginFindPorts(
            MediAddress address,
            string name,
            TimeSpan timeout,
            bool completeImmediately,
            AsyncCallback callback,
            object state)
        {
            this.Logger.Trace("BeginFindPorts({0},'{1}',{2},{3})", address, name, timeout, completeImmediately);
            var result = new SimpleAsyncResult<IPortInfo[]>(callback, state);

            var ports = new List<IPortInfo>();
            var timer = TimerFactory.Current.CreateTimer("FindPorts-" + address + " -> " + name);
            timer.AutoReset = false;
            timer.Interval = timeout;

            var request = this.messageHandler.SendQueryPortsRequest(
                address,
                name,
                p =>
                    {
                        this.Logger.Trace("Got {0} additional ports for '{1}' on {2}", p.Count, name, address);
                        ports.AddRange(p);
                        if (!completeImmediately
                            || (ports.Count == 0 && (address.Unit == "*" || address.Application == "*")))
                        {
                            return;
                        }

                        lock (result)
                        {
                            if (!result.IsCompleted)
                            {
                                timer.Enabled = false;
                                this.Logger.Trace(
                                    "Completing immediately with {0} ports for '{1}' on {2}",
                                    ports.Count,
                                    name,
                                    address);
                                result.Complete(ports.ToArray(), false);
                            }
                        }
                    });

            timer.Elapsed += (s, e) =>
                {
                    request.Dispose();
                    lock (result)
                    {
                        this.Logger.Trace(
                            "Completing after timeout with {0} ports for '{1}' on {2}", ports.Count, name, address);
                        if (!result.IsCompleted)
                        {
                            result.Complete(ports.ToArray(), false);
                        }
                    }
                };
            timer.Enabled = true;
            return result;
        }

        private IPort CreateRemotePort(IPortInfo info)
        {
            var wrapper = (PortInfoWrapper)info;
            return new RemotePort(wrapper, wrapper.InitialValue, this.Dispatcher);
        }
    }
}