// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameEncoder.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameEncoder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System.IO;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Encoder for <see cref="FrameBase"/>s.
    /// </summary>
    public class FrameEncoder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameEncoder"/> class.
        /// </summary>
        /// <param name="isHighSpeed">
        /// Flag indicating if we are encoding high-speed frames.
        /// </param>
        public FrameEncoder(bool isHighSpeed)
        {
            this.IsHighSpeed = isHighSpeed;
        }

        /// <summary>
        /// Gets a value indicating whether we are encoding high-speed frames.
        /// </summary>
        public bool IsHighSpeed { get; private set; }

        /// <summary>
        /// Encodes the given frame into a byte array.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <returns>
        /// The byte array containing the encoded frame
        /// </returns>
        public byte[] Encode(FrameBase frame)
        {
            var memory = new MemoryStream();
            this.Encode(frame, memory);
            return memory.ToArray();
        }

        /// <summary>
        /// Encodes the given frame to a stream.
        /// </summary>
        /// <param name="frame">
        /// The frame.
        /// </param>
        /// <param name="output">
        /// The output stream to write to.
        /// </param>
        public void Encode(FrameBase frame, Stream output)
        {
            var writer = new FrameWriter(output, this.IsHighSpeed);
            frame.WriteTo(writer);
        }
    }
}