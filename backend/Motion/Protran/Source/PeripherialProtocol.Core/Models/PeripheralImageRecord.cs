// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralImageRecord.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    // IMAGE_RECORD
    // typedef struct _g3_imagerecord_t
    // {
    // PCP_HEADER hdr;
    // uint_8 recordData[1];      // variable length record data - Intel Hex
    // // insert checksum here
    // } PACKED(PCP_07_IMAGE_RECORD_PKT);

    /// <summary>The peripheral image record.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralImageRecord : IPeripheralImageRecord
    {
        /// <summary>The size.</summary>
        public const int Size = PeripheralBaseMessage.Size + sizeof(byte); // this model will vary in size see Buffer[]

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageRecord" /> class.</summary>
        public PeripheralImageRecord()
        {
            this.Header = new PeripheralHeader
                              {
                                  MessageType = PeripheralMessageType.ImageRecord, 
                                  SystemType = PeripheralSystemMessageType.AudioGeneration3, 
                                  Length = (ushort)Marshal.SizeOf(this)
                              };

            this.Checksum = 0;
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageRecord"/> class.</summary>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system Message Type.</param>
        public PeripheralImageRecord(ushort address, PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.AudioGeneration3)
        {
            this.Header.Address = address;
            this.Header.SystemType = systemMessageType;
            this.Checksum = 0;
        }

        /// <summary>Gets or sets the header.</summary>
        [Order(0)]
        public PeripheralHeader Header { get; set; }

        /// <summary>The buffer.</summary>
        [Order(1)]
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = sizeof(byte))]
        public byte[] buffer;

        /// <summary>The checksum.</summary>
        [Order(2)]
        public byte Checksum { get; set; }

        /// <summary>Gets or sets the record buffer.</summary>
        [IgnoreDataMember]
        public byte[] Buffer
        {
            get
            {
                return this.buffer;
            }

            set
            {
                buffer = new byte[value.Length];
                Array.Copy(value, this.buffer, value.Length);
                this.Checksum = CheckSumUtil.CalculateCheckSum(this);
            }
        }
    }
}