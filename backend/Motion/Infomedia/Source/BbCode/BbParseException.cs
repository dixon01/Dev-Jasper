// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BbParseException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the BbParseException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.BbCode
{
    using System;

    /// <summary>
    /// Exception that is thrown whenever something goes
    /// wrong during <see cref="BbParser.Parse(string)"/>.
    /// </summary>
    [Serializable]
    public partial class BbParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BbParseException"/> class.
        /// </summary>
        public BbParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BbParseException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public BbParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BbParseException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner.
        /// </param>
        public BbParseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}