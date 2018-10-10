// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionMessageEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SessionMessageEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    /// <summary>
    /// Event arguments for a message coming from a given session.
    /// </summary>
    internal class SessionMessageEventArgs : MessageEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionMessageEventArgs"/> class.
        /// </summary>
        /// <param name="session">
        /// The session.
        /// </param>
        /// <param name="source">
        /// The source.
        /// </param>
        /// <param name="destination">
        /// The destination.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        internal SessionMessageEventArgs(
            ITransportSession session, MediAddress source, MediAddress destination, object message)
            : base(source, destination, message)
        {
            this.Session = session;
        }

        /// <summary>
        /// Gets the session.
        /// </summary>
        public ITransportSession Session { get; private set; }
    }
}
