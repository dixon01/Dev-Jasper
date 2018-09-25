// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MediMessageEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the MediMessageEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers
{
    using System;

    using Gorba.Common.Medi.Core.Peers.Session;

    /// <summary>
    /// Event args that are used to signal an arriving message.
    /// </summary>
    internal class MediMessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MediMessageEventArgs"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        public MediMessageEventArgs(ITransportSession session, MediMessage message)
        {
            this.Session = session;
            this.Message = message;
        }

        /// <summary>
        /// Gets the session from which the message was received.
        /// </summary>
        public ITransportSession Session { get; private set; }

        /// <summary>
        /// Gets the received message.
        /// </summary>
        public MediMessage Message { get; private set; }
    }
}