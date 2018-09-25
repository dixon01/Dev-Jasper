// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileFormatException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FileFormatException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT
{
    using System;

    /// <summary>
    /// Exception that is thrown when a given file can't be read.
    /// </summary>
    [Serializable]
    public class FileFormatException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormatException"/> class.
        /// </summary>
        public FileFormatException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormatException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public FileFormatException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileFormatException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public FileFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
