// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StreamFrameHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the StreamFrameHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System.IO;

    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Helper class that allows to handle encoding and decoding frames from
    /// a <see cref="Stream"/>.
    /// </summary>
    public class StreamFrameHandler : IFrameHandler
    {
        private readonly Stream stream;

        private readonly FrameDecoder decoder;

        private readonly FrameEncoder encoder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamFrameHandler"/> class.
        /// </summary>
        /// <param name="stream">
        /// The stream to read from and write to.
        /// </param>
        /// <param name="isHighSpeed">
        /// Flag indicating if we are reading and writing high-speed frames.
        /// </param>
        public StreamFrameHandler(Stream stream, bool isHighSpeed)
        {
            this.stream = stream;
            this.decoder = new FrameDecoder(isHighSpeed);
            this.encoder = new FrameEncoder(isHighSpeed);
        }

        /// <summary>
        /// Reads the next frame from the underlying stream.
        /// This method blocks until an entire frame is available or the
        /// end of the stream was reached (EOS).
        /// </summary>
        /// <returns>
        /// The decoded <see cref="FrameBase"/> or null if the end of the stream was reached (EOS).
        /// </returns>
        public FrameBase ReadNextFrame()
        {
            int read;
            while ((read = this.stream.ReadByte()) != -1)
            {
                var frame = this.decoder.AddByte((byte)read);
                if (frame != null)
                {
                    return frame;
                }
            }

            return null;
        }

        /// <summary>
        /// Writes an entire frame to the underlying stream.
        /// </summary>
        /// <param name="frame">
        /// The frame to write.
        /// </param>
        public void WriteFrame(FrameBase frame)
        {
            this.encoder.Encode(frame, this.stream);
        }
    }
}
