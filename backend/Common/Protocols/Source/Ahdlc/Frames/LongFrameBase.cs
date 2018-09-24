// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongFrameBase.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the LongFrameBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    /// <summary>
    /// Base class for long frames (<c>Langtelegramm</c>).
    /// Long frames have an optional payload and always a checksum.
    /// </summary>
    public abstract class LongFrameBase : FrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LongFrameBase"/> class.
        /// </summary>
        /// <param name="functionCode">
        /// The function code.
        /// </param>
        protected LongFrameBase(FunctionCode functionCode)
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
            this.WritePayload(writer);
            writer.WriteChecksum();
        }

        /// <summary>
        /// Writes the payload of this frame (without the command byte) to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal abstract void WritePayload(FrameWriter writer);

        /// <summary>
        /// Reads the payload of this frame (without the command byte) from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal abstract void ReadPayload(FrameReader reader);
    }
}