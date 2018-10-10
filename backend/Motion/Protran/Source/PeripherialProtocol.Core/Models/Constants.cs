// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Luminator.PeripheralProtocol.Core.Models
{
    /// <summary>The constants.</summary>
    public static class Constants
    {
        #region Constants

        /// <summary>The default max peripheral framing bytes.</summary>
        public const int DefaultMaxPeripheralFramingBytes = 2;

        /// <summary>The default address.</summary>
        public const ushort DefaultPeripheralAddress = 0xF000;

        /// <summary>The default wait for Ack timeout in milliseconds.</summary>
        public const int DefaultWaitForAckTimeout = 500;

        public const int DefaultMaxWritesWhenWaitForAck = 1;

        /// <summary>The max peripheral message size.</summary>
        public const int MaxPeriperialMessageSize = PeripheralAudioConfig.Size;

        /// <summary>The default max volume.</summary>
        public const byte MaxVolume = 100;

        /// <summary>The min audio status update interval.</summary>
        public const int MinAudioStatusUpdateInterval = 250;

        /// <summary>The default min volume.</summary>
        public const byte MinVolume = 0;

        /// <summary>The framing byte.</summary>
        public const byte PeripheralFramingByte = 0x7E;

        #endregion
    }
}