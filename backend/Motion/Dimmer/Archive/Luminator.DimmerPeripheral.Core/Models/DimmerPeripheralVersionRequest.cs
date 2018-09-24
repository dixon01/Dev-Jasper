namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.Runtime.InteropServices;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;

    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralVersionRequest : DimmerPeripheralBaseMessage
    {
        public new const int Size = DimmerPeripheralBaseMessage.Size;

        #region Constructors
        
        public DimmerPeripheralVersionRequest()
            : base(DimmerPeripheralMessageType.VersionRequest)
        {
            this.Checksum = this.CheckSum(this.GetBytes());
        }
        
        public DimmerPeripheralVersionRequest(ushort address)
            : base(DimmerPeripheralMessageType.VersionRequest)
        {
            this.Header.Address = address;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        #endregion
    }
}
