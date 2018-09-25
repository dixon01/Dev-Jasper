// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LastStopMode.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LastStopMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.Admin.Core.Controllers.UnitConfig.Protran.Ibis
{
    /// <summary>
    /// The mode how the last stop should be handled in DS021... based telegrams.
    /// </summary>
    public enum LastStopMode
    {
        /// <summary>
        /// Show all stops, do not hide anything.
        /// </summary>
        ShowAll,

        /// <summary>
        /// Hide the last stop from the stop list (and rather take it from the destination).
        /// </summary>
        HideLastStop,

        /// <summary>
        /// Hide the destination once the number of stops in the list is below a given threshold.
        /// </summary>
        HideDestination
    }
}