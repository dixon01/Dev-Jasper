namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core.Types;

    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralQueryRequest : DimmerPeripheralBaseMessage
    {
        #region Constructors

        public DimmerPeripheralQueryRequest() : base(DimmerPeripheralMessageType.QueryRequest)
        {
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralQueryRequest(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.QueryRequest, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        #endregion
    }
}
