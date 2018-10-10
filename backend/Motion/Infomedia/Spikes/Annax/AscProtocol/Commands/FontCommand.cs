namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class FontCommand : CommandBase
    {
        public FontCommand()
            : base(Command.Font)
        {
        }

        public FontCommand(FrameReader reader)
            : this()
        {
        }

        public override void WriteTo(FrameWriter frameWriter)
        {
            throw new System.NotImplementedException();
        }
    }
}