namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;

    public class DataEventArgs : EventArgs
    {
        public DataEventArgs(byte[] data, int offset, int count)
        {
            this.Count = count;
            this.Offset = offset;
            this.Data = data;
        }

        public byte[] Data { get; private set; }

        public int Offset { get; private set; }

        public int Count { get; private set; }
    }
}