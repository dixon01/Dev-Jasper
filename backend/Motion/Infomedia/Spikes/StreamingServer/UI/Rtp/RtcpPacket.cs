// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RtcpPacket.cs" company="Gorba AG">
//   Copyright © 2011-2012 Gorba AG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.IO;

    /// <summary>
    /// The rtcp packet.
    /// </summary>
    public abstract class RtcpPacket
    {
        #region Constants and Fields

        private readonly byte packetType;

        private readonly bool padding;

        private readonly int sourceCount;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RtcpPacket"/> class.
        /// </summary>
        /// <param name="padding">
        /// The padding.
        /// </param>
        /// <param name="sourceCount">
        /// The source count.
        /// </param>
        /// <param name="packetType">
        /// The packet type.
        /// </param>
        public RtcpPacket(bool padding, int sourceCount, byte packetType)
        {
            this.padding = padding;
            this.sourceCount = sourceCount;
            this.packetType = packetType;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Get the data.
        /// </summary>
        /// <returns>
        /// </returns>
        public byte[] GetData()
        {
            var ms = new MemoryStream();
            var writer = new BinaryWriter(ms);

            writer.Write((byte)(0x80 | (this.padding ? 0x20 : 0x00) | (this.sourceCount & 0x1F)));
            writer.Write(this.packetType);
            writer.Write((short)0); // length, to be filled later
            this.AddBody(writer);
            writer.Flush();

            var data = new byte[ms.Length];
            Array.Copy(ms.GetBuffer(), data, data.Length);
            long length = (data.Length / 4) - 1;
            data[2] = (byte)(0xFF & (length >> 8));
            data[3] = (byte)(0xFF & length);

            return data;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add body.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected abstract void AddBody(BinaryWriter writer);

        #endregion
    }
}