namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;

    using Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol.Commands;

    public interface IApplicationLayer
    {
        event EventHandler<CommandEventArgs> CommandReceived;

        void SetNetworkLayer(INetworkLayer networkLayer);

        void ReadData(NetworkServiceType serviceType, FrameReader reader);

        int WriteData(NetworkServiceType serviceType, CommandBase command);
    }
}