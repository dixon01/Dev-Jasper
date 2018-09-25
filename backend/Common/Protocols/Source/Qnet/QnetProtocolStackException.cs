// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QnetProtocolStackException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Exception class for qnet protocol stack
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Qnet
{
    using System;

    /// <summary>
    /// Exception class for qnet protocol stack
    /// </summary>
    public class QnetProtocolStackException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnetProtocolStackException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public QnetProtocolStackException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnetProtocolStackException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="ex">
        /// The ex.
        /// </param>
        public QnetProtocolStackException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}
