namespace Gorba.Motion.Infomedia.Spikes.Annax.AscProtocol
{
    public interface INetworkLayer
    {
        byte ProtocolId { get; }

        void SetPhysicalLayer(IPhysicalLayer physicalLayer);

        void SetApplicationLayer(IApplicationLayer applicationLayer);

        void ReadPacket(byte sourceAddr, NetworkServiceType serviceType, FrameReader reader);

        int WritePacket(NetworkServiceType serviceType, FrameWriter writer);
    }
}