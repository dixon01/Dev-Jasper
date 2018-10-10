// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="Constants.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralProtocol.Core.Models
{
    using Luminator.PeripheralProtocol.Core.AudioSwitchPeripheral;

    /// <summary>The Peripheral constants.</summary>
    public static class Constants
    {
        #region Constants

        /// <summary>The default audio switch address.</summary>
        public const ushort DefaultPeripheralAudioSwitchAddress = 0xF000;

        /// <summary>The default dimmer address.</summary>
        public const ushort DefaultPeripheralDimmerAddress = 0x0;

        /// <summary>The peripheral header default bytes size.</summary>
        public const int PeripheralHeaderSize = 6;

        /// <summary>The max peripheral message size.</summary>
        public const int MaxPeriperialMessageSize = PeripheralAudioConfig.Size;

        /// <summary>The framing byte.</summary>
        public const byte PeripheralFramingByte = 0x7E;

        public const int DefaultMaxPeripheralFramingBytes = 2;

        public const int MinAudioStatusUpdateInterval = 250;
     
        /// <summary>The default max volume.</summary>
        public const byte MaxVolume = 100;

        /// <summary>The default min volume.</summary>
        public const byte MinVolume = 0;

        #endregion
    }
}