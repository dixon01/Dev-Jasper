// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HandshakeErrorEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the HandshakeErrorEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transport.Stream
{
    using System;

    /// <summary>
    /// Error event arguments of a stream transport handshake.
    /// </summary>
    internal class HandshakeErrorEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HandshakeErrorEventArgs"/> class.
        /// </summary>
        /// <param name="cause">
        /// The cause.
        /// </param>
        public HandshakeErrorEventArgs(HandshakeError cause)
        {
            this.Cause = cause;
        }

        /// <summary>
        /// Gets the error cause.
        /// </summary>
        public HandshakeError Cause { get; private set; }
    }
}