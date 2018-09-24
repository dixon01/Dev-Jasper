// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFrameHandler.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the IFrameHandler type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using Gorba.Common.Protocols.Ahdlc.Frames;

    /// <summary>
    /// Interface for classes that can read and write frames to an AHDLC connection (e.g. serial port).
    /// </summary>
    public interface IFrameHandler
    {
        /// <summary>
        /// Reads the next frame from the underlying stream.
        /// This method blocks until an entire frame is available or the
        /// end of the stream was reached (EOS).
        /// </summary>
        /// <returns>
        /// The decoded <see cref="FrameBase"/> or null if the end of the stream was reached (EOS).
        /// </returns>
        FrameBase ReadNextFrame();

        /// <summary>
        /// Writes an entire frame to the underlying stream.
        /// </summary>
        /// <param name="frame">
        /// The frame to write.
        /// </param>
        void WriteFrame(FrameBase frame);
    }
}