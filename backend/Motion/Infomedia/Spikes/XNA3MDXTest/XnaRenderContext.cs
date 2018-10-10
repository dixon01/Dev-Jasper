// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XnaRenderContext.cs" company="Gorba AG">
//   Copyright © 2011-2013 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace XNA3MDXTest
{
    using System;

    /// <summary>
    /// The XnaRenderContext.
    /// </summary>
    public class XnaRenderContext : IXnaRenderContext
    {
        private int firstTime;

        /// <summary>
        /// Gets a value indicating whether BlinkOn.
        /// </summary>
        public bool BlinkOn { get; private set; }

        /// <summary>
        /// Gets AlternationCounter.
        /// </summary>
        public int AlternationCounter { get; private set; }

        /// <summary>
        /// Gets ScrollCounter.
        /// </summary>
        public int ScrollCounter { get; private set; }

        /// <summary>
        /// Gets MillisecondsCounter.
        /// </summary>
        public int MillisecondsCounter { get; private set; }

        /// <summary>
        /// Resets the context.
        /// </summary>
        public void Reset()
        {
            this.firstTime = Environment.TickCount;
        }

        /// <summary>
        /// Updates the context.
        /// </summary>
        public void Update()
        {
            this.MillisecondsCounter = Environment.TickCount;

            // TODO: make 3 seconds alt and 0.5 seconds blink configurable
            int diff = this.MillisecondsCounter - this.firstTime;
            this.AlternationCounter = diff / 3000;
            this.BlinkOn = ((diff / 500) % 2) == 0;
            this.ScrollCounter = diff;
        }
    }
}
