// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameDecodingException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameDecodingException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System;

    /// <summary>
    /// Exception thrown when decoding AHDLC frames.
    /// </summary>
    [Serializable]
    public partial class FrameDecodingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameDecodingException"/> class.
        /// </summary>
        public FrameDecodingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameDecodingException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public FrameDecodingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameDecodingException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public FrameDecodingException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}