// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlMessageCodec.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the XmlMessageCodec type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Medi.Core.Transcoder.Xml
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    using Gorba.Common.Medi.Core.Peers;
    using Gorba.Common.Medi.Core.Peers.Codec;
    using Gorba.Common.Medi.Core.Peers.Session;
    using Gorba.Common.Medi.Core.Peers.Transport;
    using Gorba.Common.Medi.Core.Utility;

    /// <summary>
    /// Message codec using .NET XML serialization.
    /// Messages are delimited with a '\0' character.
    /// </summary>
    internal class XmlMessageCodec : MessageCodec<XmlCodecConfig>
    {
        private const byte MessageEndMarker = 0;

        private const int FrameHeaderSize = 17;

        private static readonly CodecIdentification CurrentVersion = new CodecIdentification('X', 1);

        private readonly XmlSerializer serializer = new XmlSerializer(typeof(MediMessage));

        private readonly byte[] headerBytes = new byte[17];

        private XmlWriterSettings settings;

        /// <summary>
        /// Gets the identification of this codec.
        /// </summary>
        public override CodecIdentification Identification
        {
            get { return CurrentVersion; }
        }

        /// <summary>
        /// Configures this codec with the given configuration.
        /// </summary>
        /// <param name="config">
        /// The config object.
        /// </param>
        public override void Configure(XmlCodecConfig config)
        {
            this.settings = new XmlWriterSettings
            {
                Encoding = Encoding.GetEncoding(config.Encoding),
                CloseOutput = false
            };
        }

        /// <summary>
        /// Converts the message to buffers. More precisely creates a
        /// provider that can be queried for the message buffers.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <returns>
        /// a message buffer provider.
        /// </returns>
        public override IMessageBufferProvider Encode(MediMessage message)
        {
            return new MessageBufferProvider(message, this.serializer, this.settings);
        }

        /// <summary>
        /// Converts a buffer from a certain session to a message.
        /// </summary>
        /// <param name="buffer">
        ///   The buffer. The bytes consumed by this method are removed from the message buffer, but
        ///   the message buffer might not be empty after this method returns.
        /// </param>
        /// <param name="readResult">
        /// The read result including the session which received the message buffer.
        /// </param>
        /// <returns>
        /// the decoded message or null if the buffer was only a part of
        /// a message.
        /// </returns>
        public override MediMessage Decode(MessageBuffer buffer, MessageReadResult readResult)
        {
            int endMarker = Array.IndexOf(buffer.Buffer, MessageEndMarker, 0, buffer.Count);
            if (endMarker < 0)
            {
                return null;
            }

            var input = buffer.OpenRead(0, endMarker);
            try
            {
                if (!this.ReadFrameInfo(readResult.Session.FrameController, input))
                {
                    return null;
                }

                return this.serializer.Deserialize(input) as MediMessage;
            }
            finally
            {
                buffer.Remove(endMarker + 1);
            }
        }

        private bool ReadFrameInfo(IFrameController frameController, Stream input)
        {
            if (frameController == null)
            {
                return true;
            }

            int pos = 0;
            int r;
            while ((r = input.Read(this.headerBytes, pos, FrameHeaderSize - pos)) > 0 && pos < FrameHeaderSize)
            {
                pos += r;
            }

            if (this.headerBytes[8] != '-' || pos != FrameHeaderSize)
            {
                throw new ArgumentException("Bad frame header: " + Encoding.ASCII.GetString(this.headerBytes, 0, pos));
            }

            var frame = new FrameInfo(
                uint.Parse(Encoding.ASCII.GetString(this.headerBytes, 0, 8), NumberStyles.HexNumber),
                uint.Parse(Encoding.ASCII.GetString(this.headerBytes, 9, 8), NumberStyles.HexNumber));
            if (frameController.VerifyIncoming(frame) == FrameCheck.DuplicateFrame)
            {
                // we ignore duplicate frames
                return false;
            }

            return true;
        }

        private class MessageBufferProvider : IMessageBufferProvider
        {
            private readonly MediMessage message;
            private readonly XmlSerializer serializer;
            private readonly XmlWriterSettings settings;

            public MessageBufferProvider(MediMessage message, XmlSerializer serializer, XmlWriterSettings settings)
            {
                this.message = message;
                this.serializer = serializer;
                this.settings = settings;
            }

            public IEnumerable<SendMessageBuffer> GetMessageBuffers(
                ITransportSession destination, CodecIdentification codecId)
            {
                var memory = new MemoryStream();

                uint frameId = 0;
                if (destination.FrameController != null)
                {
                    // write the frame information
                    var frame = destination.FrameController.GetNextFrameInfo();
                    frameId = frame.SendFrameId;
                    var frameHeaderStr = string.Format("{0:X8}-{1:X8}", frame.SendFrameId, frame.AckFrameId);
                    var frameHeader = Encoding.ASCII.GetBytes(frameHeaderStr);
                    memory.Write(frameHeader, 0, FrameHeaderSize);
                }

                var writer = XmlWriter.Create(memory, this.settings);
                this.serializer.Serialize(writer, this.message);
                memory.WriteByte(MessageEndMarker);

                var buffer = new SendMessageBuffer(frameId, memory.ToArray(), 0, (int)memory.Length);
                yield return buffer;
            }
        }
    }
}
