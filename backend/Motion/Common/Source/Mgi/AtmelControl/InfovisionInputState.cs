// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InfovisionInputState.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the InfovisionInputState type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Common.Mgi.AtmelControl
{
    /// <summary>
    /// The Atmel Controller input state.
    /// </summary>
    public class InfovisionInputState : AtmelControlObject
    {
        /// <summary>
        /// Gets or sets the stop 0 value.
        /// This value is unused on PC-2.
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? Stop0 { get; set; }

        /// <summary>
        /// Gets or sets the stop 1 value.
        /// This value is unused on PC-2.
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? Stop1 { get; set; }

        /// <summary>
        /// Gets or sets the ignition state.
        /// Valid values: 1 = on, 0 = off
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? Ignition { get; set; }

        /// <summary>
        /// Gets or sets the currently configured IBIS address.
        /// Valid values: 1..15
        /// Can be null if it hasn't changed.
        /// </summary>
        public int? Address { get; set; }
    }
}