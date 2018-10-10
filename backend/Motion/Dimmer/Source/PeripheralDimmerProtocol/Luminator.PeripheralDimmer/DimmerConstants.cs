// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerConstants.cs">
//   Copyright © 2011-2017 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer
{
    using System;

    using Luminator.PeripheralDimmer.Models;

    /// <summary>The dimmer constants.</summary>
    public static class DimmerConstants
    {
        #region Constants

        /// <summary>The default dimmer scale.</summary>
        public const int DefaultDimmerScale = 0;

        /// <summary>The dimmer address.</summary>
        public const int DimmerAddress = 0x0;

        /// <summary>The dimmer system message type.</summary>
        public const byte DimmerSystemMessageType = 0x6;

        /// <summary>The framing byte.</summary>
        public const int PeripheralFramingByte = 0x7E;

        /// <summary>Gets or sets the receive bytes threshold.</summary>
        public const int RecieveBytesThreshold = 7;

        /// <summary>The smallest message size.</summary>
        public const int SmallestMessageSize = PeripheralHeader.HeaderSize + sizeof(byte); // Checksum

        /// <summary>The write delay default.</summary>
        public const byte WriteDelayDefault = 100;

        #endregion

        #region Public Properties

        /// <summary>Gets the default com port.</summary>
        public static string DefaultComPort
        {
            get
            {
                var comport = Environment.GetEnvironmentVariable("DimmerComPort");
                return string.IsNullOrEmpty(comport) ? "COM2" : comport;
            }
        }

        #endregion
    }
}