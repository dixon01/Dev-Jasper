namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class ClearBitmapCommand : CommandBase
    {
        public ClearBitmapCommand()
            : base(Command.ClearBitmap)
        {
        }

        public ClearBitmapCommand(FrameReader reader)
            : this()
        {
            this.BitmapNumber = reader.ReadUInt16();
        }

        public int BitmapNumber { get; set; }

        public override void WriteTo(FrameWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteUInt16((ushort)this.BitmapNumber);
        }

        public override string ToString()
        {
            return string.Format("ClearBitmap: bmp={0}", this.BitmapNumber);
        }
    }
}