namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class BitmapCommand : CommandBase
    {
        public BitmapCommand()
            : base(Command.Bitmap)
        {
        }

        public BitmapCommand(FrameReader reader)
            : this()
        {
        }

        public override void WriteTo(FrameWriter writer)
        {
            throw new System.NotImplementedException();
        }
    }
}