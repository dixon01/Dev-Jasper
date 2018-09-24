// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InstallationCancelledException.FX20.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InstallationCancelledException type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Update.Core.Agent
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception that is thrown when an installation has been cancelled.
    /// </summary>
    public partial class InstallationCancelledException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InstallationCancelledException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected InstallationCancelledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}