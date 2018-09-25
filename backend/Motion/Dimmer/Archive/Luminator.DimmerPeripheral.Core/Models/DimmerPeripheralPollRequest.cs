namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core.Types;

    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralPollRequest : DimmerPeripheralBaseMessage
    {
        #region Constructors

        public DimmerPeripheralPollRequest() : base(DimmerPeripheralMessageType.PollRequest)
        {
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralPollRequest(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.PollRequest, systemMessageType)
        {
            base.Header.Address = address;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        #endregion
    }
}
