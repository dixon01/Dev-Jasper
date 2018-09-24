// -----------------------------------------------------------------------
// <copyright file="GorbaException.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
namespace Gorba.Common.ComponentModel.Base
{
    using System;

    /// <summary>
    /// Base Gorba exception.
    /// </summary>
    [Serializable]
    public class GorbaException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaException"/> class.
        /// </summary>
        public GorbaException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public GorbaException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GorbaException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="innerException">The inner exception.</param>
        public GorbaException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
