// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralImageUpdateStart.cs">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Models;
    using Luminator.PeripheralProtocol.Core.Types;

    #region Notes

    // IMAGE_START
    // Command to start an application image download to the audio switch
    // typedef struct _g3_imagestart_t
    // {
    // PCP_HEADER hdr;
    // uint_16 recordCount;        // number of image data records to expect in the download
    // uint_8 chksum;
    // } PACKED(PCP_07_IMAGE_START_PKT); 
    #endregion

    /// <summary>The peripheral image update start.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralImageUpdateStart : IPeripheralImageUpdateStart
    {
        /// <summary>The size.</summary>
        public const int Size = PeripheralHeaderAudioSwitch.HeaderSize + sizeof(ushort) + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageUpdateStart"/> class. Initializes a new instance of
        ///     the<see cref="PeripheralImageUpdateStart"/> class. Initializes a new
        ///     instance of the <see cref="PeripheralAck"/> class.</summary>
        /// <param name="records">The records.</param>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system Message Type.</param>
        public PeripheralImageUpdateStart(
            ushort records, 
            ushort address = Constants.DefaultPeripheralAudioSwitchAddress, 
            PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.AudioGeneration3)
        {
            this.Header = new PeripheralHeaderAudioSwitch()
                              {
                                  MessageType = PeripheralAudioSwitchMessageType.ImageUpdateStart, 
                                  SystemType = systemMessageType, 
                                  Length = Size, 
                                  Address = address
                              };
            this.Records = records;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageUpdateStart"/> class.</summary>
        public PeripheralImageUpdateStart() : this(0, Constants.DefaultPeripheralAudioSwitchAddress)
        { 
        }

        /// <summary>Gets or sets the header.</summary>
        [Order(0)]
        public PeripheralHeader<PeripheralAudioSwitchMessageType> Header { get; set; }

        /// <summary>The total number of records.</summary>
        [Order(1)]
        public ushort Records { get; set; }

        /// <summary>The checksum.</summary>
        [Order(2)]
        public byte Checksum { get; set; }
    }
}