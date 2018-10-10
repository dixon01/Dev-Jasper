// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsSdException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the DnsSdException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.DnsServiceDiscovery.Server
{
    using System;

    /// <summary>
    /// The DNS-SD exception.
    /// </summary>
    [Serializable]
    public partial class DnsSdException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DnsSdException"/> class.
        /// </summary>
        public DnsSdException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsSdException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public DnsSdException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DnsSdException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public DnsSdException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}