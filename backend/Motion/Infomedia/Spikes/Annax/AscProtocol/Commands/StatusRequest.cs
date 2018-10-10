namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands
{
    public class StatusRequest : CommandBase
    {
        public StatusRequest()
            : base(Command.GetStatus)
        {
        }

        public StatusRequest(FrameReader reader)
            : this()
        {
        }
    }
}