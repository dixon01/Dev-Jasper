namespace Luminator.DimmerPeripheral.Core.Models
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    using Luminator.DimmerPeripheral.Core.Interfaces;
    using Luminator.DimmerPeripheral.Core.Types;
    using Luminator.PeripheralProtocol.Core;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    /// <summary>
    /// Base dimmer peripheral message
    /// </summary>
    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class DimmerPeripheralBaseMessage : PeripheralBaseMessage, IDimmerPeripheralBaseMessage
    {
        public new const int Size = PeripheralHeader.Size;

        #region Public Properties

        [Order]
        public new DimmerPeripheralHeader Header { get; set; }

        #endregion

        #region Constructors

        protected DimmerPeripheralBaseMessage(
            DimmerPeripheralMessageType peripheralMessageType,
            PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.DimmerGeneration3)
        {
            this.Header = new DimmerPeripheralHeader
            {
                MessageType = peripheralMessageType,
                SystemType = systemMessageType,
                Length = (ushort)Marshal.SizeOf(this)
            };
            this.Checksum = 0;
        }
        
        protected DimmerPeripheralBaseMessage(DimmerPeripheralHeader header)
        {
            this.Header = header;
            this.Checksum = 0;
        }
        
        public DimmerPeripheralBaseMessage()
            : this(DimmerPeripheralMessageType.Unknown, PeripheralSystemMessageType.DimmerGeneration3)
        {
        }

        #endregion

        /// <summary>
        /// Work around since base reflection will go through all the properties, ToBytes marshalling needs further work.
        /// </summary>
        /// <returns>message bytes</returns>
        public virtual byte[] GetBytes()
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
                writer.Write((byte)Checksum);
                return stream.ToArray();
            }
        }

        public byte CheckSum(byte[] array)
        {
            byte sum = 0;
            if (array != null)
            {
                // exclude the framing byte which is the first byte in the array
                // exclude the checksum which is the last byte in the array
                for (var i = 1; i < array.Length - 1; i++)
                {
                    sum += array[i];
                }
            }

            // Two's compliment:
            sum = (byte)((~sum) + 1);
            return sum;
        }
    }
}
