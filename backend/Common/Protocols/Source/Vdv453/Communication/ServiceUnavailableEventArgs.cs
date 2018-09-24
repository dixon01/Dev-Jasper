// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceUnavailableEventArgs.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ServiceUnavailableEventArgs type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv453.Communication
{
    using System;

    /// <summary>
    /// Defines the args of an event raised when a service is not available for a given address.
    /// </summary>
    [Serializable]
    public class ServiceUnavailableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableEventArgs"/> class.
        /// </summary>
        /// <param name="address">The address for which the server is unavailable.</param>
        public ServiceUnavailableEventArgs(string address)
        {
            this.Address = address;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceUnavailableEventArgs"/> class.
        /// </summary>
        public ServiceUnavailableEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets the address for which the server is unavailable.
        /// </summary>
        /// <value>
        /// The address for which the server is unavailable.
        /// </value>
        public string Address { get; set; }
    }
}
