// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReasonException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Media.Core.Exceptions
{
    using System;

    /// <summary>
    /// The reason exception. Should be used to rethrow an exception adding a (translated) text as reason message.
    /// </summary>
    public class ReasonException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonException"/> class.
        /// </summary>
        public ReasonException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ReasonException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReasonException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public ReasonException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
