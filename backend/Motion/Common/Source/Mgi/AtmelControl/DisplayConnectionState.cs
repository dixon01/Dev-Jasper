// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisplayConnectionState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ConnectionState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// The state of the connection.
    /// </summary>
    public enum DisplayConnectionState
    {
        // ReSharper disable InconsistentNaming

        /// <summary>
        /// Device is present.
        /// </summary>
        connected,

        /// <summary>
        /// Device has been discovered but is missing now.
        /// </summary>
        missing

        // ReSharper restore InconsistentNaming
    }
}