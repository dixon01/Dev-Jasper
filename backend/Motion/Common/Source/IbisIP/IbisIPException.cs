// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IbisIPException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IbisIPException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.IbisIP
{
    using System;

    /// <summary>
    /// Exception thrown if something goes wrong in IBIS-IP.
    /// </summary>
    [Serializable]
    public partial class IbisIPException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IbisIPException"/> class.
        /// </summary>
        public IbisIPException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisIPException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public IbisIPException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IbisIPException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public IbisIPException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
