// --------------------------------------------------------------------------------------------------------------------
// <copyright file="evSetServiceAck.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the evSetServiceAck type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Edi.Core
{
    /// <summary>
    /// The set service acknowledge.
    /// </summary>
    public class evSetServiceAck
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="evSetServiceAck"/> class.
        /// </summary>
        public evSetServiceAck()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="evSetServiceAck"/> class.
        /// </summary>
        /// <param name="success">
        /// The success flag.
        /// </param>
        public evSetServiceAck(bool success)
        {
            this.Success = success;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the service was set successfully.
        /// </summary>
        public bool Success { get; set; }
    }
}