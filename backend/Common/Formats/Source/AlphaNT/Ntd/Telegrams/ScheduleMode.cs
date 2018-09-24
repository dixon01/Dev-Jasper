// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScheduleMode.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScheduleMode type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Telegrams
{
    using System;

    /// <summary>
    /// The schedule mode. These values can be combined (i.e. <see cref="Inverted"/> and <see cref="Periodic"/>.
    /// </summary>
    [Flags]
    public enum ScheduleMode
    {
        /// <summary>
        /// The default (normal) schedule mode.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Inverted schedule mode.
        /// </summary>
        Inverted = 1,

        /// <summary>
        /// Periodic schedule mode.
        /// </summary>
        Periodic = 2,
    }
}