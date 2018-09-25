// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Class1.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /*
    // GPIO_UPDATE: - Asynchronous Message from Audio Board
    typedef struct _g3audiogpioupdate_t
    {
        PCP_HEADER      hdr;
        uint_16         changeMask;         // bits mask of changes in gpioStatus field
        uint_16         gpioStatus;         // input levels - bit order of PCP_07_GPIO_MEANING
        uint_16         rawPins;            // bitmask of raw pins - no bit order translation
        uint_8          chksum;
    } __attribute__ ((packed)) PCP_07_GPIO_UPDATE_PKT;

    */

    /// <summary>The peripheral audio gpio update.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioGpioStatus : IPeripheralAudioGpioStatus
    {
        /// <summary>The Expected size.</summary>
        public const int Size = PeripheralHeader.Size + (3 * sizeof(ushort)) + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioGpioStatus"/> class.</summary>
        public PeripheralAudioGpioStatus()
        {
            this.Header = new PeripheralHeader(PeripheralMessageType.GpioStatusResponse, Size);
            this.Checksum = 0;  // Set before writing out to target
        }

        /// <summary>Gets or sets the header.</summary>
        [Order]
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the change mask. bits mask of changes in gpioStatus field</summary>
        [Order]
        public ushort ChangeMask { get; set; }

        /// <summary>Gets or sets the gpio status. input levels - bit order of PeripheralGpioType</summary>
        [Order]
        public ushort GpioStatus { get; set; }

        /// <summary>Gets or sets the raw pin status. bitmask of raw pins - no bit order translation</summary>
        [Order]
        public ushort RawPinStatus { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        public byte Checksum { get; set; }

    }
}