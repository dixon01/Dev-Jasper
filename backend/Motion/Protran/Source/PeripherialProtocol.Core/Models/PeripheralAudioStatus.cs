// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PeripheralAudioStatus.cs" company="Luminator Technology Group">
//   Copyright © 2011-2016 LTG. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;
    using Luminator.PeripheralProtocol.Core.Interfaces;
    using Luminator.PeripheralProtocol.Core.Types;

    /*
    // AUDIO_STATUS:
    typedef struct _g3audiostatus_t
    {
        PCP_HEADER      hdr;
        uint_8          intVolume;          // Interior Volume 0 - 100
        uint_8          intNoiseLevel;      // Interior Noise 0 - 16 noise levels 0 = no compensation
        uint_8          interiorGain;       // Interior Gain Level (0-10)
        uint_8          extVolume;          // Exterior Volume 0 - 100
        uint_8          extNoiseLevel;      // Exterior Noise 0 - 16 noise levels 0 = no compensation
        uint_8          exteriorGain;       // Exterior Gain Level (0-10)
        uint_8          activeSource;       // see PCP_07_PRIORITY_IDS values
        uint_8          activeSpeaker;      // output enable bitmap 0x1 = interior 0x02 = exterior
        uint_32         pttLockoutTime;     // time in seconds since PTT lockout activated 
        uint_8          chksum;
    } __attribute__ ((packed)) PCP_07_AUDIO_STATUS_PKT;
    */

    /// <summary>The peripheral audio status.</summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public class PeripheralAudioStatus : IPeripheralAudioStatus
    {
        /// <summary>Initializes a new instance of the <see cref="PeripheralAudioStatus"/> class.</summary>
        public PeripheralAudioStatus()
        {
            this.Header = new PeripheralHeader()
                              {
                                  MessageType = PeripheralMessageType.AudioStatusResponse, 
                                  SystemType = PeripheralSystemMessageType.AudioGeneration3, 
                                  Length = (ushort)Marshal.SizeOf(this)
                              };

            this.ActiveSource = AudioSourcePriority.None;
            this.ActiveSpeaker = ActiveSpeakerZone.None;
            this.Checksum = 0;
        }

        /// <summary>The Expected size.</summary>
        public const int Size = PeripheralHeader.Size + 13;

        /// <summary>Gets or sets the header.</summary>
        [Order]
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the interior volume.</summary>
        [Order]
        public byte InteriorVolume { get; set; } // Interior Volume 0 - 100

        /// <summary>Gets or sets the interior noise level.</summary>
        [Order]
        public byte InteriorNoiseLevel { get; set; } // Interior Noise 0 - 16 noise levels 0 = no compensation

        /// <summary>Gets or sets the interior gain.</summary>
        [Order]
        public byte InteriorGain { get; set; } // Interior Gain Level (0-10)

        /// <summary>Gets or sets the exterior volume.</summary>
        [Order]
        public byte ExteriorVolume { get; set; } // Exterior Volume 0 - 100

        /// <summary>Gets or sets the exterior noise level.</summary>
        [Order]
        public byte ExteriorNoiseLevel { get; set; } // Exterior Noise 0 - 16 noise levels 0 = no compensation

        /// <summary>Gets or sets the exterior gain.</summary>
        [Order]
        public byte ExteriorGain { get; set; } // Exterior Gain Level (0-10)

        /// <summary>Gets or sets the active source.</summary>
        [Order]
        public AudioSourcePriority ActiveSource { get; set; } // see PCP_07_PRIORITY_IDS values

        /// <summary>Gets or sets the active speaker.</summary>
        [Order]
        public ActiveSpeakerZone ActiveSpeaker { get; set; } // output enable bitmap 0x1 = interior 0x02 = exterior

        /// <summary>Gets or sets the push to talk lockout time.</summary>
        [Order]
        public int PttLockoutTime { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        [Order]
        public byte Checksum { get; set; }
    }
}