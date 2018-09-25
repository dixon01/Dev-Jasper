// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAppDomainMediProxy.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IAppDomainMediProxy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests.Utils
{
    /// <summary>
    /// Interface for a proxi for using AppDomains when testing multiple instance of MessageDispatcher.
    /// </summary>
    public interface IAppDomainMediProxy
    {
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
        void Configure(string configXmlString, string unitName, string appName);

        /// <summary>
        /// Start the message dispatcher on its app domain.
        /// </summary>
        /// <param name="eventReceiver">
        /// The event receiver.
        /// </param>
        void Start(AppDomainMediReceiver eventReceiver);

        /// <summary>
        /// Stop the message dispatcher in its app domain.
        /// </summary>
        void Stop();

        /// <summary>
        /// Subscribe to a certain message.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to subscribe to.
        /// </typeparam>
        void Subscribe<T>() where T : class;

        /// <summary>
        /// Unsubscribe from a certain message.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the message to unsubscribe from.
        /// </typeparam>
        void Unsubscribe<T>() where T : class;

        /// <summary>
        /// Broadcast a message from the message dispatcher on its app domain.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        void BroadcastMessage(object message);
    }
}