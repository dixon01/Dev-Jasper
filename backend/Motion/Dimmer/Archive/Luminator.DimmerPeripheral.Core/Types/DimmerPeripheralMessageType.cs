namespace Luminator.DimmerPeripheral.Core.Types
{
    /// <summary>
    /// Message Types for the Luminator Dimmer Module
    /// </summary>
    public enum DimmerPeripheralMessageType : byte
    {
        Unknown = 0x00,
        PollRequest = 0x01,
        VersionRequest = 0x10,
        SetLightSensorScale = 0x11,
        SetBrightness = 0x12,
        SetMonitorPower = 0x13,
        SetMode = 0x14,
        QueryRequest = 0x15,

        // responses:
        PollResponse = 0x02,
        Ack = 0x04,
        Nak = 0x05,
        VersionResponse = 0x20,
        QueryResponse = 0x21,
    }
}
