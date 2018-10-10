// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TelegramInfoBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the TelegramInfoBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Formats.AlphaNT.Ntd.Telegrams
{
    using System.Collections.Generic;

    using Gorba.Common.Formats.AlphaNT.Common;
    using Gorba.Common.Formats.AlphaNT.Ntd.Primitives;

    /// <summary>
    /// Base class for <see cref="ITelegramInfo"/> implementations.
    /// This class is only to be used from within <see cref="NtdFile"/>.
    /// </summary>
    internal abstract class TelegramInfoBase : ITelegramInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TelegramInfoBase"/> class.
        /// </summary>
        /// <param name="mode">
        /// The mode byte.
        /// </param>
        protected TelegramInfoBase(byte mode)
        {
            this.Schedule = (Schedule)(mode >> 6);
            this.ScheduleMode = (ScheduleMode)((mode >> 4) & 0x03);
        }

        /// <summary>
        /// Gets or sets the schedule types (1, 2 or both)
        /// </summary>
        public Schedule Schedule { get; protected set; }

        /// <summary>
        /// Gets or sets the schedule mode (default, periodic, inverted or periodic and inverted).
        /// </summary>
        public ScheduleMode ScheduleMode { get; protected set; }

        /// <summary>
        /// Gets or sets the telegram number (<c>lXXX</c> or <c>zXXX</c>).
        /// </summary>
        public int TelegramNumber { get; protected set; }

        /// <summary>
        /// Gets or sets the number of primitives defined for this telegram.
        /// </summary>
        public int PrimitiveCount { get; protected set; }

        /// <summary>
        /// Gets or sets the background color of this telegram.
        /// </summary>
        public IColor BackgroundColor { get; protected set; }

        /// <summary>
        /// Gets all primitives.
        /// </summary>
        /// <returns>
        /// The list of <see cref="GraphicPrimitiveBase"/>-subclass objects.
        /// </returns>
        public abstract IEnumerable<GraphicPrimitiveBase> GetPrimitives();
    }
}