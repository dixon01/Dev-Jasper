// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerSetBrightness.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer set brightness.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer set brightness.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerSetBrightness : IPeripheralBaseMessage
    {
        public const int Size = PeripheralHeader.HeaderSize + (2 * sizeof(byte));

        /// <summary>Initializes a new instance of the <see cref="DimmerSetBrightness"/> class. Initializes a new instance of the<see cref="DimmerVersionResponse"/> class.</summary>
        /// <param name="brightnessLevel">The brightness Level.</param>
        public DimmerSetBrightness(byte brightnessLevel)
            : this()
        {
            this.BrightnessLevel = brightnessLevel;
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerSetBrightness"/> class.</summary>
        public DimmerSetBrightness()
        {
            this.Header = new DimmerPeripheralHeader(DimmerMessageType.SetBrightness, Marshal.SizeOf(this));
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the brightness level.</summary>
        public byte BrightnessLevel { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}