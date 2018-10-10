namespace Luminator.PeripheralProtocol.Core.Types
{
    /// <summary>
    /// Message Types for the Luminator Audio Switch
    /// </summary>
    
    public enum PeripheralMessageType : byte
    {
        Unknown = 0,
        Poll = 1,
        Status = 2,
        Data = 3,
        Ack = 4,
        Nak = 5,        
    }
}