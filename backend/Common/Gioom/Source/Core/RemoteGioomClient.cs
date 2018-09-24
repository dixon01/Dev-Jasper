// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteGioomClient.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the RemoteGioomClient type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Gioom.Core
{
    using System;

    using Gorba.Common.Medi.Core;

    /// <summary>
    /// Gorba I/O over Medi client that only searches for other ports using a specified
    /// message dispatcher.
    /// This class should only be used when there is the need for using a specific message dispatcher,
    /// otherwise use <see cref="GioomClient.Instance"/>.
    /// </summary>
    public sealed class RemoteGioomClient : GioomClientBase, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteGioomClient"/> class.
        /// </summary>
        /// <param name="messageDispatcher">
        /// The message dispatcher.
        /// </param>
        public RemoteGioomClient(IMessageDispatcher messageDispatcher)
            : base(messageDispatcher)
        {
            this.Start();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Creates a new <see cref="MessageHandler"/> that will handle all Medi messages related
        /// to this client.
        /// </summary>
        /// <returns>
        /// The newly created <see cref="MessageHandler"/>.
        /// </returns>
        internal override MessageHandler CreateMessageHandler()
        {
            return new MessageHandler(this.Dispatcher);
        }
    }
}