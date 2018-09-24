// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITimeService.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITimeService type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Vdv301.Services
{
    /// <summary>
    /// The IBIS-IP time service interface.
    /// This service is very special since it's not an HTTP nor a UDP service,
    /// but rather uses SNTP for time synchronization.
    /// Therefore this interface just provides the service access information, not the service itself.
    /// </summary>
    public interface ITimeService : IVdv301Service
    {
        /// <summary>
        /// Gets the IP address of the SNTP server.
        /// </summary>
        string IPAddress { get; }

        /// <summary>
        /// Gets the UDP port of the SNTP server.
        /// </summary>
        int Port { get; }
    }
}
