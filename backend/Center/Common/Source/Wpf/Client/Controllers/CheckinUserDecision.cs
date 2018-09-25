// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CheckinUserDecision.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The check in user decision. Returned by the check in trap.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    /// <summary>
    /// The check in user decision. Returned by the check in trap.
    /// </summary>
    public enum CheckinUserDecision
    {
        /// <summary>
        /// The undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// The no check in required.
        /// </summary>
        NoCheckinRequired,

        /// <summary>
        /// The cancel.
        /// </summary>
        Cancel,

        /// <summary>
        /// Do a check in.
        /// </summary>
        Checkin,

        /// <summary>
        /// The skip.
        /// </summary>
        Skip
    }
}