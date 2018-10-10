// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageEventArgs{T}.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Event argument for MEDI message events.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core
{
    /// <summary>
    /// Event argument for MEDI message events.
    /// </summary>
    /// <typeparam name="T">
    /// Type of the message.
    /// </typeparam>
    public class MessageEventArgs<T> : MessageEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageEventArgs{T}"/> class.
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
        internal MessageEventArgs(MediAddress source, MediAddress destination, T message)
            : base(source, destination, message)
        {
            this.Message = message;
        }

        /// <summary>
        /// Gets Message.
        /// </summary>
        public new T Message { get; private set; }
    }
}