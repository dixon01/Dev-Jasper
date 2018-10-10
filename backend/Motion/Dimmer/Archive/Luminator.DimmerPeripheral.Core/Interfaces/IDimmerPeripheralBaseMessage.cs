namespace Luminator.DimmerPeripheral.Core.Interfaces
{
    using System;

    using Luminator.DimmerPeripheral.Core.Models;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    
    [Obsolete("See Luminator.PeripheralDimmer")]
    public interface IDimmerPeripheralBaseMessage : IPeripheralBaseMessage
    {
        new DimmerPeripheralHeader Header { get; set; }
    }
}
