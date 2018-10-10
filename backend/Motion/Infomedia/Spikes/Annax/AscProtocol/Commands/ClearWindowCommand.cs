namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class ClearWindowCommand : CommandBase
    {
        public ClearWindowCommand()
            : base(Command.ClearWindow)
        {
        }

        public ClearWindowCommand(FrameReader reader)
            : this()
        {
            this.WindowNumber = reader.ReadUInt16();
        }

        public int WindowNumber { get; set; }

        public override void WriteTo(FrameWriter writer)
        {
            base.WriteTo(writer);
            writer.WriteUInt16((ushort)this.WindowNumber);
        }

        public override string ToString()
        {
            return string.Format("ClearWindow: win={0}", this.WindowNumber);
        }
    }
}