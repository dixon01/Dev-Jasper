// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpdateException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the UpdateException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Update.ServiceModel
{
    using System;

    /// <summary>
    /// Exception thrown in the update system if something went wrong.
    /// </summary>
    [Serializable]
    public partial class UpdateException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateException"/> class.
        /// </summary>
        public UpdateException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public UpdateException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public UpdateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
