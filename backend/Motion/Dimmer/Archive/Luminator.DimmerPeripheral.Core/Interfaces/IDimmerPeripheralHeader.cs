namespace Luminator.DimmerPeripheral.Core.Interfaces
{
    using System;
    
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    
    [Obsolete("See Luminator.PeripheralDimmer")]
    public interface IDimmerPeripheralHeader : IPeripheralHeader
    {
        /// <summary>
        /// Gets or sets the message type.  This is a workaround as to not have to refactor Luminator.Peripheral.Protocol.Core
        /// </summary>
        new DimmerPeripheralMessageType MessageType { get; set; }
    }
}
