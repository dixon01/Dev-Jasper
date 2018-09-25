// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SaveUserDecision.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   The save user decision. Returned by the save trap.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Common.Wpf.Client.Controllers
{
    /// <summary>
    /// The save user decision. Returned by the save trap.
    /// </summary>
    public enum SaveUserDecision
    {
        /// <summary>
        /// The undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// The no save required.
        /// </summary>
        NoSaveRequired,

        /// <summary>
        /// Save local changes
        /// </summary>
        Save,

        /// <summary>
        /// Discard local changes.
        /// </summary>
        Discard,

        /// <summary>
        /// The cancel.
        /// </summary>
        Cancel
    }
}