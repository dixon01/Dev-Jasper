// -----------------------------------------------------------------------
// <copyright file="CommsMessagingServiceProxy.cs" company="HP">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace ProxyProviderTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ServiceModel;
    using System.Text;

    using Gorba.Center.CommS.Core.ComponentModel.Messages;
    using Gorba.Center.CommS.Wcf.ServiceModel;

    /// <summary>
    /// Implementation of the <see cref="ICommsMessagingService"/> client.
    /// </summary>
    internal class CommsMessagingServiceProxy : ClientBase<ICommsMessagingService>, ICommsMessagingService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommsMessagingServiceProxy"/> class.
        /// </summary>
        /// <param name="endpointConfigurationName">Name of the endpoint configuration.</param>
        public CommsMessagingServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CommsMessagingServiceProxy"/> class.
        /// </summary>
        public CommsMessagingServiceProxy()
        {
        }

        /// <summary>
        /// Enables to send a comms message to the comm.s to be forwarded to the right unit(s) according to the destination address called UnitId.
        /// </summary>
        /// <param name="message">
        /// The inherited comms message. See <see cref="CommsMessage"/> for more details.
        /// </param>
        public void SendMessage(CommsMessage message)
        {
            this.Channel.SendMessage(message);
        }
    }
}
