namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;
    using System.IO;

    public class FrameReader
    {
        private readonly byte[] buffer;
        private readonly int offset;
        private readonly int count;

        private readonly MemoryStream stream;

        private readonly BinaryReader reader;

        public FrameReader(byte[] buffer, int offset, int count)
        {
            this.buffer = buffer;
            this.offset = offset;
            this.count = count;
            this.stream = new MemoryStream(buffer, offset, count, false, false);
            this.reader = new BinaryReader(this.stream);
        }

        public int Available
        {
            get
            {
                return this.count - (int)this.stream.Position;
            }
        }

        public byte ReadByte()
        {
            return this.reader.ReadByte();
        }

        public ushort ReadUInt16()
        {
            var value = this.reader.ReadUInt16();
            return (ushort)(((value & 0xFF) << 8) | ((value & 0xFF00) >> 8));
        }

        public sbyte ReadSByte()
        {
            return this.reader.ReadSByte();
        }

        public byte[] ReadRemainingBytes()
        {
            var data = new byte[this.Available];
            Array.Copy(this.buffer, (int)(this.offset + this.stream.Position), data, 0, this.Available);
            this.stream.Seek(0, SeekOrigin.End);
            return data;
        }
    }
}