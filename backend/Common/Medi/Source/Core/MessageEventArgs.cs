// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Event argument for MEDI message events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    using System;

    /// <summary>
    /// Event argument for MEDI message events.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs"/> class.
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
        internal MessageEventArgs(MediAddress source, MediAddress destination, object message)
        {
            this.Source = source;
            this.Destination = destination;
            this.Message = message;
        }

        /// <summary>
        /// Gets the source address.
        /// </summary>
        public MediAddress Source { get; private set; }

        /// <summary>
        /// Gets the destination address.
        /// </summary>
        public MediAddress Destination { get; private set; }

        /// <summary>
        /// Gets Message.
        /// </summary>
        public object Message { get; private set; }
    }
}
