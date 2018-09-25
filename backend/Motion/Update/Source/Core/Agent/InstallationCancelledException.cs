// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationCancelledException.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationCancelledException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;

    /// <summary>
    /// Exception that is thrown when an installation has been cancelled.
    /// </summary>
    [Serializable]
    public partial class InstallationCancelledException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationCancelledException"/> class.
        /// </summary>
        public InstallationCancelledException()
            : this("Installation was cancelled")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationCancelledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public InstallationCancelledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationCancelledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="inner">
        /// The inner exception.
        /// </param>
        public InstallationCancelledException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}