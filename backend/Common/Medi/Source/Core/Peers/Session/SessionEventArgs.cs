// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SessionEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Event args for an event that carries an  as a property.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Peers.Session
{
    using System;

    /// <summary>
    /// Event args for an event that carries an <see cref="ITransportSession"/> as a property.
    /// </summary>
    internal class SessionEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SessionEventArgs"/> class.
        /// </summary>
        /// <param name="session">
        /// The session to be associated with this object.
        /// </param>
        public SessionEventArgs(ITransportSession session)
        {
            this.Session = session;
        }

        /// <summary>
        /// Gets the session implementation.
        /// </summary>
        public ITransportSession Session { get; private set; }
    }
}