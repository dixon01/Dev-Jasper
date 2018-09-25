// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralAudioVersions.cs">
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

    // typedef struct _g3audioversions_t
    // {
    // PCP_HEADER hdr;
    // uint_8 hwVersion[4];       // hardware version values [0].[1].[2].[3]
    // uint_8 hwSerialNum[14];    // binary of serial number
    // uint_8 swVersion[4];       // software version values [0].[1].[2].[3]
    // uint_8 chksum;
    // }

    /// <summary>The peripheral audio versions.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioVersions : IPeripheralAudioVersions
    {
        /// <summary>The Expected size.</summary>
        public const int Size = PeripheralHeader.Size + PeripheralVersionsInfo.Size + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioVersions" /> class.</summary>
        public PeripheralAudioVersions()
        {
            this.Header = new PeripheralHeader(PeripheralMessageType.AudioVersionResponse, Marshal.SizeOf(this));
            this.VersionsInfo = new PeripheralVersionsInfo(null, null, null);
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Gets or sets the header.</summary>
        [Order(0)]
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the versions info.</summary>
        [Order(1)]
        public PeripheralVersionsInfo VersionsInfo { get; set; }

        /// <summary>The checksum.</summary>
        [Order(3)]
        public byte Checksum { get; set; }

        /// <summary>Gets or sets the hardware version.</summary>
        [IgnoreDataMember]
        public string HardwareVersion
        {
            get
            {
                return this.VersionsInfo.HardwareVersionText;
            }

            set
            {
                this.VersionsInfo.HardwareVersionText = value;
            }
        }

        /// <summary>Gets or sets the serial number.</summary>
        [IgnoreDataMember]
        public string SerialNumber
        {
            get
            {
                return this.VersionsInfo.SerialNumberText;
            }

            set
            {
                 this.VersionsInfo.SerialNumberText = value;
            }
        }

        /// <summary>Gets or sets the software version.</summary>
        [IgnoreDataMember]
        public string SoftwareVersion
        {
            get
            {
                return this.VersionsInfo.SoftwareVersionText;
            }

            set
            {
                 this.VersionsInfo.SoftwareVersionText = value;
            }
        }

        /// <summary>The equals.</summary>
        /// <param name="obj">The obj.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public override bool Equals(object obj)
        {
            return base.Equals(obj) && ((PeripheralAudioVersions)obj).VersionsInfo.Equals(this.VersionsInfo);
        }

        /// <summary>The get hash code.</summary>
        /// <returns>The <see cref="int"/>.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return this.VersionsInfo?.ToString() ?? base.ToString();
        }
    }
}