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
    public class DimmerPeripheralQueryResponse : DimmerPeripheralBaseMessage
    {
        public new const int Size = PeripheralHeader.Size + 6 * sizeof(byte);

        [Order]
        public ushort AmbientLevel { get; set; }

        [Order]
        public byte RangeScale { get; set; }

        [Order]
        public DimmerPeripheralPowerType PowerStatus { get; set; }

        [Order]
        public byte Brightness { get; set; }

        [Order]
        public DimmerPeripheralMode Mode { get; set; }

        [Order]
        public new byte Checksum { get; set; }

        #region Constructors

        public DimmerPeripheralQueryResponse() : base(DimmerPeripheralMessageType.QueryResponse)
        {
            this.PowerStatus = DimmerPeripheralPowerType.Off;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralQueryResponse(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.QueryResponse, systemMessageType)
        {
            base.Header.Address = address;
            this.PowerStatus = DimmerPeripheralPowerType.Off;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralQueryResponse(byte[] bytes) : 
            base(DimmerPeripheralMessageType.VersionResponse, PeripheralSystemMessageType.DimmerGeneration3)
        {
            if (bytes == null || bytes.Length != Size + 2)
                return; // eat it

            // TODO
            Header.Address = (ushort)((bytes[2] << 8) + bytes[3]);
            Header.SystemType = (PeripheralSystemMessageType)bytes[4];
            Header.MessageType = (DimmerPeripheralMessageType)bytes[5];
            AmbientLevel = (ushort)((bytes[6] << 8) + bytes[7]);
            RangeScale = bytes[8];
            PowerStatus = (DimmerPeripheralPowerType)bytes[9];
            Brightness = bytes[10];
            Mode = (DimmerPeripheralMode)bytes[11];
            this.Checksum = bytes[12];
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
                writer.Write((byte)((AmbientLevel & 0xff00) >> 8));
                writer.Write((byte)(AmbientLevel & 0xff));
                writer.Write((byte)RangeScale);
                writer.Write((byte)PowerStatus);
                writer.Write((byte)Brightness);
                writer.Write((byte)Mode);
                writer.Write((byte)Checksum);
                return stream.ToArray();
            }
        }
    }
}
