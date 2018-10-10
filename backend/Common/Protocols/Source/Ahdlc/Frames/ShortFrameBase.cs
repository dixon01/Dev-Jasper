// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ShortFrameBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the ShortFrameBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// Base class for short frames (<c>Kurztelegramm</c>).
    /// Short frames don't have any payload and no checksum.
    /// </summary>
    public abstract class ShortFrameBase : FrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShortFrameBase"/> class.
        /// </summary>
        /// <param name="functionCode">
        /// The function code.
        /// </param>
        protected ShortFrameBase(FunctionCode functionCode)
            : base(functionCode)
        {
        }

        /// <summary>
        /// Writes the contents (everything after the command byte and before the end boundary)
        /// of this frame to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal sealed override void WriteContents(FrameWriter writer)
        {
            // short frames don't have a payload
        }
    }
}