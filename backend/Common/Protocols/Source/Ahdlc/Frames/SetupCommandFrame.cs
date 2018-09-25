// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SetupCommandFrame.cs" company="Gorba AG">
//   Copyright © 2011-2014 Gorba AG. All rights reserved.
// </copyright>
// <summary>
//   Defines the SetupCommandFrame type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Common.Protocols.Ahdlc.Frames
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The setup command frame (0x01).
    /// This frame is sent from the master to the slave.
    /// </summary>
    public class SetupCommandFrame : LongFrameBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SetupCommandFrame"/> class.
        /// </summary>
        /// <param name="mode">
        /// The display mode.
        /// </param>
        public SetupCommandFrame(DisplayMode mode)
            : base(FunctionCode.SetupCommand)
        {
            this.Mode = mode;
            this.Data = this.CreateSetupData();
            this.ScrollBlocks = new List<ScrollBlockInfo>();
        }

        /// <summary>
        /// Gets the display mode (D1).
        /// </summary>
        public DisplayMode Mode { get; private set; }

        /// <summary>
        /// Gets or sets the data block count (D2, for some also D3).
        /// </summary>
        public int DataBlockCount { get; set; }

        /// <summary>
        /// Gets the data (D3...D8).
        /// The data is accessed with its index (being 3...8).
        /// </summary>
        public SetupData Data { get; private set; }

        /// <summary>
        /// Gets the scroll blocks.
        /// </summary>
        public List<ScrollBlockInfo> ScrollBlocks { get; private set; }

        /// <summary>
        /// Reads the payload of this frame (without the command byte) from the given reader.
        /// </summary>
        /// <param name="reader">
        /// The reader.
        /// </param>
        internal override void ReadPayload(FrameReader reader)
        {
            var frame = reader.ReadAll();
            if (frame.Length < 6)
            {
                throw new FrameDecodingException("Wrong Setup Command frame length (<6)");
            }

            int offset = 0;
            if (frame.Length == 6)
            {
                this.Mode = DisplayMode.Color;
            }
            else
            {
                this.Mode = (DisplayMode)frame[offset++];
                if (this.GetBlockCountByteWidth() == 2)
                {
                    var blockCount = frame[offset++] << 8;
                    blockCount |= frame[offset++];
                    this.DataBlockCount = blockCount;
                }
                else
                {
                    this.DataBlockCount = frame[offset++];
                }
            }

            this.CreateSetupData();
            offset = this.Data.ReadFrom(frame, offset);

            int scrollBlockInfoSize;
            switch (this.Mode)
            {
                case DisplayMode.BlockScrollBitmap:
                    scrollBlockInfoSize = 6;
                    break;
                case DisplayMode.BlockScrollSpeedBitmap:
                    scrollBlockInfoSize = 7;
                    break;
                case DisplayMode.BlockScrollLargeBitmap:
                    scrollBlockInfoSize = 8;
                    break;
                default:
                    // all other modes don't contain scroll blocks
                    return;
            }

            this.ScrollBlocks.Clear();

            for (int i = offset; i < frame.Length; i += scrollBlockInfoSize)
            {
                this.ScrollBlocks.Add(new ScrollBlockInfo(frame, i, scrollBlockInfoSize));
            }
        }

        /// <summary>
        /// Writes the payload of this frame (without the command byte) to the given writer.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        internal override void WritePayload(FrameWriter writer)
        {
            if (this.Mode != DisplayMode.Color)
            {
                writer.WriteByte((byte)this.Mode);
            }

            switch (this.GetBlockCountByteWidth())
            {
                case 1:
                    writer.WriteByte((byte)this.DataBlockCount);
                    break;
                case 2:
                    writer.WriteByte((byte)(this.DataBlockCount >> 8));
                    writer.WriteByte((byte)(this.DataBlockCount & 0xFF));
                    break;
            }

            this.Data.WriteTo(writer);
            foreach (var scrollBlock in this.ScrollBlocks)
            {
                scrollBlock.WriteTo(writer);
            }
        }

        private SetupData CreateSetupData()
        {
            switch (this.Mode)
            {
                case DisplayMode.BlockScrollLargeBitmap:
                    return new SetupData(4, 8);
                case DisplayMode.Color:
                    return new SetupData(1, 6);
                default:
                    return new SetupData(3, 8);
            }
        }

        private int GetBlockCountByteWidth()
        {
            switch (this.Mode)
            {
                case DisplayMode.BlockScrollLargeBitmap:
                    return 2;
                case DisplayMode.Color:
                    return 0;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// The generic data bytes in the setup command.
        /// </summary>
        public class SetupData
        {
            private readonly byte[] data;

            private readonly int offset;

            /// <summary>
            /// Initializes a new instance of the <see cref="SetupData"/> class.
            /// </summary>
            /// <param name="startOffset">
            /// The first valid offset indexed from 1 (3 = D3 or 4 = D4).
            /// </param>
            /// <param name="endOffset">
            /// The last valid offset indexed from 1 (8...11 for D8...D11).
            /// </param>
            internal SetupData(int startOffset, int endOffset)
            {
                this.data = new byte[endOffset - startOffset + 1];
                this.offset = startOffset;
            }

            /// <summary>
            /// Gets the length of the data.
            /// </summary>
            public int Length
            {
                get
                {
                    return this.data.Length;
                }
            }

            /// <summary>
            /// Gets or sets the value of a byte in the generic data bytes.
            /// </summary>
            /// <param name="index">
            /// The index (3...11 for D3...D11).
            /// </param>
            /// <returns>
            /// The <see cref="byte"/> at the given index.
            /// </returns>
            public byte this[int index]
            {
                get
                {
                    if (index < this.offset || index > this.offset + this.data.Length)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    return this.data[index - this.offset];
                }

                set
                {
                    if (index < this.offset || index > this.offset + this.data.Length)
                    {
                        throw new ArgumentOutOfRangeException("index");
                    }

                    this.data[index - this.offset] = value;
                }
            }

            /// <summary>
            /// Fills this setup data with the data from the given <see cref="frame"/>.
            /// </summary>
            /// <param name="frame">
            /// The frame data.
            /// </param>
            /// <param name="startPos">
            /// The start position.
            /// </param>
            /// <returns>
            /// The new offset after reading the data.
            /// </returns>
            internal int ReadFrom(byte[] frame, int startPos)
            {
                Array.Copy(frame, startPos, this.data, 0, this.data.Length);
                return startPos + this.data.Length;
            }

            /// <summary>
            /// Writes the 6 bytes to the given writer.
            /// </summary>
            /// <param name="writer">
            /// The writer.
            /// </param>
            internal void WriteTo(FrameWriter writer)
            {
                writer.WriteBytes(this.data);
            }
        }

        /// <summary>
        /// Information about a scroll block.
        /// </summary>
        public class ScrollBlockInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ScrollBlockInfo"/> class.
            /// </summary>
            /// <param name="data">
            /// The data.
            /// </param>
            /// <param name="offset">
            /// The offset.
            /// </param>
            /// <param name="count">
            /// The count.
            /// </param>
            internal ScrollBlockInfo(byte[] data, int offset, int count)
            {
                if (data.Length < offset + count)
                {
                    throw new FrameDecodingException("Scrollblock doesn't contain " + count + " bytes");
                }

                this.Data = new byte[count];
                Array.Copy(data, offset, this.Data, 0, count);
            }

            /// <summary>
            /// Gets the data bytes.
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Writes the 6 scroll block bytes to the given writer.
            /// </summary>
            /// <param name="writer">
            /// The writer.
            /// </param>
            internal void WriteTo(FrameWriter writer)
            {
                writer.WriteBytes(this.Data);
            }
        }
    }
}