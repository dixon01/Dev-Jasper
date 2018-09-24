// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RemoteComputerStatus.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Enum that contains all the possible status
//   for a remote board computer.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Protran.Ibis.Remote
{
    /// <summary>
    /// Enum that contains all the possible status
    /// for a remote board computer.
    /// </summary>
    public enum RemoteComputerStatus
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