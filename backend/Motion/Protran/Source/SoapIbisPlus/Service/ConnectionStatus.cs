// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionStatus.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.SoapIbisPlus.Service
{
    /// <summary>
    /// Enum that contains all the possible status
    /// for a remote board computer.
    /// </summary>
    public enum ConnectionStatus
    {
        /// <summary>
        /// Unknown status.
        /// </summary>
        Unknown,

        /// <summary>
        /// The remote board computer is actually active.
        /// </summary>
        Active,

        /// <summary>
        /// The remote board computer is actually inactive.
        /// </summary>
        Inactive
    }
}
