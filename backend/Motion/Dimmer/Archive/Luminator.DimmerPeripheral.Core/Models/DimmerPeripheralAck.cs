namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>The peripheral nak.</summary>
    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralAck : DimmerPeripheralBaseMessage
    {
        #region Constructors

        public DimmerPeripheralAck() : base(DimmerPeripheralMessageType.Ack)
        {
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralAck(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.Ack, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        #endregion
    }
}
