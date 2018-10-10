// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralImageStatus.cs">
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
    // image transfer ending status record
    // typedef struct _g3_imagestatus_t
    // {
    // PCP_HEADER hdr;
    // uint_8 status;             // download status - see PCP_07_STATUS_CODES
    // uint_8 chksum;
    // } PACKED(PCP_07_IMAGE_STATUS_PKT);

    #endregion
    /// <summary>The peripheral image status.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralImageStatus : IPeripheralAudioSwtichBaseMessage
    {
        /// <summary>The expected size.</summary>
        public const ushort Size = PeripheralHeaderAudioSwitch.HeaderSize + sizeof(PeripheralImageStatusType) + sizeof(byte) ;

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageStatus"/> class.</summary>
        /// <param name="statusType">The status type.</param>
        public PeripheralImageStatus(PeripheralImageStatusType statusType)
            : this(statusType, Constants.DefaultPeripheralAudioSwitchAddress, PeripheralSystemMessageType.AudioGeneration3)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageStatus"/> class.</summary>
        public PeripheralImageStatus()
            : this(PeripheralImageStatusType.Undefined)
        {
            this.Header = new PeripheralHeaderAudioSwitch(PeripheralAudioSwitchMessageType.ImageStatusResponse, Size);
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralImageStatus"/> class.</summary>
        /// <param name="statusType">The status type.</param>
        /// <param name="address">The address.</param>
        /// <param name="systemMessageType">The system message type.</param>
        public PeripheralImageStatus(
            PeripheralImageStatusType statusType, 
            ushort address = Constants.DefaultPeripheralAudioSwitchAddress, 
            PeripheralSystemMessageType systemMessageType = PeripheralSystemMessageType.AudioGeneration3)
        {
            this.Header = new PeripheralHeaderAudioSwitch
                              {
                                  MessageType = PeripheralAudioSwitchMessageType.ImageStatusResponse, 
                                  SystemType = systemMessageType, 
                                  Length = Size, 
                                  Address = address
                              };

            this.Status = statusType;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Gets or sets the header.</summary>
        [Order(0)]
        public PeripheralHeader<PeripheralAudioSwitchMessageType> Header { get; set; }

        /// <summary>Gets or sets the status.</summary>
        [Order(1)]
        public PeripheralImageStatusType Status { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        [Order(2)]
        public byte Checksum { get; set; }
    }
}