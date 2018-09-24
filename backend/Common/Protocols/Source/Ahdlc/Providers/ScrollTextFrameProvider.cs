// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ScrollTextFrameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ScrollTextFrameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Provider for scroll text frames (mode 0x11).
    /// </summary>
    public class ScrollTextFrameProvider : TextFrameProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollTextFrameProvider"/> class.
        /// </summary>
        /// <param name="blockGap">
        /// The gap between text blocks.
        /// </param>
        /// <param name="padding">
        /// The padding after the last text block.
        /// </param>
        /// <param name="texts">
        /// The texts (maximum: 3).
        /// </param>
        public ScrollTextFrameProvider(int blockGap, int padding, params string[] texts)
            : base(DisplayMode.ScrollText, blockGap, padding, texts)
        {
        }
    }
}
