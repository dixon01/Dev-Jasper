// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpChannelException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Implements a class that implements the HTTP communication Layer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;

    /// <summary>
    /// Exception class for the Communication namespace.
    /// </summary>
    public class HttpChannelException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the HttpChannelException class.
        /// </summary>
        public HttpChannelException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpChannelException class.
        /// </summary>
        /// <param name="message">
        /// The message of the exception.
        /// </param>
        public HttpChannelException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the HttpChannelException class.
        /// </summary>
        /// <param name="message">
        /// The message of the exception.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public HttpChannelException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}