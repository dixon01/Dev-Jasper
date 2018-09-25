using System;

namespace Luminator.DimmerPeripheral.Core
{
    using Luminator.DimmerPeripheral.Core.Models;

    [Obsolete("See Luminator.PeripheralDimmer")]
    public static class DimmerConstants
    {
        #region Constants

        /// <summary>The dimmer address.</summary>
        public const int DimmerAddress = 0x0;
        
        /// <summary>The framing byte.</summary>
        public const int PeripheralFramingByte = 0x7E;

        public const int SmallestMessageSize = DimmerPeripheralHeader.Size + sizeof(byte);  // Checksum

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the recieve bytes threshold.</summary>
        public static int RecieveBytesThreshold { get; set; } = 7;

        #endregion
    }
}