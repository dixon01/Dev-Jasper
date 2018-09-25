// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Addresses.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Addresses type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.SystemManagement.Messages
{
    /// <summary>
    /// Static class to access addresses used for communication.
    /// </summary>
    public static class Addresses
    {
        /// <summary>
        /// The system management controller name.
        /// The System Management controller can always be accessed using this name as the
        /// application name (independent of how the actual system management application is called).
        /// </summary>
        public static readonly string SystemManagerDispatcher = "SystemManagementController";
    }
}
