// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InvalidBackgroundSystemConfigurationException.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InvalidBackgroundSystemConfigurationException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Portal.Host.Configuration
{
    using System;

    /// <summary>
    /// Exception thrown when an invalid BackgroundSystem configuration is found.
    /// </summary>
    public class InvalidBackgroundSystemConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBackgroundSystemConfigurationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public InvalidBackgroundSystemConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBackgroundSystemConfigurationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InvalidBackgroundSystemConfigurationException(string message)
            : base(message)
        {
        }
    }
}