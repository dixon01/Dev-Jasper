namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.Runtime.InteropServices;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral nak.</summary>
    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralNak : DimmerPeripheralBaseMessage
    {
        #region Constructors

        public DimmerPeripheralNak() : base(DimmerPeripheralMessageType.Nak)
        {
            this.Checksum = this.CheckSum(this.GetBytes());
        }
        
        public DimmerPeripheralNak(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.Nak, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        #endregion
    }
}
