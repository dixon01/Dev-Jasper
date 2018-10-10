namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;
    using System.IO;

    public class FrameWriter
    {
        private readonly MemoryStream stream;

        private readonly BinaryWriter writer;

        public FrameWriter()
        {
            this.stream = new MemoryStream(new byte[10000], 0, 10000, true, true);
            this.writer = new BinaryWriter(this.stream);
        }

        public int Length
        {
            get
            {
                return (int)this.stream.Length;
            }
        }

        public void WriteByte(byte value)
        {
            this.writer.Write(value);
        }

        public void WriteUInt16(ushort value)
        {
            this.writer.Write((ushort)(((value & 0xFF) << 8) | ((value & 0xFF00) >> 8)));
        }

        public void WriteSByte(sbyte value)
        {
            this.writer.Write(value);
        }

        public void WriteData(byte[] data)
        {
            this.writer.Write(data, 0, data.Length);
        }

        public void Clear()
        {
            this.writer.Flush();
            this.stream.Position = 0;
            this.stream.SetLength(0);
        }

        public void CopyTo(int sourceIndex, byte[] destination, int destinationIndex, int length)
        {
            length = Math.Min(length, this.Length - sourceIndex);
            Array.Copy(this.stream.GetBuffer(), sourceIndex, destination, destinationIndex, length);
        }
    }
}