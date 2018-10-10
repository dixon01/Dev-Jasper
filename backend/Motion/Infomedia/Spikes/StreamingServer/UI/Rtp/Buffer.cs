// -----------------------------------------------------------------------
// <copyright file="Buffer.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Gorba.Motion.Infomedia.Spike.StreamingServer.UI.Rtp
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Buffer
    {
        public Buffer(byte[] data, int offset, int size)
        {
            this.Data = data;
            this.Offset = offset;
            this.Size = size;
        }

        public byte[] Data { get; private set; }

        public int Offset { get; private set; }
        
        public int Size { get; private set; }

        public override string ToString()
        {
            return BitConverter.ToString(this.Data, this.Offset, this.Size);
        }
    }
}
