// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTestException.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IntegrationTestException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.IntegrationTests
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception thrown by Medi integration tests.
    /// </summary>
    [Serializable]
    public class IntegrationTestException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestException"/> class.
        /// </summary>
        public IntegrationTestException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public IntegrationTestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public IntegrationTestException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTestException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected IntegrationTestException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
