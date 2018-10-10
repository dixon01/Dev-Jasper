// --------------------------------------------------------------------------------------------------------------------
// <copyright company="Luminator Technology Group" file="DimmerVersionResponse.cs">
//   Copyright © 2011-2015 LTG. All rights reserved.
// </copyright>
// <author>Kevin Hartman</author>
// <summary>
//   The dimmer version response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Luminator.PeripheralDimmer.Models
{
    using System;
    using System.Runtime.InteropServices;

    using Luminator.PeripheralDimmer.Interfaces;
    using Luminator.PeripheralDimmer.Types;

    /// <summary>The dimmer version response.</summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi)]
    [Serializable]
    public class DimmerVersionResponse : IPeripheralBaseMessage
    {
        /// <summary>The size.</summary>
        public const int Size = PeripheralHeader.HeaderSize + DimmerVersionInfo.Size + sizeof(byte);

        /// <summary>Initializes a new instance of the <see cref="DimmerVersionResponse" /> class.</summary>
        public DimmerVersionResponse()
        {
            this.Header = new DimmerPeripheralHeader(DimmerMessageType.VersionResponse, Marshal.SizeOf(this));
        }

        /// <summary>Gets or sets the header.</summary>
        public PeripheralHeader Header { get; set; }

        /// <summary>Gets or sets the data.</summary>
        public DimmerVersionInfo VersionInfo { get; set; }

        /// <summary>Gets or sets the checksum.</summary>
        public byte Checksum { get; set; }

        public override string ToString()
        {
            return this.VersionInfo?.ToString() ?? string.Empty;
        }
    }
}