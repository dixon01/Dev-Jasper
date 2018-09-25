// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SchedulerSlot.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SchedulerSlot type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Center.BackgroundSystem.Core.Qube.Configuration
{
    /// <summary>
    /// The scheduler slot.
    /// </summary>
    internal enum SchedulerSlot
    {
        /// <summary>
        /// Defines the slot '0'.
        /// </summary>
        Slot0 = 1, // don't change the value. When serialized it must match the unit (which starts from 1)

        /// <summary>
        /// The slot '1'.
        /// </summary>
        Slot1 = 2,

        /// <summary>
        /// The slot '2'.
        /// </summary>
        Slot2 = 3,

        /// <summary>
        /// The slot '3'.
        /// </summary>
        Slot3 = 4,

        /// <summary>
        /// Defines the slot '4'.
        /// </summary>
        Slot4 = 5,

        /// <summary>
        /// Defines the slot '5'.
        /// </summary>
        Slot5 = 6
    }
}