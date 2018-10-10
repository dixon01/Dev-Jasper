// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ConnectionState.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Diag.Core.ViewModels.Unit
{
    /// <summary>
    /// The state of the connection to a unit.
    /// </summary>
    public enum ConnectionState
    {
        /// <summary>
        /// We are not connected to the given unit.
        /// </summary>
        Disconnected,

        /// <summary>
        /// We are connecting to the unit but haven't received any data yet.
        /// </summary>
        Connecting,

        /// <summary>
        /// We are fully connected to the unit and are receiving data.
        /// </summary>
        Connected
    }
}
