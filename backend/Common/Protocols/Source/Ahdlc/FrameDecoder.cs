// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FrameDecoder.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the FrameDecoder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Gorba.Common.Protocols.Ahdlc.Frames;
    using Gorba.Common.Utility.Core;

    using NLog;

    /// <summary>
    /// Decoder for <see cref="FrameBase"/>s.
    /// </summary>
    public class FrameDecoder
    {
        private static readonly Logger Logger = LogHelper.GetLogger<FrameDecoder>();

        private readonly MemoryStream buffer = new MemoryStream();

        private State state = State.OutsideFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameDecoder"/> class.
        /// </summary>
        /// <param name="isHighSpeed">
        /// Flag indicating if we are decoding high-speed frames.
        /// </param>
        public FrameDecoder(bool isHighSpeed)
        {
            this.IsHighSpeed = isHighSpeed;
        }

        private enum State
        {
            OutsideFrame,
            FrameStart,
            Content, // command byte, payload or checksum
            EscapedContent
        }

        /// <summary>
        /// Gets a value indicating whether we are decoding high-speed frames.
        /// </summary>
        public bool IsHighSpeed { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether to ignore the frame start.
        /// If the frame start is ignored, we will still decode a frame even if its
        /// start marker is missing. This can be used in cases where the first
        /// bits of the response are crippled because of timing issues.
        /// </summary>
        public bool IgnoreFrameStart { get; set; }

        /// <summary>
        /// Adds multiple bytes to the decoder and tries to read all available
        /// frames in the given data. Unused data will be cached for later use.
        /// IMPORTANT: This method doesn't throw an exception but just discards (and logs) bad bytes.
        /// </summary>
        /// <param name="data">
        /// The data to add.
        /// </param>
        /// <returns>
        /// An enumeration over all decoded frames.
        /// </returns>
        public IEnumerable<FrameBase> AddBytes(byte[] data)
        {
            foreach (var b in data)
            {
                FrameBase frame;
                try
                {
                    frame = this.AddByte(b);
                }
                catch (FrameDecodingException ex)
                {
                    Logger.Warn(ex, "Couldn't add byte");
                    continue;
                }

                if (frame != null)
                {
                    yield return frame;
                }
            }
        }

        /// <summary>
        /// Adds multiple bytes to the decoder and tries to read all available
        /// frames in the given data. Unused data will be cached for later use.
        /// IMPORTANT: This method doesn't throw an exception but just discards (and logs) bad bytes.
        /// </summary>
        /// <param name="data">
        /// The data to add.
        /// </param>
        /// <param name="offset">
        /// The offset into the <see cref="data"/>.
        /// </param>
        /// <param name="length">
        /// The number of bytes to get from <see cref="data"/> starting at <see cref="offset"/>.
        /// </param>
        /// <returns>
        /// An enumeration over all decoded frames.
        /// </returns>
        public IEnumerable<FrameBase> AddBytes(byte[] data, int offset, int length)
        {
            for (int i = offset; i < offset + length; i++)
            {
                FrameBase frame;
                try
                {
                    frame = this.AddByte(data[i]);
                }
                catch (FrameDecodingException ex)
                {
                    Logger.Warn(ex, "Couldn't add byte");
                    continue;
                }

                if (frame != null)
                {
                    yield return frame;
                }
            }
        }

        /// <summary>
        /// Adds a single byte to the decoder.
        /// </summary>
        /// <param name="b">
        /// The byte.
        /// </param>
        /// <returns>
        /// A <see cref="FrameBase"/> object, if an entire frame was successfully decoded, otherwise null.
        /// </returns>
        /// <exception cref="FrameDecodingException">
        /// If a frame was found but there was an error in the frame.
        /// </exception>
        public FrameBase AddByte(byte b)
        {
            Logger.Trace("Adding byte {0:X2} in state {1}", b, this.state);
            FrameBase frame = null;
            try
            {
                switch (this.state)
                {
                    case State.OutsideFrame:
                        this.HandleOutsideFrame(b);
                        break;
                    case State.FrameStart:
                        this.HandleFrameStart(b);
                        break;
                    case State.Content:
                        frame = this.HandleContent(b);
                        break;
                    case State.EscapedContent:
                        this.HandleEscapedContent(b);
                        break;
                    default:
                        this.state = State.OutsideFrame;
                        break;
                }
            }
            finally
            {
                Logger.Trace(
                    frame != null ? "State changed to {0}, decoded {1}" : "State changed to {0}", this.state, frame);
            }

            return frame;
        }

        private void HandleOutsideFrame(byte b)
        {
            if (b == FrameConstants.Boundary)
            {
                this.state = State.FrameStart;
                return;
            }

            if (!this.IgnoreFrameStart)
            {
                Logger.Debug("Got byte outside frame, ignoring it");
                return;
            }

            Logger.Debug("Got byte outside frame, assuming we had a frame start before");
            this.HandleFrameStart(b);
        }

        private void HandleFrameStart(byte b)
        {
            if (b == FrameConstants.Boundary)
            {
                this.state = State.FrameStart;
                return;
            }

            if (b == 0x00)
            {
                Logger.Debug("Got byte 0x00 at frame start, ignoring frame");
                this.state = State.FrameStart;
                return;
            }

            this.buffer.SetLength(0);
            this.buffer.WriteByte(b);
            this.state = State.Content;
        }

        private FrameBase HandleContent(byte b)
        {
            if (b == FrameConstants.Boundary)
            {
                this.state = State.OutsideFrame;
                return this.Decode();
            }

            if (b == FrameConstants.Escape)
            {
                this.state = State.EscapedContent;
                return null;
            }

            this.buffer.WriteByte(b);
            return null;
        }

        private void HandleEscapedContent(byte b)
        {
            if (b == FrameConstants.Boundary)
            {
                this.state = State.OutsideFrame;
                throw new FrameDecodingException("Unexpected boundry byte (7E) after escape (7D)");
            }

            if (b == FrameConstants.Escape)
            {
                this.state = State.OutsideFrame;
                throw new FrameDecodingException("Unexpected escape byte (7D) after escape (7D)");
            }

            this.state = State.Content;
            this.buffer.WriteByte((byte)(b ^ FrameConstants.XOr));
        }

        private FrameBase Decode()
        {
            int checksum = -1;
            if (this.buffer.Position > 1)
            {
                // read and remove the checksum if we are in a long telegram
                this.buffer.Seek(-1, SeekOrigin.Current);
                checksum = this.buffer.ReadByte();
                this.buffer.SetLength(this.buffer.Length - 1);
            }

            FrameDecodingException firstException = null;
            for (int startOffset = 0; startOffset < this.buffer.Length; startOffset++)
            {
                this.buffer.Position = startOffset;
                var reader = new FrameReader(this.buffer, this.IsHighSpeed);
                var commandByte = reader.ReadByte();
                var address = commandByte & 0x0F;
                var functionCode = (FunctionCode)(commandByte & 0xF0);
                FrameBase frame;
                try
                {
                    frame = FrameBase.Create(functionCode);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    if (firstException == null)
                    {
                        firstException =
                            new FrameDecodingException(
                                "Unknown function code 0x" + ((byte)functionCode).ToString("X2"), ex);
                    }

                    Logger.Warn(
                        "Unknown function code 0x{0:X2}, retrying with {1} byte(s) offset",
                        (byte)functionCode,
                        startOffset + 1);
                    continue;
                }

                frame.Address = address;
                var longFrame = frame as LongFrameBase;
                if (longFrame != null)
                {
                    longFrame.ReadPayload(reader);
                    reader.VerifyChecksum((byte)checksum);
                }

                return frame;
            }

            throw firstException ?? new FrameDecodingException("No valid frame found");
        }
    }
}