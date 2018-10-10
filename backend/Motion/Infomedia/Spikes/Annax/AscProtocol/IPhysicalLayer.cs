namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    using System;

    public interface IPhysicalLayer
    {
        event EventHandler<DataEventArgs> DataReady;

        int MaxPayloadSize { get; }

        void SetNetworkLayer(INetworkLayer networkLayer);

        int Decode(byte[] buffer, int offset, int count);

        void Encode(INetworkLayer type, NetworkServiceType serviceType, byte[] buffer, int offset, int count);
    }
}