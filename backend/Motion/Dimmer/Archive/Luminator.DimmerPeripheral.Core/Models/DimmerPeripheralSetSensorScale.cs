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
    public class DimmerPeripheralSetSensorScale : DimmerPeripheralBaseMessage
    {
        public new const int Size = PeripheralHeader.Size + sizeof(byte);

        private DimmerPeripheralLightSensorScale scale;

        [Order]
        public DimmerPeripheralLightSensorScale Scale
        {
            get { return this.scale; }
            set
            {
                this.scale = value;
                this.Checksum = this.CheckSum(this.GetBytes());
            }
        }

        [Order]
        public new byte Checksum { get; set; }

        #region Constructors

        public DimmerPeripheralSetSensorScale() : base(DimmerPeripheralMessageType.SetLightSensorScale)
        {
            this.Scale = DimmerPeripheralLightSensorScale.Range1;
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralSetSensorScale(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3) : base(DimmerPeripheralMessageType.SetLightSensorScale, systemMessageType)
        {
            base.Header.Address = address;
            this.Scale = DimmerPeripheralLightSensorScale.Range1;
            this.Checksum = this.CheckSum(this.GetBytes());
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
                writer.Write((byte)Scale);
                writer.Write((byte)Checksum);
                return stream.ToArray();
            }
        }
    }
}
