// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DriverAlarmIconState.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   State of the driver alarm icon.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Obc.Terminal.Core
{
    /// <summary>
    /// State of the driver alarm icon.
    /// </summary>
    public enum DriverAlarmIconState
    {
        /// <summary>
        /// Hides any driver alarm icon.
        /// </summary>
        None,

        /// <summary>
        /// Shows a driver alarm sent icon.
        /// </summary>
        Sent,

        /// <summary>
        /// Shows a driver alarm acknowledged icon.
        /// </summary>
        Acknowledged
    }
}