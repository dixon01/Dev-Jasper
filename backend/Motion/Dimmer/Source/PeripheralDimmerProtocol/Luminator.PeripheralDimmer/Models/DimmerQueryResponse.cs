// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerQueryResponse.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer query response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer query response.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerQueryResponse : IPeripheralBaseMessage
    {
        /// <summary>The size.</summary>
        public const int Size = PeripheralHeader.HeaderSize + 7;

        /// <summary>Initializes a new instance of the <see cref="DimmerQueryResponse" /> class.</summary>
        public DimmerQueryResponse()
        {
            this.Header = new DimmerPeripheralHeader(DimmerMessageType.QueryResponse, Marshal.SizeOf(this));
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerQueryResponse"/> class.</summary>
        /// <param name="brightness">The brightness.</param>
        /// <param name="lightLevel">The light level.</param>
        /// <param name="lightSensorScale">The light sensor scale.</param>
        /// <param name="mode">The mode.</param>
        /// <param name="powerOnMode">The power on mode.</param>
        public DimmerQueryResponse(byte brightness, ushort lightLevel, byte lightSensorScale, byte mode, byte powerOnMode)
            : this()
        {
            this.Brightness = brightness;
            this.LightLevel = lightLevel;
            this.LightSensorScale = lightSensorScale;
            this.Mode = mode;
            this.PowerOnMode = powerOnMode;
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the brightness level.</summary>
        public ushort LightLevel { get; set; }

        /// <summary>Gets or sets the light sensor scale.</summary>
        public byte LightSensorScale { get; set; }

        /// <summary>Gets or sets the power on mode.</summary>
        public byte PowerOnMode { get; set; }

        /// <summary>Gets or sets the brightness.</summary>
        public byte Brightness { get; set; }

        /// <summary>Gets or sets the mode.</summary>
        public byte Mode { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}