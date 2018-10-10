// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceName.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareManager.Vdv301
{
    /// <summary>
    /// The different services available in VDV301
    /// </summary>
    public enum ServiceName
    {
        /// <summary>
        /// The customer information service.
        /// </summary>
        CustomerInformationService,

        /// <summary>
        /// The device management service.
        /// </summary>
        DeviceManagementService,

        /// <summary>
        /// The journey information service.
        /// </summary>
        JourneyInformationService,

        /// <summary>
        /// The beacon location service.
        /// </summary>
        BeaconLocationService,

        /// <summary>
        /// The distance location service.
        /// </summary>
        DistanceLocationService,

        /// <summary>
        /// The gnss location service.
        /// </summary>
        GNSSLocationService,

        /// <summary>
        /// The network location service.
        /// </summary>
        NetworkLocationService,

        /// <summary>
        /// The system documentation service.
        /// </summary>
        SystemDocumentationService,

        /// <summary>
        /// The system management service.
        /// </summary>
        SystemManagementService,

        /// <summary>
        /// The ticketing service.
        /// </summary>
        TicketingService,

        /// <summary>
        /// The time service.
        /// </summary>
        TimeService,

        /// <summary>
        /// The test service.
        /// </summary>
        TestService,
    }
}
