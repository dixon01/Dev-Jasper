namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    public enum AudioSourcePriority : byte
    {
        None = 0,
        DefaultPlatform = 1,
        DriverMic = 2,
        LineIn = 3
    }
}