// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerSetPowerOnMode.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer set power on mode.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer set power on mode.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerSetPowerOnMode : IPeripheralBaseMessage
    {
        /// <summary>Initializes a new instance of the <see cref="DimmerSetPowerOnMode"/> class.</summary>
        /// <param name="powerOnOnMode">The power on on mode.</param>
        public DimmerSetPowerOnMode(PowerOnMode powerOnOnMode)
            : this()
        {
            this.PowerOnOnMode = powerOnOnMode;
        }

        /// <summary>Initializes a new instance of the <see cref="DimmerSetPowerOnMode"/> class.</summary>
        public DimmerSetPowerOnMode()
        {
            this.Header = new DimmerPeripheralHeader(DimmerMessageType.SetMonitorPower, Marshal.SizeOf(this));
            this.PowerOnOnMode = PowerOnMode.Default;
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the power on on mode.</summary>
        public PowerOnMode PowerOnOnMode { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }
    }
}