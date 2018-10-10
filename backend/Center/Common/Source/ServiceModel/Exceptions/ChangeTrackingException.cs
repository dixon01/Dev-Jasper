// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeTrackingException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ChangeTrackingException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Exceptions
{
    using System;

    /// <summary>
    /// An exception occurred in change tracking.
    /// </summary>
    [Serializable]
    public class ChangeTrackingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public ChangeTrackingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public ChangeTrackingException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangeTrackingException"/> class.
        /// </summary>
        public ChangeTrackingException()
        {
        }
    }
}