// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AutoTextFrameProvider.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the AutoTextFrameProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Providers
{
    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Provider for auto text frames (mode 0x10).
    /// </summary>
    public class AutoTextFrameProvider : TextFrameProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutoTextFrameProvider"/> class.
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
        public AutoTextFrameProvider(int blockGap, int padding, params string[] texts)
            : base(DisplayMode.AutoText, blockGap, padding, texts)
        {
        }
    }
}
