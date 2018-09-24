namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class ClearAllCommand : CommandBase
    {
        public ClearAllCommand()
            : base(Command.ClearAll)
        {
        }

        public ClearAllCommand(FrameReader reader)
            : this()
        {
        }
    }
}