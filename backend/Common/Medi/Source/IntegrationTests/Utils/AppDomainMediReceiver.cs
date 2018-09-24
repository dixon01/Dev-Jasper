// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainMediReceiver.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDomainMediReceiver type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    using System;

    using Gorba.Common.Medi.Core;

    /// <summary>
    /// Local receiver for a MessageDispatcher running in a different AppDomain.
    /// </summary>
    /// <seealso cref="IAppDomainMediProxy"/>
    public class AppDomainMediReceiver : MarshalByRefObject
    {
        private readonly IAppDomainMediProxy proxy;

        private readonly AppDomain appDomain;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDomainMediReceiver"/> class.
        /// </summary>
        /// <param name="proxy">
        /// The proxy.
        /// </param>
        /// <param name="appDomain">
        /// The app domain.
        /// </param>
        public AppDomainMediReceiver(IAppDomainMediProxy proxy, AppDomain appDomain)
        {
            this.proxy = proxy;
            this.appDomain = appDomain;
        }

        /// <summary>
        /// Event that is fired if a first chance exception happens in the remote app domain.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> FirstChanceException;

        /// <summary>
        /// Event that is fired if an unhandled exception happens in the remote app domain.
        /// </summary>
        public event EventHandler<ExceptionEventArgs> UnhandledException;

        /// <summary>
        /// Event that is fired if a message is received in the remote app domain.
        /// </summary>
        public event EventHandler<MessageEventArgs> MessageReceived;

        /// <summary>
        /// Start the proxy.
        /// </summary>
        public void Start()
        {
            this.proxy.Start(this);
        }

        /// <summary>
        /// Stop the proxy by unloading the app domain.
        /// </summary>
        public void Stop()
        {
            this.proxy.Stop();
            AppDomain.Unload(this.appDomain);
        }

        /// <summary>
        /// Subscribe to a certain message on the remote app domain.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to subscribe to.
        /// </typeparam>
        public void Subscribe<T>() where T : class
        {
            this.proxy.Subscribe<T>();
        }

        /// <summary>
        /// Unsubscribe from a certain message on the remote app domain.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to unsubscribe from.
        /// </typeparam>
        public void Unsubscribe<T>() where T : class
        {
            this.proxy.Unsubscribe<T>();
        }

        /// <summary>
        /// Broadcast a message from the remote message dispatcher on its app domain.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void BroadcastMessage(object message)
        {
            this.proxy.BroadcastMessage(message);
        }

        /// <summary>
        /// Fires the <see cref="FirstChanceException"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void RaiseFirstChanceException(ExceptionEventArgs e)
        {
            var handler = this.FirstChanceException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the <see cref="UnhandledException"/> event.
        /// </summary>
        /// <param name="e">
        /// The event arguments.
        /// </param>
        public void RaiseUnhandledException(ExceptionEventArgs e)
        {
            var handler = this.UnhandledException;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the <see cref="MessageReceived"/> event.
        /// </summary>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public void RaiseMessageReceived(MediAddress source, MediAddress destination, object message)
        {
            var handler = this.MessageReceived;
            if (handler != null)
            {
                handler(this, new MessageEventArgs(source, destination, message));
            }
        }
    }
}