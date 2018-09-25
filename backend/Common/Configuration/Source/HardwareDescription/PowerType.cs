// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PowerType.cs" company="Gorba AG">
//   Copyright © 2011-2015 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Configuration.HardwareDescription
{
    /// <summary>
    /// The type of power supplied to the unit.
    /// </summary>
    public enum PowerType
    {
        /// <summary>
        /// The unit has solar power.
        /// </summary>
        Solar,

        /// <summary>
        /// The unit has line power in the night.
        /// </summary>
        Line
    }
}
