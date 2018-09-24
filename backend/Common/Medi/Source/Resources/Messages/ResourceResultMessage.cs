// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResourceResultMessage.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ResourceResultMessage type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Resources.Messages
{
    using Gorba.Common.Medi.Resources.Services;

    /// <summary>
    /// Return value message for calls to <see cref="ResourceServiceBase"/>
    /// </summary>
    /// <typeparam name="T">
    /// the type of the return value.
    /// </typeparam>
    public class ResourceResultMessage<T>
    {
        /// <summary>
        /// Gets or sets the id matching the request.
        /// </summary>
        public int RequestId { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        public T Result { get; set; }

        /// <summary>
        /// Gets or sets the type of the exception thrown remotely.
        /// Is null if no exception was thrown.
        /// </summary>
        public string ExceptionType { get; set; }

        /// <summary>
        /// Gets or sets the message of the exception thrown remotely.
        /// Is null if no exception was thrown.
        /// </summary>
        public string ExceptionMessage { get; set; }
    }
}