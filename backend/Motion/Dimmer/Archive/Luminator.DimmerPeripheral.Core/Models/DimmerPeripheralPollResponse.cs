using System.Diagnostics;
using Luminator.PeripheralProtocol.Core.Models;

namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Types;

    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralPollResponse : DimmerPeripheralBaseMessage
    {
        public new const int Size = PeripheralHeader.Size + sizeof(byte);
        
        [Order]
        public PeripheralStatusType Status { get; set; }

        [Order]
        public new byte Checksum { get; set; }

        #region Constructors

        public DimmerPeripheralPollResponse() : base(DimmerPeripheralMessageType.PollResponse)
        {
            this.Status = PeripheralStatusType.Unknown;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralPollResponse(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.PollResponse, systemMessageType)
        {
            base.Header.Address = address;
            this.Status = PeripheralStatusType.Unknown;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralPollResponse(byte[] bytes) : base(DimmerPeripheralMessageType.PollResponse)
        {
            if (bytes == null || bytes.Length != Size + 2)
                return; // eat it

            // TODO
            Header.Address = (ushort)((bytes[2] << 8) + bytes[3]);
            Header.SystemType = (PeripheralSystemMessageType)bytes[4];
            Header.MessageType = (DimmerPeripheralMessageType)bytes[5];
            Status = (PeripheralStatusType)bytes[6];
            this.Checksum = bytes[7];
        }

        #endregion

        /// <summary>
        /// Work around since base reflection will go through all the properties, ToBytes marshalling needs further work.
        /// </summary>
        /// <returns>message bytes</returns>
        public override byte[] GetBytes()
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write((byte)DimmerConstants.PeripheralFramingByte);
                writer.Write((byte)((Size & 0xff00) >> 8));
                writer.Write((byte)(Size & 0xff));
                writer.Write((byte)((Header.Address & 0xff00) >> 8));
                writer.Write((byte)(Header.Address & 0xff));
                writer.Write((byte)Header.SystemType);
                writer.Write((byte)Header.MessageType);
                writer.Write((byte)Status);
                writer.Write((byte)Checksum);
                return stream.ToArray();
            }
        }

        public override string ToString()
        {
            switch (Status)
            {
                case PeripheralStatusType.Fault:
                    return "Fault";

                case PeripheralStatusType.Ok:
                    return "Ok";

                case PeripheralStatusType.Unknown:
                default:
                    return "Unknown";
            }
        }
    }
}
