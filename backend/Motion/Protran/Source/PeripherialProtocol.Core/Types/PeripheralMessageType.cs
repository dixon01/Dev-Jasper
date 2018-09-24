namespace Luminator.PeripheralProtocol.Core.Types
{
    /// <summary>
    /// Message Types for the Luminator Audio Mux
    /// </summary>
    public enum PeripheralMessageType : byte
    {
        Unknown = 0,
        Poll = 1,
        Status = 2,
        Data = 3,
        Ack = 4,
        Nak = 5,
        SwitchRoles = 6,
        AudioConfig = 0x10,
        AudioOutputEnable = 0x11,
        SetVolume = 0x12,
        ImageUpdateStart = 0x13,
        ImageRecord = 0x14,
        ImageUpdateCancel = 0x15,
        RequestVersion = 0x16,

        // responses:
        AudioVersionResponse = 0x80,
        GpioStatusResponse = 0x81,
        AudioStatusResponse = 0x82,
        ImageStatusResponse = 0x83
    }
}