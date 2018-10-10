
using System.IO;

using Luminator.PeripheralProtocol.Core.Models;

namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.Runtime.InteropServices;
    using Luminator.DimmerPeripheral.Core.Interfaces;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Types;

    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = Size)]
    public class DimmerPeripheralVersionResponse : DimmerPeripheralBaseMessage, IDimmerPeripheralVersion
    {
        public const int HardwareVersionSize = 16;
        
        public const int SoftwareVersionSize = 16;
        
        public new const int Size = PeripheralHeader.Size + HardwareVersionSize + SoftwareVersionSize;

        #region Public Properties

        /// <summary>The hardware version.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = HardwareVersionSize)]
        [Order]
        public byte[] HardwareVersion;

        /// <summary>The software version.</summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SoftwareVersionSize)]
        [Order]
        public byte[] SoftwareVersion;
        
        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        public new byte Checksum { get; set; }

        #endregion

        #region Constructors

        public DimmerPeripheralVersionResponse() : 
            base(DimmerPeripheralMessageType.VersionResponse, PeripheralSystemMessageType.DimmerGeneration3)
        {
            this.HardwareVersion = new byte[HardwareVersionSize];
            this.SoftwareVersion = new byte[SoftwareVersionSize];
            this.Checksum = this.CheckSum(this.GetBytes());
        }

        public DimmerPeripheralVersionResponse(byte[] bytes) : 
            base(DimmerPeripheralMessageType.VersionResponse, PeripheralSystemMessageType.DimmerGeneration3)
        {
            if (bytes == null || bytes.Length != Size + 2)
                return; // eat it

            // TODO
            Header.Address = (ushort) ((bytes[2] << 8) + bytes[3]);
            Header.SystemType = (PeripheralSystemMessageType) bytes[4];
            Header.MessageType = (DimmerPeripheralMessageType) bytes[5];
            this.HardwareVersion = new byte[HardwareVersionSize];
            this.SoftwareVersion = new byte[SoftwareVersionSize];
            Buffer.BlockCopy(bytes, 6, this.HardwareVersion, 0, HardwareVersionSize);
            Buffer.BlockCopy(bytes, 22, this.SoftwareVersion, 0, SoftwareVersionSize);
            this.Checksum = bytes[38];
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
                writer.Write((byte[])this.HardwareVersion);
                writer.Write((byte[])this.SoftwareVersion);
                writer.Write((byte)Checksum);
                return stream.ToArray();
                
            }
        }

        public override string ToString()
        {
            return String.Format("Hardware: {0}          Software: {1}", 
                System.Text.Encoding.ASCII.GetString(this.HardwareVersion),
                System.Text.Encoding.ASCII.GetString(this.SoftwareVersion));
        }
    }
}
