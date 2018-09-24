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

    [Obsolete("See Luminator.PeripheralDimmer")]
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1, Size = DimmerPeripheralHeader.Size)]
    public class DimmerPeripheralHeader : PeripheralHeader, IDimmerPeripheralHeader
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the message type. The Message ID indicates the packet type being transmitted. 
        /// The Message ID is specific to the System ID given. 
        /// Thus System ID 0x07 and Message ID 0x03 is a different packet type than System ID 0x08 and Message ID 0x03.</summary>
        [Order(3)]
        public new DimmerPeripheralMessageType MessageType { get; set; }

        #endregion

        #region Constructors

        public DimmerPeripheralHeader(
            PeripheralMessageType messageType,
            int length = Size,
            PeripheralSystemMessageType systemMessage = PeripheralSystemMessageType.DimmerGeneration3,
            ushort address = 0) : base(messageType, length, systemMessage, address)
        {
            this.MessageType = DimmerPeripheralMessageType.Unknown;
        }

        public DimmerPeripheralHeader(
            DimmerPeripheralMessageType messageType,
            int length = Size,
            PeripheralSystemMessageType systemMessage = PeripheralSystemMessageType.DimmerGeneration3,
            ushort address = 0)
        {
            this.Address = address;
            this.Length = (ushort)length;
            this.MessageType = messageType;
            this.SystemType = systemMessage;
        }

        public DimmerPeripheralHeader()
            : base(PeripheralMessageType.Unknown, Size, PeripheralSystemMessageType.DimmerGeneration3, 0)
        {
            this.MessageType = DimmerPeripheralMessageType.Unknown;
        }

        public DimmerPeripheralHeader(byte[] buffer, bool headerIsNetworkByteOrder = true)
        {
            var header = buffer.GetValidDimmerPeripheralHeader(headerIsNetworkByteOrder);
            if (header != null)
            {
                this.Address = header.Address;
                this.Length = header.Length;
                this.MessageType = header.MessageType;
                this.SystemType = header.SystemType;
            }
            else
            {
                throw new ArgumentException("Invalid Buffer for DimmerPeripheralHeader!", nameof(buffer));
            }
        }

        #endregion
    }
}