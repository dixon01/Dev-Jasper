// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IItemDrawContext.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IItemDrawContext type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace TestDirectXRenderer
{
    /// <summary>
    /// The context in which to draw something.
    /// </summary>
    public interface IItemDrawContext
    {
        /// <summary>
        /// Gets a value indicating whether a blinking item should be drawn or not.
        /// This flag changes approximately every 0.5 seconds.
        /// [bl][/bl] BBcode requires this.
        /// </summary>
        bool BlinkOn { get; }

        /// <summary>
        /// Gets a the current counter which is incremented roughly every second.
        /// [a][|][/a] BBcode requires this.
        /// </summary>
        int AlternationCounter { get; }
    }
}
