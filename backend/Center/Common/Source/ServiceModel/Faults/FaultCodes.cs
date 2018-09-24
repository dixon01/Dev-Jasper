// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FaultCodes.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FaultCodes type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.ServiceModel.Faults
{
    /// <summary>
    /// Defines the fault codes.
    /// </summary>
    public static class FaultCodes
    {
        /// <summary>
        /// The system is under maintenance.
        /// </summary>
        public static readonly string MaintenanceMode = "MAINTENANCEMODE";

        /// <summary>
        /// The system is currently not available.
        /// </summary>
        public static readonly string SystemNotAvailable = "SYSTEMNOTAVAILABLE";
    }
}