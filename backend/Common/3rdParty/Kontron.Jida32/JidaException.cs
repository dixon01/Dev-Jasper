// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JidaException.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the JidaException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Kontron.Jida32
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;

    /// <summary>
    /// The JIDA API exception.
    /// </summary>
    [Serializable]
    public class JidaException : IOException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JidaException"/> class.
        /// </summary>
        public JidaException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JidaException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public JidaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JidaException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public JidaException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JidaException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The serialization context.
        /// </param>
        protected JidaException(
            SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}