namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;

    using Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands;

    using NLog;

    public class ApplicationLayer : IApplicationLayer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly FrameWriter frameWriter = new FrameWriter();

        private INetworkLayer networkLayer;

        public event EventHandler<CommandEventArgs> CommandReceived;

        protected virtual void RaiseCommandReceived(CommandEventArgs e)
        {
            var handler = this.CommandReceived;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void SetNetworkLayer(INetworkLayer net)
        {
            if (this.networkLayer == net)
            {
                return;
            }

            this.networkLayer = net;
            this.networkLayer.SetApplicationLayer(this);
        }

        public void ReadData(NetworkServiceType serviceType, FrameReader reader)
        {
            try
            {
                var command = CommandBase.Parse(serviceType, reader);
                Logger.Info(command);
                this.RaiseCommandReceived(new CommandEventArgs(command));
            }
            catch (ArgumentException ex)
            {
                Logger.Warn(ex.Message);
            }
        }

        public int WriteData(NetworkServiceType serviceType, CommandBase command)
        {
            this.frameWriter.Clear();
            command.WriteTo(this.frameWriter);
            return this.networkLayer.WritePacket(serviceType, this.frameWriter);
        }
    }
}