// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="PeripheralSetVolume.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    //// SET_VOLUME:
    // typedef struct _g3audiosetvolume_t
    // {
    // PCP_HEADER hdr;
    // uint_8 intVolume;          // 0 - 100
    // uint_8 extVolume;          // 0 - 100
    // uint_8 chksum;
    // } __attribute__((packed)) PCP_07_SET_VOLUME_PKT;

    /// <summary>The peripheral set volume.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralSetVolume : IPeripheralSetVolume
    {
        /// <summary>The Expected size.</summary>
        public const int Size = PeripheralHeader.Size + (3 * sizeof(byte));

        /// <summary>Initializes a new instance of the <see cref="PeripheralSetVolume" /> class.</summary>
        public PeripheralSetVolume()
            : this(0, 0)
        {
            this.Header = new PeripheralHeader(PeripheralMessageType.SetVolume, (ushort)Marshal.SizeOf(this));
        }

        /// <summary>Initializes a new instance of the <see cref="PeripheralSetVolume"/> class.</summary>
        /// <param name="interiorVolume">The interior volume.</param>
        /// <param name="exteriorVolume">The exterior volume.</param>
        public PeripheralSetVolume(byte interiorVolume, byte exteriorVolume)
        {
            this.Header = new PeripheralHeader(PeripheralMessageType.SetVolume, (ushort)Marshal.SizeOf(this));
            this.InteriorVolume = interiorVolume;
            this.ExteriorVolume = exteriorVolume;
            this.Checksum = CheckSumUtil.CalculateCheckSum(this);
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the interior volume.</summary>
        public byte InteriorVolume { get; set; }

        /// <summary>Gets or sets the exterior volume.</summary>
        public byte ExteriorVolume { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}