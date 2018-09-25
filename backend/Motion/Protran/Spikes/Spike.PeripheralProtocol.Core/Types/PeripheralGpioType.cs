namespace Luminator.PeripheralProtocol.Core.Types
{
    public enum PeripheralGpioType : byte
    {
        None = 0,
        Door1,
        Door2,
        StopRequest,
        AdaStopRequest,
        PushToTalk,
        InteriorActive,
        ExteriorActive,
        Odometer,
        Reverse,
        InteriorSpeakerMuted,
        ExteriorSpeakerMuted,
        RadioSpeakerMuted,
        InterorSpeakerNonMuting,
        ExteriorSpeakerNonMuting,
        RadioSpeakerNonMuting,
    }
}