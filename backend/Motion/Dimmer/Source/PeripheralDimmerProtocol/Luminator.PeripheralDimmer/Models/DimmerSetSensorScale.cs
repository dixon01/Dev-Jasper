// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerSetSensorScale.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer set sensor scale.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer set sensor scale.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerSetSensorScale : IPeripheralBaseMessage
    {
        public const int Size = PeripheralHeader.HeaderSize + (2 * sizeof(byte));

        /// <summary>Initializes a new instance of the <see cref="DimmerSetSensorScale"/> class. Initializes a new instance of the<see cref="DimmerVersionResponse"/> class.</summary>
        /// <param name="scale">The scale Level.</param>
        public DimmerSetSensorScale(byte scale)
            : this()
        {
            this.Scale = scale;
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerSetSensorScale"/> class.</summary>
        public DimmerSetSensorScale()
        {
            this.Header = new DimmerPeripheralHeader(DimmerMessageType.SetSensorScale, Marshal.SizeOf(this));
            this.Scale = DimmerConstants.DefaultDimmerScale;
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the Scale level.</summary>
        public byte Scale { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}