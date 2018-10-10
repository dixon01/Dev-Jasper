// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppDomainMediProxy.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AppDomainMediProxy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    using System;
    using System.Runtime.ExceptionServices;

    using Gorba.Common.Medi.Core;
    using Gorba.Common.Medi.Core.Config;

    /// <summary>
    /// The proxy for using AppDomains when testing multiple instance of MessageDispatcher.
    /// </summary>
    public class AppDomainMediProxy : MarshalByRefObject, IAppDomainMediProxy
    {
        private IConfigurator configurator;

        private AppDomainMediReceiver receiver;

        /// <summary>
        /// Configure this object.
        /// </summary>
        /// <param name="configXmlString">
        /// The config xml string.
        /// </param>
        /// <param name="unitName">
        /// The unit name.
        /// </param>
        /// <param name="appName">
        /// The app name.
        /// </param>
        public void Configure(string configXmlString, string unitName, string appName)
        {
            this.configurator = new StringConfigurator(configXmlString, unitName, appName);
        }

        /// <summary>
        /// Start the message dispatcher on its app domain.
        /// </summary>
        /// <param name="eventReceiver">
        /// The event receiver.
        /// </param>
        public void Start(AppDomainMediReceiver eventReceiver)
        {
            this.receiver = eventReceiver;
            AppDomain.CurrentDomain.FirstChanceException += this.AppDomainFirstChanceException;
            AppDomain.CurrentDomain.UnhandledException += this.AppDomainUnhandledException;
            MessageDispatcher.Instance.Configure(this.configurator);
        }

        /// <summary>
        /// Stop the message dispatcher in its app domain.
        /// </summary>
        public void Stop()
        {
            MessageDispatcher.Instance.Configure(
                new ObjectConfigurator(new MediConfig { InterceptLocalLogs = false }, string.Empty, string.Empty));
        }

        /// <summary>
        /// Subscribe to a certain message.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to subscribe to.
        /// </typeparam>
        public void Subscribe<T>() where T : class
        {
            MessageDispatcher.Instance.Subscribe<T>(this.MessageReceived);
        }

        /// <summary>
        /// Unsubscribe from a certain message.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to unsubscribe from.
        /// </typeparam>
        public void Unsubscribe<T>() where T : class
        {
            MessageDispatcher.Instance.Unsubscribe<T>(this.MessageReceived);
        }

        /// <summary>
        /// Broadcast a message from the message dispatcher on its app domain.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void BroadcastMessage(object message)
        {
            MessageDispatcher.Instance.Broadcast(message);
        }

        private void MessageReceived<T>(object sender, MessageEventArgs<T> e)
        {
            this.receiver.RaiseMessageReceived(e.Source, e.Destination, e.Message);
        }

        private void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.receiver.RaiseUnhandledException(
                new ExceptionEventArgs { Exception = e.ExceptionObject as Exception });
        }

        private void AppDomainFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        {
            this.receiver.RaiseFirstChanceException(new ExceptionEventArgs { Exception = e.Exception });
        }
    }
}