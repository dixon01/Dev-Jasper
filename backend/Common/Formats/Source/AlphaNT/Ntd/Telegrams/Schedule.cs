// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Schedule.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the Schedule type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Telegrams
{
    using System;

    /// <summary>
    /// The schedule type. These values can be combined (i.e. <see cref="Schedule1"/> and <see cref="Schedule2"/>.
    /// </summary>
    [Flags]
    public enum Schedule
    {
        /// <summary>
        /// The first schedule.
        /// </summary>
        Schedule1 = 1,

        /// <summary>
        /// The second schedule.
        /// </summary>
        Schedule2 = 2,
    }
}