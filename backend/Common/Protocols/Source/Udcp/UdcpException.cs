// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UdcpException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UdcpException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Udcp
{
    using System;

    /// <summary>
    /// The UDCP exception thrown by classes in this library.
    /// </summary>
    [Serializable]
    public class UdcpException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpException"/> class.
        /// </summary>
        public UdcpException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public UdcpException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UdcpException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public UdcpException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}