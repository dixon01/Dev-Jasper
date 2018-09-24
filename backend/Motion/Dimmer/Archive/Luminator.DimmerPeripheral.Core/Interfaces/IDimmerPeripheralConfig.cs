namespace Luminator.DimmerPeripheral.Core.Interfaces
{
    using System;

    using Luminator.PeripheralProtocol.Core.Interfaces;

    /// <summary>
    /// Dimmer peripheral config
    /// </summary>
    [Obsolete("See Luminator.PeripheralDimmer")]
    public interface IDimmerPeripheralConfig : IPeripheralConfig
    {
        /// <summary>
        /// Gets or sets the range 1 scale upper limit to switch to range 2
        /// </summary>
        ushort Range1Upper { get; set; }

        /// <summary>
        /// Gets or sets the range 2 scale lower limit to switch to range 1
        /// </summary>
        ushort Range2Lower { get; set; }

        /// <summary>
        /// Gets or sets the range 2 scale upper limit to switch to range 3
        /// </summary>
        ushort Range2Upper { get; set; }

        /// <summary>
        /// Gets or sets the range 3 scale lower limit to switch to range 2
        /// </summary>
        ushort Range3Lower { get; set; }

        /// <summary>
        /// Gets or sets the range 3 scale upper limit to switch to range 4
        /// </summary>
        ushort Range3Upper { get; set; }
        
        /// <summary>
        /// Gets or sets the range 4 scale lower limit to switch to range 3
        /// </summary>
        ushort Range4Lower { get; set; }
    }
}
