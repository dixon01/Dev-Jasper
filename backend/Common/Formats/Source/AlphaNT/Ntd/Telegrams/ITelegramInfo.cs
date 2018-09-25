// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITelegramInfo.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ITelegramInfo type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Telegrams
{
    using System.Collections.Generic;

    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Ntd.Primitives;

    /// <summary>
    /// Drawing information for a single telegram.
    /// </summary>
    public interface ITelegramInfo
    {
        /// <summary>
        /// Gets the schedule types (1, 2 or both)
        /// </summary>
        Schedule Schedule { get; }

        /// <summary>
        /// Gets the schedule mode (default, periodic, inverted or periodic and inverted).
        /// </summary>
        ScheduleMode ScheduleMode { get; }

        /// <summary>
        /// Gets the telegram number (<c>lXXX</c> or <c>zXXX</c>).
        /// </summary>
        int TelegramNumber { get; }

        /// <summary>
        /// Gets the number of primitives defined for this telegram.
        /// </summary>
        int PrimitiveCount { get; }

        /// <summary>
        /// Gets the background color of this telegram.
        /// </summary>
        IColor BackgroundColor { get; }

        /// <summary>
        /// Gets all primitives.
        /// </summary>
        /// <returns>
        /// The list of <see cref="GraphicPrimitiveBase"/>-subclass objects.
        /// </returns>
        IEnumerable<GraphicPrimitiveBase> GetPrimitives();
    }
}