// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StaticTextFrameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Provider for static text frames (mode 0x12).
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Provider for static text frames (mode 0x12).
    /// </summary>
    public class StaticTextFrameProvider : TextFrameProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticTextFrameProvider"/> class.
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
        public StaticTextFrameProvider(int blockGap, int padding, params string[] texts)
            : base(DisplayMode.StaticText, blockGap, padding, texts)
        {
        }
    }
}
